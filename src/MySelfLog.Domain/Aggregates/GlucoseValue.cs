using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfLog.Domain.Aggregates
{
    public class GlucoseValue
    {
        public int Value { get; }
        public decimal MmolValue { get; }
        public string Message { get; }
        public DateTime LogDate { get; }

        public GlucoseValue(int value, decimal mmolValue, string message, DateTime logDate)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            LogDate = logDate;
        }
    }
}
