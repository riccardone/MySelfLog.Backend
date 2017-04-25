using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreated : Event
    {
        public Guid CorrelationId { get; }

        public DiaryCreated(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
