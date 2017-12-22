using System;
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
            var cmd = new CreateDiary("123", string.Empty, String.Empty);
            var logGlucose = new LogValue("123", 100, 0, "test", new DateTime(2017, 5, 1));

            // Act
            var aggregate = Diary.Create(cmd);
            aggregate.LogValue(logGlucose);

            // Verify
            Assert.IsTrue(((GlucoseLogged)aggregate.UncommitedEvents().Last()).Value.Equals(100));
        }
    }
}
