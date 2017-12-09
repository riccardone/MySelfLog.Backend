using System;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogValue : Command
    {
        public string CorrelationId { get; }
        public string Message { get; }
        public int Value { get; }
        public decimal MmolValue { get; }
        public DateTime LogDate { get; }

        public LogValue(string correlationId, int value, decimal mmolValue, string message, DateTime logDate)
        {
            CorrelationId = correlationId;
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            LogDate = logDate;
        }
    }
}
