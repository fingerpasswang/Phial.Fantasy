using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LoginServer
{
    class ServerGroupManager
    {
        static readonly Dictionary<string, DateTime> Groups = new Dictionary<string, DateTime>();
        private static Timer timer;

        static ServerGroupManager()
        {
            timer = new Timer(Clear, null, 0, 1000);
        }

        private static void Clear(object o)
        {
            var now = DateTime.Now;
            lock (Groups)
            {
                foreach (string d in Groups.Keys.ToList())
                {
                    if ((now - Groups[d]).TotalSeconds > 6)
                    {
                        Groups.Remove(d);
                        Console.WriteLine(string.Format("ServerGroupManager clear: {0}", d));
                    }
                }
            }
        }

        public static void Update(string districts)
        {
            var s = DateTime.Now;
            lock (Groups)
            {
                if (!Groups.ContainsKey(districts))
                {
                    Console.WriteLine(string.Format("ServerGroupManager new districts: {0}", districts));
                }
                Groups[districts] = s;
            }
        }

        public static List<string> GetServerGroups()
        {
            lock (Groups)
            {
                return Groups.Keys.ToList();
            }
        }

    }
}
