using System;
using System.Text;
using System.Threading.Tasks;
using Evento;
using EventStore.ClientAPI;
using log4net;
using MySelfLog.Adapter.Mappings;
using MySelfLog.Domain.Aggregates;

namespace MySelfLog.Adapter
{
    public class EndPoint
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EndPoint));
        private readonly IDomainRepositoryV2 _domainRepository;
        private readonly IEventStoreConnection _connection;
        private const string InputStream = "diary-input";
        private const string PersistentSubscriptionGroup = "myselflog-processors";

        public EndPoint(IDomainRepositoryV2 domainRepository, IEventStoreConnection connection)
        {
            _domainRepository = domainRepository;
            _connection = connection;
        }

        public bool Start()
        {
            try
            {
                Subscribe();
                Log.Info($"Listening from '{InputStream}' stream");
                Log.Info($"Joined '{PersistentSubscriptionGroup}' group");
                Log.Info($"Log EndPoint started");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public void Stop()
        {
            _connection.Close();
        }
        private void Subscribe()
        {
            _connection.ConnectToPersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup, EventAppeared, SubscriptionDropped).Wait();
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, ResolvedEvent resolvedEvent)
        {
            try
            {
                var e = resolvedEvent.Event;
                var eventJson = Encoding.UTF8.GetString(e.Data);
                var metadataJson = Encoding.UTF8.GetString(e.Metadata);
                var log = new LogFromJson(eventJson, metadataJson);
                Diary aggregate;
                try
                {
                    aggregate = _domainRepository.GetById<Diary>(log.CorrelationId);
                }
                catch (AggregateNotFoundException)
                {
                    aggregate = Diary.Create(log.BuildCreateDiary());
                }
                aggregate.LogValue(log.BuildLogValue());
                aggregate.LogTerapy(log.BuildLogTerapy());
                aggregate.LogFood(log.BuildLogFood());
                _domainRepository.Save(aggregate);

                Log.Info($"'{e.EventType}' handled with CorrelationId '{aggregate.AggregateId}'");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    ex.GetBaseException().Message);
            }
            return Task.CompletedTask;
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            Log.Error(subscriptionDropReason.ToString(), arg3);
        }
    }
}
