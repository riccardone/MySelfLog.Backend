using System;
using EventStore.Tools.Infrastructure;
using MySelfLog.Domain.Aggregates;
using MySelfLog.Messages;

namespace MySelfLog.AppService
{
    public class LogsHandler : IHandle<LogValue>, IHandle<CreateDiary>
    {
        private readonly IDomainRepository _repository;

        public LogsHandler(IDomainRepository repository)
        {
            _repository = repository;
        }

        public IAggregate Handle(LogValue command)
        {
            var aggregate = _repository.GetById<DiaryAggregate>(command.CorrelationId.ToString());
            aggregate.LogValue(command);
            return aggregate;
        }

        public IAggregate Handle(CreateDiary command)
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
            return aggregate;
        }
    }
}
