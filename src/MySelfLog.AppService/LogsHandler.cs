using System;
using EventStore.Tools.Infrastructure;
using MySelfLog.Domain.Aggregates;
using MySelfLog.Messages;

namespace MySelfLog.AppService
{
    public class LogsHandler : IHandle<LogValue>
    {
        private readonly IDomainRepository _repository;

        public LogsHandler(IDomainRepository repository)
        {
            _repository = repository;
        }

        public IAggregate Handle(LogValue command)
        {
            DiaryAggregate aggregate;
            try
            {
                aggregate = _repository.GetById<DiaryAggregate>(command.CorrelationId.ToString());
            }
            catch (AggregateNotFoundException)
            {
                aggregate = DiaryAggregate.Create(command.CorrelationId);
            }
            aggregate.LogValue(command);
            return aggregate;
        }
    }
}
