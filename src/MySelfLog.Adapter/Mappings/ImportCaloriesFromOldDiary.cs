using System;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class ImportCaloriesFromOldDiary : LogBase
    {
        public ImportCaloriesFromOldDiary(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<Metadata>(metadataAsJson);
            
            Applies = DateTime.Parse(body.LogDate.ToString());
            Calories = int.Parse(body.Calories.ToString());
            CorrelationId = metadata.CorrelationId; 
            Source = "MySelfLog-OldDiary";
        }
    }
}
