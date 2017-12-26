using System;
using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class FoodLoggedV1 : Event
    {
        public string Message { get; }
        public int Calories { get; }
        public string FoodTypes { get; }
        public IDictionary<string, string> Metadata { get; }
        public FoodLoggedV1(int calories, string foodTypes, string message, IDictionary<string, string> metadata)
        {
            Calories = calories;
            FoodTypes = foodTypes;
            Message = message;
            Metadata = metadata;
        }
    }
}
