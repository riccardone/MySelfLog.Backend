using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace MySelfLog.Adapter.Mappings
{
    public class ImportTerapyFromOldDiary : LogBase
    {
        public ImportTerapyFromOldDiary(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>(metadataAsJson);
            
            var isSlow = bool.Parse(body.IsSlow.ToString());
            int terapy = int.Parse(body.Value.ToString());
            if (isSlow)
                SlowTerapy = terapy;
            else
                FastTerapy = terapy;

            if (!metadata.ContainsKey("Source"))
                metadata["Source"] = "MySelfLog-OldDiary";
            metadata.Add("Applies", DateTime.Parse(body.LogDate.ToString()).ToString("o"));
            Metadata = metadata;
        }
    }
}
