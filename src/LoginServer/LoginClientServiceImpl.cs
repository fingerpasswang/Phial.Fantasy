using System;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;
using ServerUtils;

namespace LoginServer
{
    class LoginClientServiceImpl : ILoginClientImpl
    {
        public async Task<ServerList> AskLogin(string user, string password,  byte[] uuid)
        {
            Console.WriteLine("LoginServer.AskLogin user={0},  pass={1},  uuid={2}", user,password, new Guid(uuid));

            using (var accountAccesser = await Program.CacheDataService.GetAccountAccesser(user))
            {
                var pid = await accountAccesser.LoadPidAtAccountAsync();

                Console.WriteLine("LoginServer.AskLogin Load_Account_Pid pid={0}", pid);

                if (pid == 0)
                {
                    pid = Program.MemIdProvider.GenId(IdSegTypePersistence.Player);

                    Console.WriteLine("LoginServer.AskLogin Load_Account_Pid alloc pid pid={0}", pid);

                    accountAccesser.UpdatePidAtAccount(pid);
                }

                using (var sessionAccesser = await Program.CacheDataService.GetSessionAccesser(pid))
                {
                    sessionAccesser.UpdateSessionIdAtSession(uuid);

                    await accountAccesser.SubmitChangesWith(sessionAccesser);

                    return new ServerList()
                    {
                        Pid = pid,
                        SessionId = uuid,
                        Servers = ServerGroupManager.GetServerGroups(),
                    };
                }
            }
        }
        private readonly byte[] sessionId;

        private LoginClientServiceImpl(byte[] sid)
        {
            sessionId = sid;
        }
        public LoginClientServiceImpl()
        {
        }

        public IRpcImplInstnce SetSourceUuid(byte[] sid)
        {
            return new LoginClientServiceImpl(sid);
        }
    }
}
