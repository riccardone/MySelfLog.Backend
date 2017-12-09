using System;
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
            RegisterTransition<DiaryCreated>(Apply);
        }

        private void Apply(DiaryCreated obj)
        {
            CorrelationId = obj.CorrelationId;
            SecurityLink = obj.SecurityLink;
        }

        public Diary(string correlationId, string securityLink) : this()
        {
            RaiseEvent(new DiaryCreated(correlationId, securityLink));
        }

        public static Diary Create(CreateDiary cmd)
        {
            Ensure.NotNullOrWhiteSpace(cmd.CorrelationId, nameof(cmd.CorrelationId));

            return new Diary(cmd.CorrelationId, GetSecurityLink());
        }

        private static string GetSecurityLink()
        {
            return Guid.NewGuid().GetHashCode().ToString();
        }

        public void LogValue(LogValue log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.Value, nameof(log.Value));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            RaiseEvent(new GlucoseLogged(log.Value, log.Message, log.LogDate, SecurityLink));
        }

        public void LogTerapy(LogTerapy log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.FastTerapy, nameof(log.FastTerapy));
            Ensure.Nonnegative(log.SlowTerapy, nameof(log.SlowTerapy));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            if (log.FastTerapy > 0)
                RaiseEvent(new TerapyLogged(log.FastTerapy, log.Message, log.LogDate, false, SecurityLink));
            else if (log.SlowTerapy > 0)
                RaiseEvent(new TerapyLogged(log.FastTerapy, log.Message, log.LogDate, false, SecurityLink));
        }

        public void LogFood(LogFood log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.Calories, nameof(log.Calories));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            if (log.Calories > 0)
                RaiseEvent(new FoodLogged(log.Calories, log.FoodTypes, log.Message, log.LogDate));
        }
    }
}
