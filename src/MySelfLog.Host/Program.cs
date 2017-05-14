﻿using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Tools.Infrastructure.Repository;
using log4net.Config;
using MySelfLog.AppService;
using Topshelf;

namespace MySelfLog.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            HostFactory.Run(x =>
            {
                x.UseLinuxIfAvailable();
                x.UseLog4Net();
                var esConfig = new EventStoreConfiguration();
                x.Service<LogEndPoint>(s =>
                {
                    var connSettings = ConnectionSettings.Create().SetDefaultUserCredentials(new UserCredentials(esConfig.UserName, esConfig.Password))
                        .KeepReconnecting().KeepRetrying().Build();
                    var endpointConnection = EventStoreConnection.Create(connSettings,
                        new IPEndPoint(IPAddress.Parse(esConfig.Node1HostName), esConfig.Node1TcpPort), "ES-Subscriber");
                    var domainConnection = EventStoreConnection.Create(connSettings,
                        new IPEndPoint(IPAddress.Parse(esConfig.Node1HostName), esConfig.Node1TcpPort), "ES-Processor");
                    endpointConnection.ConnectAsync().Wait();
                    domainConnection.ConnectAsync().Wait();
                    var subscriptionManager = new PersistentSubscriptionManager(endpointConnection, esConfig);
                    subscriptionManager.CreateSubscription();
                    s.ConstructUsing(
                        name =>
                            new LogEndPoint(new EventStoreDomainRepository("logs", domainConnection),
                                endpointConnection));
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
    }
}