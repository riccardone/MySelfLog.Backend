using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Evento;
using Evento.Repository;
using Microsoft.Extensions.Configuration;
using MySelfLog.Backend.Adapter;
using NLog;
using Prometheus;

namespace MySelfLog.Backend.Host
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var settings = BuildConfig();

            try
            {
                SetupRequirements(settings);
                Log.Info($"Metrics endpoint configured: <host>:{settings.Metrics_port}/metrics");
            }
            catch (HttpListenerException e)
            {
                Log.Warn("Metrics endpoint not configured, try to run the program with administrative permission");
            }
            catch (Exception ex)
            {
                Log.Warn($"Error while configuring metrics endpoint: {ex.GetBaseException().Message}");
            }

            Worker endpoint = null;

            try
            {
                Log.Info("Starting Backend Component");
                endpoint = BuildWorkerUsingEventStore(settings);
                if (!endpoint.Start())
                    throw new Exception("Fatal error while starting the endpoint");
                Log.Info("Started Backend Component");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }

            Log.Info("Shutting down naturally: Backend Component");
            endpoint?.Stop();
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

        private static Worker BuildWorkerUsingEventStore(Settings settings)
        {
            var queue = string.IsNullOrWhiteSpace(settings.Input_queue_forced)
                ? $"{settings.Input_queue}-{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}"
                : settings.Input_queue_forced;
            var subscriberFromEventStore = new MessageReceiverFromEventStore(BuilderForSubscriber(settings), queue,
                $"{settings.DomainCategory}-processors");
            var endpoint = new Worker(BuildDomainRepositories(settings), subscriberFromEventStore, settings);
            return endpoint;
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
