using System;
using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class FoodLogged : Event
    {
        public string Message { get; }
        public DateTime LogDate { get; }
        public int Calories { get; }
        public string FoodTypes { get; }
        public IDictionary<string, string> Metadata { get; }
        public FoodLogged(int calories, string foodTypes, string message, IDictionary<string, string> metadata)
        {
            Calories = calories;
            FoodTypes = foodTypes;
            Message = message;
            Metadata = metadata;
        }
    }
}
