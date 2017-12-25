using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class TerapyLoggedV1 : Event
    {
        public int Value { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public bool IsSlow { get; }
        public TerapyLoggedV1(int value, string message, DateTime logDate, bool isSlow)
        {
            Value = value;
            Message = message;
            LogDate = logDate;
            IsSlow = isSlow;
        }
    }
}
