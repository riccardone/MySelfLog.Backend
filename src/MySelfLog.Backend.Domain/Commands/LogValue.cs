using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Commands
{
    public class LogValue : Command
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
