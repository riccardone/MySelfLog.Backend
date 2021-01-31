using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evento;
using MySelfLog.Contracts;
using NLog;

namespace MySelfLog.Adapter
{
    public class Worker
    {
        private readonly ILogger Log;
        private readonly Dictionary<string, IDomainRepository> _domainRepositories;
        private readonly Dictionary<string, Func<CloudEventRequest, Command>> _deserializers = CreateDeserializersMapping();
        
        public Worker(Dictionary<string, IDomainRepository> domainRepositories, LogFactory logFactory)
        {
            _domainRepositories = domainRepositories;
            Log = logFactory.GetCurrentClassLogger();
        }

        public void Process(CloudEventRequest cloudRequest)
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
                    //case ACommand aCommand:
                    //    aggregate = Handle(aCommand);
                    //    break;
                   // ...
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

        private string HandleFailedEvent(Event failedEvent, Command command)
        {
            var errMessage = !failedEvent.Metadata.ContainsKey("error")
                ? $"Error while submitting a '{command.Metadata["source"]}' policy (no error message has been set in command metadata)"
                : $"Error while submitting a '{command.Metadata["source"]}' policy: {failedEvent.Metadata["error"]}";
            var errStack = !failedEvent.Metadata.ContainsKey("error-stack")
                ? string.Empty
                : $"StackTrace: {failedEvent.Metadata["error-stack"]}";
            var err = $"{errMessage} - {errStack}";
            var policyNumber = failedEvent.Metadata.ContainsKey("policyNumber")
                ? failedEvent.Metadata["policyNumber"]
                : "undefined";
            var correlationId = failedEvent.Metadata.ContainsKey("$correlationId")
                ? failedEvent.Metadata["$correlationId"]
                : "undefined";
            var errForLogging =
                failedEvent.Metadata.ContainsKey("error") ? failedEvent.Metadata["error"] : "undefined";
            var msgToLog = $"policyNumber:'{policyNumber}';CorrelationId:'{correlationId}';{errForLogging}";
            Log.Error(msgToLog);
            return err;
        }

        private static Dictionary<string, Func<CloudEventRequest, Command>> CreateDeserializersMapping()
        {
            throw new NotImplementedException();
            //// TODO make this automatic loading all the available mappers using reflection
            //var aMapper = new AMapper();
            //var deserialisers = new Dictionary<string, Func<CloudEventRequest, Command>>
            //{
            //    {aMapper.Schema.ToString(), aMapper.Map}}
            //};
            //return deserialisers;
        }

        //public IAggregate Handle(ACommand command)
        //{
        //    AnAggregate aggregate;

        //    try
        //    {
        //        aggregate = _domainRepositories[command.Metadata["source"]]
        //            .GetById<AnAggregate>(command.Metadata["$correlationId"]);
        //    }
        //    catch (AggregateNotFoundException)
        //    {
        //        aggregate = AnAggregate.Create();
        //    }

        //    aggregate.DoSomething(command).Wait();

        //    return aggregate;
        //}
    }
}
