using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class FoodLogged : Event
    {
        public string Message { get; }
        public DateTime LogDate { get; }
        public int Calories { get; }
        public string FoodTypes { get; }

        public FoodLogged(int calories, string foodTypes, string message, DateTime logDate)
        {
            Calories = calories;
            FoodTypes = foodTypes;
            Message = message;
            LogDate = logDate;
        }
    }
}
