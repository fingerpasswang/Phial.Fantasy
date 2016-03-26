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
        internal const string Cli2Logic = "Cli2Logic";
        internal const string Scene2Cli = "Scene2Cli";
        internal const string Cli2Scene = "Cli2Scene";
        internal const string Cli2Login = "Cli2Login";
        internal const string Logic2Cli = "Logic2Cli";
        internal const string Logic2Scheduler = "Logic2Scheduler";
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
            MetaData.SetServiceId(typeof(ICli2LogicImpl), Cli2Logic);
            MetaData.SetMethodSerializer(typeof(ICli2LogicImpl), Cli2LogicSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ICli2LogicImpl), Cli2LogicDispatcher.Instance);

            MetaData.SetServiceId(typeof(ICli2SceneImpl), Cli2Scene);
            MetaData.SetMethodSerializer(typeof(ICli2SceneImpl), Cli2SceneSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ICli2SceneImpl), Cli2SceneDispatcher.Instance);
            MetaData.SetServiceId(typeof(ICli2LoginImpl), Cli2Login);
            MetaData.SetMethodSerializer(typeof(ICli2LoginImpl), Cli2LoginSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ICli2LoginImpl), Cli2LoginDispatcher.Instance);

            MetaData.SetServiceId(typeof(ILogic2SchedulerImpl), Logic2Scheduler);
            MetaData.SetMethodSerializer(typeof(ILogic2SchedulerImpl), Logic2SchedulerSerializer.Instance);
            MetaData.SetServiceMethodDispatcher(typeof(ILogic2SchedulerImpl), Logic2SchedulerDispatcher.Instance);
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
            MetaData.SetServiceRoutingRule(Cli2Logic, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.Cli2Logic.invoke", districts),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.Cli2Logic.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "amq.topic",
                    ImplExchangeName = () => "Cli2Logic.invoke",
                    ImplQueueName = districts => string.Format("{0}.Cli2Logic.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 4,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Cli2Logic),
                },
            });
            MetaData.SetServiceRoutingRule(Scene2Cli, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    DelegateRoutingKey = (districts, uuid) => string.Format("{0}.Scene2Cli.sync.{1}", districts, uuid),
                    DelegateExchangeName = () => "amq.topic",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 5,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Scene2Cli),
                },
            });
            MetaData.SetServiceRoutingRule(Cli2Scene, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.Cli2Scene.invoke", districts),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.Cli2Scene.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "amq.topic",
                    ImplExchangeName = () => "Cli2Scene.invoke",
                    ImplQueueName = districts => string.Format("{0}.Cli2Scene.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 6,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Cli2Scene),
                },
            });
            MetaData.SetServiceRoutingRule(Cli2Login, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.Cli2Login.invoke", districts),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.Cli2Login.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "amq.topic",
                    ImplExchangeName = () => "Cli2Login.invoke",
                    ImplQueueName = districts => string.Format("{0}.Cli2Login.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 7,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Cli2Login),
                },
            });
            MetaData.SetServiceRoutingRule(Logic2Cli, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    DelegateRoutingKey = (districts, uuid) => string.Format("{0}.Logic2Cli.sync.{1}", districts, uuid),
                    DelegateExchangeName = () => "amq.topic",
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 8,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Logic2Cli),
                },
            });
            MetaData.SetServiceRoutingRule(Logic2Scheduler, new RoutingRule()
            {
                AmqpRule = new RoutingRule.AmqpRoutingRule()
                {
                    ImplBindingKey = (districts, uuid) => string.Format("{0}.Logic2Scheduler.invoke", districts),
                    DelegateRoutingKey = (districts, uuid) => string.Format("{0}.Logic2Scheduler.invoke", districts),
                    ReturnBindingKey = (districts, uuid) => string.Format("{0}.Logic2Scheduler.return.{1}", districts, uuid),
                    ReturnRoutingKey = (districts, uuid) => string.Format("{0}.Logic2Scheduler.return.{1}", districts, uuid),
                    ReturnExchangeName = () => "Logic2Scheduler.return",
                    DelegateExchangeName = () => "Logic2Scheduler.invoke",
                    ImplExchangeName = () => "Logic2Scheduler.invoke",
                    ImplQueueName = districts => string.Format("{0}.Logic2Scheduler.invoke", districts),
                },
                GateRule = new RoutingRule.GateRoutingRule() {ServiceId  = s => 9,},
                ZkRule = new RoutingRule.ZkRoutingRule()
                {
                    ServicePath = (districts) => string.Format("/{0}/{1}", districts, Logic2Scheduler),
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
    #region Cli2Logic

    public interface ICli2LogicImpl: IRpcImplInstnce
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

    public class Cli2LogicDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly Cli2LogicDispatcher Instance = new Cli2LogicDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ICli2LogicImpl)impl).AskChangeName((UInt64)(method.Args[0]), (String)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 2:
                ((ICli2LogicImpl)impl).AskAddMoney((UInt64)(method.Args[0]), (UInt32)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 3:
                ((ICli2LogicImpl)impl).AskLearnSkill((UInt64)(method.Args[0]), (UInt32)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 4:
                ((ICli2LogicImpl)impl).TestEnum((UInt64)(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 5:
                ((ICli2LogicImpl)impl).TestArray((UInt64[])(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 6:
                ((ICli2LogicImpl)impl).TestList((List<Boolean>)(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 7:
                ((ICli2LogicImpl)impl).TestDict((Dictionary<Boolean, PlayerInfo>)(method.Args[0]), (TowerState)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 8:
                ((ICli2LogicImpl)impl).RequestPlayerInfo((UInt64)(method.Args[0])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 9:
                ((ICli2LogicImpl)impl).TestHierarchy((TestBaseClass)(method.Args[0]), (TestDerived1Class)(method.Args[1]), (TestDerived11Class)(method.Args[2])).ContinueWith(t => DoContinue(t, cont));
                break;
            case 10:
                ((ICli2LogicImpl)impl).TestHierarch2y((TestBaseClass)(method.Args[0]), (TestDerived1Class)(method.Args[1]), (TestDerived11Class)(method.Args[2])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
        }
    }
    public class Cli2LogicSerializer : IMethodSerializer
    {
        public static readonly Cli2LogicSerializer Instance = new Cli2LogicSerializer();
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
    #region Scene2Cli


    public class Scene2CliServiceDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public Scene2CliServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, Scene2CliSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.Scene2Cli));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.Scene2Cli);
        }
        private readonly byte[] forwardKey;
        private Scene2CliServiceDelegate(ServiceDelegateStub serviceDelegateStub, byte[] forwardKey)
        {
            this.forwardKey = forwardKey;
            this.serviceDelegateStub = serviceDelegateStub;
        }
        public Scene2CliServiceDelegate Forward(byte[] forwardId)
        {
            return new Scene2CliServiceDelegate(serviceDelegateStub, forwardId);
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

    public class Scene2CliSerializer : IMethodSerializer
    {
        public static readonly Scene2CliSerializer Instance = new Scene2CliSerializer();
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
    #region Cli2Scene

    public interface ICli2SceneImpl: IRpcImplInstnce
    {
        Task<Boolean> AskMoveTo(Int32 x, Int32 y);
    }

    public class Cli2SceneDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly Cli2SceneDispatcher Instance = new Cli2SceneDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ICli2SceneImpl)impl).AskMoveTo((Int32)(method.Args[0]), (Int32)(method.Args[1])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
        }
    }
    public class Cli2SceneSerializer : IMethodSerializer
    {
        public static readonly Cli2SceneSerializer Instance = new Cli2SceneSerializer();
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
    #region Cli2Login

    public interface ICli2LoginImpl: IRpcImplInstnce
    {
        Task<ServerList> AskLogin(String username, String password, Byte[] uuid);
    }

    public class Cli2LoginDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly Cli2LoginDispatcher Instance = new Cli2LoginDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ICli2LoginImpl)impl).AskLogin((String)(method.Args[0]), (String)(method.Args[1]), (Byte[])(method.Args[2])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
        }
    }
    public class Cli2LoginSerializer : IMethodSerializer
    {
        public static readonly Cli2LoginSerializer Instance = new Cli2LoginSerializer();
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
    #region Logic2Cli


    public class Logic2CliServiceDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public Logic2CliServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, Logic2CliSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.Logic2Cli));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.Logic2Cli);
        }
        private readonly byte[] forwardKey;
        private Logic2CliServiceDelegate(ServiceDelegateStub serviceDelegateStub, byte[] forwardKey)
        {
            this.forwardKey = forwardKey;
            this.serviceDelegateStub = serviceDelegateStub;
        }
        public Logic2CliServiceDelegate Forward(byte[] forwardId)
        {
            return new Logic2CliServiceDelegate(serviceDelegateStub, forwardId);
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

    public class Logic2CliSerializer : IMethodSerializer
    {
        public static readonly Logic2CliSerializer Instance = new Logic2CliSerializer();
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
    #region Logic2Scheduler

    public interface ILogic2SchedulerImpl: IRpcImplInstnce
    {
        Task<UInt64> RequestScheduleJob(Int32 job);
    }
    public class Logic2SchedulerServiceDelegate
    {
        private readonly ServiceDelegateStub serviceDelegateStub;
        public Logic2SchedulerServiceDelegate(IDataSender dataSender)
        {
            serviceDelegateStub = new ServiceDelegateStub(dataSender, Logic2SchedulerSerializer.Instance, MetaData.GetServiceRoutingRule(AutoInit.Logic2Scheduler));
            dataSender.RegisterDelegate(serviceDelegateStub, AutoInit.Logic2Scheduler);
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
    public class Logic2SchedulerDispatcher : ServiceMethodDispatcherEx, IServiceMethodDispatcher
    {
        public static readonly Logic2SchedulerDispatcher Instance = new Logic2SchedulerDispatcher();
        public void Dispatch(IRpcImplInstnce impl, RpcMethod method, ServiceImplementStub.SendResult cont)
        {
            switch (method.MethodId)
            {
            case 1:
                ((ILogic2SchedulerImpl)impl).RequestScheduleJob((Int32)(method.Args[0])).ContinueWith(t => DoContinue(t, cont));
                break;
            }
        }
    }
    public class Logic2SchedulerSerializer : IMethodSerializer
    {
        public static readonly Logic2SchedulerSerializer Instance = new Logic2SchedulerSerializer();
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
            return "e27cac217e3915fcd7dc940c4bbaacaa";
        }
    }
}
