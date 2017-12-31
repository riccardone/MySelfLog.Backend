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
        public DateTime? Applies { get; protected set; }
        public string Reverses { get; protected set; }

        public CreateDiary BuildCreateDiary()
        {
            return new CreateDiary(string.Empty, string.Empty, GetMetadata());
        }

        public LogFood BuildLogFood()
        {
            return new LogFood(Comment, Calories, string.Empty, GetMetadata());
        }

        public LogTerapy BuildLogTerapy()
        {
            return new LogTerapy(Comment, SlowTerapy, FastTerapy, GetMetadata());
        }

        public LogValue BuildLogValue()
        {
            return new LogValue(Value, MmolValue, Comment, GetMetadata());
        }
        public IDictionary<string, string> GetMetadata()
        {
            var metadata = new Dictionary<string, string>
            {
                {"$correlationId", CorrelationId}
            };

            if (Applies.HasValue)
                metadata.Add("Applies", Applies.Value.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(Reverses))
                metadata.Add("Reverses", Reverses);

            if (!string.IsNullOrWhiteSpace(Source))
                metadata.Add("Source", Source);

            return metadata;
        }
    }
}
