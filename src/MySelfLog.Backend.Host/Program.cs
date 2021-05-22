using System;
using System.Collections.Generic;
using System.IO;
using Evento;
using Evento.Repository;
using Microsoft.Extensions.Configuration;
using MySelfLog.Backend.Adapter;
using NLog;

namespace MySelfLog.Backend.Host
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var settings = BuildConfig();
            Log.Info("Starting Endpoint setup");
            
            var endpoint = BuildWorkerUsingEventStore(settings);
           
            Log.Info("Completed Endpoint setup");

            try
            {
                Log.Info("Starting: Backend Component");
                if (!endpoint.Start())
                    throw new Exception("Fatal error while starting the endpoint");
                Log.Info("Started: Backend Component");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }

            Log.Info("Shutting down naturally: Backend Component");
            endpoint.Stop();
        }

        private static Settings BuildConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            return builder.Build().Get<Settings>();
        }

        private static Worker BuildWorkerUsingEventStore(Settings settings)
        {
            var subscriberFromEventStore =
                new MessageReceiverFromEventStore(BuilderForSubscriber(settings), settings.Input_queue, $"{settings.DomainCategory}-processors");
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
