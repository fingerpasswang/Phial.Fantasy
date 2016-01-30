using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherService
{
    class WatchGroup
    {
        public string GroupName { get; set; }
        public int DownAfterPeriod { get; set; }
        public int Quorum { get; set; }
        public int ParallelSync { get; set; }
        public int FailoverTime { get; set; }

        public MasterInstance Master { get; private set; }
        public Dictionary<string, SlaveInstance> Slaves { get; private set; }

        public HashSet<Watcher> Watchers { get; private set; }

        public WatchGroup()
        {
            Slaves = new Dictionary<string, SlaveInstance>();
            Watchers = new HashSet<Watcher>();
        }

        public void SetCurrentMaster(MasterInstance master)
        {
            Master = master;
        }

        public void AddSlave(SlaveInstance slave)
        {
            Slaves[slave.EndPoint.ToString()] = slave;
        }
        public void RemoveSlave(SlaveInstance slave)
        {
            Slaves.Remove(slave.EndPoint.ToString());
        }

        public void AddWatcher(Watcher watcher)
        {
            lock (Watchers)
            {
                Watchers.Add(watcher);
            }
        }
    }
}
