using Evento;
using MySelfLog.Domain.Aggregates;
using MySelfLog.Domain.Commands;

namespace MySelfLog.AppService
{
    public class LogsHandler : 
        IHandle<LogValue>, 
        IHandle<CreateDiary>,
        IHandle<LogFood>,
        IHandle<LogTerapy>
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

        public IAggregate Handle(LogFood command)
        {
            var aggregate = _repository.GetById<DiaryAggregate>(command.CorrelationId.ToString());
            aggregate.LogFood(command);
            return aggregate;
        }

        public IAggregate Handle(LogTerapy command)
        {
            var aggregate = _repository.GetById<DiaryAggregate>(command.CorrelationId.ToString());
            aggregate.LogTerapy(command);
            return aggregate;
        }

        //public IAggregate Handle(LogDto command)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
