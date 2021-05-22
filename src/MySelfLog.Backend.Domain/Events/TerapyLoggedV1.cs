using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Events
{
    public class TerapyLoggedV1 : Event
    {
        public int Value { get; }
        public string Message { get; }
        public bool IsSlow { get; }
        public IDictionary<string, string> Metadata { get; }
        public TerapyLoggedV1(int value, string message, bool isSlow, IDictionary<string, string> metadata)
        {
            Value = value;
            Message = message;
            IsSlow = isSlow;
            Metadata = metadata;
        }
    }
}
