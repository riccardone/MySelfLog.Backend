using Newtonsoft.Json;

namespace MySelfLog.AppService
{
    public class LogFromJsonWrapper : LogDto
    {
        public LogFromJsonWrapper(string jsonAsString)
        {
            var json = JsonConvert.DeserializeObject<dynamic>(jsonAsString);
            Value = int.Parse(json.value.Value);
            // TODO to be continued...
        }
    }
}
