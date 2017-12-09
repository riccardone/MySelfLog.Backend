using System;
using MySelfLog.Domain.Commands;

namespace MySelfLog.Adapter.Mappings
{
    public class LogBase : ICommandFactory
    {
        public int Value { get; protected set; }
        public int MmolValue { get; protected set; }
        public int SlowTerapy { get; protected set; }
        public int FastTerapy { get; protected set; }
        public int Calories { get; protected set; }
        public string Comment { get; protected set; }
        public string CorrelationId { get; protected set; }
        public string Source { get; protected set; }
        public DateTime Applies { get; protected set; }
        public string Reverses { get; protected set; }

        public CreateDiary BuildCreateDiary()
        {
            return new CreateDiary(CorrelationId, string.Empty, string.Empty);
        }

        public LogFood BuildLogFood()
        {
            return new LogFood(CorrelationId, Comment, Applies, Calories, string.Empty);
        }

        public LogTerapy BuildLogTerapy()
        {
            return new LogTerapy(CorrelationId, Comment, Applies, SlowTerapy, FastTerapy);
        }

        public LogValue BuildLogValue()
        {
            return new LogValue(CorrelationId, Value, MmolValue, Comment, Applies);
        }
    }
}
