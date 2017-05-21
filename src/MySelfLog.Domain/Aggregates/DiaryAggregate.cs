using System;
using Evento;
using MySelfLog.Domain.Commands;
using MySelfLog.Domain.Events;

namespace MySelfLog.Domain.Aggregates
{
    public class DiaryAggregate : AggregateBase
    {
        public override string AggregateId => CorrelationId.ToString();
        private Guid CorrelationId { get; set; }
        private string SecurityLink { get; set; }

        public DiaryAggregate()
        {
            RegisterTransition<DiaryCreated>(Apply);
        }

        private void Apply(DiaryCreated obj)
        {
            CorrelationId = obj.CorrelationId;
            SecurityLink = obj.SecurityLink;
        }

        public DiaryAggregate(Guid correlationId, string securityLink) : this()
        {
            RaiseEvent(new DiaryCreated(correlationId, securityLink));
        }

        public static DiaryAggregate Create(Guid correlationId)
        {
            Ensure.NotEmptyGuid(correlationId, nameof(correlationId));

            return new DiaryAggregate(correlationId, GetSecurityLink());
        }

        private static string GetSecurityLink()
        {
            return Guid.NewGuid().GetHashCode().ToString();
        }

        public void LogValue(LogValue log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotEmptyGuid(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.Value, nameof(log.Value));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            RaiseEvent(new GlucoseLogged(log.Value, log.Message, log.LogDate, SecurityLink));
        }

        public void LogTerapy(LogTerapy log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotEmptyGuid(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.TerapyValue, nameof(log.TerapyValue));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            RaiseEvent(new TerapyLogged(log.TerapyValue, log.Message, log.LogDate, log.IsSlow, SecurityLink));
        }

        public void LogFood(LogFood log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotEmptyGuid(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.Calories, nameof(log.Calories));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            RaiseEvent(new FoodLogged(log.Calories, log.FoodTypes, log.Message, log.LogDate));
        }
    }
}
