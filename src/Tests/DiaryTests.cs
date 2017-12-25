using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Evento;
using MySelfLog.Domain.Aggregates;
using MySelfLog.Domain.Commands;
using MySelfLog.Domain.Events;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DiaryTests
    {
        [Test]
        public void LogGlucose()
        {
            // Set up
            var cmd = new CreateDiary(string.Empty, string.Empty,
                new Dictionary<string, string>
                {
                    {"", "123"},
                    {"Applies", DateTime.Now.ToString(CultureInfo.InvariantCulture)}
                });
            var logGlucose = new LogValue(100, 0, "test", new Dictionary<string, string>
            {
                {"", "123"},
                {"Applies", DateTime.Now.ToString(CultureInfo.InvariantCulture)}
            });

            // Act
            var aggregate = Diary.Create(cmd);
            aggregate.LogValue(logGlucose);

            // Verify
            Assert.IsTrue(((GlucoseLoggedV2)aggregate.UncommitedEvents().Last()).Value.Equals(100));
        }
    }
}
