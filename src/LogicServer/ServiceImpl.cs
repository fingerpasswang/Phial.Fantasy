using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;
using ServerUtils;

namespace LogicServer
{
    class LogicClientServiceImpl : ILogicClientImpl
    {
        private async Task<bool> CheckValid(ulong pid)
        {
            using (var s = await Program.CacheService.GetSessionReader(pid))
            {
                var sid = await s.LoadSessionIdAtSessionAsync();

                if (sid == null)
                {
                    return false;
                }

                Console.WriteLine("LogicServiceImpl.CheckValid sid={0} sessionid={1}", new Guid(sid), new Guid(sessionId));

                if (!sid.SessionIdEquals(sessionId))
                {
                    Console.WriteLine("LogicServiceImpl.CheckValid sid != sessionid");

                    // todo sync a server message error
                    return false;
                }

                Console.WriteLine("LogicServiceImpl.CheckValid sid == sessionid");

                return true;
            }
        }

        public async Task<bool> AskChangeName(ulong pid, string newName)
        {
            if (!await CheckValid(pid))
            {
                return false;
            }

            using (var accesser = await Program.CacheService.GetPlayerInfoAccesser(pid))
            {
                var info = await accesser.LoadPlayerInfoAsync();

                Console.WriteLine("LogicServiceImpl.AskChangeName load PlayerInfo complete pid={0}", pid);

                info.Name = newName;

                accesser.UpdatePlayerInfo(info);

                await accesser.SubmitChanges();

                Program.ClientLogicService.Forward(sessionId).ServerMessageOk();

                return true;
            }
        }

        public async Task<bool> AskAddMoney(ulong pid, uint money)
        {
            if (!await CheckValid(pid))
            {
                return false;
            }

            using (var accesser = await Program.CacheService.GetPlayerInfoAccesser(pid))
            {
                var infoItem = await accesser.LoadItemAtPlayerInfoAsync();

                Console.WriteLine("LogicServiceImpl.AskAddMoney load InfoItem complete pid={0}", pid);

                if (infoItem == null)
                {
                    Console.WriteLine("LogicServiceImpl.AskAddMoney InfoItem is null pid={0}", pid);
                    infoItem = new PlayerInfoItem() { ItemList = new List<uint>() };
                }

                infoItem.Money += money;

                accesser.UpdateItemAtPlayerInfo(infoItem);

                await accesser.SubmitChanges();

                Program.ClientLogicService.Forward(sessionId).ServerMessageOk();

                return true;
            }
        }

        public async Task<bool> AskLearnSkill(ulong pid, uint skillId)
        {
            if (!await CheckValid(pid))
            {
                return false;
            }

            using (var accesser = await Program.CacheService.GetPlayerInfoAccesser(pid))
            {
                var infoSkill = await accesser.LoadSkillAtPlayerInfoAsync();

                Console.WriteLine("LogicServiceImpl.AskLearnSkill load InfoSkill complete pid={0}", pid);

                if (infoSkill == null)
                {
                    Console.WriteLine("LogicServiceImpl.AskLearnSkill InfoSkill is null pid={0}", pid);
                    infoSkill = new PlayerInfoSkill() { SkillList = new List<uint>() };
                }
                if (infoSkill.SkillList == null)
                {
                    infoSkill.SkillList = new List<uint>();
                }

                infoSkill.SkillList.Add(skillId);

                accesser.UpdateSkillAtPlayerInfo(infoSkill);

                await accesser.SubmitChanges();

                Program.ClientLogicService.Forward(sessionId).ServerMessageOk();

                return true;
            }
        }

        public Task<bool> TestEnum(ulong pid, TowerState state)
        {
            throw new NotImplementedException();
        }

        public Task<bool[]> TestArray(ulong[] pids, TowerState state)
        {
            throw new NotImplementedException();
        }

        public Task<List<bool>> TestList(List<bool> pids, TowerState state)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<bool, byte[]>> TestDict(Dictionary<bool, PlayerInfo> pids, TowerState state)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerInfo> RequestPlayerInfo(ulong pid)
        {
            if (!await CheckValid(pid))
            {
                return null;
            }

            using (var accesser = await Program.CacheService.GetPlayerInfoAccesser(pid))
            {
                var info = await accesser.LoadPlayerInfoAsync();

                if (info != null)
                {
                    Console.WriteLine("LogicServiceImpl.RequestPlayerInfo load PlayerInfo complete pid={0}", pid);

                    return info;
                }

                Console.WriteLine("LogicServiceImpl.RequestPlayerInfo cache miss pid={0}", pid);

                using (var dbAccesser = await Program.DbService.GetPlayerInfoReader(pid))
                {
                    info = await dbAccesser.LoadPlayerInfoAsync();

                    if (info == null)
                    {
                        Console.WriteLine("LogicServiceImpl.RequestPlayerInfo db not exist create new pid={0}", pid);

                        info = new PlayerInfo()
                        {
                            Pid = pid,
                            Name = "default",
                            Level = 1,
                            Item = new PlayerInfoItem() { ItemList = new List<uint>() { 10001 }, Money = 100 },
                            Skill = new PlayerInfoSkill() { SkillList = new List<uint>() { 1001 }, },
                        };
                    }

                    accesser.UpdatePlayerInfo(info);

                    await accesser.SubmitChanges();

                    return info;
                }
            }
        }

        public async Task<TestBaseClass> TestHierarchy(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11)
        {
            Console.WriteLine(b);
            Console.WriteLine(d1);
            Console.WriteLine(d11);



            return null;
        }

        public Task<TestBaseClass> TestHierarch2y(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11)
        {
            throw new NotImplementedException();
        }

        private readonly byte[] sessionId;

        private LogicClientServiceImpl(byte[] sid)
        {
            sessionId = sid;
        }
        public LogicClientServiceImpl()
        {
        }

        public IRpcImplInstnce SetSourceUuid(byte[] sid)
        {
            return new LogicClientServiceImpl(sid);
        }
    }
}
