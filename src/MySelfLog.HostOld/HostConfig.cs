using System;
using System.Configuration;

namespace MySelfLog.Host
{
    class HostConfig 
    {
        public string UserName
        {
            get
            {
                var eventStoreUserName = ConfigurationManager.AppSettings["EventStoreUserName"];
                return string.IsNullOrEmpty(eventStoreUserName) ? "admin" : eventStoreUserName;
            }
        }

        public string Password
        {
            get
            {
                var eventStorePassword = ConfigurationManager.AppSettings["EventStorePassword"];
                return string.IsNullOrEmpty(eventStorePassword) ? "changeit" : eventStorePassword;
            }
        }

        public Uri EventStoreSubscriberLink
        {
            get
            {
                var host = ConfigurationManager.AppSettings["EventStoreSubscriberLink"];
                return string.IsNullOrEmpty(host) ? new Uri("tcp://admin:changeit@eventstore:1113") : new Uri(host);
            }
        }
        public Uri EventStoreProcessorLink
        {
            get
            {
                var host = ConfigurationManager.AppSettings["EventStoreProcessorLink"];
                return string.IsNullOrEmpty(host) ? new Uri("tcp://admin:changeit@eventstore:1113") : new Uri(host);
            }
        }
        public Uri ElasticSearchLink
        {
            get
            {
                var host = ConfigurationManager.AppSettings["ElasticSearchLink"];
                return string.IsNullOrEmpty(host) ? new Uri("http://elasticsearch:9200") : new Uri(host);
            }
        }
    }
}
