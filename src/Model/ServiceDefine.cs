using System.Collections.Generic;
using RPCBase;

namespace DataModel
{
    [Notify]
    public interface ILoginNotify
    {
        void NotifyLogicServerWorking(string districts);
    }

    [Notify]
    public interface IDbClientNotify
    {
        void NotifyConnectionInfo(string ip, int port);
    }

    [Notify]
    public interface IWatcherServiceNotify
    {
        void NotifyWatchingMaster(byte[] srcUuid, string masterAddr);
        void NotifyInstanceSubjectiveDown(byte[] srcUuid, string addr);
    }

    [Service]
    public interface ILogicClientService
    {
        bool AskChangeName(ulong pid, string newName);
        bool AskAddMoney(ulong pid, uint money);
        bool AskLearnSkill(ulong pid, uint skillId);
        bool TestEnum(ulong pid, TowerState state);
        bool[] TestArray(ulong[] pids, TowerState state);
        List<bool> TestList(List<bool> pids, TowerState state);
        Dictionary<bool, byte[]> TestDict(Dictionary<bool, PlayerInfo> pids, TowerState state);
        PlayerInfo RequestPlayerInfo(ulong pid);
        TestBaseClass TestHierarchy(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11);
        TestBaseClass TestHierarch2y(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11);
    }

    [Sync(true)]
    public interface IClientSceneService
    {
        void SyncPosition(int x, int y);
    }

    [Service]
    public interface ISceneClientService
    {
        bool AskMoveTo(int x, int y);
    }

    [Service(false)]
    public interface ILoginClientService
    {
        ServerList AskLogin(string username, string password, byte[] uuid);
    }

    [Sync]
    public interface IClientLogicService
    {
        void ServerMessageOk();
    }
    [Service]
    public interface ISchedulerLogicService
    {
        ulong RequestScheduleJob(int job);
    }
}
