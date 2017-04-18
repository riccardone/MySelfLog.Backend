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

        // TODO implement creatediary handling

        public IAggregate Handle(LogValue command)
        {
            var aggregate = _repository.GetById<Diary>(command.CorrelationId.ToString());
            // TODO log the value
            return aggregate;
        }
    }
}
