using Evento;
using MySelfLog.Adapter.Mappings;
using MySelfLog.Domain.Aggregates;
using MySelfLog.Domain.Commands;

namespace MySelfLog.Adapter
{
    public class Handlers :
        IHandle<CreateDiary>,
        IHandle<ChangeDiaryName>,
        IHandle<Log>
    {
        private readonly IDomainRepository _repository;

        public Handlers(IDomainRepository repository)
        {
            _repository = repository;
        }

        public IAggregate Handle(CreateDiary command)
        {
            Diary aggregate;
            try
            {
                return _repository.GetById<Diary>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = Diary.Create(command);
            }
            return aggregate;
        }

        public IAggregate Handle(ChangeDiaryName command)
        {
            var aggregate = _repository.GetById<Diary>(command.Metadata["$correlationId"]);
            aggregate.ChangeDiaryName(command);
            return aggregate;
        }

        public IAggregate Handle(Log command)
        {
            var logFactory = new LogBase(command.Value, command.MmolValue, command.SlowTerapy, command.FastTerapy,
                command.Calories, command.Comment, command.Metadata);
            var aggregate = _repository.GetById<Diary>(command.Metadata["$correlationId"]);
            aggregate.LogValue(logFactory.BuildLogValue());
            aggregate.LogTerapy(logFactory.BuildLogTerapy());
            aggregate.LogFood(logFactory.BuildLogFood());
            return aggregate;
        }
    }
}
