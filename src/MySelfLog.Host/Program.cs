﻿using System;
using Evento.Repository;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using log4net.Config;
using MySelfLog.Adapter;
using MySelfLog.DiaryCache;
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
                log4net.GlobalContext.Properties["Domain"] = "MySelfLog.Backend";
                var esConfig = new HostConfig();
                x.Service<Adapter.EndPoint>(s =>
                {
                    var connSettings = ConnectionSettings.Create().SetDefaultUserCredentials(new UserCredentials(esConfig.UserName, esConfig.Password))
                        .KeepReconnecting().KeepRetrying().Build();
                    var endpointConnection = EventStoreConnection.Create(connSettings, esConfig.EventStoreLink, "ES-Subscriber");
                    var domainConnection = EventStoreConnection.Create(connSettings, esConfig.EventStoreLink, "ES-Processor");
                    endpointConnection.ConnectAsync().Wait();
                    domainConnection.ConnectAsync().Wait();
                    var subscriptionManager = new PersistentSubscriptionManager(endpointConnection, esConfig);
                    subscriptionManager.CreateSubscription();
                    var repo = new EventStoreDomainRepository("domain", domainConnection);
                    var diaryCache = new DiaryCacheService(esConfig.ElasticSearchLink);
                    s.ConstructUsing(name => new EndPoint(repo, endpointConnection, new Handlers(repo, diaryCache)));
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
