using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class ChangeDiaryName : Command
    {
        public string Name { get; }
        public IDictionary<string, string> Metadata { get; }
        public ChangeDiaryName(string name, IDictionary<string, string> metadata)
        {
            Name = name;
            Metadata = metadata;
        }
    }
}
