using Evento;

namespace MySelfLog.Domain.Events
{
    public class SecurityLinkChanged : Event
    {
        public string SecurityLink { get; }

        public SecurityLinkChanged(string securityLink)
        {
            SecurityLink = securityLink;
        }
    }
}
