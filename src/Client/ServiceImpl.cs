using System;
using Auto.Client;
using RPCBase;

namespace ClientTest
{
    class ClientLogicServiceImpl : ILogic2CliImpl
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
