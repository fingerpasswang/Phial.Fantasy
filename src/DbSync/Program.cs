using System;
using System.Threading;
using System.Threading.Tasks;
using Auto.DataAccess;
using Auto.Server;
using Config;
using DataAccess;

namespace DbSync
{
    class Program
    {
        public static RedisDataServiceDelegate CacheDataService;
        public static MysqlDataServiceDelegate PersistenceDataService;

        public static RedisAdaptor RedisAdaptor;
        public static MysqlAdaptor MysqlAdaptor;

        static void Main(string[] args)
        {
            RedisAdaptor = new RedisAdaptor(ConfigManager.CacheConn());
            MysqlAdaptor = new MysqlAdaptor(ConfigManager.DbIP(), ConfigManager.DbName(), ConfigManager.DbUser(), ConfigManager.DbPass());
            
            CacheDataService = new RedisDataServiceDelegate(RedisAdaptor);
            PersistenceDataService = new MysqlDataServiceDelegate(MysqlAdaptor);

            while (true)
            {
                Console.WriteLine("begin check...");
                Console.WriteLine(ThreadInfo());

                Work().Wait();

                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }

        static string ThreadInfo()
        {
            return string.Format("ThreadCtx:{0} Thread:{1}", Thread.CurrentContext.ContextID,
                Thread.CurrentThread.ManagedThreadId);
        }

        static async Task<bool> Work()
        {
            Console.WriteLine(ThreadInfo());
            for (int i = 0; i < 4; i++)
            {
                var key = string.Format("task:{0}", i);
                var tasks = await RedisAdaptor.GetTasks(key, 1);

                Console.WriteLine(ThreadInfo());

                foreach (var task in tasks)
                {
                    //var vals = task.Split(':');
                    //var id = ulong.Parse(vals[2]);
                    //var info = await CacheDataService.LoadPlayerInfoAsync(id);

                    //Console.WriteLine(ThreadInfo());

                    //await PersistenceDataService.UpdatePlayerInfoAsync(id, info);

                    //Console.WriteLine(ThreadInfo());

                    //await RedisAdaptor.DeleteTask(key, task);

                    //Console.WriteLine(ThreadInfo());

                    //Console.WriteLine("save {0} to mysql", id);
                }
            }

            return true;
        }
    }
}
