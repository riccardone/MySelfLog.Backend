using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace MySelfLog.Host
{
    class PersistentSubscriptionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreConfiguration _configuration;
        private const string InputStream = "diary-input";
        private const string PersistentSubscriptionGroup = "myselflog-processors";

        public PersistentSubscriptionManager(IEventStoreConnection connection, EventStoreConfiguration configuration)
        {
            _connection = connection;
            _configuration = configuration;
        }

        public void CreateSubscription()
        {
            try
            {
                _connection.CreatePersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup,
                    PersistentSubscriptionSettings.Create().StartFromBeginning().DoNotResolveLinkTos(),
                    new UserCredentials(_configuration.UserName, _configuration.Password)).Wait();
            }
            catch (Exception)
            {
                // Already exist
            }
        }
    }
}
