using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreatedV1 : Event
    {
        public string CorrelationId { get; }
        public string SecurityLink { get; }
        public DiaryCreatedV1(string correlationId, string securityLink)
        {
            CorrelationId = correlationId;
            SecurityLink = securityLink;
        }
    }
}
