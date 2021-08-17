using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Evento;
using Evento.Repository;
using EventStore.ClientAPI;
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
            test(settings);

            //try
            //{
            //    SetupRequirements(settings);
            //    Log.Info($"Metrics endpoint configured: <host>:{settings.Metrics_port}/metrics");
            //}
            //catch (HttpListenerException e)
            //{
            //    Log.Warn("Metrics endpoint not configured, try to run the program with administrative permission");
            //}
            //catch (Exception ex)
            //{
            //    Log.Warn($"Error while configuring metrics endpoint: {ex.GetBaseException().Message}");
            //}

            //Worker endpoint = null;

            //try
            //{
            //    Log.Info("Starting Backend Component");
            //    endpoint = BuildWorkerUsingEventStore(settings);
            //    if (!endpoint.Start())
            //        throw new Exception("Fatal error while starting the endpoint");
            //    Log.Info("Started Backend Component");
            //    Console.ReadLine();
            //}
            //catch (Exception e)
            //{
            //    Log.Fatal(e);
            //}

            //endpoint?.Stop();
            Log.Info("Shutting down naturally: Backend Component");
        }

        private static void test(Settings settings)
        {
            var connBuilder = BuilderForSubscriber(settings);
            var conn = connBuilder.Build(false);
            conn.ConnectAsync().Wait();
            var eventData = new EventData(Guid.NewGuid(), "test-event", true, Encoding.UTF8.GetBytes("{\"id\": \"1\" \"value\": \"some value\"}"), null);
            try
            {
                Log.Debug("Writing a test event...");
                var res = conn.AppendToStreamAsync(GetQueueName(settings), ExpectedVersion.Any, new List<EventData> { eventData }).Result;
                Log.Debug($"LogPosition: {res.LogPosition} NextVer: {res.NextExpectedVersion}");
            }
            catch (Exception ex)
            {
                Log.Info(ex.GetBaseException().Message);
                //throw;
            }
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
            string queue = GetQueueName(settings);
            var subscriberFromEventStore = new MessageReceiverFromEventStore(BuilderForSubscriber(settings), queue,
                $"{settings.DomainCategory}-processors");
            var endpoint = new Worker(BuildDomainRepositories(settings), subscriberFromEventStore, settings);
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
