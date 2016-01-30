using System;
using Auto.Client;
using RPCBase;

namespace ClientTest
{
    class ClientLogicServiceImpl : IClientLogicImpl
    {
        public void ServerMessageOk()
        {
            Console.WriteLine("ServerMessageOK");
        }

        public IRpcImplInstnce SetSourceUuid(byte[] sid)
        {
            return this;
        }
    }
}
