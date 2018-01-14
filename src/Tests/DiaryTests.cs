using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var cmd = new CreateDiary("test", 
                new Dictionary<string, string>
                {
                    {"$correlationId", "123"},
                    {"Applies", DateTime.Now.ToString(CultureInfo.InvariantCulture)},
                    {"Source", "test"},
                    {"Email", "test@test.com"}
                });
            var logGlucose = new LogValue(100, 0, "test", new Dictionary<string, string>
            {
                {"$correlationId", "123"},
                {"Applies", DateTime.Now.ToString(CultureInfo.InvariantCulture)},
                {"Source", "test"}
            });

            // Act
            var aggregate = Diary.Create(cmd);
            aggregate.LogValue(logGlucose);

            // Verify
            Assert.IsTrue(((GlucoseLoggedV1)aggregate.UncommitedEvents().Last()).Value.Equals(100));
        }
    }
}
