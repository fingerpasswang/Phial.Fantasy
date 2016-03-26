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

    [Service(ServiceScope.ClientToServer)]
    public interface ICli2Logic
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
    public interface IScene2Cli
    {
        void SyncPosition(int x, int y);
    }

    [Service(ServiceScope.ClientToServer)]
    public interface ICli2Scene
    {
        bool AskMoveTo(int x, int y);
    }

    [Service(ServiceScope.ClientToServer), Divisional(false)]
    public interface ICli2Login
    {
        ServerList AskLogin(string username, string password, byte[] uuid);
    }

    [Sync]
    public interface ILogic2Cli
    {
        void ServerMessageOk();
    }

    [Service(ServiceScope.InterServer)]
    public interface ILogic2Scheduler
    {
        ulong RequestScheduleJob(int job);
    }
}
