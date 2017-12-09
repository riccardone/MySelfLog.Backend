using MySelfLog.Domain.Commands;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class LogFromJson : LogBase
    {
        public LogFromJson(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<Metadata>(metadataAsJson);

            if (int.TryParse(body.value.Value, out int value))
                Value = value;
            if (int.TryParse(body.calories.Value, out int calories))
                Calories = calories;
            if (int.TryParse(body.fastTerapy.Value, out int fastTerapy))
                FastTerapy = fastTerapy;
            if (int.TryParse(body.mmolvalue.Value, out int mmolValue))
                MmolValue = mmolValue;
            if (int.TryParse(body.slowTerapy.Value, out int slowTerapy))
                SlowTerapy = slowTerapy;
            Comment = body.comment.Value;

            CorrelationId = metadata.CorrelationId; // TODO build a deterministic id here
            Applies = metadata.Applies;
            Reverses = metadata.Reverses;
            Source = metadata.Source;
        }
    }
}
