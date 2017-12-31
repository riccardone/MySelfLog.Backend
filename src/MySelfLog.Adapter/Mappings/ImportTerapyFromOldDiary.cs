using System;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class ImportTerapyFromOldDiary : LogBase
    {
        public ImportTerapyFromOldDiary(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<Metadata>(metadataAsJson);
            
            Applies = DateTime.Parse(body.LogDate.ToString());
            var isSlow = bool.Parse(body.IsSlow.ToString());
            int terapy = int.Parse(body.Value.ToString());
            if (isSlow)
                SlowTerapy = terapy;
            else
                FastTerapy = terapy;
            
            CorrelationId = metadata.CorrelationId; 
            Source = "MySelfLog-OldDiary";
        }
    }
}
