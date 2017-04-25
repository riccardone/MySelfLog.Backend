using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Domain.Events
{
    public class TerapyLogged : Event
    {
        public int Value { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public bool IsSlow { get; }
        public string SecurityLink { get; }

        public TerapyLogged(int value, string message, DateTime logDate, bool isSlow, string securityLink)
        {
            Value = value;
            Message = message;
            LogDate = logDate;
            IsSlow = isSlow;
            SecurityLink = securityLink;
        }
    }
}
