using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySelfLog.Backend.Adapter
{
    public interface IMessageReceiver
    {
        bool Start();
        void Stop();
        void RegisterMessageHandler(Func<object, CancellationToken, Task> handler);
    }
}
