using System;
using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class TerapyLogged : Event
    {
        public int Value { get; }
        public string Message { get; }
        public bool IsSlow { get; }
        public IDictionary<string, string> Metadata { get; }
        public TerapyLogged(int value, string message, bool isSlow, IDictionary<string, string> metadata)
        {
            Value = value;
            Message = message;
            IsSlow = isSlow;
            Metadata = metadata;
        }
    }
}
