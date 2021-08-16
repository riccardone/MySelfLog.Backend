using System;
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
        private readonly IConnectionBuilder _connectionBuilder;
        private readonly string _inputStream;
        private IEventStoreConnection _connection;
        private readonly string _persistentSubscriptionGroup;
        private List<Func<object, CancellationToken, Task>> _handlers;

        public MessageReceiverFromEventStore(IConnectionBuilder subscriberBuilder, string inputStream, string persistenSubscriptionGroupName)
        {
            _connectionBuilder = subscriberBuilder;
            _inputStream = inputStream; //$"{inputStream}-{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}";
            _persistentSubscriptionGroup = persistenSubscriptionGroupName;
        }
        public bool Start()
        {
            _connection?.Close();
            _connection = _connectionBuilder.Build(false);
            _connection.Connected += _connection_Connected;
            _connection.Disconnected += _connection_Disconnected;
            _connection.ErrorOccurred += _connection_ErrorOccurred;
            _connection.Closed += _connection_Closed;
            _connection.Reconnecting += _connection_Reconnecting;
            _connection.AuthenticationFailed += _connection_AuthenticationFailed;
            Log.Info("Connecting to EventStore...");
            _connection.ConnectAsync().Wait();
            Log.Info($"Listening from '{_inputStream}' stream");
            Log.Info($"Joined '{_persistentSubscriptionGroup}' group");
            Log.Info($"EndPoint started");
            return true;
        }

        public void Stop()
        {
            _connection.ErrorOccurred -= _connection_ErrorOccurred;
            _connection.Disconnected -= _connection_Disconnected;
            _connection.AuthenticationFailed -= _connection_AuthenticationFailed;
            _connection.Connected -= _connection_Connected;
            _connection.Reconnecting -= _connection_Reconnecting;

            _connection?.Close();
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
            await _connection.CreatePersistentSubscriptionAsync(_inputStream, _persistentSubscriptionGroup,
                PersistentSubscriptionSettings.Create().StartFromBeginning().DoNotResolveLinkTos(),
                _connectionBuilder.Credentials);
        }

        private async Task Subscribe()
        {
            await _connection.ConnectToPersistentSubscriptionAsync(_inputStream, _persistentSubscriptionGroup,
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
            try
            {
                CreateSubscription().Wait();
            }
            catch (Exception)
            {
                // already exist
            }
            Subscribe().Wait();
        }

        private static void _connection_ErrorOccurred(object sender, ClientErrorEventArgs e)
        {
            Log.Error($"{nameof(MessageReceiverFromEventStore)} ErrorOccurred: {e.Exception.Message}");
        }

        private static void _connection_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            Log.Error($"{nameof(MessageReceiverFromEventStore)} Disconnected from {e.RemoteEndPoint}");
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
