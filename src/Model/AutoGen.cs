using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCBase;

namespace Common
{
    public class ServiceIdGen
    {
        public const byte DbSyncNotify = 1;
        public const byte LoginNotify = 2;
        public const byte LogicClient = 3;
        public const byte LoginClient = 4;
        public const byte ClientLogic = 5;
        public const byte ClientLogin = 6;
    }

    #region DbSyncNotify
    public interface IDbSyncNotifyImpl
    {
        Task NotifyPlayerLoaded(ulong pid);
    }

    public class DbSyncNotifyDelegate : ServiceDelegate, IDbSyncNotify
    {
        #region meta data
        public class MethodId
        {
            public const int NotifyPlayerLoaded = 1001;
        }
        readonly ServiceMethod methodNotifyPlayerLoaded = new ServiceMethod()
        {
            MethodId = MethodId.NotifyPlayerLoaded
        };
        #endregion

        public void NotifyPlayerLoaded(ulong pid)
        {
            Invoke<IDbSyncNotify>(methodNotifyPlayerLoaded, pid);
        }

        public DbSyncNotifyDelegate(IDataInjector dataInjector, IMethodSerializer methodSerializer) : base(dataInjector, methodSerializer)
        {
            dataInjector.RegisterDelegate(this, ServiceIdGen.DbSyncNotify);
        }
    }
    #endregion

    #region LoginNotify
    public interface ILoginNotifyImpl
    {
        Task NotifyPlayerLogin(ulong pid);
    }
    public class LoginNotifyDelegate : ServiceDelegate, ILoginNotify
    {
        #region meta data
        public class MethodId
        {
            public const int NotifyPlayerLogin = 2001;
        }
        readonly ServiceMethod methodNotifyPlayerLogin = new ServiceMethod()
        {
            MethodId = MethodId.NotifyPlayerLogin
        };
        #endregion

        public void NotifyPlayerLogin(ulong pid)
        {
            Invoke<ILoginNotify>(methodNotifyPlayerLogin, pid);
        }

        public LoginNotifyDelegate(IDataInjector dataInjector, IMethodSerializer methodSerializer) : base(dataInjector, methodSerializer)
        {
            dataInjector.RegisterDelegate(this, ServiceIdGen.LoginNotify);
        }
    }
    #endregion

    #region LogicClient
    public interface ILogicClientImpl
    {
        Task<bool> AskChangeName(ulong pid, string newName);
        Task<bool> AskAddMoney(ulong pid, uint money);
        Task<bool> AskLearnSkill(ulong pid, uint skillId);
    }
    public interface ILogicClientInvoke
    {
        InvokeOperation<bool> AskChangeName(ulong pid, string newName);
        InvokeOperation<bool> AskAddMoney(ulong pid, uint money);
        InvokeOperation<bool> AskLearnSkill(ulong pid, uint skillId);
    }

    public class LogicClientServiceDelegate : ServiceDelegate, ILogicClientInvoke, IMessageConsumer
    {
        #region meta data
        public class MethodId
        {
            public const int AskChangeName = 3001;
            public const int AskAddMoney = 3002;
            public const int AskLearnSkill = 3003;
        }
        readonly ServiceMethod methodAskChangeName = new ServiceMethod()
        {
            MethodId = MethodId.AskChangeName
        };
        readonly ServiceMethod methodAskAddMoney = new ServiceMethod()
        {
            MethodId = MethodId.AskAddMoney
        };
        readonly ServiceMethod methodAskLearnSkill = new ServiceMethod()
        {
            MethodId = MethodId.AskLearnSkill
        };
        #endregion

        public LogicClientServiceDelegate(IDataInjector dataInjector, IMethodSerializer methodSerializer) : base(dataInjector, methodSerializer)
        {
            dataInjector.RegisterDelegate(this, ServiceIdGen.LogicClient);
        }

        public InvokeOperation<bool> AskChangeName(ulong pid, string newName)
        {
            return Invoke<bool>(methodAskChangeName, pid, newName);
        }

        public InvokeOperation<bool> AskAddMoney(ulong pid, uint money)
        {
            return Invoke<bool>(methodAskAddMoney, pid, money);
        }

        public InvokeOperation<bool> AskLearnSkill(ulong pid, uint skillId)
        {
            return Invoke<bool>(methodAskLearnSkill, pid, skillId);
        }
    }
    #endregion

    #region LoginClient
    public interface ILoginClientImpl
    {
        Task<bool> AskLogin(ulong pid);
    }
    public interface ILoginClientInvoke
    {
        InvokeOperation<bool> AskLogin(ulong pid);
    }
    public class LoginClientServiceDelegate : ServiceDelegate, ILoginClientInvoke, IMessageConsumer
    {
        #region meta data
        public class MethodId
        {
            public const int AskLogin = 4001;
        }
        readonly ServiceMethod methodAskLogin = new ServiceMethod()
        {
            MethodId = MethodId.AskLogin
        };
        #endregion

        public LoginClientServiceDelegate(IDataInjector dataInjector, IMethodSerializer methodSerializer)
            : base(dataInjector, methodSerializer)
        {
            dataInjector.RegisterDelegate(this, ServiceIdGen.LoginClient);
        }

        public InvokeOperation<bool> AskLogin(ulong pid)
        {
            return Invoke<bool>(methodAskLogin, pid);
        }
    }
    #endregion

    #region ClientLogic
    public class ClientLogicServiceDelegate : ServiceDelegate, IClientLogicService
    {
        #region meta data
        public class MethodId
        {
            public const int ServerMessageOk = 5001;
        }
        readonly ServiceMethod methodServerMessageOk = new ServiceMethod()
        {
            MethodId = MethodId.ServerMessageOk
        };
        #endregion

        public ClientLogicServiceDelegate(IDataInjector dataInjector, IMethodSerializer methodSerializer)
            : base(dataInjector, methodSerializer)
        {
            dataInjector.RegisterDelegate(this, ServiceIdGen.ClientLogic);
        }

        public void ServerMessageOk()
        {
            Invoke<IClientLogicService>(methodServerMessageOk);
        }
        public ClientLogicServiceDelegate Forward(string key)
        {
            forwardKey = key;
            return this;
        }
    }
    #endregion

    #region ClientLogin
    public class ClientLoginServiceDelegate : ServiceDelegate, IClientLoginService
    {
        #region meta data
        public class MethodId
        {
            public const int SyncPlayerInfo = 6001;
        }
        readonly ServiceMethod methodSyncPlayerInfo = new ServiceMethod()
        {
            MethodId = MethodId.SyncPlayerInfo
        };
        #endregion

        public ClientLoginServiceDelegate(IDataInjector dataInjector, IMethodSerializer methodSerializer)
            : base(dataInjector, methodSerializer)
        {
            dataInjector.RegisterDelegate(this, ServiceIdGen.ClientLogin);
        }

        public void SyncPlayerInfo(PlayerInfo pInfo)
        {
            Invoke<IClientLoginService>(methodSyncPlayerInfo, pInfo);
        }
        public ClientLoginServiceDelegate Forward(string key)
        {
            forwardKey = key;
            return this;
        }
    }
    #endregion

    public partial class ClientMethodDispatcher : IClientServiceMethodDispatcher
    {
        public void Dispatch(ClientServiceImplement impl, ServiceMethod method)
        {
            switch (method.MethodId)
            {
                case 5001:
                    (impl as IClientLogicService).ServerMessageOk();
                    break;
                case 6001:
                    (impl as IClientLoginService).SyncPlayerInfo((PlayerInfo)method.Args[0]);
                    break;
            }
        }
    }

    public partial class MethodDispatcher : IServiceMethodDispatcher
    {
        public Task Dispatch(ServerServiceImplement impl, ServiceMethod method)
        {
            switch (method.MethodId)
            {
                case 1001:
                    return (impl as IDbSyncNotifyImpl).NotifyPlayerLoaded((ulong)method.Args[0]);
                case 2001:
                    return (impl as ILoginNotifyImpl).NotifyPlayerLogin((ulong)method.Args[0]);
                case 3001:
                    return (impl as ILogicClientImpl).AskChangeName((ulong)method.Args[0], (string)method.Args[1]);
                case 3002:
                    return (impl as ILogicClientImpl).AskAddMoney((ulong)method.Args[0], (uint)method.Args[1]);
                case 3003:
                    return (impl as ILogicClientImpl).AskLearnSkill((ulong)method.Args[0], (uint)method.Args[1]);
                case 4001:
                    return (impl as ILoginClientImpl).AskLogin((ulong)method.Args[0]);
            }

            return Task.Run(() => { });
        }
        public void Write(ServiceMethod method, Task handle, BinaryWriter bw)
        {
            switch (method.MethodId)
            {
                case 3001:
                    bw.Write((handle as Task<bool>).Result);
                    break;
                case 3002:
                    bw.Write((handle as Task<bool>).Result);
                    break;
                case 3003:
                    bw.Write((handle as Task<bool>).Result);
                    break;
                case 4001:
                    bw.Write((handle as Task<bool>).Result);
                    break;
            }
        }
    }

    public partial class MethodSerializer : IMethodSerializer
    {
        void Read1001(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[1];
            method.Args[0] = br.ReadUInt64();
        }
        void Write1001(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write((ulong)method.Args[0]);
        }
        void Read2001(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[1];
            method.Args[0] = br.ReadUInt64();
        }
        void Write2001(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write((ulong)method.Args[0]);
        }
        void Read3001(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[2];
            method.Args[0] = br.ReadUInt64();
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                method.Args[1] = br.ReadString();
            }
        }
        void Write3001(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write((ulong)method.Args[0]);
            if (method.Args[1] == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                (method.Args[1] as string).Write(bw);
            }
        }
        void Read3002(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[2];
            method.Args[0] = br.ReadUInt64();
            method.Args[1] = br.ReadUInt32();
        }
        void Write3002(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write((ulong)method.Args[0]);
            bw.Write((uint)method.Args[1]);
        }
        void Read3003(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[2];
            method.Args[0] = br.ReadUInt64();
            method.Args[1] = br.ReadUInt32();
        }
        void Write3003(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write((ulong)method.Args[0]);
            bw.Write((uint)method.Args[1]);
        }
        void Read4001(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[1];
            method.Args[0] = br.ReadUInt64();
        }
        void Write4001(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write((ulong)method.Args[0]);
        }
        void Read5001(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[0];
        }
        void Write5001(ServiceMethod method, BinaryWriter bw)
        {
        }
        void Read6001(ServiceMethod method, BinaryReader br)
        {
            method.Args = new object[1];
            if (br.ReadByte() == (byte)SerializeObjectMark.Common)
            {
                method.Args[0] = (new PlayerInfo()).Read(br);
            }
        }
        void Write6001(ServiceMethod method, BinaryWriter bw)
        {
            if (method.Args[0] == null)
            {
                bw.Write((byte)SerializeObjectMark.IsNull);
            }
            else
            {
                bw.Write((byte)SerializeObjectMark.Common);
                (method.Args[0] as PlayerInfo).Write(bw);
            }
        }

        public void Read(ServiceMethod method, BinaryReader br)
        {
            method.MethodId = br.ReadUInt32();
            switch (method.MethodId)
            {
                case 1001:
                    Read1001(method, br);
                    break;
                case 2001:
                    Read2001(method, br);
                    break;
                case 3001:
                    Read3001(method, br);
                    method.NeedReturn = true;
                    break;
                case 3002:
                    Read3002(method, br);
                    method.NeedReturn = true;
                    break;
                case 3003:
                    Read3003(method, br);
                    method.NeedReturn = true;
                    break;
                case 4001:
                    Read4001(method, br);
                    method.NeedReturn = true;
                    break;
                case 5001:
                    Read5001(method, br);
                    break;
                case 6001:
                    Read6001(method, br);
                    break;
            }
        }
        public void Write(ServiceMethod method, BinaryWriter bw)
        {
            bw.Write(method.MethodId);
            switch (method.MethodId)
            {
                case 1001:
                    Write1001(method, bw);
                    break;
                case 2001:
                    Write2001(method, bw);
                    break;
                case 3001:
                    Write3001(method, bw);
                    break;
                case 3002:
                    Write3002(method, bw);
                    break;
                case 3003:
                    Write3003(method, bw);
                    break;
                case 4001:
                    Write4001(method, bw);
                    break;
                case 5001:
                    Write5001(method, bw);
                    break;
                case 6001:
                    Write6001(method, bw);
                    break;
            }
        }

        public object ReadReturn(uint methodId, BinaryReader br)
        {
            switch (methodId)
            {
                case 3001:
                    return br.ReadBoolean();
                case 3002:
                    return br.ReadBoolean();
                case 3003:
                    return br.ReadBoolean();
                case 4001:
                    return br.ReadBoolean();
            }
            return new object();
        }
    }

    public partial class ServiceMeta
    {
        public Mode GetMode(string mode)
        {
            if (mode.EndsWith("notify"))
                return Mode.Notify;
            if (mode.EndsWith("sync"))
                return Mode.Sync;
            if (mode.EndsWith("invoke"))
                return Mode.Invoke;
            if (mode.EndsWith("return"))
                return Mode.Return;

            return Mode.Invoke;
        }

        public byte GetServiceId(string service)
        {
            if (service.EndsWith("dbsync"))
            {
                return 1;
            }
            else if (service.EndsWith("login"))
            {
                return 2;
            }
            else if (service.EndsWith("LoginClient"))
            {
                return 4;
            }
            else if (service.EndsWith("LogicClient"))
            {
                return 3;
            }
            else if (service.EndsWith("ClientLogin"))
            {
                return 6;
            }
            else if (service.EndsWith("ClientLogic"))
            {
                return 5;
            }
            return 0;
        }

        public string GetClientRoutingKey(byte sid, ulong pid)
        {
            throw new NotImplementedException();
        }

        public string GetClientBindingKey(byte sid, ulong pid)
        {
            throw new NotImplementedException();
        }

        public string GetRoutingKey(byte sid)
        {
            throw new NotImplementedException();
        }

        public string GetBindingKey(byte sid)
        {
            throw new NotImplementedException();
        }

        public string GetExchange(byte sid)
        {
            throw new NotImplementedException();
        }

        public bool IsServiceForward(byte sid)
        {
            throw new NotImplementedException();
        }

        static ServiceMeta()
        {
            ServiceHelper<IDbSyncNotifyImpl>.ServiceIdGetter = 1;
            ServiceHelper<ILoginNotifyImpl>.ServiceIdGetter = 2;
            ServiceHelper<ILogicClientService>.ServiceIdGetter = 3;
            ServiceHelper<ILogicClientImpl>.ServiceIdGetter = 3;
            ServiceHelper<ILoginClientService>.ServiceIdGetter = 4;
            ServiceHelper<ILoginClientImpl>.ServiceIdGetter = 4;
            ServiceHelper<IClientLogicService>.ServiceIdGetter = 5;
            ServiceHelper<IClientLoginService>.ServiceIdGetter = 6;

            serviceMetaInfos[ServiceIdGen.DbSyncNotify] = new ServiceMetaInfo()
            {
                ServiceId = ServiceIdGen.DbSyncNotify,
                ImplBindingKey = (meta, fid) => "server.dbsync.notify",
                DelegateRoutingKey = (meta, fid) => "server.dbsync.notify",
                ExchangeKey = (meta) => "amq.topic",
            };
            serviceMetaInfos[ServiceIdGen.LoginNotify] = new ServiceMetaInfo()
            {
                ServiceId = ServiceIdGen.LoginNotify,
                ImplBindingKey = (meta, fid) => "server.login.notify",
                DelegateRoutingKey = (meta, fid) => "server.login.notify",
                ExchangeKey = (meta) => "amq.topic",
            };
            serviceMetaInfos[ServiceIdGen.ClientLogic] = new ServiceMetaInfo()
            {
                ServiceId = ServiceIdGen.ClientLogic,
                ImplBindingKey = (meta, fid) => string.Format("client/ClientLogic/sync/{0}", fid),
                DelegateRoutingKey = (meta, fid) => string.Format("client.ClientLogic.sync.{0}", fid),
                ExchangeKey = (meta) => "amq.topic",
            };
            serviceMetaInfos[ServiceIdGen.ClientLogin] = new ServiceMetaInfo()
            {
                ServiceId = ServiceIdGen.ClientLogin,
                ImplBindingKey = (meta, fid) => string.Format("client/ClientLogin/sync/{0}", fid),
                DelegateRoutingKey = (meta, fid) => string.Format("client.ClientLogin.sync.{0}", fid),
                ExchangeKey = (meta) => "amq.topic",
            };
            serviceMetaInfos[ServiceIdGen.LogicClient] = new ServiceMetaInfo()
            {
                ServiceId = ServiceIdGen.LogicClient,
                ImplBindingKey = (meta, fid) => "client.LogicClient.invoke.*",
                DelegateRoutingKey = (meta, fid) => string.Format("client/LogicClient/invoke/{0}", fid),
                ImplRoutingKey = (meta, fid) => string.Format("client.LogicClient.return.{0}", fid),
                DelegateBindingKey = (meta, fid) => string.Format("client/LogicClient/return/{0}", fid),
                ExchangeKey = (meta) => "amq.topic",
            };
            serviceMetaInfos[ServiceIdGen.LoginClient] = new ServiceMetaInfo()
            {
                ServiceId = ServiceIdGen.LoginClient,
                ImplBindingKey = (meta, fid) => "client.LoginClient.invoke.*",
                DelegateRoutingKey = (meta, fid) => string.Format("client/LoginClient/invoke/{0}", fid),
                ImplRoutingKey = (meta, fid) => string.Format("client.LoginClient.return.{0}", fid),
                DelegateBindingKey = (meta, fid) => string.Format("client/LoginClient/return/{0}", fid),
                ExchangeKey = (meta) => "amq.topic",
            };
        }
    }
}
