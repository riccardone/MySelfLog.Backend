using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class GlucoseLoggedV2 : EventV2
    {
        public int Value { get; }
        public decimal MmolValue { get; }
        public string Message { get; }
        public IDictionary<string, string> Metadata { get; }
        public GlucoseLoggedV2(int value, decimal mmolValue, string message, IDictionary<string, string> metadata)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            Metadata = metadata;
        }
    }
}
