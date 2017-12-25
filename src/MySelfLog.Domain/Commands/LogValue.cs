using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogValue : CommandV2
    {
        public string Message { get; }
        public int Value { get; }
        public decimal MmolValue { get; }
        public IDictionary<string, string> Metadata { get; }
        public LogValue(int value, decimal mmolValue, string message, IDictionary<string, string> metadata)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            Metadata = metadata;
        }
    }
}
