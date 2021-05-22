using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Commands
{
    public class CreateDiary : Command
    {
        public string Name { get; }
        public string CorrelationId { get; set; }
        public IDictionary<string, string> Metadata { get; internal set; }
        public CreateDiary(string name, string correlationId, IDictionary<string, string> metadata)
        {
            Name = name;
            CorrelationId = correlationId;
            Metadata = metadata;
        }
    }
}
