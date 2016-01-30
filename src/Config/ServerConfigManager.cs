using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Config
{
    public class ServerConfigManager
    {
        static readonly string[] ConfigNameCollection = {@"Common", @"Connection"};
        static IEnumerable<TItem> JoinItems<TItem>(IEnumerable<IEnumerable<TItem>> tCollection)
        {
            foreach (var entry in tCollection)
            {
                if (entry == null)
                {
                    continue;
                }

                foreach (var item in entry)
                {
                    yield return item;
                }
            }
        }

        private static ServerConfigManager instance;
        public static ServerConfigManager Instance
        {
            get { return instance ?? (instance = new ServerConfigManager()); }
        }

        private readonly IniFileItemRecorder itemRecorder;

        private ServerConfigManager()
        {
            const string configDir = @"configdir/";
            if (!Directory.Exists(configDir))
            {
                Console.WriteLine("ServerConfigManager config directory error dir={0}", configDir);
                return;
            }

            var query = JoinItems(Array.ConvertAll(ConfigNameCollection, (name =>
            {
                var path = configDir + name;

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }

                var iniFile = new IniFile(path);

                return iniFile.Items;
            })));

            var items = new List<IniFileItem>();
            foreach (var iniFileItem in query)
            {
                items.Add(iniFileItem);
            }

            itemRecorder = new IniFileItemRecorder(items);
        }

        public delegate T ConfigParserDelegate<T>(string value);
        public static void RegisterParser<T>(ConfigParserDelegate<T> parser)
        {
            IniFileItemRecorder.ConvertHelper<T>.Converter = value => parser(value);
            IniFileItemRecorder.ConvertHelper<List<T>>.Converter = IniFileItemRecorder.ConvertList<T>;
        }

        #region Read接口
        /// <summary>
        /// 从对象中以section/keyword读取出指定的值
        /// </summary>
        /// <param name="section">区域值</param>
        /// <param name="keyword">关键词值</param>
        /// <returns>值内容</returns>
        public T Read<T>(string section, string keyword)
        {
            return itemRecorder.Read<T>(section, keyword);
        }
        #endregion

        #region Temp

        public static string MQIP()
        {
            return Instance.Read<string>("MQService", "MQIP");
        }
        public static int MQTTPort()
        {
            return Instance.Read<int>("MQService", "MQTTPort");
        }
        public static int AMQPPort()
        {
            return Instance.Read<int>("MQService", "AMQPPort");
        }
        public static string MQUser()
        {
            return Instance.Read<string>("MQService", "MQUser");
        }
        public static string MQPass()
        {
            return Instance.Read<string>("MQService", "MQPass");
        }

        public static string DbIP()
        {
            return Instance.Read<string>("DataService", "DbIP");
        }
        public static string DbName()
        {
            return Instance.Read<string>("DataService", "DbName");
        }
        public static string DbUser()
        {
            return Instance.Read<string>("DataService", "DbUser");
        }
        public static string DbPass()
        {
            return Instance.Read<string>("DataService", "DbPass");
        }

        public static List<EndPoint> CacheConn()
        {
            return Instance.Read<List<EndPoint>>("DataService", "CacheConn");
        }

        public static string DistrictsName()
        {
            try
            {
                return Instance.Read<string>("Common", "DistrictsName");
            }
            catch (Exception)
            {
                return Environment.UserName;
            }
        }

        #endregion
    }
}
