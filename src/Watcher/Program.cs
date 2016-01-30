using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Auto.Server;
using Config;
using RPCBase;
using RPCBase.Server;

namespace WatcherService
{
    class Program
    {
        public static AmqpAdaptor amqpAdaptor;
        public static ZkAdaptor zkAdaptor;

        private static DbClientNotifyDelegate dbClientNotifyDelegate;
        public static WatcherServiceNotifyDelegate watcherServiceNotifyDelegate;

        public static WatcherManager watcherManager;

        static void InitService()
        {
            AutoInit.Init();

            zkAdaptor = new ZkAdaptor();
            amqpAdaptor = new AmqpAdaptor(ConfigManager.MQIP(), ConfigManager.AMQPPort(),
                ConfigManager.MQUser(), ConfigManager.MQPass());

            ServiceImplementStub.Bind<IWatcherServiceNotifyImpl>(amqpAdaptor, new WatcherServiceImpl());

            //dbClientNotifyDelegate = new DbClientNotifyDelegate(zkAdaptor);
            watcherServiceNotifyDelegate = new WatcherServiceNotifyDelegate(amqpAdaptor);

            amqpAdaptor.BeginReceive();
            zkAdaptor.BeginReceive();

            var toWatch = ConfigManager.Instance.Read<int>("WatcherService", "WatchNum");
            var watchGroups = new List<WatchGroup>();

            for (int i = 1; i <= toWatch; i++)
            {
                var group = new WatchGroup()
                {
                    GroupName = ConfigManager.Instance.Read<string>("WatcherService", "WatchName" + i),
                    Quorum = ConfigManager.Instance.Read<int>("WatcherService", "WatchQuorum" + i),
                    DownAfterPeriod = ConfigManager.Instance.Read<int>("WatcherService", "WatchDownAfter" + i),
                    ParallelSync = ConfigManager.Instance.Read<int>("WatcherService", "WatchParallelSync" + i),
                    FailoverTime = ConfigManager.Instance.Read<int>("WatcherService", "WatchFailoverTimeout" + i),
                };

                group.SetCurrentMaster(new MasterInstance(group)
                {
                    EndPoint = ConfigManager.Instance.Read<EndPoint>("WatcherService", "WatchAddr" + i),
                });

                watchGroups.Add(group);
            }

            watcherManager = new WatcherManager(watchGroups);
        }

        // to synchronize
        private static bool ready = true;
        static void Main(string[] args)
        {
            InitService();

            while (true)
            {
                Check();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        static void Check()
        {
            if (ready)
            {
                ready = false;
                watcherManager.OneLoop().ContinueWith(
                    t =>
                    {
                        ready = true;
                    });
            }
        }
    }
}
