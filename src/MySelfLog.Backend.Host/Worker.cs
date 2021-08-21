using Evento;
using Evento.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySelfLog.Backend.Adapter;
using NLog;
using Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MySelfLog.Backend.Host
{
    public class Worker : BackgroundService
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {            
            Run();
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        private void Run()
        {
            var settings = BuildConfig();

            try
            {
                SetupRequirements(settings);
                _logger.LogInformation($"Metrics endpoint configured: <host>:{settings.Metrics_port}/metrics");
            }
            catch (HttpListenerException e)
            {
                _logger.LogWarning("Metrics endpoint not configured, try to run the program with administrative permission");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error while configuring metrics endpoint: {ex.GetBaseException().Message}");
            }

            _logger.LogInformation("Starting Backend Component");
            Adapter.Worker endpoint = BuildWorkerUsingEventStore(settings);
            if (!endpoint.Start())
                throw new Exception("Fatal error while starting the endpoint");
            _logger.LogInformation("Started Backend Component");
        }

        private static void SetupRequirements(Settings settings)
        {
            var metricServer = new MetricServer(port: settings.Metrics_port);
            metricServer.Start();
        }

        private static Settings BuildConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddJsonFile("config/appsettings.docker.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build().Get<Settings>();
        }

        private static Adapter.Worker BuildWorkerUsingEventStore(Settings settings)
        {
            string queue = GetQueueName(settings);
            var subscriberFromEventStore = new MessageReceiverFromEventStore(BuilderForSubscriber(settings), queue,
                $"{settings.DomainCategory}-processors");
            var endpoint = new Adapter.Worker(BuildDomainRepositories(settings), subscriberFromEventStore, settings);
            return endpoint;
        }

        private static string GetQueueName(Settings settings)
        {
            return string.IsNullOrWhiteSpace(settings.Input_queue_forced)
                            ? $"{settings.Input_queue}-{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}"
                            : settings.Input_queue_forced;
        }

        private static Dictionary<string, IDomainRepository> BuildDomainRepositories(Settings settings)
        {
            // category must match cloudRequest.Source for each accepted source
            var domainRepositoryForMySelfLog = new EventStoreDomainRepository(settings.DomainCategory,
                BuilderForDomain(settings).Build());

            var domainRepositories = new Dictionary<string, IDomainRepository>
            {
                {domainRepositoryForMySelfLog.Category, domainRepositoryForMySelfLog}
            };
            return domainRepositories;
        }

        private static ConnectionBuilder BuilderForDomain(Settings settings)
        {
            var builderForDomain = new ConnectionBuilder(new Uri(settings.EventStore_ProcessorLink),
                $"{settings.DomainCategory}-backend-processor", settings);
            return builderForDomain;
        }

        private static ConnectionBuilder BuilderForSubscriber(Settings settings)
        {
            var builderForSubscriber = new ConnectionBuilder(new Uri(settings.EventStore_SubscriberLink),
                 $"{settings.DomainCategory}-backend-subscriber", settings);
            return builderForSubscriber;
        }
    }
}
