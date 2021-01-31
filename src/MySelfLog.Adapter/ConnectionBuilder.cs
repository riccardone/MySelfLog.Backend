using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using NLog;

namespace MySelfLog.Adapter
{
    public class ConnectionBuilder : IConnectionBuilder
    {
        private readonly Logger Log;

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

        public ConnectionBuilder(Uri connectionString, ConnectionSettings connectionSettings, string connectionName, UserCredentials credentials, LogFactory logFactory)
        {
            ConnectionString = connectionString;
            ConnectionSettings = connectionSettings;
            ConnectionName = connectionName;
            Credentials = credentials;
            Log = logFactory.GetCurrentClassLogger();
        }
    }
}
