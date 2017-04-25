using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace MySelfLog.Host
{
    class PersistentSubscriptionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreConfiguration _configuration;
        private const string InputStream = "log-input";
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
            catch (Exception ex)
            {
                // Already exist
            }
        }
    }
}
