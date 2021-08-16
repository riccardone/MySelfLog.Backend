using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using NLog;

namespace MySelfLog.Backend.Adapter
{
    public class ConnectionBuilder : IConnectionBuilder
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public Uri ConnectionString { get; }
        public ConnectionSettings ConnectionSettings { get; }
        public string ConnectionName { get; }
        public UserCredentials Credentials { get; }
        
        public IEventStoreConnection Build(bool openConnection = true)
        {
            //Log.Debug($"Building connection name '{ConnectionName}' using connstring '{ConnectionString}'");
            var conn = EventStoreConnection.Create(ConnectionSettings, ConnectionString, ConnectionName);            
            if (openConnection)
                conn.ConnectAsync().Wait();

            return conn;
        }        

        public ConnectionBuilder(Uri connectionString, ConnectionSettings connectionSettings, string connectionName, UserCredentials credentials)
        {
            ConnectionString = connectionString;
            ConnectionSettings = connectionSettings;
            ConnectionName = connectionName;
            Credentials = credentials;
        }

        public ConnectionBuilder(Uri connectionString, string connectionName, Settings settings)
        {
            ConnectionString = connectionString;
            ConnectionSettings = BuildConnectionSettings(settings);
            ConnectionName = connectionName;
            Credentials = new UserCredentials(settings.EventStore_Username, settings.EventStore_Password);
        }

        public static ConnectionSettings BuildConnectionSettings(Settings settings)
        {
            var connSettings = string.IsNullOrWhiteSpace(settings.CertificateFqdn)
                ? ConnectionSettings.Create()
                    .SetDefaultUserCredentials(new UserCredentials(settings.EventStore_Username,
                        settings.EventStore_Password))
                    .SetHeartbeatInterval(TimeSpan.FromSeconds(10))
                    .SetHeartbeatTimeout(TimeSpan.FromSeconds(5))
                    .KeepReconnecting().KeepRetrying().SetReconnectionDelayTo(TimeSpan.FromSeconds(2)).Build()
                : ConnectionSettings.Create()
                    .UseSslConnection(settings.CertificateFqdn, true)
                    .SetDefaultUserCredentials(new UserCredentials(settings.EventStore_Username,
                        settings.EventStore_Password))
                    .KeepReconnecting().KeepRetrying();

            return connSettings;
        }
    }
}
