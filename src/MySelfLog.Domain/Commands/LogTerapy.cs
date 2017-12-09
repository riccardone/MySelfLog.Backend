using System;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogTerapy : Command
    {
        public string CorrelationId { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public int SlowTerapy { get; }
        public int FastTerapy { get; }

        public LogTerapy(string correlationId, string message, DateTime logDate, int slowTerapy, int fastTerapy)
        {
            CorrelationId = correlationId;
            Message = message;
            LogDate = logDate;
            SlowTerapy = slowTerapy;
            FastTerapy = fastTerapy;
        }
    }
}
