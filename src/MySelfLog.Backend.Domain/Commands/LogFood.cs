﻿using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Commands
{
    public class LogFood : Command
    {
        public string Message { get; }
        public int Calories { get; }
        public string FoodTypes { get; }
        public IDictionary<string, string> Metadata { get; }
        public LogFood(string message, int calories, string foodTypes, IDictionary<string, string> metadata)
        {
            Message = message;
            Calories = calories;
            FoodTypes = foodTypes;
            Metadata = metadata;
        }
    }
}
