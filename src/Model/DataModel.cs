using System.Collections.Generic;
using DataAccess;

namespace DataModel
{
    public enum TowerState
    {
        Normal = 10,
        Building,
        Upgrading,
    }

    [Serialize]
    [MainCache(CacheName = "session")]
    public class Session
    {
        [Key]
        public ulong Pid;

        [Field]
        public byte[] SessionId;
    }

    [Serialize]
    [MainCache(CacheName = "account")]
    [Persistence(DbTableName = "mem_account")]
    public class Account
    {
        [Key]
        public string User;

        [Field]
        public ulong Pid;
        [Field]
        public ulong[] TestArr;
        [Field]
        public Dictionary<ulong, uint[]> TestDict;
        [Field]
        public List<List<uint>> TestList;
    }

    [Serialize]
    [MainCache(CacheName = "player")]
    [Persistence(DbTableName = "mem_player")]
    public class PlayerInfo
    {
        [Key]
        public ulong Pid;

        [Field]
        public uint Uid;
        [Field]
        public string Name;
        [Field]
        public uint Level;

        //[Field]
        //public TowerState Test;

        [Field]
        public PlayerInfoSkill Skill;
        [Field]
        public PlayerInfoItem Item;
    }

    [Serialize]
    public class PlayerInfoSkill
    {
        [FieldIndex(Index = 1)]
        public List<uint> SkillList;

        // for test
        [FieldIndex(Index = 22)]
        public TestBaseClass Test;
    }

    [Serialize]
    public class TestBaseClass
    {
        [FieldIndex(Index = 1)]
        public int baseId;
    }

    [Serialize]
    public class TestDerived1Class : TestBaseClass
    {
        [FieldIndex(Index = 11)]
        public int derived1Id;
    }

    [Serialize]
    public class TestDerived2Class : TestBaseClass
    {
        [FieldIndex(Index = 21)]
        public int derived2Id;
    }

    [Serialize]
    public class TestDerived11Class : TestDerived1Class
    {
        [FieldIndex(Index = 111)]
        public int derived11Id;
    }


    [Serialize]
    public class PlayerInfoItem
    {
        [FieldIndex(Index = 1)]
        public List<uint> ItemList;
        [FieldIndex(Index = 2)]
        public uint Money;
    }

    [Serialize]
    public class ServerList
    {
        public ulong Pid;
        public byte[] SessionId;
        public List<string> Servers;
    }
}
