using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Messages
{
    public class CreateDiary : Command
    {
        public Guid CorrelationId { get; }
        public string Name { get; }
        public Guid ProfileId { get; }

        public CreateDiary(Guid correlationId, string name, Guid profileId)
        {
            CorrelationId = correlationId;
            Name = name;
            ProfileId = profileId;
        }
    }
}
