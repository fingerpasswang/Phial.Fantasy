using System;
using System.Collections.Generic;
using System.IO;
using DataAccess;
using RPCBase;

namespace Auto.Server
{
    public enum TowerState
    {
        Normal = 10,
        Building = 11,
        Upgrading = 12,
    }
    public class Session
    {
        public UInt64 Pid;

        public Byte[] SessionId;
        public void Write(BinaryWriter bw)
        {
            bw.Write((UInt64)Pid);
            if (SessionId == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((byte[])SessionId).Length);
                bw.Write((byte[])SessionId);
            }
        }
        public Session Read(BinaryReader br)
        {
            Pid = br.ReadUInt64();
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                SessionId = br.ReadBytes(br.ReadInt32());
            }
            return this;
        }
        public override string ToString()
        {
            return string.Format("Session:\nPid={0}\nSessionId={1}\n", Pid, SessionId != null ? string.Join(", ", Array.ConvertAll(SessionId, input => input.ToString())) : "null");
        }
    }
    public class Account
    {
        public String User;

        public UInt64 Pid;

        public UInt64[] TestArr;

        public Dictionary<UInt64, UInt32[]> TestDict;

        public List<List<UInt32>> TestList;
        public void Write(BinaryWriter bw)
        {
            if (User == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write((String)User);
            }
            bw.Write((UInt64)Pid);
            if (TestArr == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((UInt64[])TestArr).Length);
                foreach (var item0 in (UInt64[])TestArr)
                {
                    bw.Write((UInt64)item0);
                }
            }
            if (TestDict == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((Dictionary<UInt64, UInt32[]>)TestDict).Count);
                foreach (var item0 in (Dictionary<UInt64, UInt32[]>)TestDict)
                {
                    bw.Write((UInt64)(item0.Key));
                    bw.Write(((UInt32[])(item0.Value)).Length);
                    foreach (var item1 in (UInt32[])(item0.Value))
                    {
                        bw.Write((UInt32)item1);
                    }
                }
            }
            if (TestList == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((List<List<UInt32>>)TestList).Count);
                foreach (var item0 in (List<List<UInt32>>)TestList)
                {
                    bw.Write(((List<UInt32>)item0).Count);
                    foreach (var item1 in (List<UInt32>)item0)
                    {
                        bw.Write((UInt32)item1);
                    }
                }
            }
        }
        public Account Read(BinaryReader br)
        {
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                User = br.ReadString();
            }
            Pid = br.ReadUInt64();
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                {
                    var count0 = br.ReadInt32();
                    var arrayVal0 = new UInt64[count0];
                    if (count0 > 0)
                    {
                        for (int i0 = 0; i0 < count0; i0++)
                        {
                            arrayVal0[i0] = br.ReadUInt64();
                        }
                    }
                    TestArr = arrayVal0;
                }
            }
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                {
                    var count0 = br.ReadInt32();
                    var dictVal0 = new Dictionary<UInt64, UInt32[]>(count0);
                    if (count0 > 0)
                    {
                        for (int i0 = 0; i0 < count0; i0++)
                        {
                            var key0 = default(UInt64);
                            var value0 = default(UInt32[]);
                            key0 = br.ReadUInt64();
                            {
                                var count1 = br.ReadInt32();
                                var arrayVal1 = new UInt32[count1];
                                if (count1 > 0)
                                {
                                    for (int i1 = 0; i1 < count1; i1++)
                                    {
                                        arrayVal1[i1] = br.ReadUInt32();
                                    }
                                }
                                value0 = arrayVal1;
                            }
                            dictVal0.Add(key0, value0);
                        }
                    }
                    TestDict = dictVal0;
                }
            }
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                {
                    var count0 = br.ReadInt32();
                    var listVal0 = new List<List<UInt32>>(count0);
                    if (count0 > 0)
                    {
                        for (int i0 = 0; i0 < count0; i0++)
                        {
                            var item0 = default(List<UInt32>);
                            {
                                var count1 = br.ReadInt32();
                                var listVal1 = new List<UInt32>(count1);
                                if (count1 > 0)
                                {
                                    for (int i1 = 0; i1 < count1; i1++)
                                    {
                                        var item1 = default(UInt32);
                                        item1 = br.ReadUInt32();
                                        listVal1.Add(item1);
                                    }
                                }
                                item0 = listVal1;
                            }
                            listVal0.Add(item0);
                        }
                    }
                    TestList = listVal0;
                }
            }
            return this;
        }
        public override string ToString()
        {
            return string.Format("Account:\nUser={0}\nPid={1}\nTestArr={2}\nTestDict={3}\nTestList={4}\n", User, Pid, TestArr != null ? string.Join(", ", Array.ConvertAll(TestArr, input => input.ToString())) : "null", TestDict, TestList != null ? string.Join(", ", Array.ConvertAll(TestList.ToArray(), input => input.ToString())) : "null");
        }
    }
    public class PlayerInfo
    {
        public UInt64 Pid;

        public UInt32 Uid;

        public String Name;

        public UInt32 Level;

        public PlayerInfoSkill Skill;

        public PlayerInfoItem Item;
        public void Write(BinaryWriter bw)
        {
            bw.Write((UInt64)Pid);
            bw.Write((UInt32)Uid);
            if (Name == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write((String)Name);
            }
            bw.Write((UInt32)Level);
            if (Skill == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                ((PlayerInfoSkill)Skill).Write(bw);
            }
            if (Item == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                ((PlayerInfoItem)Item).Write(bw);
            }
        }
        public PlayerInfo Read(BinaryReader br)
        {
            Pid = br.ReadUInt64();
            Uid = br.ReadUInt32();
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                Name = br.ReadString();
            }
            Level = br.ReadUInt32();
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                Skill = (new PlayerInfoSkill()).Read(br);
            }
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                Item = (new PlayerInfoItem()).Read(br);
            }
            return this;
        }
        public override string ToString()
        {
            return string.Format("PlayerInfo:\nPid={0}\nUid={1}\nName={2}\nLevel={3}\nSkill={4}\nItem={5}\n", Pid, Uid, Name, Level, Skill, Item);
        }
    }
    public class PlayerInfoSkill
    {
        [FieldIndex(Index = 1)]
        public List<UInt32> SkillList;
        [FieldIndex(Index = 22)]
        public TestBaseClass Test;
        public void Write(BinaryWriter bw)
        {
            if (SkillList == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((List<UInt32>)SkillList).Count);
                foreach (var item0 in (List<UInt32>)SkillList)
                {
                    bw.Write((UInt32)item0);
                }
            }
            if (Test == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                ((TestBaseClass)Test).Write(bw);
            }
        }
        public PlayerInfoSkill Read(BinaryReader br)
        {
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                {
                    var count0 = br.ReadInt32();
                    var listVal0 = new List<UInt32>(count0);
                    if (count0 > 0)
                    {
                        for (int i0 = 0; i0 < count0; i0++)
                        {
                            var item0 = default(UInt32);
                            item0 = br.ReadUInt32();
                            listVal0.Add(item0);
                        }
                    }
                    SkillList = listVal0;
                }
            }
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                Test = (new TestBaseClass()).Read(br);
            }
            return this;
        }
        public override string ToString()
        {
            return string.Format("PlayerInfoSkill:\nSkillList={0}\nTest={1}\n", SkillList != null ? string.Join(", ", Array.ConvertAll(SkillList.ToArray(), input => input.ToString())) : "null", Test);
        }
    }
    public class TestBaseClass
    {
        [FieldIndex(Index = 1)]
        public Int32 baseId;
        public virtual void Write(BinaryWriter bw)
        {
            bw.Write((byte)1);
            bw.Write((Int32)baseId);
        }
        public new TestBaseClass ReadImpl(BinaryReader br)
        {
            baseId = br.ReadInt32();
            return this;
        }
        public new TestBaseClass Read(BinaryReader br)
        {
            return (TestBaseClass)ReadStatic(br);
        }
        public static TestBaseClass ReadStatic(BinaryReader br)
        {
            byte tp = br.ReadByte();
            if (tp != (byte)SerializeObjectMark.IsNull)
            {
                switch (tp)
                {
                case 2:
                    return (new TestDerived1Class()).ReadImpl(br);
                case 3:
                    return (new TestDerived2Class()).ReadImpl(br);
                case 4:
                    return (new TestDerived11Class()).ReadImpl(br);
                default:
                    return (new TestBaseClass()).ReadImpl(br);
                }
            }
            return null;
        }
        public override string ToString()
        {
            return string.Format("TestBaseClass:\nbaseId={0}\n", baseId);
        }
    }
    public class TestDerived1Class: TestBaseClass
    {
        [FieldIndex(Index = 11)]
        public Int32 derived1Id;
        public override void Write(BinaryWriter bw)
        {
            bw.Write((byte)2);
            bw.Write((Int32)derived1Id);
            bw.Write((Int32)baseId);
        }
        public new TestDerived1Class ReadImpl(BinaryReader br)
        {
            derived1Id = br.ReadInt32();
            baseId = br.ReadInt32();
            return this;
        }
        public new TestDerived1Class Read(BinaryReader br)
        {
            return (TestDerived1Class)ReadStatic(br);
        }
        public override string ToString()
        {
            return string.Format("TestDerived1Class:\nderived1Id={0}\nbaseId={1}\n", derived1Id, baseId);
        }
    }
    public class TestDerived2Class: TestBaseClass
    {
        [FieldIndex(Index = 21)]
        public Int32 derived2Id;
        public override void Write(BinaryWriter bw)
        {
            bw.Write((byte)3);
            bw.Write((Int32)derived2Id);
            bw.Write((Int32)baseId);
        }
        public new TestDerived2Class ReadImpl(BinaryReader br)
        {
            derived2Id = br.ReadInt32();
            baseId = br.ReadInt32();
            return this;
        }
        public new TestDerived2Class Read(BinaryReader br)
        {
            return (TestDerived2Class)ReadStatic(br);
        }
        public override string ToString()
        {
            return string.Format("TestDerived2Class:\nderived2Id={0}\nbaseId={1}\n", derived2Id, baseId);
        }
    }
    public class TestDerived11Class: TestDerived1Class
    {
        [FieldIndex(Index = 111)]
        public Int32 derived11Id;

        public override void Write(BinaryWriter bw)
        {
            bw.Write((byte)4);
            bw.Write((Int32)derived11Id);
            bw.Write((Int32)derived1Id);
            bw.Write((Int32)baseId);
        }
        public new TestDerived11Class ReadImpl(BinaryReader br)
        {
            derived11Id = br.ReadInt32();
            derived1Id = br.ReadInt32();
            baseId = br.ReadInt32();
            return this;
        }
        public new TestDerived11Class Read(BinaryReader br)
        {
            return (TestDerived11Class)ReadStatic(br);
        }
        public override string ToString()
        {
            return string.Format("TestDerived11Class:\nderived11Id={0}\nderived1Id={1}\nbaseId={2}\n", derived11Id, derived1Id, baseId);
        }
    }
    public class PlayerInfoItem
    {
        [FieldIndex(Index = 1)]
        public List<UInt32> ItemList;
        [FieldIndex(Index = 2)]
        public UInt32 Money;
        public void Write(BinaryWriter bw)
        {
            if (ItemList == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((List<UInt32>)ItemList).Count);
                foreach (var item0 in (List<UInt32>)ItemList)
                {
                    bw.Write((UInt32)item0);
                }
            }
            bw.Write((UInt32)Money);
        }
        public PlayerInfoItem Read(BinaryReader br)
        {
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                {
                    var count0 = br.ReadInt32();
                    var listVal0 = new List<UInt32>(count0);
                    if (count0 > 0)
                    {
                        for (int i0 = 0; i0 < count0; i0++)
                        {
                            var item0 = default(UInt32);
                            item0 = br.ReadUInt32();
                            listVal0.Add(item0);
                        }
                    }
                    ItemList = listVal0;
                }
            }
            Money = br.ReadUInt32();
            return this;
        }
        public override string ToString()
        {
            return string.Format("PlayerInfoItem:\nItemList={0}\nMoney={1}\n", ItemList != null ? string.Join(", ", Array.ConvertAll(ItemList.ToArray(), input => input.ToString())) : "null", Money);
        }
    }
    public class ServerList
    {
        public UInt64 Pid;

        public Byte[] SessionId;

        public List<String> Servers;
        public void Write(BinaryWriter bw)
        {
            bw.Write((UInt64)Pid);
            if (SessionId == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((byte[])SessionId).Length);
                bw.Write((byte[])SessionId);
            }
            if (Servers == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                bw.Write(((List<String>)Servers).Count);
                foreach (var item0 in (List<String>)Servers)
                {
                    bw.Write((String)item0);
                }
            }
        }
        public ServerList Read(BinaryReader br)
        {
            Pid = br.ReadUInt64();
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                SessionId = br.ReadBytes(br.ReadInt32());
            }
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                {
                    var count0 = br.ReadInt32();
                    var listVal0 = new List<String>(count0);
                    if (count0 > 0)
                    {
                        for (int i0 = 0; i0 < count0; i0++)
                        {
                            var item0 = default(String);
                            item0 = br.ReadString();
                            listVal0.Add(item0);
                        }
                    }
                    Servers = listVal0;
                }
            }
            return this;
        }
        public override string ToString()
        {
            return string.Format("ServerList:\nPid={0}\nSessionId={1}\nServers={2}\n", Pid, SessionId != null ? string.Join(", ", Array.ConvertAll(SessionId, input => input.ToString())) : "null", Servers != null ? string.Join(", ", Array.ConvertAll(Servers.ToArray(), input => input.ToString())) : "null");
        }
    }

    public static class ModelVersionMeta
    {
        public static string GetModelVersion()
        {
            return "ea147886109f9a1fa3e8457bf0589a38";
        }
    }
}
