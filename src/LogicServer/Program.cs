using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Auto.DataAccess;
using Auto.Server;
using Config;
using DataAccess;
using RPCBase;
using RPCBase.Server;

namespace LogicServer
{
    static class Program
    {
        private static AmqpAdaptor amqpAdaptor;
        private static ZkAdaptor zkAdaptor;

        public static ClientLogicServiceDelegate ClientLogicService;

        public static RedisDataServiceDelegate CacheService;
        public static MysqlDataServiceDelegate DbService;

        private static DbClientNotifyDelegate dbClientNotifyDelegate;
        private static LoginNotifyDelegate loginNotifyDelegate;
        private static SchedulerLogicServiceDelegate SchedulerLogicService;
        static void InitService()
        {
            AutoInit.Init();
            TypeRegister.Init();

            RoutingRule.DistrictsName = ConfigManager.DistrictsName();

            zkAdaptor = new ZkAdaptor();
            //amqpAdaptor = new AmqpAdaptor(ConfigManager.MQIP(), ConfigManager.AMQPPort(),
            //    ConfigManager.MQUser(), ConfigManager.MQPass());

            //ServiceImplementStub.Bind<LogicClientServiceImpl, ILogicClientImpl>(amqpAdaptor, new LogicClientServiceImpl());
            ServiceImplementStub.Bind<IDbClientNotifyImpl>(zkAdaptor, new NotifyServiceImpl());

            //ClientLogicService = new ClientLogicServiceDelegate(amqpAdaptor);
            //loginNotifyDelegate = new LoginNotifyDelegate(amqpAdaptor);
            //SchedulerLogicService = new SchedulerLogicServiceDelegate(amqpAdaptor);

            dbClientNotifyDelegate = new DbClientNotifyDelegate(zkAdaptor);

            //ConfigManager.RegisterParser(new ConfigManager.ConfigParserDelegate<EndPoint>(s =>
            //{
            //    var rets = s.Split(':');
            //    var host = rets[0];
            //    var port = 0;

            //    int.TryParse(rets[1], out port);

            //    IPAddress ip;
            //    if (IPAddress.TryParse(host, out ip))
            //    {
            //        return new IPEndPoint(ip, port);
            //    }

            //    return new DnsEndPoint(host, port);
            //}));

            CacheService = new RedisDataServiceDelegate(new RedisAdaptor(ConfigManager.Instance.Read<List<EndPoint>>("DataService", "CacheConn")));
            //DbService = new MysqlDataServiceDelegate(new MysqlAdaptor(ConfigManager.DbIP(), ConfigManager.DbName(), ConfigManager.DbUser(), ConfigManager.DbPass()));

            zkAdaptor.BeginReceive();
            //amqpAdaptor.BeginReceive();

            zkAdaptor.Identity("Logic");

            //Heartbeat();
        }

        private static void OneLoop()
        {

        }

        static void Main(string[] args)
        {
            InitService();

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
