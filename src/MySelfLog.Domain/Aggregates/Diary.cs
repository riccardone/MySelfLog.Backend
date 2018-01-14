using System.Collections.Generic;
using Evento;
using MySelfLog.Domain.Commands;
using MySelfLog.Domain.Events;

namespace MySelfLog.Domain.Aggregates
{
    public class Diary : AggregateBase
    {
        public override string AggregateId => CorrelationId;
        private string CorrelationId { get; set; }
        private string Name { get; set; }
        public Diary()
        {
            RegisterTransition<DiaryCreatedV1>(Apply);
            RegisterTransition<DiaryNameChangedV1>(Apply);
        }
        public Diary(string name, IDictionary<string, string> metadata) : this()
        {
            RaiseEvent(new DiaryCreatedV1(name, metadata));
        }

        private void Apply(DiaryCreatedV1 obj)
        {
            CorrelationId = obj.Metadata["$correlationId"];
            Name = obj.Name;
        }

        private void Apply(DiaryNameChangedV1 obj)
        {
            Name = obj.Name;
        }
        public static Diary Create(CreateDiary cmd)
        {
            ValidateRequiredMetadata(cmd);

            return new Diary(cmd.Name, cmd.Metadata);
        }

        public void LogValue(LogValue log)
        {
            ValidateRequiredMetadata(log);
            Ensure.Nonnegative(log.Value, nameof(log.Value));

            if (log.Value > 0 || log.MmolValue > 0)
                RaiseEvent(new GlucoseLoggedV1(log.Value, log.MmolValue, log.Message, log.Metadata));
        }

        public void LogTerapy(LogTerapy log)
        {
            ValidateRequiredMetadata(log);
            Ensure.Nonnegative(log.FastTerapy, nameof(log.FastTerapy));
            Ensure.Nonnegative(log.SlowTerapy, nameof(log.SlowTerapy));

            if (log.FastTerapy > 0)
                RaiseEvent(new TerapyLoggedV1(log.FastTerapy, log.Message, false, log.Metadata));
            if (log.SlowTerapy > 0)
                RaiseEvent(new TerapyLoggedV1(log.SlowTerapy, log.Message, true, log.Metadata));
        }

        public void LogFood(LogFood log)
        {
            ValidateRequiredMetadata(log);
            Ensure.Nonnegative(log.Calories, nameof(log.Calories));

            if (log.Calories > 0)
                RaiseEvent(new FoodLoggedV1(log.Calories, log.FoodTypes, log.Message, log.Metadata));
        }

        public void ChangeDiaryName(ChangeDiaryName cmd)
        {
            ValidateRequiredMetadata(cmd);
            Ensure.NotNull(cmd.Name, nameof(cmd.Name));

            if (cmd.Name.Equals(Name))
                return;

            RaiseEvent(new DiaryNameChangedV1(cmd.Name, cmd.Metadata));
        }

        private static void ValidateRequiredMetadata(Message msg)
        {
            Ensure.NotNull(msg.Metadata, nameof(msg.Metadata));
            Ensure.NotNullOrWhiteSpace(msg.Metadata["$correlationId"], "$correlationId");
            Ensure.NotNullOrWhiteSpace(msg.Metadata["Source"], "Source");
            Ensure.NotNullOrWhiteSpace(msg.Metadata["Applies"], "Applies");
        }
    }
}
