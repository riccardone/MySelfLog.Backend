using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Events
{
    public class GlucoseLoggedV1 : Event
    {
        public int Value { get; }
        public decimal MmolValue { get; }
        public string Message { get; }
        public IDictionary<string, string> Metadata { get; }
        public GlucoseLoggedV1(int value, decimal mmolValue, string message, IDictionary<string, string> metadata)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            Metadata = metadata;
        }
    }
}
