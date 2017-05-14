using System;
using EventStore.Tools.Infrastructure;

namespace MySelfLog.Messages
{
    public class CreateDiary : Command
    {
        public Guid CorrelationId { get; }
        public string Name { get; }
        public string Email { get; }

        public CreateDiary(Guid correlationId, string name, string email)
        {
            CorrelationId = correlationId;
            Name = name;
            Email = email;
        }
    }
}
