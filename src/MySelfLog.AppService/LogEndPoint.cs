﻿using System;
using System.Text;
using Evento;
using EventStore.ClientAPI;
using log4net;
using MySelfLog.Domain.Commands;
using Newtonsoft.Json;

namespace MySelfLog.AppService
{
    public class LogEndPoint
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LogEndPoint));
        private readonly IDomainRepository _domainRepository;
        private readonly IEventStoreConnection _connection;
        private readonly LogsHandler _logsHandler;
        private const string InputStream = "diary-input";
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
                Subscribe();
                Log.Info("Log EndPoint started");
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
                // TODO deserialise and use metadata (especially the $correlationId)
                var ciccio = new LogFromJsonWrapper(eventJson);
                //var aggregate = _logsHandler.Handle(JsonConvert.DeserializeObject<LogDto>(eventJson));
                //_domainRepository.Save(aggregate);
                //Log.Info($"'{e.EventType}' handled with CorrelationId '{aggregate.AggregateId}'");
            }
            catch (AggregateNotFoundException exNotFound)
            {
                Log.Error(exNotFound);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    exNotFound.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Unknown,
                    ex.GetBaseException().Message);
            }
        }
        public void Stop()
        {
            _connection.Close();
        }

        // TODO used a command factory
        //switch (e.EventType)
        //{
        //    case "CreateDiary":
        //        aggregate = _logsHandler.Handle(JsonConvert.DeserializeObject<CreateDiary>(eventJson));
        //        break;
        //    case "LogValue":
        //        aggregate = _logsHandler.Handle(JsonConvert.DeserializeObject<LogValue>(eventJson));
        //        break;
        //    case "LogFood":
        //        aggregate = _logsHandler.Handle(JsonConvert.DeserializeObject<LogFood>(eventJson));
        //        break;
        //    case "LogTerapy":
        //        aggregate = _logsHandler.Handle(JsonConvert.DeserializeObject<LogTerapy>(eventJson));
        //        break;
        //    default:
        //        return;
        //}

        #region BoilerplateCode
        private void Subscribe()
        {
            _connection.ConnectToPersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup, EventAppeared, SubscriptionDropped).Wait();
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            Log.Error(subscriptionDropReason.ToString(), arg3);
        }


        #endregion
    }
}
