using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class CreateDiary : CommandV2
    {
        public string Name { get; }
        public string Email { get; }
        public IDictionary<string, string> Metadata { get; }
        public CreateDiary(string name, string email, IDictionary<string, string> metadata)
        {
            Name = name;
            Email = email;
            Metadata = metadata;
        }
    }
}
