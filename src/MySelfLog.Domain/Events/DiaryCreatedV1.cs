using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreatedV1 : Event
    {
        public string SecurityLink { get; }
        public IDictionary<string, string> Metadata { get; }
        public DiaryCreatedV1(string securityLink, IDictionary<string, string> metadata)
        {
            SecurityLink = securityLink;
            Metadata = metadata;
        }
    }
}
