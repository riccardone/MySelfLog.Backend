using System.Collections.Generic;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class LogFromJson : LogBase
    {
        public LogFromJson(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>(metadataAsJson);

            if (int.TryParse(body.value.Value.ToString(), out int value))
                Value = value;
            if (int.TryParse(body.calories.Value.ToString(), out int calories))
                Calories = calories;
            if (int.TryParse(body.fastTerapy.Value.ToString(), out int fastTerapy))
                FastTerapy = fastTerapy;
            if (int.TryParse(body.mmolvalue.Value.ToString(), out int mmolValue))
                MmolValue = mmolValue;
            if (int.TryParse(body.slowTerapy.Value.ToString(), out int slowTerapy))
                SlowTerapy = slowTerapy;
            Comment = body.comment.Value;
            Metadata = metadata;
        }
    }
}
