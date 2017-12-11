﻿using System;
using Evento;

namespace MySelfLog.Domain.Events
{
    public class GlucoseLogged : Event
    {
        public int Value { get; }
        public decimal MmolValue { get; }
        public string Message { get; }
        public DateTime LogDate { get; }

        public GlucoseLogged(int value, decimal mmolValue, string message, DateTime logDate)
        {
            Value = value;
            MmolValue = mmolValue;
            Message = message;
            LogDate = logDate;
        }
    }
}
