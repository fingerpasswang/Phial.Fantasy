using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using RPCBase;
using RPCBase.Server;

namespace Auto.Server
{

    public static class AutoInit
    {
        internal const string Login = "Login";
        internal const string DbClient = "DbClient";
        internal const string WatcherService = "WatcherService";
        internal const string LogicClient = "LogicClient";
        internal const string ClientScene = "ClientScene";
        internal const string SceneClient = "SceneClient";
        internal const string LoginClient = "LoginClient";
        internal const string ClientLogic = "ClientLogic";
        internal const string SchedulerLogic = "SchedulerLogic";
        static AutoInit()
        {
            MetaData.SetServiceId(typeof(ILoginNotifyImpl), Login);
            MetaData.SetMethodSerializer(typeof(ILoginNotifyImpl), LoginNotifySerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ILoginNotifyImpl), LoginNotifyDispatcher.Instance);
            MetaData.SetServiceId(typeof(IDbClientNotifyImpl), DbClient);
            MetaData.SetMethodSerializer(typeof(IDbClientNotifyImpl), DbClientNotifySerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(IDbClientNotifyImpl), DbClientNotifyDispatcher.Instance);
            MetaData.SetServiceId(typeof(IWatcherServiceNotifyImpl), WatcherService);
            MetaData.SetMethodSerializer(typeof(IWatcherServiceNotifyImpl), WatcherServiceNotifySerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(IWatcherServiceNotifyImpl), WatcherServiceNotifyDispatcher.Instance);
            MetaData.SetServiceId(typeof(ILogicClientImpl), LogicClient);
            MetaData.SetMethodSerializer(typeof(ILogicClientImpl), LogicClientSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ILogicClientImpl), LogicClientDispatcher.Instance);

            MetaData.SetServiceId(typeof(ISceneClientImpl), SceneClient);
            MetaData.SetMethodSerializer(typeof(ISceneClientImpl), SceneClientSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ISceneClientImpl), SceneClientDispatcher.Instance);
            MetaData.SetServiceId(typeof(ILoginClientImpl), LoginClient);
            MetaData.SetMethodSerializer(typeof(ILoginClientImpl), LoginClientSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ILoginClientImpl), LoginClientDispatcher.Instance);

            MetaData.SetServiceId(typeof(ISchedulerLogicImpl), SchedulerLogic);
            MetaData.SetMethodSerializer(typeof(ISchedulerLogicImpl), SchedulerLogicSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ISchedulerLogicImpl), SchedulerLogicDispatcher.Instance);
            MetaData.SetServiceRoutingRule(Login, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => "padding.Login.notify",
                    DelegateRoutingKey = (districts, fid) => "padding.Login.notify",
                    DelegateExchangeName = () => "Login.notify",
                    ImplExchangeName = () => "Login.notify",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 1,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Login),
                },
            });
            MetaData.SetServiceRoutingRule(DbClient, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => "padding.DbClient.notify",
                    DelegateRoutingKey = (districts, fid) => "padding.DbClient.notify",
                    DelegateExchangeName = () => "DbClient.notify",
                    ImplExchangeName = () => "DbClient.notify",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 2,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, DbClient),
                },
            });
            MetaData.SetServiceRoutingRule(WatcherService, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => "padding.WatcherService.notify",
                    DelegateRoutingKey = (districts, fid) => "padding.WatcherService.notify",
                    DelegateExchangeName = () => "WatcherService.notify",
                    ImplExchangeName = () => "WatcherService.notify",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 3,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, WatcherService),
                },
            });
            MetaData.SetServiceRoutingRule(LogicClient, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.LogicClient.invoke", districts),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.LogicClient.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "amq.topic",
                    ImplExchangeName = () => "LogicClient.invoke",
                    ImplQueueName = districts => string.Format("{0}.LogicClient.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 4,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, LogicClient),
                },
            });
            MetaData.SetServiceRoutingRule(ClientScene, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    DelegateRoutingKey = (districts, uuid) => string.Format("{0}.ClientScene.sync.{1}", districts, uuid),
                    DelegateExchangeName = () => "amq.topic",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 5,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, ClientScene),
                },
            });
            MetaData.SetServiceRoutingRule(SceneClient, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.SceneClient.invoke", districts),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.SceneClient.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "amq.topic",
                    ImplExchangeName = () => "SceneClient.invoke",
                    ImplQueueName = districts => string.Format("{0}.SceneClient.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 6,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, SceneClient),
                },
            });
            MetaData.SetServiceRoutingRule(LoginClient, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("padding.LoginClient.invoke"),
                    ReturnRoutingKey = (districts, uuid) => string.Format("padding.LoginClient.return.{0}", uuid),
                    ReturnExchangeName = () => "amq.topic",
                    ImplExchangeName = () => "LoginClient.invoke",
                    ImplQueueName = districts => string.Format("padding.LoginClient.invoke"),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 7,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/global/{1}", districts, LoginClient),
                },
            });
            MetaData.SetServiceRoutingRule(ClientLogic, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    DelegateRoutingKey = (districts, uuid) => string.Format("{0}.ClientLogic.sync.{1}", districts, uuid),
                    DelegateExchangeName = () => "amq.topic",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 8,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, ClientLogic),
                },
            });
            MetaData.SetServiceRoutingRule(SchedulerLogic, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.SchedulerLogic.invoke", districts),
                    DelegateRoutingKey = (districts, uuid) => string.Format("{0}.SchedulerLogic.invoke", districts),
                    ReturnBindingKey = (districts, uuid) => string.Format("{0}.SchedulerLogic.return.{1}", districts, uuid),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.SchedulerLogic.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "SchedulerLogic.return",
                    DelegateExchangeName = () => "SchedulerLogic.invoke",
                    ImplExchangeName = () => "SchedulerLogic.invoke",
                    ImplQueueName = districts => string.Format("{0}.SchedulerLogic.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 9,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, SchedulerLogic),
                },
            });
        } public static void Init() {}
    }
    #region LoginNotify

    public interface ILoginNotifyImpl: IRpcImplInstnce
    {
        Task NotifyLogicServerWorking(String districts);
    }
    public class LoginNotifyDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public LoginNotifyDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, LoginNotifySerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.Login));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.Login);
        }

        #region meta data
        private static class MethodId
        {
            public const uint NotifyLogicServerWorking = 1;
        }
        #endregion
        public void NotifyLogicServerWorking(String districts)
        {
            serviceDelegateStub.Notify(MethodId.NotifyLogicServerWorking, null, districts);
        }
    }
    public class LoginNotifyDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly LoginNotifyDispatcher Instance = new LoginNotifyDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ILoginNotifyImpl)impl).NotifyLogicServerWorking((String)(method.Args[0]));
                break;
            }
        }
    }
    public class LoginNotifySerializer : IMethodSerializer
    {
        public static readonly LoginNotifySerializer Instance = new LoginNotifySerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[1];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = br.ReadString();
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
    #region DbClientNotify

    public interface IDbClientNotifyImpl: IRpcImplInstnce
    {
        Task NotifyConnectionInfo(String ip, Int32 port);
    }
    public class DbClientNotifyDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public DbClientNotifyDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, DbClientNotifySerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.DbClient));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.DbClient);
        }

        #region meta data
        private static class MethodId
        {
            public const uint NotifyConnectionInfo = 1;
        }
        #endregion
        public void NotifyConnectionInfo(String ip, Int32 port)
        {
            serviceDelegateStub.Notify(MethodId.NotifyConnectionInfo, null, ip, port);
        }
    }
    public class DbClientNotifyDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly DbClientNotifyDispatcher Instance = new DbClientNotifyDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((IDbClientNotifyImpl)impl).NotifyConnectionInfo((String)(method.Args[0]), (Int32)(method.Args[1]));
                break;
            }
        }
    }
    public class DbClientNotifySerializer : IMethodSerializer
    {
        public static readonly DbClientNotifySerializer Instance = new DbClientNotifySerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[2];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = br.ReadString();
                }
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
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write((String)args[0]);
                }
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
    #region WatcherServiceNotify

    public interface IWatcherServiceNotifyImpl: IRpcImplInstnce
    {
        Task NotifyWatchingMaster(Byte[] srcUuid, String masterAddr);
        Task NotifyInstanceSubjectiveDown(Byte[] srcUuid, String addr);
    }
    public class WatcherServiceNotifyDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public WatcherServiceNotifyDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, WatcherServiceNotifySerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.WatcherService));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.WatcherService);
        }

        #region meta data
        private static class MethodId
        {
            public const uint NotifyWatchingMaster = 1;
            public const uint NotifyInstanceSubjectiveDown = 2;
        }
        #endregion
        public void NotifyWatchingMaster(Byte[] srcUuid, String masterAddr)
        {
            serviceDelegateStub.Notify(MethodId.NotifyWatchingMaster, null, srcUuid, masterAddr);
        }

        public void NotifyInstanceSubjectiveDown(Byte[] srcUuid, String addr)
        {
            serviceDelegateStub.Notify(MethodId.NotifyInstanceSubjectiveDown, null, srcUuid, addr);
        }

    }
    public class WatcherServiceNotifyDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly WatcherServiceNotifyDispatcher Instance = new WatcherServiceNotifyDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((IWatcherServiceNotifyImpl)impl).NotifyWatchingMaster((Byte[])(method.Args[0]), (String)(method.Args[1]));
                break;
            case 2:
                ((IWatcherServiceNotifyImpl)impl).NotifyInstanceSubjectiveDown((Byte[])(method.Args[0]), (String)(method.Args[1]));
                break;
            }
        }
    }
    public class WatcherServiceNotifySerializer : IMethodSerializer
    {
        public static readonly WatcherServiceNotifySerializer Instance = new WatcherServiceNotifySerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[2];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = br.ReadBytes(br.ReadInt32());
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[1] = br.ReadString();
                }
                break;
            case 2:
                method.Args = new object[2];
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[0] = br.ReadBytes(br.ReadInt32());
                }
                if (br.ReadByte() == (byte)SerializeObjectMark.Common)
                {
                    method.Args[1] = br.ReadString();
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
                    bw.Write(((byte[])args[0]).Length);
                    bw.Write((byte[])args[0]);
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
                break;
            }
            case 2:
            {
                if (args[0] == null)
                {
                    bw.Write((byte)SerializeObjectMark.IsNull);
                }
                else
                {
                    bw.Write((byte)SerializeObjectMark.Common);
                    bw.Write(((byte[])args[0]).Length);
                    bw.Write((byte[])args[0]);
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
    #region LogicClient

    public interface ILogicClientImpl: IRpcImplInstnce
    {
        Task<Boolean> AskChangeName(UInt64 pid, String newName);
        Task<Boolean> AskAddMoney(UInt64 pid, UInt32 money);
        Task<Boolean> AskLearnSkill(UInt64 pid, UInt32 skillId);
        Task<Boolean> TestEnum(UInt64 pid, TowerState state);
        Task<Boolean[]> TestArray(UInt64[] pids, TowerState state);
        Task<List<Boolean>> TestList(List<Boolean> pids, TowerState state);
        Task<Dictionary<Boolean, Byte[]>> TestDict(Dictionary<Boolean, PlayerInfo> pids, TowerState state);
        Task<PlayerInfo> RequestPlayerInfo(UInt64 pid);
        Task<TestBaseClass> TestHierarchy(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11);
        Task<TestBaseClass> TestHierarch2y(TestBaseClass b, TestDerived1Class d1, TestDerived11Class d11);
    }

    public class LogicClientDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly LogicClientDispatcher Instance = new LogicClientDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ILogicClientImpl)impl).AskChangeName((UInt64)(method.Args[0]), (String)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 2:
                ((ILogicClientImpl)impl).AskAddMoney((UInt64)(method.Args[0]), (UInt32)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 3:
                ((ILogicClientImpl)impl).AskLearnSkill((UInt64)(method.Args[0]), (UInt32)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 4:
                ((ILogicClientImpl)impl).TestEnum((UInt64)(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 5:
                ((ILogicClientImpl)impl).TestArray((UInt64[])(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 6:
                ((ILogicClientImpl)impl).TestList((List<Boolean>)(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 7:
                ((ILogicClientImpl)impl).TestDict((Dictionary<Boolean, PlayerInfo>)(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 8:
                ((ILogicClientImpl)impl).RequestPlayerInfo((UInt64)(method.Args[0])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 9:
                ((ILogicClientImpl)impl).TestHierarchy((TestBaseClass)(method.Args[0]), (TestDerived1Class)(method.Args[1]), (TestDerived11Class)(method.Args[2])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 10:
                ((ILogicClientImpl)impl).TestHierarch2y((TestBaseClass)(method.Args[0]), (TestDerived1Class)(method.Args[1]), (TestDerived11Class)(method.Args[2])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
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


    public class ClientSceneServiceDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public ClientSceneServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, ClientSceneSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.ClientScene));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.ClientScene);
        }
        private readonly byte[] forwardKey;
        private ClientSceneServiceDelegate(ServiceDelegateStub serviceDelegateStub, byte[] forwardKey)
        {
            this.forwardKey = forwardKey;
            this.serviceDelegateStub = serviceDelegateStub;
        }
        public ClientSceneServiceDelegate Forward(byte[] forwardId)
        {
            return new ClientSceneServiceDelegate(serviceDelegateStub, forwardId);
        }

        #region meta data
        private static class MethodId
        {
            public const uint SyncPosition = 1;
        }
        #endregion
        public void SyncPosition(Int32 x, Int32 y)
        {
            serviceDelegateStub.Notify(MethodId.SyncPosition, forwardKey, x, y);
        }
        public void SyncPositionMulticast(int groupId, Int32 x, Int32 y)
        {
            serviceDelegateStub.Multicast(MethodId.SyncPosition, groupId, x, y);
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

    public interface ISceneClientImpl: IRpcImplInstnce
    {
        Task<Boolean> AskMoveTo(Int32 x, Int32 y);
    }

    public class SceneClientDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly SceneClientDispatcher Instance = new SceneClientDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ISceneClientImpl)impl).AskMoveTo((Int32)(method.Args[0]), (Int32)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
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

    public interface ILoginClientImpl: IRpcImplInstnce
    {
        Task<ServerList> AskLogin(String username, String password, Byte[] uuid);
    }

    public class LoginClientDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly LoginClientDispatcher Instance = new LoginClientDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ILoginClientImpl)impl).AskLogin((String)(method.Args[0]), (String)(method.Args[1]), (Byte[])(method.Args[2])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
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


    public class ClientLogicServiceDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public ClientLogicServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, ClientLogicSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.ClientLogic));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.ClientLogic);
        }
        private readonly byte[] forwardKey;
        private ClientLogicServiceDelegate(ServiceDelegateStub serviceDelegateStub, byte[] forwardKey)
        {
            this.forwardKey = forwardKey;
            this.serviceDelegateStub = serviceDelegateStub;
        }
        public ClientLogicServiceDelegate Forward(byte[] forwardId)
        {
            return new ClientLogicServiceDelegate(serviceDelegateStub, forwardId);
        }

        #region meta data
        private static class MethodId
        {
            public const uint ServerMessageOk = 1;
        }
        #endregion
        public void ServerMessageOk()
        {
            serviceDelegateStub.Notify(MethodId.ServerMessageOk, forwardKey);
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
    #region SchedulerLogic

    public interface ISchedulerLogicImpl: IRpcImplInstnce
    {
        Task<UInt64> RequestScheduleJob(Int32 job);
    }
    public class SchedulerLogicServiceDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public SchedulerLogicServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, SchedulerLogicSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.SchedulerLogic));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.SchedulerLogic);
        }

        #region meta data
        private static class MethodId
        {
            public const uint RequestScheduleJob = 1;
        }
        #endregion
        public Task<UInt64> RequestScheduleJob(Int32 job)
        {
            return serviceDelegateStub.InvokeT<UInt64>(MethodId.RequestScheduleJob, null, job);
        }
    }
    public class SchedulerLogicDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly SchedulerLogicDispatcher Instance = new SchedulerLogicDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ISchedulerLogicImpl)impl).RequestScheduleJob((Int32)(method.Args[0])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
        }
    }
    public class SchedulerLogicSerializer : IMethodSerializer
    {
        public static readonly SchedulerLogicSerializer Instance = new SchedulerLogicSerializer();
        public RpcMethod Read(BinaryReader br)
        {
            RpcMethod method = new RpcMethod();
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
            case 1:
                method.Args = new object[1];
                method.Args[0] = br.ReadInt32();
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
                returnVal = br.ReadUInt64();
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
                bw.Write((UInt64)value);
                break;
            }
        }
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
