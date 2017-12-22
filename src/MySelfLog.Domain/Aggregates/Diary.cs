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
        //private List<GlucoseValue> GlucoseValues { get; set; } 

        public Diary()
        {
            //GlucoseValues = new List<GlucoseValue>();
            RegisterTransition<DiaryCreated>(Apply);
            RegisterTransition<SecurityLinkChanged>(Apply);
            //RegisterTransition<GlucoseLogged>(Apply);
        }

        //private void Apply(GlucoseLogged obj)
        //{
        //   GlucoseValues.Add(new GlucoseValue(obj.Value, obj.MmolValue, obj.Message, obj.LogDate));
        //}

        private void Apply(SecurityLinkChanged obj)
        {
            SecurityLink = obj.SecurityLink;
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

            if (log.Value > 0 || log.MmolValue > 0)
                RaiseEvent(new GlucoseLogged(log.Value, log.MmolValue, log.Message, log.LogDate));
        }

        public void LogTerapy(LogTerapy log)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNullOrWhiteSpace(log.CorrelationId, nameof(log.CorrelationId));
            Ensure.Nonnegative(log.FastTerapy, nameof(log.FastTerapy));
            Ensure.Nonnegative(log.SlowTerapy, nameof(log.SlowTerapy));
            Ensure.NonLessThan50Years(log.LogDate, nameof(log.LogDate));

            if (log.FastTerapy > 0)
                RaiseEvent(new TerapyLogged(log.FastTerapy, log.Message, log.LogDate, false));
            if (log.SlowTerapy > 0)
                RaiseEvent(new TerapyLogged(log.SlowTerapy, log.Message, log.LogDate, true));
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

        public void ChangeSecurityLink()
        {
            RaiseEvent(new SecurityLinkChanged(GetSecurityLink()));
        }
    }
}
