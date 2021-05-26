using System;
using System.Collections.Generic;
using System.IO;
using CloudEventData;
using System.Text.Json;

namespace MySelfLog.Backend.Tests
{
    public class Helpers
    {
        public static CloudEventRequest BuilCloudRequest(string payloadPath)
        {
            var request = File.ReadAllText(payloadPath); 
            var req = JsonSerializer.Deserialize<CloudEventRequest>(request, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            req.Data = req.Data.ToString();
            return req;
        }

        public static IDictionary<string, string> BuildMetadata(string schema = "something/1,0", string source = "test-source", string correlationId = "123456789")
        {
            return new Dictionary<string, string>
            {
                {"$correlationId", correlationId},
                {"source", source},
                {"$applies", "2021-02-09"},
                {"schema", schema},
                {"content-type", "application/json"}
            };
        }
    }
}
