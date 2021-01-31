namespace MySelfLog.Adapter
{
    public class Settings
    {
        public string EventStore_Username { get; set; }
        public string EventStore_Password { get; set; }
        public string EventStore_ProcessorLink { get; set; }
        public string ElasticSearch_Link { get; set; }
        public string ElasticSearch_Index { get; set; }
        public string CryptoKey { get; set; }
    }
}
