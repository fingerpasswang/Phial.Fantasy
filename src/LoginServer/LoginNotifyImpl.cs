using System;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;

namespace LoginServer
{
    class LoginNotifyImpl : ILoginNotifyImpl
    {
        public async Task NotifyLogicServerWorking(string districts)
        {
           ServerGroupManager.Update(districts);
        }
        public LoginNotifyImpl()
        {
        }

        public IRpcImplInstnce SetSourceUuid(byte[] sid)
        {
            return this;
        }
    }
}
