﻿using System.Collections.Generic;
using Evento;

namespace MySelfLog.Domain.Commands
{
    public class CreateDiary : Command
    {
        public string Name { get; }
        public IDictionary<string, string> Metadata { get; }
        public CreateDiary(string name, IDictionary<string, string> metadata)
        {
            Name = name;
            Metadata = metadata;
        }
    }
}
