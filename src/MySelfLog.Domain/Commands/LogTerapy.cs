using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogTerapy : CommandV2
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
