using System;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class LogTerapy : Command
    {
        public Guid CorrelationId { get; }
        public string Message { get; }
        public DateTime LogDate { get; }
        public int TerapyValue { get; }
        public bool IsSlow { get; }

        public LogTerapy(Guid correlationId, string message, DateTime logDate, int terapyValue, bool isSlow)
        {
            CorrelationId = correlationId;
            Message = message;
            LogDate = logDate;
            TerapyValue = terapyValue;
            IsSlow = isSlow;
        }
    }
}
