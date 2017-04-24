using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Domain.Events
{
    public class FoodLogged : Event
    {
        public string Message { get; }
        public DateTime LogDate { get; }
        public string SecurityLink { get; set; }
        public int Calories { get; }
        public string FoodTypes { get; }

        public FoodLogged(int calories, string foodTypes, string message, DateTime logDate, string securityLink)
        {
            Calories = calories;
            FoodTypes = foodTypes;
            Message = message;
            LogDate = logDate;
            SecurityLink = securityLink;
        }
    }
}
