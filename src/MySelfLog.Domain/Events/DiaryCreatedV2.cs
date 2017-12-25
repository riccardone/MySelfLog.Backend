using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class DiaryCreatedV2 : EventV2
    {
        public string SecurityLink { get; }
        public IDictionary<string, string> Metadata { get; }
        public DiaryCreatedV2(string securityLink, IDictionary<string, string> metadata)
        {
            SecurityLink = securityLink;
            Metadata = metadata;
        }
    }
}
