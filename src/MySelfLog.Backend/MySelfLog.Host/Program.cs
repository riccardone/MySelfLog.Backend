using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
                x.Service<LogEndPoint>(s =>
                {
                    var connSettings = ConnectionSettings.Create().SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                        .KeepReconnecting().KeepRetrying().Build();
                    var endpointConnection = EventStoreConnection.Create(connSettings,
                        new IPEndPoint(IPAddress.Loopback, 1113), "ES-Subscriber");
                    var domainConnection = EventStoreConnection.Create(connSettings,
                        new IPEndPoint(IPAddress.Loopback, 1113), "ES-Processor");
                    endpointConnection.ConnectAsync().Wait();
                    domainConnection.ConnectAsync().Wait();
                    s.ConstructUsing(
                        name =>
                            new LogEndPoint(new EventStoreDomainRepository("diary", domainConnection),
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
