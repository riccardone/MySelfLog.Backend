using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Messages
{
    public class LogValue : Command
    {
        public Guid CorrelationId { get; }
        public string Message { get; }
        public int Value { get; }
        public DateTime LogDate { get; }
        public string Email { get; }
        public int TerapyValue { get; }
        public bool IsSlow { get; }
        public int Calories { get; }
        public string FoodTypes { get; }

        public LogValue(Guid correlationId, int value, string message, DateTime logDate, string email, int terapyValue,
            bool isSlow, int calories, string foodTypes)
        {
            CorrelationId = correlationId;
            Value = value;
            Message = message;
            LogDate = logDate;
            Email = email;
            TerapyValue = terapyValue;
            IsSlow = isSlow;
            Calories = calories;
            FoodTypes = foodTypes;
        }
    }
}
