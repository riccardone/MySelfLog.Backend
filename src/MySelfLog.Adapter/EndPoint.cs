using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Evento;
using EventStore.ClientAPI;
using log4net;
using MySelfLog.Adapter.Mappings;
using MySelfLog.Domain.Aggregates;
using MySelfLog.Domain.Commands;

namespace MySelfLog.Adapter
{
    public class EndPoint
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EndPoint));
        private readonly IDomainRepository _domainRepository;
        private readonly IEventStoreConnection _connection;
        private const string InputStream = "diary-input";
        private const string PersistentSubscriptionGroup = "myselflog-processors";
        private readonly Dictionary<string, Func<string[], LogBase>> _deserialisers;

        public EndPoint(IDomainRepository domainRepository, IEventStoreConnection connection)
        {
            _deserialisers = CreateDeserialisersMapping();
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
                Process(e.EventType, metadataJson, eventJson);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    ex.GetBaseException().Message);
            }
            return Task.CompletedTask;
        }

        private void Process(string eventType, string metadataJson, string eventJson)
        {
            var logBase = _deserialisers[eventType](new[] {metadataJson, eventJson});
            Diary aggregate;
            try
            {
                aggregate = _domainRepository.GetById<Diary>(logBase.GetMetadata()["$correlationId"], 5);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = Diary.Create(logBase.BuildCreateDiary());
            }
            aggregate.LogValue(logBase.BuildLogValue());
            aggregate.LogTerapy(logBase.BuildLogTerapy());
            aggregate.LogFood(logBase.BuildLogFood());
            _domainRepository.Save(aggregate);

            Log.Debug($"'{eventType}' handled with CorrelationId '{aggregate.AggregateId}'");
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            Log.Error(subscriptionDropReason.ToString(), arg3);
        }

        private static Dictionary<string, Func<string[], LogBase>> CreateDeserialisersMapping()
        {
            return new Dictionary<string, Func<string[], LogBase>>
            {
                {"MySelfLogValueReceived", ToLogFromJson},
                {"OldCaloriesReceived", ToCaloriesFromOldDiaryJson},
                {"OldTerapyReceived", ToImportTerapyFromOldDiary},
                {"OldGlucoseValueReceived", ToGlucoseValueFromOldDiaryJson},
            };
        }

        private static LogBase ToImportTerapyFromOldDiary(string[] arg)
        {
            return new ImportTerapyFromOldDiary(arg[1], arg[0]);
        }
        private static LogBase ToCaloriesFromOldDiaryJson(string[] arg)
        {
            return new ImportCaloriesFromOldDiary(arg[1], arg[0]);
        }
        private static LogBase ToGlucoseValueFromOldDiaryJson(string[] arg)
        {
            return new ImportGlucoseValueFromOldDiary(arg[1], arg[0]);
        }
        private static LogBase ToLogFromJson(string[] arg)
        {
            return new LogFromJson(arg[1], arg[0]);
        }
    }
}
