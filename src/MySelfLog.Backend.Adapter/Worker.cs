using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudEventData;
using Evento;
using MySelfLog.Backend.Domain;
using MySelfLog.Backend.Domain.Aggregates;
using MySelfLog.Backend.Domain.Commands;
using NLog;

namespace MySelfLog.Backend.Adapter
{
    public class Worker :
        IHandle<CreateDiary>,
        IHandle<Log>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, IDomainRepository> _domainRepositories;
        private readonly IDataReader _dataReader;
        private readonly Dictionary<string, Func<CloudEventRequest, Command>> _deserializers = CreateDeserializersMapping();
        private IMessageReceiver _messageReceiver;
        private readonly Settings _settings;
        private int _maxLengthForLogs = 255;
        private Dictionary<string, Func<CloudEventRequest, Command>> _deserialisers;

        public Worker(Dictionary<string, IDomainRepository> domainRepositories, IMessageReceiver messageReceiver, Settings settings)
        {
            _domainRepositories = domainRepositories;
            _messageReceiver = messageReceiver;
            _settings = settings;
        }

        public bool Start()
        {
            try
            {
                _deserialisers = CreateDeserialisersMapping();
                _messageReceiver.RegisterMessageHandler(Subscriber);
                return _messageReceiver.Start();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message.ToString());
                return false;
            }
        }

        public void Stop()
        {
            //_messageReceiver.Stop();
        }

        private Task Subscriber(object arg1, CancellationToken arg2)
        {
            try
            {
                if (!(arg1 is CloudEventRequest cloudRequest))
                    return Task.CompletedTask;
                Process(cloudRequest);
            }
            //catch (JsonReaderException jre)
            catch(Exception jre)
            {
                //Log.Error($"Error while parsing the message (LineNumber:{jre.LineNumber};LinePosition:{jre.LinePosition};Path:{jre.Path})");
                Log.Error(jre);
                throw;
            }

            return Task.CompletedTask;
        }

        internal void Process(CloudEventRequest cloudRequest)
        {
            if (!_deserializers.ContainsKey(cloudRequest.DataSchema.ToString()) &&
                !_deserializers.ContainsKey($"{cloudRequest.DataSchema}{cloudRequest.Source}"))
                throw new Exception(
                    $"I can't find a mapper for schema:'{cloudRequest.DataSchema}' source:''{cloudRequest.Source}''");

            var command = _deserializers.ContainsKey(cloudRequest.DataSchema.ToString())
                ? _deserializers[cloudRequest.DataSchema.ToString()](cloudRequest)
                : _deserializers[$"{cloudRequest.DataSchema}{cloudRequest.Source}"](cloudRequest);

            if (command == null)
                throw new Exception(
                    $"I received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' but I was unable to deserialize a Command out of it");

            IAggregate aggregate = null;
            try
            {
                switch (command)
                {
                    case CreateDiary createDiary:
                        aggregate = Handle(createDiary);
                        break;
                    case Log log:
                        aggregate = Handle(log);
                        break;
                }

                // Add here any further command matches

                if (aggregate == null)
                    throw new Exception(
                        $"Received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' but I can't find an available handler for it");
            }
            finally
            {
                if (aggregate != null && aggregate.UncommitedEvents().Any())
                {
                    var uncommittedEventsList = aggregate.UncommitedEvents().ToList();
                    _domainRepositories[command.Metadata["source"]].Save(aggregate);

                    var error = new StringBuilder();
                    foreach (var uncommittedEvent in uncommittedEventsList)
                    {
                        Log.Info($"Handled '{cloudRequest.Type}' AggregateId:'{aggregate.AggregateId}' [0]Resulted event:'{uncommittedEvent.GetType()}'");

                        if (uncommittedEvent.GetType().ToString().EndsWith("FailedV1"))
                        {
                            error.Append(HandleFailedEvent(uncommittedEvent, command));

                        }
                    }

                    if (error.Length > 0)
                    {
                        throw new BusinessException(error.ToString());
                    }
                }
                else
                    Log.Info(
                        $"Handled CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' with no events to save");
            }
        }

        private static Dictionary<string, Func<CloudEventRequest, Command>> CreateDeserialisersMapping()
        {
            // TODO make this automatic loading all the available mappers using reflection
            var createDiaryMapper = new CreateDiaryMapper();
            var logMapper = new LogMapper();
            
            var deserialisers = new Dictionary<string, Func<CloudEventRequest, Command>>
            {
                {createDiaryMapper.Schema.ToString(), createDiaryMapper.Map},
                {logMapper.Schema.ToString(), logMapper.Map}
            };
            return deserialisers;
        }

        private string HandleFailedEvent(Event failedEvent, Command command)
        {
            var errMessage = string.Empty;
            var errForLogging = string.Empty;
           
            var errStack = !failedEvent.Metadata.ContainsKey("error-stack")
                ? string.Empty
                : $"StackTrace: {failedEvent.Metadata["error-stack"]}";
            var err = $"{errMessage} - {errStack}";
            var correlationId = failedEvent.Metadata.ContainsKey("$correlationId")
                ? failedEvent.Metadata["$correlationId"]
                : "undefined";
             
            var msgToLog = $"CorrelationId:'{correlationId}';{errForLogging}";
            Log.Error(TruncateFieldIfNecessary(msgToLog));
            return err;
        }

        private static Dictionary<string, Func<CloudEventRequest, Command>> CreateDeserializersMapping()
        {
            // TODO make this automatic loading all the available mappers using reflection
            var createDiaryMapper = new CreateDiaryMapper();
            var logMapper = new LogMapper();
            var deserialisers = new Dictionary<string, Func<CloudEventRequest, Command>>
            {
                {createDiaryMapper.Schema.ToString(), createDiaryMapper.Map},
                {logMapper.Schema.ToString(), logMapper.Map}
            };
            return deserialisers;
        }

        private string TruncateFieldIfNecessary(string field)
        {
            return field.Length > _maxLengthForLogs ? field.Substring(0, _maxLengthForLogs) : field;
        }

        public IAggregate Handle(CreateDiary command)
        {
            Diary aggregate;

            try
            {
                aggregate = _domainRepositories[command.Metadata["source"]]
                    .GetById<Diary>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = Diary.Create(command);
            }

            return aggregate;
        }

        public IAggregate Handle(Log command)
        {
            Diary aggregate;

            try
            {
                aggregate = _domainRepositories[command.Metadata["source"]]
                    .GetById<Diary>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                throw new BusinessException("Diary not found");
            }

            if (command.Calories > 0)
                aggregate.LogFood(new LogFood(command.Comment, command.Calories, string.Empty, command.Metadata));

            if (command.FastTerapy > 0 || command.SlowTerapy > 0)
                aggregate.LogTerapy(new LogTerapy(command.Comment, command.SlowTerapy, command.FastTerapy, command.Metadata));

            if (command.Value > 0)
                aggregate.LogValue(new LogValue(command.Value, command.MmolValue, command.Comment, command.Metadata));

            return aggregate;
        }
    }
}
