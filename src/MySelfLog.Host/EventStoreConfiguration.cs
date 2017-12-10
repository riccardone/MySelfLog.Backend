using System.Configuration;

namespace MySelfLog.Host
{
    class EventStoreConfiguration 
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

        public System.Uri EventStoreLink
        {
            get
            {
                var host = ConfigurationManager.AppSettings["EventStoreLink"];
                return string.IsNullOrEmpty(host) ? new System.Uri("tcp://admin:changeit@localhost:1113") : new System.Uri(host);
            }
        }
    }
}
