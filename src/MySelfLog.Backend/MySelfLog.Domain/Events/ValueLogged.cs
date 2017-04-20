using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Domain.Events
{
    public class ValueLogged : Event
    {
        public int Value { get; }
        public string Message { get; }
        public DateTime LogDate { get; }

        public ValueLogged(int value, string message, DateTime logDate)
        {
            Value = value;
            Message = message;
            LogDate = logDate;
        }
    }
}
