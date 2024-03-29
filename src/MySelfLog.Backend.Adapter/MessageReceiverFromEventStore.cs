﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudEventData;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NLog;

namespace MySelfLog.Backend.Adapter
{
    public class MessageReceiverFromEventStore : IMessageReceiver
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IConnectionBuilder _subscriberBuilder;
        private readonly string _inputStream;
        private IEventStoreConnection _subscriber;
        private readonly string _persistentSubscriptionGroup;
        private List<Func<object, CancellationToken, Task>> _handlers;

        public MessageReceiverFromEventStore(IConnectionBuilder subscriberBuilder, string inputStream, string persistenSubscriptionGroupName)
        {
            _subscriberBuilder = subscriberBuilder;
            _inputStream = inputStream; //$"{inputStream}-{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}";
            _persistentSubscriptionGroup = persistenSubscriptionGroupName;
        }
        public bool Start()
        {
            _subscriber?.Close();
            _subscriber = _subscriberBuilder.Build(false);
            _subscriber.Connected += _connection_Connected;
            _subscriber.Disconnected += _connection_Disconnected;
            _subscriber.ErrorOccurred += _connection_ErrorOccurred;
            _subscriber.Closed += _connection_Closed;
            _subscriber.Reconnecting += _connection_Reconnecting;
            _subscriber.AuthenticationFailed += _connection_AuthenticationFailed;   
            _subscriber.ConnectAsync().Wait();            
            Subscribe().Wait();
            return true;
        }
       
        public void Stop()
        {
            _subscriber.ErrorOccurred -= _connection_ErrorOccurred;
            _subscriber.Disconnected -= _connection_Disconnected;
            _subscriber.AuthenticationFailed -= _connection_AuthenticationFailed;
            _subscriber.Connected -= _connection_Connected;
            _subscriber.Reconnecting -= _connection_Reconnecting;

            _subscriber?.Close();
            Log.Info($"{nameof(EndPoint)} stopped");
        }

        public void RegisterMessageHandler(Func<object, CancellationToken, Task> handler)
        {
            if (_handlers == null)
                _handlers = new List<Func<object, CancellationToken, Task>>();
            _handlers.Add(handler);
        }

        private async Task CreateSubscription()
        {               
            await _subscriber.CreatePersistentSubscriptionAsync(_inputStream, _persistentSubscriptionGroup,
                PersistentSubscriptionSettings.Create().StartFromBeginning().DoNotResolveLinkTos(),
                _subscriberBuilder.Credentials);            
        }

        private async Task Subscribe()
        {           
            CreateSubscription();
            await _subscriber.ConnectToPersistentSubscriptionAsync(_inputStream, _persistentSubscriptionGroup,
                EventAppeared, SubscriptionDropped);
        }

        private static void SubscriptionDropped(
            EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            Log.Error(arg3, subscriptionDropReason.ToString());
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            ResolvedEvent resolvedEvent)
        {
            try
            {
                if (resolvedEvent.OriginalEvent.EventType.StartsWith("$$"))
                    return Task.CompletedTask;
                var data = DeserializeObject<CloudEventRequest>(resolvedEvent.OriginalEvent.Data);
                if (data == null)
                    return Task.CompletedTask;
                foreach (var handler in _handlers)
                {
                    handler(data, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                // any specific log must be catch before this point
                Log.Error($"Error while processing: '{ex.GetBaseException().Message}' StackTrace: {ex.StackTrace}");
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    ex.GetBaseException().Message);
            }

            return Task.CompletedTask;
        }

        public static T DeserializeObject<T>(byte[] data)
        {
            return (T)(DeserializeObject(data, typeof(T).AssemblyQualifiedName));
        }

        public static object DeserializeObject(byte[] data, string typeName)
        {
            try
            {
                var jsonString = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region connectionevents
        private void _connection_Connected(object sender, ClientConnectionEventArgs e)
        {
            Log.Info($"{nameof(MessageReceiverFromEventStore)} Connected to {e.RemoteEndPoint}");           
        }

        private static void _connection_ErrorOccurred(object sender, ClientErrorEventArgs e)
        {
            Log.Error($"{nameof(MessageReceiverFromEventStore)} ErrorOccurred: {e.Exception.Message}");
        }

        private void _connection_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            Log.Error($"{nameof(MessageReceiverFromEventStore)} Disconnected from {e.RemoteEndPoint}");
            // Some black magic is going on with es and this is needed to re-establish the persistent subscription
            Stop();
            Start();
        }
        private void _connection_AuthenticationFailed(object sender, ClientAuthenticationFailedEventArgs e)
        {
            Log.Error($"{nameof(MessageReceiverFromEventStore)} AuthenticationFailed: {e.Reason}");
        }

        private void _connection_Reconnecting(object sender, ClientReconnectingEventArgs e)
        {
            Log.Warn($"{nameof(MessageReceiverFromEventStore)} Reconnecting...");
        }

        private void _connection_Closed(object sender, ClientClosedEventArgs e)
        {
            Log.Info($"{nameof(MessageReceiverFromEventStore)} Closed: {e.Reason}");
        }
        #endregion
    }
}
