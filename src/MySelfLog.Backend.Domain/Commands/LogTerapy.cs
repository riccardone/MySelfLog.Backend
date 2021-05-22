using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Commands
{
    public class LogTerapy : Command
    {
        public string Message { get; }
        public int SlowTerapy { get; }
        public int FastTerapy { get; }
        public IDictionary<string, string> Metadata { get; }
        public LogTerapy(string message, int slowTerapy, int fastTerapy, IDictionary<string, string> metadata)
        {
            Message = message;
            SlowTerapy = slowTerapy;
            FastTerapy = fastTerapy;
            Metadata = metadata;
        }
    }
}
