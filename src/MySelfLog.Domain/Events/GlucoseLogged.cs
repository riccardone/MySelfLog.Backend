using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class GlucoseLogged : Event
    {
        public int Value { get; }
        public decimal MmolValue { get; }
        public string Message { get; }
        public IDictionary<string, string> Metadata { get; }
        public GlucoseLogged(int value, decimal mmolValue, string message, IDictionary<string, string> metadata)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            Metadata = metadata;
        }
    }
}
