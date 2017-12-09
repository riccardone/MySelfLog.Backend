using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreated : Event
    {
        public string CorrelationId { get; }
        public string SecurityLink { get; }

        public DiaryCreated(string correlationId, string securityLink)
        {
            CorrelationId = correlationId;
            SecurityLink = securityLink;
        }
    }
}
