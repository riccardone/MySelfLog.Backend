using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class GlucoseLoggedV1 : Event
    {
        public int Value { get; }
        public decimal MmolValue { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public GlucoseLoggedV1(int value, decimal mmolValue, string message, DateTime logDate)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            LogDate = logDate;
        }
    }
}
