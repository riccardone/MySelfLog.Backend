using System.Collections.Generic;
using Evento;

namespace MySelfLog.Backend.Domain.Events
{
    public class DiaryNameChangedV1 : Event
    {
        public string Name { get; }
        public IDictionary<string, string> Metadata { get; }
        public DiaryNameChangedV1(string name, IDictionary<string, string> metadata)
        {
            Name = name;
            Metadata = metadata;
        }
    }
}
