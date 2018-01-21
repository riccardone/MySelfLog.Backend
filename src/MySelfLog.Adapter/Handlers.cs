using System;
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
        private readonly IDiaryCache _diaryCache;

        public Handlers(IDomainRepository repository, IDiaryCache diaryCache)
        {
            _repository = repository;
            _diaryCache = diaryCache;
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
            Diary aggregate;
            try
            {
                aggregate = _repository.GetById<Diary>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                // Build a createDiary command: the diary name must be in the cache! 
                // (otherwise how we end up having this Log command here?)
                var diaryName = _diaryCache.GetDiaries()[command.Metadata["$correlationId"]];
                aggregate = Handle(new CreateDiary(diaryName, command.Metadata)) as Diary;
            }
            if (aggregate == null)
                throw new AggregateNotFoundException($"AggregateId '{command.Metadata["$correlationId"]}' not found");
            var logFactory = new LogBase(command.Value, command.MmolValue, command.SlowTerapy, command.FastTerapy,
                command.Calories, command.Comment, command.Metadata);
            aggregate.LogValue(logFactory.BuildLogValue());
            aggregate.LogTerapy(logFactory.BuildLogTerapy());
            aggregate.LogFood(logFactory.BuildLogFood());
            return aggregate;
        }
    }
}
