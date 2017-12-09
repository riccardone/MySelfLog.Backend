using System;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class Metadata
    {
        [JsonProperty("$correlationId")]
        public string CorrelationId { get; set; }
        public string Source { get; set; }
        public DateTime Applies { get; set; }
        public string Reverses { get; set; }
    }
}
