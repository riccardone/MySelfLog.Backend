using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class Log : Command
    {
        public Log(int value, int mmolValue, int slowTerapy, int fastTerapy, int calories, string comment,
            IDictionary<string, string> metadata)
        {
            Value = value;
            MmolValue = mmolValue;
            SlowTerapy = slowTerapy;
            FastTerapy = fastTerapy;
            Calories = calories;
            Comment = comment;
            Metadata = metadata;
        }

        public int Value { get; }
        public int MmolValue { get; }
        public int SlowTerapy { get; }
        public int FastTerapy { get; }
        public int Calories { get; }
        public string Comment { get; }
        public IDictionary<string, string> Metadata { get; }
    }
}
