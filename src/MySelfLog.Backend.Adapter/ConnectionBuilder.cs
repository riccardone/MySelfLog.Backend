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
            var conn = EventStoreConnection.Create(ConnectionSettings, ConnectionString, ConnectionName);
            conn.Disconnected += Conn_Disconnected;
            conn.Reconnecting += Conn_Reconnecting;
            conn.Connected += Conn_Connected;
            if (openConnection)
                conn.ConnectAsync().Wait();

            return conn;
        }

        private void Conn_Connected(object sender, ClientConnectionEventArgs e)
        {
            Log.Debug($"Connected to EventStore RemoteEndPoint:'{e.RemoteEndPoint}';ConnectionName:'{e.Connection.ConnectionName}'");
        }

        private void Conn_Reconnecting(object sender, ClientReconnectingEventArgs e)
        {
            Log.Debug($"Reconnecting to EventStore ConnectionName:'{e.Connection.ConnectionName}'");
        }

        private void Conn_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            Log.Error($"Disconnected from EventStore RemoteEndPoint:'{e.RemoteEndPoint}';ConnectionName:'{e.Connection.ConnectionName}'");
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
