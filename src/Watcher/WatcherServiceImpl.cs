using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;

namespace WatcherService
{
    class WatcherServiceImpl : IWatcherServiceNotifyImpl
    {
        public IRpcImplInstnce SetSourceUuid(byte[] srcUuid)
        {
            return this;
        }

        public async Task NotifyWatchingMaster(byte[] srcUuid, string masterAddr)
        {
            var uuid = new Guid(srcUuid);

            //Console.WriteLine("NotifyWatchingMaster {0} => {1}", uuid, masterAddr);

            Program.watcherManager.NotifyWatchingMaster(uuid, masterAddr);
        }

        public async Task NotifyInstanceSubjectiveDown(byte[] srcUuid, string addr)
        {
            var uuid = new Guid(srcUuid);

            //Console.WriteLine("NotifyInstanceSubjectiveDown{0} => {1}", uuid, addr);

            Program.watcherManager.NotifyInstanceSubjectiveDown(uuid, addr);
        }
    }
}
