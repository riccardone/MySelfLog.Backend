using System.Collections.Generic;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class ImportGlucoseValueFromOldDiary : LogBase
    {
        public ImportGlucoseValueFromOldDiary(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>(metadataAsJson);
            
            Value = int.Parse(body.Value.ToString());
            if (!metadata.ContainsKey("Source"))
                metadata["Source"] = "MySelfLog-OldDiary";
            Metadata = metadata;
        }
    }
}
