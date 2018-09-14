using System;
using Evento.Repository;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using log4net;
using log4net.Config;
using MySelfLog.Adapter;
using MySelfLog.DiaryCache;
using Topshelf;

namespace MySelfLog.Host
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            HostFactory.Run(x =>
            {
                x.UseLinuxIfAvailable();
                x.UseLog4Net();
                GlobalContext.Properties["Domain"] = "MySelfLog.Backend";
                var esConfig = new HostConfig();
                x.Service<EndPoint>(s =>
                {
                    var connSettings = ConnectionSettings.Create().SetDefaultUserCredentials(new UserCredentials(esConfig.UserName, esConfig.Password))
                        .SetHeartbeatInterval(TimeSpan.FromSeconds(3))
                        .SetHeartbeatTimeout(TimeSpan.FromSeconds(6))
                        .KeepReconnecting().KeepRetrying().Build();
                    var builderForEndPoint = new ConnectionBuilder(esConfig.EventStoreSubscriberLink,
                        connSettings, "MySelfLog-Backend-Subscriber",
                        new UserCredentials(esConfig.UserName, esConfig.Password));
                    var subscriptionManager = new PersistentSubscriptionManager(builderForEndPoint, esConfig);
                    subscriptionManager.CreateSubscription();
                    var builderForDomain = new ConnectionBuilder(esConfig.EventStoreProcessorLink,
                        connSettings, "MySelfLog-Backend-Processor",
                        new UserCredentials(esConfig.UserName, esConfig.Password));
                    var domainConnection = builderForDomain.Build();
                    domainConnection.Connected += DomainConnection_Connected;
                    domainConnection.Disconnected += DomainConnection_Disconnected;
                    domainConnection.ConnectAsync().Wait();
                    var repo = new EventStoreDomainRepository("domain", domainConnection);
                    s.ConstructUsing(name => new EndPoint(repo, builderForEndPoint, new Handlers(repo, new DiaryService(esConfig.ElasticSearchLink))));
                    s.WhenStarted((tc, hostControl) => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.UseAssemblyInfoForServiceInfo();
                x.SetDescription("MySelfLog Backend");
                x.SetDisplayName("MySelfLog Backend Host");
                x.SetServiceName("MySelfLog.Host");
            });
        }

        private static void DomainConnection_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            Log.Error($"DomainConnection Disconnected from {e.RemoteEndPoint}");
        }

        private static void DomainConnection_Connected(object sender, ClientConnectionEventArgs e)
        {
            Log.Info($"DomainConnection Connected to {e.RemoteEndPoint}");
        }
    }
}
