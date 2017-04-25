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

        public string Node1HostName
        {
            get
            {
                var host = ConfigurationManager.AppSettings["EventStoreNode1HostName"];
                return string.IsNullOrEmpty(host) ? "127.0.0.1" : host;
            }
        }

        public int Node1TcpPort
        {
            get
            {
                int port;
                return int.TryParse(ConfigurationManager.AppSettings["EventStoreNode1TcpPort"], out port) ? port : 1113;
            }
        }
    }
}
