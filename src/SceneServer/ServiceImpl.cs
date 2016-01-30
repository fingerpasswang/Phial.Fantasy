using System;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;

namespace Test.SceneServer
{
    class SceneClientServiceImpl : ISceneClientImpl
    {
        public async Task<bool> AskMoveTo(int x, int y)
        {
            Console.WriteLine("AskMoveTo x:{0} y:{1}", x, y);

            //Program.ClientSceneService.Forward(currentClientUuid).SyncPosition(x, y);

            Program.ClientSceneService.SyncPositionMulticast(1, x, y);

            return true;
        }

        private byte[] currentClientUuid;

        public IRpcImplInstnce SetSourceUuid(byte[] srcUuid)
        {
            currentClientUuid = srcUuid;

            return this;
        }
    }
}
