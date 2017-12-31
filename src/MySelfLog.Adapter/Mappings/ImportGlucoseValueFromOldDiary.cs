using System;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class ImportGlucoseValueFromOldDiary : LogBase
    {
        public ImportGlucoseValueFromOldDiary(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<Metadata>(metadataAsJson);

            Applies = DateTime.Parse(body.LogDate.ToString());
            Value = int.Parse(body.Value.ToString());
            CorrelationId = metadata.CorrelationId;
            Source = "MySelfLog-OldDiary";
        }
    }
}
