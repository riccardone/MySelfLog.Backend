using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreated : Event
    {
        public Guid CorrelationId { get; }
        public string SecurityLink { get; }

        public DiaryCreated(Guid correlationId, string securityLink)
        {
            CorrelationId = correlationId;
            SecurityLink = securityLink;
        }
    }
}
