using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auto.Server;
using DataAccess;
using System.Linq;

namespace Auto.DataAccess
{
    public interface ISessionReader: IDisposable
    {
        Task<Session> LoadSessionAsync();
        Task<Byte[]> LoadSessionIdAtSessionAsync();
    }
    public interface ISessionAccesser: ISessionReader, ISubmitChangable
    {
        void UpdateSession(Session session);
        void UpdateSessionIdAtSession(Byte[] sessionid);
    }
    public interface IAccountReader: IDisposable
    {
        Task<Account> LoadAccountAsync();
        Task<UInt64> LoadPidAtAccountAsync();
        Task<UInt64[]> LoadTestArrAtAccountAsync();
        Task<Dictionary<UInt64, UInt32[]>> LoadTestDictAtAccountAsync();
        Task<List<List<UInt32>>> LoadTestListAtAccountAsync();
    }
    public interface IAccountAccesser: IAccountReader, ISubmitChangable
    {
        void UpdateAccount(Account account);
        void UpdatePidAtAccount(UInt64 pid);
        void UpdateTestArrAtAccount(UInt64[] testarr);
        void UpdateTestDictAtAccount(Dictionary<UInt64, UInt32[]> testdict);
        void UpdateTestListAtAccount(List<List<UInt32>> testlist);
    }
    public interface IPlayerInfoReader: IDisposable
    {
        Task<PlayerInfo> LoadPlayerInfoAsync();
        Task<UInt32> LoadUidAtPlayerInfoAsync();
        Task<String> LoadNameAtPlayerInfoAsync();
        Task<UInt32> LoadLevelAtPlayerInfoAsync();
        Task<PlayerInfoSkill> LoadSkillAtPlayerInfoAsync();
        Task<PlayerInfoItem> LoadItemAtPlayerInfoAsync();
    }
    public interface IPlayerInfoAccesser: IPlayerInfoReader, ISubmitChangable
    {
        void UpdatePlayerInfo(PlayerInfo playerinfo);
        void UpdateUidAtPlayerInfo(UInt32 uid);
        void UpdateNameAtPlayerInfo(String name);
        void UpdateLevelAtPlayerInfo(UInt32 level);
        void UpdateSkillAtPlayerInfo(PlayerInfoSkill skill);
        void UpdateItemAtPlayerInfo(PlayerInfoItem item);
    }

    public class RedisDataServiceDelegate
    {
        class Delegate<TKey> : DelegateBase, ISessionAccesser, IAccountAccesser, IPlayerInfoAccesser
        {
            public Delegate(RedisAdaptor adaptor, long contextLockId, TKey key, string dataId)
            {
                this.adaptor = adaptor;
                this.contextLockId = contextLockId;
                this.key = key;
                this.dataId = dataId;
            }
            public async Task<bool> SubmitChanges()
            {
                await adaptor.SubmitChanges(
                    new[] { dataId }
                    , new[] { contextLockId }
                    , new [] {updateQueue});

                return true;
            }

            public async Task<bool> SubmitChangesWith(params object[] others)
            {
                await adaptor.SubmitChanges(
                    new[] { dataId } .Concat(others.Select(d => ((DelegateBase)d).dataId)).ToArray(),
                    new[] { contextLockId } .Concat(others.Select(d => ((DelegateBase)d).contextLockId)).ToArray(),
                    new[] { updateQueue } .Concat(others.Select(d => ((DelegateBase)d).updateQueue)).ToArray());

                return true;
            }

            public void Dispose()
            {

            }

            private readonly RedisAdaptor adaptor;
            private readonly object key;

            #region Session
            public void UpdateSession(Session session)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", session.Pid)
                .WriteField("SessionId", session.SessionId)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateSessionIdAtSession(Byte[] sessionid)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("SessionId", sessionid)
                ;
                updateQueue.Add(ctx);
            }
            public async Task<Session> LoadSessionAsync()
            {
                var val = new Session();
                var reader = await adaptor.QueryAll(dataId);
                if (reader == null)
                {
                    return null;
                }
                val.Pid = (UInt64)key;
                val.SessionId = reader.ReadField<Byte[]>("SessionId");
                return val;
            }
            public async Task<Byte[]> LoadSessionIdAtSessionAsync()
            {
                var val = default(Byte[]);
                var reader = await adaptor.Query(dataId, "SessionId");
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<Byte[]>("SessionId");
                return val;
            }
            #endregion

            #region Account
            public void UpdateAccount(Account account)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("User", account.User)
                .WriteField("Pid", account.Pid)
                .WriteField("TestArr", account.TestArr)
                .WriteField("TestDict", account.TestDict)
                .WriteField("TestList", account.TestList)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdatePidAtAccount(UInt64 pid)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", pid)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateTestArrAtAccount(UInt64[] testarr)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("TestArr", testarr)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateTestDictAtAccount(Dictionary<UInt64, UInt32[]> testdict)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("TestDict", testdict)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateTestListAtAccount(List<List<UInt32>> testlist)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("TestList", testlist)
                ;
                updateQueue.Add(ctx);
            }
            public async Task<Account> LoadAccountAsync()
            {
                var val = new Account();
                var reader = await adaptor.QueryAll(dataId);
                if (reader == null)
                {
                    return null;
                }
                val.User = (String)key;
                val.Pid = reader.ReadField<UInt64>("Pid");
                val.TestArr = reader.ReadField<UInt64[]>("TestArr");
                val.TestDict = reader.ReadField<Dictionary<UInt64, UInt32[]>>("TestDict");
                val.TestList = reader.ReadField<List<List<UInt32>>>("TestList");
                return val;
            }
            public async Task<UInt64> LoadPidAtAccountAsync()
            {
                var val = default(UInt64);
                var reader = await adaptor.Query(dataId, "Pid");
                if (reader == null)
                {
                    return default(UInt64);
                }
                val = reader.ReadField<UInt64>("Pid");
                return val;
            }
            public async Task<UInt64[]> LoadTestArrAtAccountAsync()
            {
                var val = default(UInt64[]);
                var reader = await adaptor.Query(dataId, "TestArr");
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<UInt64[]>("TestArr");
                return val;
            }
            public async Task<Dictionary<UInt64, UInt32[]>> LoadTestDictAtAccountAsync()
            {
                var val = new Dictionary<UInt64, UInt32[]>();
                var reader = await adaptor.Query(dataId, "TestDict");
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<Dictionary<UInt64, UInt32[]>>("TestDict");
                return val;
            }
            public async Task<List<List<UInt32>>> LoadTestListAtAccountAsync()
            {
                var val = new List<List<UInt32>>();
                var reader = await adaptor.Query(dataId, "TestList");
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<List<List<UInt32>>>("TestList");
                return val;
            }
            #endregion

            #region PlayerInfo
            public void UpdatePlayerInfo(PlayerInfo playerinfo)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", playerinfo.Pid)
                .WriteField("Uid", playerinfo.Uid)
                .WriteField("Name", playerinfo.Name)
                .WriteField("Level", playerinfo.Level)
                .WriteField("Skill", playerinfo.Skill)
                .WriteField("Item", playerinfo.Item)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateUidAtPlayerInfo(UInt32 uid)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Uid", uid)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateNameAtPlayerInfo(String name)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Name", name)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateLevelAtPlayerInfo(UInt32 level)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Level", level)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateSkillAtPlayerInfo(PlayerInfoSkill skill)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Skill", skill)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateItemAtPlayerInfo(PlayerInfoItem item)
            {
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Item", item)
                ;
                updateQueue.Add(ctx);
            }
            public async Task<PlayerInfo> LoadPlayerInfoAsync()
            {
                var val = new PlayerInfo();
                var reader = await adaptor.QueryAll(dataId);
                if (reader == null)
                {
                    return null;
                }
                val.Pid = (UInt64)key;
                val.Uid = reader.ReadField<UInt32>("Uid");
                val.Name = reader.ReadField<String>("Name");
                val.Level = reader.ReadField<UInt32>("Level");
                val.Skill = reader.ReadField<PlayerInfoSkill>("Skill");
                val.Item = reader.ReadField<PlayerInfoItem>("Item");
                return val;
            }
            public async Task<UInt32> LoadUidAtPlayerInfoAsync()
            {
                var val = default(UInt32);
                var reader = await adaptor.Query(dataId, "Uid");
                if (reader == null)
                {
                    return default(UInt32);
                }
                val = reader.ReadField<UInt32>("Uid");
                return val;
            }
            public async Task<String> LoadNameAtPlayerInfoAsync()
            {
                var val = default(String);
                var reader = await adaptor.Query(dataId, "Name");
                if (reader == null)
                {
                    return default(String);
                }
                val = reader.ReadField<String>("Name");
                return val;
            }
            public async Task<UInt32> LoadLevelAtPlayerInfoAsync()
            {
                var val = default(UInt32);
                var reader = await adaptor.Query(dataId, "Level");
                if (reader == null)
                {
                    return default(UInt32);
                }
                val = reader.ReadField<UInt32>("Level");
                return val;
            }
            public async Task<PlayerInfoSkill> LoadSkillAtPlayerInfoAsync()
            {
                var val = new PlayerInfoSkill();
                var reader = await adaptor.Query(dataId, "Skill");
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<PlayerInfoSkill>("Skill");
                return val;
            }
            public async Task<PlayerInfoItem> LoadItemAtPlayerInfoAsync()
            {
                var val = new PlayerInfoItem();
                var reader = await adaptor.Query(dataId, "Item");
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<PlayerInfoItem>("Item");
                return val;
            }
            #endregion

        }
        public async Task<ISessionReader> GetSessionReader(UInt64 pid)
        {
            var dataId = string.Format("session:{0}", pid);
            var lockId = 0;
            return new Delegate<UInt64>(adaptor, lockId, pid, dataId);
        }
        public async Task<ISessionAccesser> GetSessionAccesser(UInt64 pid)
        {
            var dataId = string.Format("session:{0}", pid);
            var lockId = await adaptor.LockKey(dataId);
            return new Delegate<UInt64>(adaptor, lockId, pid, dataId);
        }
        public async Task<IAccountReader> GetAccountReader(String user)
        {
            var dataId = string.Format("account:{0}", user);
            var lockId = 0;
            return new Delegate<String>(adaptor, lockId, user, dataId);
        }
        public async Task<IAccountAccesser> GetAccountAccesser(String user)
        {
            var dataId = string.Format("account:{0}", user);
            var lockId = await adaptor.LockKey(dataId);
            return new Delegate<String>(adaptor, lockId, user, dataId);
        }
        public async Task<IPlayerInfoReader> GetPlayerInfoReader(UInt64 pid)
        {
            var dataId = string.Format("playerinfo:{0}", pid);
            var lockId = 0;
            return new Delegate<UInt64>(adaptor, lockId, pid, dataId);
        }
        public async Task<IPlayerInfoAccesser> GetPlayerInfoAccesser(UInt64 pid)
        {
            var dataId = string.Format("playerinfo:{0}", pid);
            var lockId = await adaptor.LockKey(dataId);
            return new Delegate<UInt64>(adaptor, lockId, pid, dataId);
        }

        private readonly RedisAdaptor adaptor;
        public RedisDataServiceDelegate(RedisAdaptor adaptor)
        {
            this.adaptor = adaptor;
        }
    }

    public class MysqlDataServiceDelegate
    {
        class Delegate<TKey> : IAccountAccesser, IPlayerInfoAccesser
        {
            public Delegate(MysqlAdaptor adaptor, TKey key)
            {
                this.adaptor = adaptor;
                this.key = key;
            }
            public Task<bool> SubmitChanges()
            {
                throw new NotImplementedException();
            }

            public Task<bool> SubmitChangesWith(params object[] others)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {

            }

            private readonly MysqlAdaptor adaptor;
            private readonly object key;
            private readonly List<IUpdateDataContext> updateQueue = new List<IUpdateDataContext>();

            #region Account
            public void UpdateAccount(Account account)
            {
                const string dataId = "INSERT INTO `mem_account`(`User`,`Pid`,`TestArr`,`TestDict`,`TestList`) VALUES(?1,?2,?3,?4,?5) ON DUPLICATE KEY UPDATE `Pid`=?2,`TestArr`=?3,`TestDict`=?4,`TestList`=?5";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("User", account.User)
                .WriteField("Pid", account.Pid)
                .WriteField("TestArr", account.TestArr)
                .WriteField("TestDict", account.TestDict)
                .WriteField("TestList", account.TestList)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdatePidAtAccount(UInt64 pid)
            {
                const string dataId = "UPDATE `mem_account` SET `Pid`=?2 WHERE `User`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("User", (String)key)
                .WriteField("Pid", pid)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateTestArrAtAccount(UInt64[] testarr)
            {
                const string dataId = "UPDATE `mem_account` SET `TestArr`=?2 WHERE `User`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("User", (String)key)
                .WriteField("TestArr", testarr)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateTestDictAtAccount(Dictionary<UInt64, UInt32[]> testdict)
            {
                const string dataId = "UPDATE `mem_account` SET `TestDict`=?2 WHERE `User`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("User", (String)key)
                .WriteField("TestDict", testdict)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateTestListAtAccount(List<List<UInt32>> testlist)
            {
                const string dataId = "UPDATE `mem_account` SET `TestList`=?2 WHERE `User`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("User", (String)key)
                .WriteField("TestList", testlist)
                ;
                updateQueue.Add(ctx);
            }
            public async Task<Account> LoadAccountAsync()
            {
                const string sql = "SELECT `User`,`Pid`,`TestArr`,`TestDict`,`TestList` FROM `mem_account` WHERE `User` = ?1";
                var val = new Account();
                var reader = await adaptor.Query(sql, (String)key);
                if (reader == null)
                {
                    return null;
                }
                val.User = (String)key;
                val.Pid = reader.ReadField<UInt64>("Pid");
                val.TestArr = reader.ReadField<UInt64[]>("TestArr");
                val.TestDict = reader.ReadField<Dictionary<UInt64, UInt32[]>>("TestDict");
                val.TestList = reader.ReadField<List<List<UInt32>>>("TestList");
                reader.Dispose();
                return val;
            }
            public async Task<UInt64> LoadPidAtAccountAsync()
            {
                const string sql = "SELECT `Pid` FROM `mem_account` WHERE `User` = ?1";
                var val = default(UInt64);
                var reader = await adaptor.Query(sql, (String)key);
                if (reader == null)
                {
                    return default(UInt64);
                }
                val = reader.ReadField<UInt64>("Pid");
                reader.Dispose();
                return val;
            }
            public async Task<UInt64[]> LoadTestArrAtAccountAsync()
            {
                const string sql = "SELECT `TestArr` FROM `mem_account` WHERE `User` = ?1";
                var val = default(UInt64[]);
                var reader = await adaptor.Query(sql, (String)key);
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<UInt64[]>("TestArr");
                reader.Dispose();
                return val;
            }
            public async Task<Dictionary<UInt64, UInt32[]>> LoadTestDictAtAccountAsync()
            {
                const string sql = "SELECT `TestDict` FROM `mem_account` WHERE `User` = ?1";
                var val = new Dictionary<UInt64, UInt32[]>();
                var reader = await adaptor.Query(sql, (String)key);
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<Dictionary<UInt64, UInt32[]>>("TestDict");
                reader.Dispose();
                return val;
            }
            public async Task<List<List<UInt32>>> LoadTestListAtAccountAsync()
            {
                const string sql = "SELECT `TestList` FROM `mem_account` WHERE `User` = ?1";
                var val = new List<List<UInt32>>();
                var reader = await adaptor.Query(sql, (String)key);
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<List<List<UInt32>>>("TestList");
                reader.Dispose();
                return val;
            }
            #endregion

            #region PlayerInfo
            public void UpdatePlayerInfo(PlayerInfo playerinfo)
            {
                const string dataId = "INSERT INTO `mem_player`(`Pid`,`Uid`,`Name`,`Level`,`Skill`,`Item`) VALUES(?1,?2,?3,?4,?5,?6) ON DUPLICATE KEY UPDATE `Uid`=?2,`Name`=?3,`Level`=?4,`Skill`=?5,`Item`=?6";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", playerinfo.Pid)
                .WriteField("Uid", playerinfo.Uid)
                .WriteField("Name", playerinfo.Name)
                .WriteField("Level", playerinfo.Level)
                .WriteField("Skill", playerinfo.Skill)
                .WriteField("Item", playerinfo.Item)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateUidAtPlayerInfo(UInt32 uid)
            {
                const string dataId = "UPDATE `mem_player` SET `Uid`=?2 WHERE `Pid`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", (UInt64)key)
                .WriteField("Uid", uid)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateNameAtPlayerInfo(String name)
            {
                const string dataId = "UPDATE `mem_player` SET `Name`=?2 WHERE `Pid`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", (UInt64)key)
                .WriteField("Name", name)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateLevelAtPlayerInfo(UInt32 level)
            {
                const string dataId = "UPDATE `mem_player` SET `Level`=?2 WHERE `Pid`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", (UInt64)key)
                .WriteField("Level", level)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateSkillAtPlayerInfo(PlayerInfoSkill skill)
            {
                const string dataId = "UPDATE `mem_player` SET `Skill`=?2 WHERE `Pid`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", (UInt64)key)
                .WriteField("Skill", skill)
                ;
                updateQueue.Add(ctx);
            }
            public void UpdateItemAtPlayerInfo(PlayerInfoItem item)
            {
                const string dataId = "UPDATE `mem_player` SET `Item`=?2 WHERE `Pid`=?1";
                var ctx =  adaptor.Update(dataId);

                ctx
                .WriteField("Pid", (UInt64)key)
                .WriteField("Item", item)
                ;
                updateQueue.Add(ctx);
            }
            public async Task<PlayerInfo> LoadPlayerInfoAsync()
            {
                const string sql = "SELECT `Pid`,`Uid`,`Name`,`Level`,`Skill`,`Item` FROM `mem_player` WHERE `Pid` = ?1";
                var val = new PlayerInfo();
                var reader = await adaptor.Query(sql, (UInt64)key);
                if (reader == null)
                {
                    return null;
                }
                val.Pid = (UInt64)key;
                val.Uid = reader.ReadField<UInt32>("Uid");
                val.Name = reader.ReadField<String>("Name");
                val.Level = reader.ReadField<UInt32>("Level");
                val.Skill = reader.ReadField<PlayerInfoSkill>("Skill");
                val.Item = reader.ReadField<PlayerInfoItem>("Item");
                reader.Dispose();
                return val;
            }
            public async Task<UInt32> LoadUidAtPlayerInfoAsync()
            {
                const string sql = "SELECT `Uid` FROM `mem_player` WHERE `Pid` = ?1";
                var val = default(UInt32);
                var reader = await adaptor.Query(sql, (UInt64)key);
                if (reader == null)
                {
                    return default(UInt32);
                }
                val = reader.ReadField<UInt32>("Uid");
                reader.Dispose();
                return val;
            }
            public async Task<String> LoadNameAtPlayerInfoAsync()
            {
                const string sql = "SELECT `Name` FROM `mem_player` WHERE `Pid` = ?1";
                var val = default(String);
                var reader = await adaptor.Query(sql, (UInt64)key);
                if (reader == null)
                {
                    return default(String);
                }
                val = reader.ReadField<String>("Name");
                reader.Dispose();
                return val;
            }
            public async Task<UInt32> LoadLevelAtPlayerInfoAsync()
            {
                const string sql = "SELECT `Level` FROM `mem_player` WHERE `Pid` = ?1";
                var val = default(UInt32);
                var reader = await adaptor.Query(sql, (UInt64)key);
                if (reader == null)
                {
                    return default(UInt32);
                }
                val = reader.ReadField<UInt32>("Level");
                reader.Dispose();
                return val;
            }
            public async Task<PlayerInfoSkill> LoadSkillAtPlayerInfoAsync()
            {
                const string sql = "SELECT `Skill` FROM `mem_player` WHERE `Pid` = ?1";
                var val = new PlayerInfoSkill();
                var reader = await adaptor.Query(sql, (UInt64)key);
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<PlayerInfoSkill>("Skill");
                reader.Dispose();
                return val;
            }
            public async Task<PlayerInfoItem> LoadItemAtPlayerInfoAsync()
            {
                const string sql = "SELECT `Item` FROM `mem_player` WHERE `Pid` = ?1";
                var val = new PlayerInfoItem();
                var reader = await adaptor.Query(sql, (UInt64)key);
                if (reader == null)
                {
                    return null;
                }
                val = reader.ReadField<PlayerInfoItem>("Item");
                reader.Dispose();
                return val;
            }
            #endregion

        }
        public async Task<IAccountReader> GetAccountReader(String user)
        {
            return new Delegate<String>(adaptor, user);
        }
        public async Task<IAccountAccesser> GetAccountAccesser(String user)
        {
            return new Delegate<String>(adaptor, user);
        }
        public async Task<IPlayerInfoReader> GetPlayerInfoReader(UInt64 pid)
        {
            return new Delegate<UInt64>(adaptor, pid);
        }
        public async Task<IPlayerInfoAccesser> GetPlayerInfoAccesser(UInt64 pid)
        {
            return new Delegate<UInt64>(adaptor, pid);
        }

        private readonly MysqlAdaptor adaptor;
        public MysqlDataServiceDelegate(MysqlAdaptor adaptor)
        {
            this.adaptor = adaptor;
        }
    }
}
