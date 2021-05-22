using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Commands
{
    public class Log : Command
    {
        public Log(string correlationId, int value, decimal mmolValue, int slowTerapy, int fastTerapy, int calories, string comment,
            IDictionary<string, string> metadata)
        {
            CorrelationId = correlationId;
            Value = value;
            MmolValue = mmolValue;
            SlowTerapy = slowTerapy;
            FastTerapy = fastTerapy;
            Calories = calories;
            Comment = comment;
            Metadata = metadata;
        }

        public string CorrelationId { get; }
        public int Value { get; }
        public decimal MmolValue { get; }
        public int SlowTerapy { get; }
        public int FastTerapy { get; }
        public int Calories { get; }
        public string Comment { get; }
        public IDictionary<string, string> Metadata { get; internal set; }
    }
}
