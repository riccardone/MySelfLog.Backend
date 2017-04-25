using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Domain.Events
{
    public class GlucoseLogged : Event
    {
        public int Value { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public string SecurityLink { get; }

        public GlucoseLogged(int value, string message, DateTime logDate, string securityLink)
        {
            Value = value;
            Message = message;
            LogDate = logDate;
            SecurityLink = securityLink;
        }
    }
}
