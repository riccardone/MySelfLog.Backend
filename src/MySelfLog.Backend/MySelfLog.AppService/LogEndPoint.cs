using System;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Tools.Infrastructure;
using log4net;
using MySelfLog.Messages;
using Newtonsoft.Json;

namespace MySelfLog.AppService
{
    public class LogEndPoint
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LogEndPoint));
        private readonly IDomainRepository _domainRepository;
        private readonly IEventStoreConnection _connection;
        private readonly LogsHandler _logsHandler;
        private const string InputStream = "log-input";
        private const string PersistentSubscriptionGroup = "myselflog-processors";

        public LogEndPoint(IDomainRepository domainRepository, IEventStoreConnection connection)
        {
            _logsHandler = new LogsHandler(domainRepository);
            _domainRepository = domainRepository;
            _connection = connection;
        }

        public bool Start()
        {
            try
            {
                CreatePersistentSubscription();
                Subscribe();
                Log.Info("AssociateAccount EndPoint started");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        private void EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, ResolvedEvent resolvedEvent)
        {
            try
            {
                var e = resolvedEvent.Event;
                var eventJson = Encoding.UTF8.GetString(e.Data);
                var aggregate = _logsHandler.Handle(JsonConvert.DeserializeObject<LogValue>(eventJson));
                _domainRepository.Save(aggregate);
                Log.Info($"'{e.EventType}' handled with CorrelationId '{aggregate.AggregateId}'");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    ex.GetBaseException().Message);
            }
        }
        public void Stop()
        {
            _connection.Close();
        }

        #region BoilerplateCode
        private void Subscribe()
        {
            _connection.ConnectToPersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup, EventAppeared, SubscriptionDropped).Wait();
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            throw new NotImplementedException();
        }

        private void CreatePersistentSubscription()
        {
            try
            {
                _connection.CreatePersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup,
                    PersistentSubscriptionSettings.Create(), new UserCredentials("admin", "changeit")).Wait();
            }
            catch (Exception ex)
            {
                // Already exist
            }
        }
        #endregion
    }
}
