using System;
using System.IO;
using System.Collections.Generic;
using RPCBase;

namespace Auto.Client
{

    public static class AutoInit
    {
        internal const string LogicClient = "LogicClient";
        internal const string ClientScene = "ClientScene";
        internal const string SceneClient = "SceneClient";
        internal const string LoginClient = "LoginClient";
        internal const string ClientLogic = "ClientLogic";
        static AutoInit()
        {
            MetaData.SetServiceId(typeof(IClientSceneImpl), ClientScene);
            MetaData.SetMethodSerializer(typeof(IClientSceneImpl), ClientSceneSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(IClientSceneImpl), ClientSceneDispatcher.Instance);


            MetaData.SetServiceId(typeof(IClientLogicImpl), ClientLogic);
            MetaData.SetMethodSerializer(typeof(IClientLogicImpl), ClientLogicSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(IClientLogicImpl), ClientLogicDispatcher.Instance);
            MetaData.SetServiceRoutingRule(LogicClient, new RoutingRule()
            {
                MqttRule = new RoutingRule.MqttRoutingRule()
                {
                    PublishKey = (districts, uuid) => string.Format("{0}/LogicClient/invoke", districts),
                    SubscribeKey = (districts, uuid) => string.Format("{0}/LogicClient/return/{1}", districts, uuid),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 4,},
            });
            MetaData.SetServiceRoutingRule(ClientScene, new RoutingRule()
            {
                MqttRule = new RoutingRule.MqttRoutingRule()
                {
                    SubscribeKey = (districts, uuid) => string.Format("{0}/ClientScene/sync/{1}", districts, uuid),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 5,},
            });
            MetaData.SetServiceRoutingRule(SceneClient, new RoutingRule()
            {
                MqttRule = new RoutingRule.MqttRoutingRule()
                {
                    PublishKey = (districts, uuid) => string.Format("{0}/SceneClient/invoke", districts),
                    SubscribeKey = (districts, uuid) => string.Format("{0}/SceneClient/return/{1}", districts, uuid),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 6,},
            });
            MetaData.SetServiceRoutingRule(LoginClient, new RoutingRule()
            {
                MqttRule = new RoutingRule.MqttRoutingRule()
                {
                    PublishKey = (districts, uuid) => string.Format("padding/LoginClient/invoke"),
                    SubscribeKey = (districts, uuid) => string.Format("padding/LoginClient/return/{0}", uuid),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 7,},
            });
            MetaData.SetServiceRoutingRule(ClientLogic, new RoutingRule()
            {
                MqttRule = new RoutingRule.MqttRoutingRule()
                {
                    SubscribeKey = (districts, uuid) => string.Format("{0}/ClientLogic/sync/{1}", districts, uuid),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 8,},
            });
        } public static void Init() {}
    }
    #region LogicClient
    public interface ILogicClientInvoke
    {
        InvokeOperation<Boolean> AskChangeName(UInt64 pid, String newName);
        InvokeOperation<Boolean> AskAddMoney(UInt64 pid, UInt32 money);
        InvokeOperation<Boolean> AskLearnSkill(UInt64 pid, UInt32 skillId);
        InvokeOperation<Boolean> TestEnum(UInt64 pid, TowerState state);
        InvokeOperation<Boolean[]> TestArray(UInt64[] pids, TowerState state);
        InvokeOperation<List<Boolean>> TestList(List<Boolean> pids, TowerState state);
        InvokeOperation<Dictionary<Boolean, Byte[]>> TestDict(Dictionary<Boolean, PlayerInfo> pids, TowerState state);
        InvokeOperation<PlayerInfo> RequestPlayerInfo(UInt64 pid);
        InvokeOperation<TestBaseClass> TestHierarchy(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11);
        InvokeOperation<TestBaseClass> TestHierarch2y(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11);
    }

    public class LogicClientServiceDelegate: ILogicClientInvoke
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public LogicClientServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, LogicClientSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.LogicClient));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.LogicClient);
        }

        #region meta data
        private static class MethodId
        {
            public const uint AskChangeName = 1;
            public const uint AskAddMoney = 2;
            public const uint AskLearnSkill = 3;
            public const uint TestEnum = 4;
            public const uint TestArray = 5;
            public const uint TestList = 6;
            public const uint TestDict = 7;
            public const uint RequestPlayerInfo = 8;
            public const uint TestHierarchy = 9;
            public const uint TestHierarch2y = 10;
        }
        #endregion
        public InvokeOperation<Boolean> AskChangeName(UInt64 pid, String newName)
        {
            return serviceDelegateStub.Invoke<Boolean>(MethodId.AskChangeName, null, pid, newName);
        }

        public InvokeOperation<Boolean> AskAddMoney(UInt64 pid, UInt32 money)
        {
            return serviceDelegateStub.Invoke<Boolean>(MethodId.AskAddMoney, null, pid, money);
        }

        public InvokeOperation<Boolean> AskLearnSkill(UInt64 pid, UInt32 skillId)
        {
            return serviceDelegateStub.Invoke<Boolean>(MethodId.AskLearnSkill, null, pid, skillId);
        }

        public InvokeOperation<Boolean> TestEnum(UInt64 pid, TowerState state)
        {
            return serviceDelegateStub.Invoke<Boolean>(MethodId.TestEnum, null, pid, state);
        }

        public InvokeOperation<Boolean[]> TestArray(UInt64[] pids, TowerState state)
        {
            return serviceDelegateStub.Invoke<Boolean[]>(MethodId.TestArray, null, pids, state);
        }

        public InvokeOperation<List<Boolean>> TestList(List<Boolean> pids, TowerState state)
        {
            return serviceDelegateStub.Invoke<List<Boolean>>(MethodId.TestList, null, pids, state);
        }

        public InvokeOperation<Dictionary<Boolean, Byte[]>> TestDict(Dictionary<Boolean, PlayerInfo> pids, TowerState state)
        {
            return serviceDelegateStub.Invoke<Dictionary<Boolean, Byte[]>>(MethodId.TestDict, null, pids, state);
        }

        public InvokeOperation<PlayerInfo> RequestPlayerInfo(UInt64 pid)
        {
            return serviceDelegateStub.Invoke<PlayerInfo>(MethodId.RequestPlayerInfo, null, pid);
        }

        public InvokeOperation<TestBaseClass> TestHierarchy(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11)
        {
            return serviceDelegateStub.Invoke<TestBaseClass>(MethodId.TestHierarchy, null, b, d1, d11);
        }

        public InvokeOperation<TestBaseClass> TestHierarch2y(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11)
        {
            return serviceDelegateStub.Invoke<TestBaseClass>(MethodId.TestHierarch2y, null, b, d1, d11);
        }









    }

    public class LogicClientSerializer : IMethodSerializer
    {
        public static readonly LogicClientSerializer Instance = new LogicClientSerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[2];
                method.Args[0] = br.ReadUInt64();
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[1] = br.ReadString();
                }
                break;
            case 2:
                method.Args = new object[2];
                method.Args[0] = br.ReadUInt64();
                method.Args[1] = br.ReadUInt32();
                break;
            case 3:
                method.Args = new object[2];
                method.Args[0] = br.ReadUInt64();
                method.Args[1] = br.ReadUInt32();
                break;
            case 4:
                method.Args = new object[2];
                method.Args[0] = br.ReadUInt64();
                method.Args[1] = (TowerState)br.ReadInt32();
                break;
            case 5:
                method.Args = new object[2];
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
                        method.Args[0] = arrayVal0;
                    }
                }
                method.Args[1] = (TowerState)br.ReadInt32();
                break;
            case 6:
                method.Args = new object[2];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    {
                        var count0 = br.ReadInt32();
                        var listVal0 = new List<Boolean>(count0);
                        if (count0 > 0)
                        {
                            for (int i0 = 0; i0 < count0; i0++)
                            {
                                var item0 = default(Boolean);
                                item0 = br.ReadBoolean();
                                listVal0.Add(item0);
                            }
                        }
                        method.Args[0] = listVal0;
                    }
                }
                method.Args[1] = (TowerState)br.ReadInt32();
                break;
            case 7:
                method.Args = new object[2];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    {
                        var count0 = br.ReadInt32();
                        var dictVal0 = new Dictionary<Boolean, PlayerInfo>(count0);
                        if (count0 > 0)
                        {
                            for (int i0 = 0; i0 < count0; i0++)
                            {
                                var key0 = default(Boolean);
                                var value0 = default(PlayerInfo);
                                key0 = br.ReadBoolean();
                                value0 = (new PlayerInfo()).Read(br);
                                dictVal0.Add(key0, value0);
                            }
                        }
                        method.Args[0] = dictVal0;
                    }
                }
                method.Args[1] = (TowerState)br.ReadInt32();
                break;
            case 8:
                method.Args = new object[1];
                method.Args[0] = br.ReadUInt64();
                break;
            case 9:
                method.Args = new object[3];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = (new TestBaseClass()).Read(br);
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[1] = (new TestDerived1Class()).Read(br);
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[2] = (new TestDerived11Class()).Read(br);
                }
                break;
            case 10:
                method.Args = new object[3];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = (new TestBaseClass()).Read(br);
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[1] = (new TestDerived1Class()).Read(br);
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[2] = (new TestDerived11Class()).Read(br);
                }
                break;
            }
            return method;
        }
        public void Write(uint methodId, object[] args, BinaryWriter bw)
        {
            bw.Write(methodId);
            switch (methodId)
            {
            case 1:
            {
                bw.Write((UInt64)args[0]);
                if (args[1] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write((String)args[1]);
                }
                break;
            }
            case 2:
            {
                bw.Write((UInt64)args[0]);
                bw.Write((UInt32)args[1]);
                break;
            }
            case 3:
            {
                bw.Write((UInt64)args[0]);
                bw.Write((UInt32)args[1]);
                break;
            }
            case 4:
            {
                bw.Write((UInt64)args[0]);
                bw.Write((int)args[1]);
                break;
            }
            case 5:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((UInt64[])args[0]).Length);
                    foreach (var item0 in (UInt64[])args[0])
                    {
                        bw.Write((UInt64)item0);
                    }
                }
                bw.Write((int)args[1]);
                break;
            }
            case 6:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((List<Boolean>)args[0]).Count);
                    foreach (var item0 in (List<Boolean>)args[0])
                    {
                        bw.Write((Boolean)item0);
                    }
                }
                bw.Write((int)args[1]);
                break;
            }
            case 7:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((Dictionary<Boolean, PlayerInfo>)args[0]).Count);
                    foreach (var item0 in (Dictionary<Boolean, PlayerInfo>)args[0])
                    {
                        bw.Write((Boolean)(item0.Key));
                        ((PlayerInfo)(item0.Value)).Write(bw);
                    }
                }
                bw.Write((int)args[1]);
                break;
            }
            case 8:
            {
                bw.Write((UInt64)args[0]);
                break;
            }
            case 9:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestBaseClass)args[0]).Write(bw);
                }
                if (args[1] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestDerived1Class)args[1]).Write(bw);
                }
                if (args[2] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestDerived11Class)args[2]).Write(bw);
                }
                break;
            }
            case 10:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestBaseClass)args[0]).Write(bw);
                }
                if (args[1] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestDerived1Class)args[1]).Write(bw);
                }
                if (args[2] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestDerived11Class)args[2]).Write(bw);
                }
                break;
            }
            }
        }
        public object ReadReturn(uint methodId, BinaryReader br)
        {
            var returnVal = new object();
            switch (methodId)
            {
            case 1:
            {
                returnVal = br.ReadBoolean();
                break;
            }
            case 2:
            {
                returnVal = br.ReadBoolean();
                break;
            }
            case 3:
            {
                returnVal = br.ReadBoolean();
                break;
            }
            case 4:
            {
                returnVal = br.ReadBoolean();
                break;
            }
            case 5:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    {
                        var count0 = br.ReadInt32();
                        var arrayVal0 = new Boolean[count0];
                        if (count0 > 0)
                        {
                            for (int i0 = 0; i0 < count0; i0++)
                            {
                                arrayVal0[i0] = br.ReadBoolean();
                            }
                        }
                        returnVal = arrayVal0;
                    }
                }
                break;
            }
            case 6:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    {
                        var count0 = br.ReadInt32();
                        var listVal0 = new List<Boolean>(count0);
                        if (count0 > 0)
                        {
                            for (int i0 = 0; i0 < count0; i0++)
                            {
                                    var item0 = default(Boolean);
                                    item0 = br.ReadBoolean();
                                    listVal0.Add(item0);
                                }
                            }
                            returnVal = listVal0;
                        }
                    }
                    break;
                }
            case 7:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    {
                        var count0 = br.ReadInt32();
                        var dictVal0 = new Dictionary<Boolean, Byte[]>(count0);
                        if (count0 > 0)
                        {
                            for (int i0 = 0; i0 < count0; i0++)
                            {
                                    var key0 = default(Boolean);
                                    var value0 = default(Byte[]);
                                    key0 = br.ReadBoolean();
                                    value0 = br.ReadBytes(br.ReadInt32());
                                    dictVal0.Add(key0, value0);
                                }
                            }
                            returnVal = dictVal0;
                        }
                    }
                    break;
                }
            case 8:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    returnVal = (new PlayerInfo()).Read(br);
                }
                break;
            }
            case 9:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    returnVal = (new TestBaseClass()).Read(br);
                }
                break;
            }
            case 10:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    returnVal = (new TestBaseClass()).Read(br);
                }
                break;
            }
            }
            return returnVal;
        }
        public void WriteReturn(RpcMethod method, BinaryWriter bw, object value)
        {
            switch (method.MethodId)
            {
            case 1:
                bw.Write((Boolean)value);
                break;
            case 2:
                bw.Write((Boolean)value);
                break;
            case 3:
                bw.Write((Boolean)value);
                break;
            case 4:
                bw.Write((Boolean)value);
                break;
            case 5:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((Boolean[])value).Length);
                    foreach (var item0 in (Boolean[])value)
                    {
                        bw.Write((Boolean)item0);
                    }
                }
                break;
            case 6:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((List<Boolean>)value).Count);
                    foreach (var item0 in (List<Boolean>)value)
                    {
                        bw.Write((Boolean)item0);
                    }
                }
                break;
            case 7:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((Dictionary<Boolean, Byte[]>)value).Count);
                    foreach (var item0 in (Dictionary<Boolean, Byte[]>)value)
                    {
                        bw.Write((Boolean)(item0.Key));
                        bw.Write(((byte[])(item0.Value)).Length);
                        bw.Write((byte[])(item0.Value));
                    }
                }
                break;
            case 8:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((PlayerInfo)value).Write(bw);
                }
                break;
            case 9:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestBaseClass)value).Write(bw);
                }
                break;
            case 10:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((TestBaseClass)value).Write(bw);
                }
                break;
            }
        }
    }
    #endregion
    #region ClientScene
    public interface IClientSceneImpl: IRpcImplInstnce
    {
        void SyncPosition(Int32 x, Int32 y);
    }


    public class ClientSceneDispatcher : IServiceMethodDispatcher
    {
        public static readonly ClientSceneDispatcher Instance = new ClientSceneDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((IClientSceneImpl)impl).SyncPosition((Int32)(method.Args[0]), (Int32)(method.Args[1]));
                break;
            }
        }
    }
    public class ClientSceneSerializer : IMethodSerializer
    {
        public static readonly ClientSceneSerializer Instance = new ClientSceneSerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[2];
                method.Args[0] = br.ReadInt32();
                method.Args[1] = br.ReadInt32();
                break;
            }
            return method;
        }
        public void Write(uint methodId, object[] args, BinaryWriter bw)
        {
            bw.Write(methodId);
            switch (methodId)
            {
            case 1:
            {
                bw.Write((Int32)args[0]);
                bw.Write((Int32)args[1]);
                break;
            }
            }
        }
        public object ReadReturn(uint methodId, BinaryReader br)
        {
            var returnVal = new object();
            return returnVal;
        }
        public void WriteReturn(RpcMethod method, BinaryWriter bw, object value) {}
    }
    #endregion
    #region SceneClient
    public interface ISceneClientInvoke
    {
        InvokeOperation<Boolean> AskMoveTo(Int32 x, Int32 y);
    }

    public class SceneClientServiceDelegate: ISceneClientInvoke
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public SceneClientServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, SceneClientSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.SceneClient));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.SceneClient);
        }

        #region meta data
        private static class MethodId
        {
            public const uint AskMoveTo = 1;
        }
        #endregion
        public InvokeOperation<Boolean> AskMoveTo(Int32 x, Int32 y)
        {
            return serviceDelegateStub.Invoke<Boolean>(MethodId.AskMoveTo, null, x, y);
        }
    }

    public class SceneClientSerializer : IMethodSerializer
    {
        public static readonly SceneClientSerializer Instance = new SceneClientSerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[2];
                method.Args[0] = br.ReadInt32();
                method.Args[1] = br.ReadInt32();
                break;
            }
            return method;
        }
        public void Write(uint methodId, object[] args, BinaryWriter bw)
        {
            bw.Write(methodId);
            switch (methodId)
            {
            case 1:
            {
                bw.Write((Int32)args[0]);
                bw.Write((Int32)args[1]);
                break;
            }
            }
        }
        public object ReadReturn(uint methodId, BinaryReader br)
        {
            var returnVal = new object();
            switch (methodId)
            {
            case 1:
            {
                returnVal = br.ReadBoolean();
                break;
            }
            }
            return returnVal;
        }
        public void WriteReturn(RpcMethod method, BinaryWriter bw, object value)
        {
            switch (method.MethodId)
            {
            case 1:
                bw.Write((Boolean)value);
                break;
            }
        }
    }
    #endregion
    #region LoginClient
    public interface ILoginClientInvoke
    {
        InvokeOperation<ServerList> AskLogin(String username, String password, Byte[] uuid);
    }

    public class LoginClientServiceDelegate: ILoginClientInvoke
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public LoginClientServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, LoginClientSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.LoginClient));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.LoginClient);
        }

        #region meta data
        private static class MethodId
        {
            public const uint AskLogin = 1;
        }
        #endregion
        public InvokeOperation<ServerList> AskLogin(String username, String password, Byte[] uuid)
        {
            return serviceDelegateStub.Invoke<ServerList>(MethodId.AskLogin, null, username, password, uuid);
        }
    }

    public class LoginClientSerializer : IMethodSerializer
    {
        public static readonly LoginClientSerializer Instance = new LoginClientSerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[3];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = br.ReadString();
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[1] = br.ReadString();
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[2] = br.ReadBytes(br.ReadInt32());
                }
                break;
            }
            return method;
        }
        public void Write(uint methodId, object[] args, BinaryWriter bw)
        {
            bw.Write(methodId);
            switch (methodId)
            {
            case 1:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write((String)args[0]);
                }
                if (args[1] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write((String)args[1]);
                }
                if (args[2] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((byte[])args[2]).Length);
                    bw.Write((byte[])args[2]);
                }
                break;
            }
            }
        }
        public object ReadReturn(uint methodId, BinaryReader br)
        {
            var returnVal = new object();
            switch (methodId)
            {
            case 1:
            {
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    returnVal = (new ServerList()).Read(br);
                }
                break;
            }
            }
            return returnVal;
        }
        public void WriteReturn(RpcMethod method, BinaryWriter bw, object value)
        {
            switch (method.MethodId)
            {
            case 1:
                if (value == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    ((ServerList)value).Write(bw);
                }
                break;
            }
        }
    }
    #endregion
    #region ClientLogic
    public interface IClientLogicImpl: IRpcImplInstnce
    {
        void ServerMessageOk();
    }


    public class ClientLogicDispatcher : IServiceMethodDispatcher
    {
        public static readonly ClientLogicDispatcher Instance = new ClientLogicDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((IClientLogicImpl)impl).ServerMessageOk();
                break;
            }
        }
    }
    public class ClientLogicSerializer : IMethodSerializer
    {
        public static readonly ClientLogicSerializer Instance = new ClientLogicSerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                break;
            }
            return method;
        }
        public void Write(uint methodId, object[] args, BinaryWriter bw)
        {
            bw.Write(methodId);
            switch (methodId)
            {
            case 1:
            {
                break;
            }
            }
        }
        public object ReadReturn(uint methodId, BinaryReader br)
        {
            var returnVal = new object();
            return returnVal;
        }
        public void WriteReturn(RpcMethod method, BinaryWriter bw, object value) {}
    }
    #endregion

    public static class ServiceVersionMeta
    {
        public static string GetServiceVersion()
        {
            return "c3d1e8ffa11351519bf0d3ce43fd8b17";
        }
    }
}
