using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreated : Event
    {
        public Guid CorrelationId { get; }
        public string Name { get; }
        public Guid ProfileId { get; }

        public DiaryCreated(Guid correlationId, string name, Guid profileId)
        {
            CorrelationId = correlationId;
            Name = name;
            ProfileId = profileId;
        }
    }
}
