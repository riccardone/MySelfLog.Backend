using System;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogValue : Command
    {
        public Guid CorrelationId { get; }
        public string Message { get; }
        public int Value { get; }
        public DateTime LogDate { get; }

        public LogValue(Guid correlationId, int value, string message, DateTime logDate)
        {
            CorrelationId = correlationId;
            Value = value;
            Message = message;
            LogDate = logDate;
        }
    }
}
