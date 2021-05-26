using MySelfLog.Backend.Domain;
using MySelfLog.Backend.Domain.Aggregates;
using MySelfLog.Backend.Domain.Commands;
using NUnit.Framework;

namespace MySelfLog.Backend.Tests
{
    public class Tests
    {
        [Test]
        public void Given_Valid_CreateDiary_I_Expect_The_Diary_Created()
        {
            // Assign
            var request = Helpers.BuilCloudRequest("PayloadSamples/creatediary.json");
            
            // Act
            var mapper = new CreateDiaryMapper();
            var cmd = mapper.Map(request) as CreateDiary;
            var result = Diary.Create(cmd);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}