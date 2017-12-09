using System;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class CreateDiary : Command
    {
        public string CorrelationId { get; }
        public string Name { get; }
        public string Email { get; }

        public CreateDiary(string correlationId, string name, string email)
        {
            CorrelationId = correlationId;
            Name = name;
            Email = email;
        }
    }
}
