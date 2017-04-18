using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Messages
{
    public class LogValue : Command
    {
        public Guid CorrelationId { get; }
        public int Value { get; }
        public string Message { get; }

        public LogValue(Guid correlationId, int value, string message)
        {
            CorrelationId = correlationId;
            Value = value;
            Message = message;
        }
    }
}
