﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Evento;
using EventStore.ClientAPI;
using log4net;
using MySelfLog.Adapter.Mappings;
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
        private readonly Dictionary<string, Func<string[], Command>> _deserialisers;
        private readonly Dictionary<string, Func<object, IAggregate>> _eventHandlerMapping;
        private readonly Handlers _handlers;

        public EndPoint(IDomainRepository domainRepository, IEventStoreConnection connection, Handlers handlers)
        {
            _deserialisers = CreateDeserialisersMapping();
            _eventHandlerMapping = CreateEventHandlerMapping();
            _domainRepository = domainRepository;
            _connection = connection;
            _handlers = handlers;
        }

        public bool Start()
        {
            try
            {
                Subscribe().Wait();
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
        private async Task Subscribe()
        {
            await _connection.ConnectToPersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup, EventAppeared, SubscriptionDropped);
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, ResolvedEvent resolvedEvent)
        {
            try
            {
                Process(resolvedEvent.Event.EventType, resolvedEvent.Event.Metadata, resolvedEvent.Event.Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    ex.GetBaseException().Message);
            }
            return Task.CompletedTask;
        }

        private void Process(string eventType, byte[] metadata, byte[] data)
        {
            if (!_deserialisers.ContainsKey(eventType))
                return;

            var command = _deserialisers[eventType](new[]
            {
                Encoding.UTF8.GetString(metadata),
                Encoding.UTF8.GetString(data)
            });

            if (command == null)
            {
                Log.Error($"Message format not recognised! EventType: {eventType}");
                return;
            }

            foreach (var key in _eventHandlerMapping.Keys)
            {
                if (!eventType.EndsWith(key))
                    continue;
                var aggregate = _eventHandlerMapping[key](command);
                _domainRepository.Save(aggregate);
                Log.Debug($"Handled '{eventType}' AggregateId: {aggregate.AggregateId}");
                return;
            }
            throw new Exception($"I can't find any handler for {eventType}");
        }

        private static void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            Log.Error(subscriptionDropReason.ToString(), arg3);
        }

        private static Dictionary<string, Func<string[], Command>> CreateDeserialisersMapping()
        {
            return new Dictionary<string, Func<string[], Command>>
            {
                {"CreateDiary", ToCreateDiaryFromJson},
                {"ChangeDiaryName", ToChangeDiaryNameFromJson},
                {"LogReceived", ToLogFromJson},
                {"OldCaloriesReceived", ToCaloriesFromOldDiaryJson},
                {"OldTerapyReceived", ToImportTerapyFromOldDiary},
                {"OldGlucoseValueReceived", ToGlucoseValueFromOldDiaryJson},
                {"DiaryEventReceived", ToLogFromJson},
            };
        }
        private Dictionary<string, Func<object, IAggregate>> CreateEventHandlerMapping()
        {
            return new Dictionary<string, Func<object, IAggregate>>
            {
                {"CreateDiary", o => _handlers.Handle(o as CreateDiary)},
                {"LogReceived", o => _handlers.Handle(o as Log)},
                {"ChangeDiaryName", o => _handlers.Handle(o as ChangeDiaryName)},
                {"OldCaloriesReceived", o => _handlers.Handle(o as Log)},
                {"OldTerapyReceived", o => _handlers.Handle(o as Log)},
                {"OldGlucoseValueReceived", o => _handlers.Handle(o as Log)},
            };
        }

        private static Command ToImportTerapyFromOldDiary(string[] arg)
        {
            return new ImportTerapyFromOldDiary(arg[1], arg[0]).BuildLog();
        }
        private static Command ToCaloriesFromOldDiaryJson(string[] arg)
        {
            return new ImportCaloriesFromOldDiary(arg[1], arg[0]).BuildLog();
        }
        private static Command ToGlucoseValueFromOldDiaryJson(string[] arg)
        {
            return new ImportGlucoseValueFromOldDiary(arg[1], arg[0]).BuildLog();
        }
        private static Command ToLogFromJson(string[] arg)
        {
            return new LogFromJson(arg[1], arg[0]).BuildLog();
        }
        private static Command ToCreateDiaryFromJson(string[] arg)
        {
            return new CreateDiaryFromJson(arg[1], arg[0]).BuildCreateDiary();
        }
        private static Command ToChangeDiaryNameFromJson(string[] arg)
        {
            return new ChangeDiaryNameFromJson(arg[1], arg[0]).BuildChangeDiaryNameValue();
        }
    }
}
