using System.Collections.Generic;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class CreateDiaryFromJson : LogBase
    {
        public CreateDiaryFromJson(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>(metadataAsJson);
           
            DiaryName = body.diaryName.Value;
            Metadata = metadata;
        }
    }
}
