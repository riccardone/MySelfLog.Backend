using System;
using EventStore.Tools.Infrastructure;
using MySelfLog.Domain.Events;
using MySelfLog.Messages;

namespace MySelfLog.Domain.Aggregates
{
    public class DiaryAggregate : AggregateBase
    {
        public override string AggregateId => CorrelationId.ToString();
        private Guid CorrelationId { get; set; }

        public DiaryAggregate()
        {
            RegisterTransition<DiaryCreated>(Apply);
        }

        private void Apply(DiaryCreated obj)
        {
            CorrelationId = obj.CorrelationId;
        }

        public DiaryAggregate(Guid correlationId) : this()
        {
            RaiseEvent(new DiaryCreated(correlationId));
        }

        public static DiaryAggregate Create(Guid correlationId)
        {
            Ensure.NotEmptyGuid(correlationId, nameof(correlationId));
            return new DiaryAggregate(correlationId);
        }

        public void LogValue(LogValue logValue)
        {
            Ensure.NotNull(logValue, nameof(logValue));
            Ensure.NotEmptyGuid(logValue.CorrelationId, nameof(logValue.CorrelationId));
            Ensure.Nonnegative(logValue.Value, nameof(logValue.Value));
            Ensure.Nonnegative(logValue.TerapyValue, nameof(logValue.TerapyValue));
            Ensure.Nonnegative(logValue.Calories, nameof(logValue.Calories));

            if (logValue.Value > 0)
            {
                RaiseEvent(new GlucoseLogged(logValue.Value, logValue.Message, logValue.LogDate, logValue.SecurityLink));
            }
            if (logValue.TerapyValue > 0)
            {
                RaiseEvent(new TerapyLogged(logValue.TerapyValue, logValue.Message, logValue.LogDate, logValue.IsSlow, logValue.SecurityLink));
            }
            if (logValue.Calories > 0)
            {
                RaiseEvent(new FoodLogged(logValue.Calories, logValue.FoodTypes, logValue.Message, logValue.LogDate, logValue.SecurityLink));
            }
        }
    }
}
