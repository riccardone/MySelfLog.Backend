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
        private string SecurityLink { get; set; }

        public Diary()
        {
            RegisterTransition<DiaryCreatedV1>(Apply);
            RegisterTransition<SecurityLinkChangedV1>(Apply);
        }
        public Diary(string securityLink, IDictionary<string, string> metadata) : this()
        {
            RaiseEvent(new DiaryCreatedV1(securityLink, metadata));
        }

        private void Apply(DiaryCreatedV1 obj)
        {
            CorrelationId = obj.Metadata["$correlationId"];
            SecurityLink = obj.SecurityLink;
        }

        private void Apply(SecurityLinkChangedV1 obj)
        {
            SecurityLink = obj.SecurityLink;
        }
        public static Diary Create(CreateDiary cmd)
        {
            Ensure.NotNull(cmd.Metadata, nameof(cmd.Metadata));
            Ensure.NotNullOrWhiteSpace(cmd.Metadata["$correlationId"], "$correlationId");

            return new Diary(GetSecurityLink(), cmd.Metadata);
        }

        private static string GetSecurityLink()
        {
            return Guid.NewGuid().GetHashCode().ToString();
        }

        public void LogValue(LogValue log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.Metadata["$correlationId"], "$correlationId");
            Ensure.Nonnegative(log.Value, nameof(log.Value));
            Ensure.NotNullOrWhiteSpace(log.Metadata["Applies"], "Applies");
            Ensure.NotNullOrWhiteSpace(log.Metadata["Source"], "Source");

            if (log.Value > 0 || log.MmolValue > 0)
                RaiseEvent(new GlucoseLoggedV1(log.Value, log.MmolValue, log.Message, log.Metadata));
        }

        public void LogTerapy(LogTerapy log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.Metadata["$correlationId"], "$correlationId");
            Ensure.Nonnegative(log.FastTerapy, nameof(log.FastTerapy));
            Ensure.Nonnegative(log.SlowTerapy, nameof(log.SlowTerapy));
            Ensure.NotNullOrWhiteSpace(log.Metadata["Applies"], "Applies");
            Ensure.NotNullOrWhiteSpace(log.Metadata["Source"], "Source");

            if (log.FastTerapy > 0)
                RaiseEvent(new TerapyLoggedV1(log.FastTerapy, log.Message, false, log.Metadata));
            if (log.SlowTerapy > 0)
                RaiseEvent(new TerapyLoggedV1(log.SlowTerapy, log.Message, true, log.Metadata));
        }

        

        public void LogFood(LogFood log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.Metadata["$correlationId"], "$correlationId");
            Ensure.Nonnegative(log.Calories, nameof(log.Calories));
            Ensure.NotNullOrWhiteSpace(log.Metadata["Applies"], "Applies");
            Ensure.NotNullOrWhiteSpace(log.Metadata["Source"], "Source");

            if (log.Calories > 0)
                RaiseEvent(new FoodLoggedV1(log.Calories, log.FoodTypes, log.Message, log.Metadata));
        }

        public void ChangeSecurityLink()
        {
            RaiseEvent(new SecurityLinkChangedV1(GetSecurityLink(),
                new Dictionary<string, string> {{"$correlationId", CorrelationId}}));
        }
    }
}
