using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class ImportGlucoseValueFromOldDiary : LogBase
    {
        public ImportGlucoseValueFromOldDiary(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<IDictionary<string, string>>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>(metadataAsJson) ??
                           new Dictionary<string, string>();

            Value = int.Parse(body["Value"]);
            if (!metadata.ContainsKey("Source"))
                metadata["Source"] = "MySelfLog-OldDiary";
            if (!metadata.ContainsKey("$correlationId") && body.ContainsKey("CorrelationId"))
                metadata.Add("$correlationId", body["CorrelationId"]);
            metadata.Add("Applies", DateTime.Parse(body["LogDate"]).ToString("o"));
            Metadata = metadata;
        }
    }
}
