//using System;
//using EventStore.ClientAPI;
//using EventStore.ClientAPI.SystemData;
//using MySelfLog.Adapter;

//namespace MySelfLog.Host
//{
//    class PersistentSubscriptionManager
//    {
//        private readonly IConnectionBuilder _connectionBuilder;
//        private readonly HostConfig _configuration;
//        private const string InputStream = "diary-input";
//        private const string PersistentSubscriptionGroup = "myselflog-processors";

//        public PersistentSubscriptionManager(IConnectionBuilder connectionBuilder, HostConfig configuration)
//        {
//            _connectionBuilder = connectionBuilder;
//            _configuration = configuration;
//        }

//        public void CreateSubscription()
//        {
//            var connection = _connectionBuilder.Build();
//            try
//            {
//                connection.ConnectAsync().Wait();
//                connection.CreatePersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup,
//                    PersistentSubscriptionSettings.Create().StartFromBeginning().DoNotResolveLinkTos(),
//                    new UserCredentials(_configuration.UserName, _configuration.Password)).Wait();
//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                // Already exist
//            }
//        }
//    }
//}
