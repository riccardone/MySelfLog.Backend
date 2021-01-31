using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Evento;
using Evento.Repository;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using MySelfLog.Adapter;
using MySelfLog.Contracts;
using MySelfLog.CryptoService;
using Newtonsoft.Json;
using NLog;
using NLog.Web;

namespace MySelfLog.Backend
{
    public class Processing
    {
        [FunctionName("MySelfLogProcessing")]
        public async Task ProcessCloudRequests(
            [ServiceBusTrigger("%QueueName%", Connection = "AzureServiceBusListeningConnectionString")]
            Microsoft.Azure.ServiceBus.Message message, MessageReceiver messageReceiver)
        {
            var logFactory = BuildLogFactory();
            var log = logFactory.GetCurrentClassLogger();
            IEventStoreConnection esConnection = null;

            try
            {
                log.Debug("Triggered...");
                var config = new ConfigurationBuilder()
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var settings = config.Get<Adapter.Settings>();
                var text = Encoding.UTF8.GetString(message.Body);
                var cloudRequest = JsonConvert.DeserializeObject<CloudEventRequest>(text);
                DecryptMessageIfNeeded(cloudRequest, settings.CryptoKey);
                esConnection = BuilderForDomain("myselflog-processing", settings,
                    BuildConnectionSettings(settings.EventStore_Username, settings.EventStore_Password)).Build();
                Worker worker = new Worker(BuildDomainRepositories(esConnection), BuildLogFactory());
                worker.Process(cloudRequest);
                await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                var errForElastic = ex.GetBaseException().Message.Length > 255
                    ? ex.GetBaseException().Message.Substring(0, 255)
                    : ex.GetBaseException().Message;

                log.Error(errForElastic);

                var errForServiceBus = ex.GetBaseException().Message.Length > 4096
                    ? ex.GetBaseException().Message.Substring(0, 4096)
                    : ex.GetBaseException().Message;

                var errDescriptionForServiceBus = ex.GetBaseException().StackTrace.Length > 4096
                    ? ex.GetBaseException().StackTrace.Substring(0, 4096)
                    : ex.GetBaseException().StackTrace;

                await messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken, errForServiceBus,
                    errDescriptionForServiceBus);
            }
            finally
            {
                esConnection?.Close();
            }
        }

        private static Dictionary<string, IDomainRepository> BuildDomainRepositories(IEventStoreConnection connection)
        {
            // category must match cloudRequest.Source for each accepted source
            var domainRepository = new EventStoreDomainRepository("myselflog", connection);
            // add here any new source (or put these in an array from settings)
            var domainRepositories = new Dictionary<string, IDomainRepository>
            {
                {domainRepository.Category, domainRepository}
            };
            return domainRepositories;
        }

        private static IConnectionBuilder BuilderForDomain(string connectionName, Adapter.Settings settings,
            EventStore.ClientAPI.ConnectionSettings connSettings)
        {
            var builderForDomain = new ConnectionBuilder(new Uri(settings.EventStore_ProcessorLink), connSettings,
                connectionName, new UserCredentials(settings.EventStore_Username, settings.EventStore_Password), BuildLogFactory());
            return builderForDomain;
        }

        private static LogFactory BuildLogFactory()
        {
            NLog.Config.ConfigurationItemFactory.Default = new NLog.Config.ConfigurationItemFactory(typeof(NLog.ILogger).GetTypeInfo().Assembly);

            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            var nlogFile = string.IsNullOrWhiteSpace(environment)
                ? "nlog.config"
                : $"{environment}-nlog.config";

            var nlogConfigPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../{nlogFile}";
            return NLogBuilder.ConfigureNLog(nlogConfigPath);
        }

        private static EventStore.ClientAPI.ConnectionSettings BuildConnectionSettings(string user, string key)
        {
            var connSettings = EventStore.ClientAPI.ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials(user, key))
                .SetHeartbeatInterval(TimeSpan.FromSeconds(10))
                .SetHeartbeatTimeout(TimeSpan.FromSeconds(5))
                .KeepReconnecting().KeepRetrying().SetReconnectionDelayTo(TimeSpan.FromSeconds(2)).Build();
            return connSettings;
        }

        private static void DecryptMessageIfNeeded(CloudEventRequest request, string cryptoKey)
        {
            if (!string.IsNullOrWhiteSpace(cryptoKey))
            {
                var cryptoService = new AesCryptoService(Convert.FromBase64String(cryptoKey));
                request.Data = cryptoService.Decrypt(Convert.FromBase64String(request.Data));
            }
        }
    }
}
