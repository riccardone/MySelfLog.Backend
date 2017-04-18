using System;
using EventStore.Tools.Infrastructure;
using MySelfLog.Domain.Events;
using MySelfLog.Messages;

namespace MySelfLog.Domain.Aggregates
{
    public class Diary : AggregateBase
    {
        public override string AggregateId => CorrelationId.ToString();
        private string Name { get; set; }
        private Guid CorrelationId { get; set; }
        private Guid ProfileId { get; set; }

        public Diary()
        {
            RegisterTransition<DiaryCreated>(Apply);
        }

        private void Apply(DiaryCreated obj)
        {
            CorrelationId = obj.CorrelationId;
            Name = obj.Name;
            ProfileId = obj.ProfileId;
        }

        public Diary(Guid correlationId, string name, Guid profileId)
        {
            RaiseEvent(new DiaryCreated(correlationId, name, profileId));
        }

        public static Diary Create(CreateDiary createDiary)
        {
            // TODO validate
            return new Diary(createDiary.CorrelationId, createDiary.Name, createDiary.ProfileId);
        }
    }
}
