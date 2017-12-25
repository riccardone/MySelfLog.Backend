using System;
using System.Collections.Generic;
using System.Globalization;
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
            return new CreateDiary(string.Empty, string.Empty, GetMetadata());
        }

        public LogFood BuildLogFood()
        {
            return new LogFood(Comment, Calories, string.Empty, GetMetadata());
        }

        private IDictionary<string, string> GetMetadata()
        {
            return new Dictionary<string, string>
            {
                {"CorrelationId", CorrelationId},
                {"Applies", Applies.ToString(CultureInfo.InvariantCulture)},
                {"Reverses", Reverses}
            };
        }

        public LogTerapy BuildLogTerapy()
        {
            return new LogTerapy(Comment, SlowTerapy, FastTerapy, GetMetadata());
        }

        public LogValue BuildLogValue()
        {
            return new LogValue(Value, MmolValue, Comment, GetMetadata());
        }
    }
}
