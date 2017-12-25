using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class FoodLoggedV1 : Event
    {
        public string Message { get; }
        public DateTime LogDate { get; }
        public int Calories { get; }
        public string FoodTypes { get; }
        public FoodLoggedV1(int calories, string foodTypes, string message, DateTime logDate)
        {
            Calories = calories;
            FoodTypes = foodTypes;
            Message = message;
            LogDate = logDate;
        }
    }
}
