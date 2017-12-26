using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class SecurityLinkChanged : Event
    {
        public string SecurityLink { get; }
        public IDictionary<string, string> Metadata { get; }
        public SecurityLinkChanged(string securityLink, IDictionary<string, string> metadata)
        {
            SecurityLink = securityLink;
            Metadata = metadata;
        }
    }
}
