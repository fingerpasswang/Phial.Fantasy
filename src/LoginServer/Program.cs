using System;
using Auto.DataAccess;
using Auto.Server;
using Config;
using DataAccess;
using RPCBase;
using RPCBase.Server;
using ServerUtils;

namespace LoginServer
{
    class Program
    {
        public static RedisAdaptor RedisAdaptor;
        public static RedisDataServiceDelegate CacheDataService;
        public static MemIdProvider MemIdProvider = new MemIdProvider(1,1);

        static void Main(string[] args)
        {
            AutoInit.Init();
            AmqpAdaptor AmqpAdaptor = new AmqpAdaptor(ConfigManager.MQIP(), ConfigManager.AMQPPort(),
                ConfigManager.MQUser(), ConfigManager.MQPass());
            ServiceImplementStub.Bind<ICli2LoginImpl>(AmqpAdaptor, new LoginClientServiceImpl());
            ServiceImplementStub.Bind<ILoginNotifyImpl>(AmqpAdaptor, new LoginNotifyImpl());
            AmqpAdaptor.BeginReceive();

            RedisAdaptor = new RedisAdaptor(ConfigManager.CacheConn());
            CacheDataService = new RedisDataServiceDelegate(RedisAdaptor);

            Console.ReadKey();
        }
    }
}
