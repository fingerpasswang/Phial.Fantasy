using System;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;

namespace LogicServer
{
    class NotifyServiceImpl : IDbClientNotifyImpl
    {
        public IRpcImplInstnce SetSourceUuid(byte[] srcUuid)
        {
            return this;
        }

        public async Task NotifyConnectionInfo(string ip, int port)
        {
            Console.WriteLine("NotifyConnectionInfo {0}:{1}", ip, port);
        }
    }
}
