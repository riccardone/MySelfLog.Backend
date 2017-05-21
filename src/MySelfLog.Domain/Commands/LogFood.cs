using System;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogFood : Command
    {
        public Guid CorrelationId { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public int Calories { get; }
        public string FoodTypes { get; }

        public LogFood(Guid correlationId, string message, DateTime logDate, int calories, string foodTypes)
        {
            CorrelationId = correlationId;
            Message = message;
            LogDate = logDate;
            Calories = calories;
            FoodTypes = foodTypes;
        }
    }
}
