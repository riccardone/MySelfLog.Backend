namespace MySelfLog.Backend.Adapter
{
    public class Settings
    {
        public string EventStore_Username { get; set; }
        public string EventStore_Password { get; set; }
        public string EventStore_ProcessorLink { get; set; }
        public string EventStore_SubscriberLink { get; set; }
        public string DomainCategory { get; set; }
        public string CryptoKey { get; set; }
        public string CertificateFqdn { get; set; }
        public string Input_queue { get; set; }
        public string Input_queue_forced { get; set; }
        public int Metrics_port { get; set; }
    }
}
