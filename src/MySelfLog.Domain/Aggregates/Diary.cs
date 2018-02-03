using System;
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
            Ensure.Nonnegative(log.MmolValue, nameof(log.MmolValue));

            if (log.Value > 0 && log.Value < 20)
                throw new Exception("warning: value is very low");
            if (log.Value < 10)
                throw new Exception("error: value is too low");
            if (log.Value > 800)
                throw new Exception("error: value is too high");
            
            if (log.MmolValue > 0 && log.MmolValue < 2)
                throw new Exception("warning: mmolvalue is very low");
            if (log.MmolValue < 1)
                throw new Exception("error: mmolvalue is too low");
            if (log.MmolValue > 35)
                throw new Exception("error: mmolvalue is too high");

            if (log.Value > 0 || log.MmolValue > 0)
                RaiseEvent(new GlucoseLoggedV1(log.Value, log.MmolValue, log.Message, log.Metadata));
        }

        public void LogTerapy(LogTerapy log)
        {
            ValidateRequiredMetadata(log);
            Ensure.Nonnegative(log.FastTerapy, nameof(log.FastTerapy));
            Ensure.Nonnegative(log.SlowTerapy, nameof(log.SlowTerapy));

            if (log.SlowTerapy > 0)
            {
                if (log.SlowTerapy > 0 && log.SlowTerapy < 3)
                    throw new Exception("warning: slow terapy is very low");
                if (log.SlowTerapy < 2)
                    throw new Exception("error: slow terapy is too low");
                if (log.SlowTerapy > 150)
                    throw new Exception("error: slow terapy is too high");
                RaiseEvent(new TerapyLoggedV1(log.SlowTerapy, log.Message, true, log.Metadata));
            }

            if (log.FastTerapy > 0)
            {
                if (log.FastTerapy > 0 && log.FastTerapy < 2)
                    throw new Exception("warning: fast terapy is very low");
                if (log.FastTerapy < 1)
                    throw new Exception("error: fast terapy is too low");
                if (log.FastTerapy > 60)
                    throw new Exception("error: fast terapy is too high");
                RaiseEvent(new TerapyLoggedV1(log.FastTerapy, log.Message, false, log.Metadata));
            }   
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
            Ensure.NotNullOrWhiteSpace(cmd.Name, nameof(cmd.Name));

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
