using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCBase;

namespace Common
{
    public partial class ClientMethodDispatcher
    {
        private static ClientMethodDispatcher instance;
        public static ClientMethodDispatcher Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientMethodDispatcher();
                return instance;
            }
        }
    }
    public partial class MethodDispatcher
    {
        private static MethodDispatcher instance;
        public static MethodDispatcher Instance
        {
            get
            {
                if (instance == null)
                    instance = new MethodDispatcher();
                return instance;
            }
        }
    }
    public partial class MethodSerializer
    {
        private static MethodSerializer instance;
        public static MethodSerializer Instance
        {
            get
            {
                if (instance == null)
                    instance = new MethodSerializer();
                return instance;
            }
        }

    }

    public partial class ServiceMeta : IServiceMeta
    {
        private static ServiceMeta instance;
        public static ServiceMeta Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServiceMeta();
                return instance;
            }
        }
        private static readonly Dictionary<byte, ServiceMetaInfo> serviceMetaInfos = new Dictionary<byte, ServiceMetaInfo>();

        static class ServiceHelper<T>
        {
            public static byte ServiceIdGetter;
        }

        public byte GetServiceId<T>()
        {
            return ServiceHelper<T>.ServiceIdGetter;
        }

        public ServiceMetaInfo GetServiceMetaInfo(byte sid)
        {
            return serviceMetaInfos[sid];
        }
    }
}
