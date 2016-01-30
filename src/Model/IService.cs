using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Notify(1)]
    public interface IDbSyncNotify
    {
        void NotifyPlayerLoaded(ulong pid);
    }

    [Notify(2)]
    public interface ILoginNotify
    {
        void NotifyPlayerLogin(ulong pid);
    }

    [Service(3)]
    public interface ILogicClientService
    {
        bool AskChangeName(ulong pid, string newName);
        bool AskAddMoney(ulong pid, uint money);
        bool AskLearnSkill(ulong pid, uint skillId);
    }

    [Service(4)]
    public interface ILoginClientService
    {
        bool AskLogin(ulong pid);
    }

    [Service(5)]
    public interface IClientLogicService
    {
        void ServerMessageOk();
    }

    [Service(6)]
    public interface IClientLoginService
    {
        void SyncPlayerInfo(PlayerInfo pInfo);
    }

}
