using System;
using System.Collections.Generic;
using System.Text;
using Auto.Client;
using RPCBase;

namespace ClientTest
{
    class SceneServiceImpl : IScene2CliImpl
    {
        public IRpcImplInstnce SetSourceUuid(byte[] srcUuid)
        {
            return this;
        }

        public void SyncPosition(int x, int y)
        {
            Console.WriteLine("SyncPosition x:{0} y:{1}", x, y);
        }
    }
}
