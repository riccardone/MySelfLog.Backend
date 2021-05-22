using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace MySelfLog.Backend.Adapter
{
    public interface IConnectionBuilder
    {
        string ConnectionName { get; }
        Uri ConnectionString { get; }
        ConnectionSettings ConnectionSettings { get; }
        UserCredentials Credentials { get; }
        IEventStoreConnection Build(bool openConnection = true);
    }
}
