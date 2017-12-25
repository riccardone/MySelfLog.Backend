using Evento;

namespace MySelfLog.Domain.Events
{
    public class SecurityLinkChangedV1 : Event
    {
        public string SecurityLink { get; }
        public SecurityLinkChangedV1(string securityLink)
        {
            SecurityLink = securityLink;
        }
    }
}
