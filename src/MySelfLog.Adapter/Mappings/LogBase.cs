using System.Collections.Generic;
using MySelfLog.Domain.Commands;

namespace MySelfLog.Adapter.Mappings
{
    public class LogBase : ICommandFactory
    {
        public int Value { get; protected set; }
        public decimal MmolValue { get; protected set; }
        public int SlowTerapy { get; protected set; }
        public int FastTerapy { get; protected set; }
        public int Calories { get; protected set; }
        public string Comment { get; protected set; }
        public IDictionary<string, string> Metadata { get; protected set; }
        public string DiaryName { get; protected set; }

        public LogBase()
        {
            
        }

        public LogBase(int value, decimal mmolValue, int slowTerapy, int fastTerapy, int calories, string comment,
            IDictionary<string, string> metadata)
        {
            Value = value;
            MmolValue = mmolValue;
            SlowTerapy = slowTerapy;
            FastTerapy = fastTerapy;
            Calories = calories;
            Comment = comment;
            Metadata = metadata;
        }

        public CreateDiary BuildCreateDiary()
        {
            return new CreateDiary(DiaryName, GetMetadata());
        }

        public Log BuildLog()
        {
            return new Log(Value, MmolValue, SlowTerapy, FastTerapy, Calories, Comment, Metadata);
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

        public ChangeDiaryName BuildChangeDiaryNameValue()
        {
            return new ChangeDiaryName(DiaryName, Metadata);
        }

        private IDictionary<string, string> GetMetadata()
        {
            return Metadata;
        }
    }
}
