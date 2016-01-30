using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace WatcherService
{
    enum InstanceState
    {
        Normal,
        SubjectiveDown,
        ObjectiveDown,
    }

    abstract class RedisInstanceBase
    {
        public IServer Server { get; set; }
        public string Name { get; set; }
        public EndPoint EndPoint { get; set; }
        public string RunId { get; set; }

        public WatchGroup Group { get; private set; }
        public InstanceState State { get; private set; }
        public DateTime LastPing { get; private set; }
        public TimeSpan Ping { get; private set; }

        public RedisInstanceBase(WatchGroup group)
        {
            Group = group;
        }

        public void OnPing(TimeSpan latency)
        {
            Ping = latency;
            LastPing = DateTime.Now;
            State = InstanceState.Normal;
        }

        public void SubjectiveDown()
        {
            var quorum = Group.Quorum;
            var count =
                Group.Watchers
                    .Aggregate(0,
                        (i, watcher) => i + watcher.SubjectiveDownSet.Count(s => s.Equals(EndPoint.ToString())));

            State = InstanceState.SubjectiveDown;

            if (count+1 >= quorum)
            {
                State = InstanceState.ObjectiveDown;
                Console.WriteLine("{0} becomes ObjectiveDown", EndPoint);
            }

            Program.watcherServiceNotifyDelegate.NotifyInstanceSubjectiveDown(Program.amqpAdaptor.Uuid.ToByteArray(), EndPoint.ToString());
        }

        public virtual void CopyFrom(RedisInstanceBase ins)
        {
            Server = ins.Server;
            Name = ins.Name;
            Group = ins.Group;
            EndPoint = ins.EndPoint;
            RunId = ins.RunId;
            State = ins.State;
            LastPing = ins.LastPing;
            Ping = ins.Ping;
        }
    }

    internal class MasterInstance : RedisInstanceBase
    {
        public MasterInstance(WatchGroup @group) : base(@group)
        {
        }
    }

    internal class SlaveInstance : RedisInstanceBase
    {
        //public string State { get; private set; }
        public long Offset { get; private set; }
        public int Lag { get; private set; }
        public string LinkStatus { get; private set; }
        public long REPLOffset { get; private set; }
        public int Priority { get; private set; }

        public void OnMasterInfoRefresh(EndPoint ep, string info)
        {
            var infos = info.Split(',');
            var state = infos[2].Split('=')[1];
            var offset = long.Parse(infos[3].Split('=')[1]);
            var lag = int.Parse(infos[4].Split('=')[1]);

            EndPoint = ep;
            Offset = offset;
            Lag = lag;
            //State = state;
        }

        public void OnSlaveInfoRefresh(IGrouping<string, KeyValuePair<string, string>> group)
        {
            var masterLinkStatus = group.First(kv => kv.Key.Equals("master_link_status")).Value;
            var replOffset = long.Parse(group.First(kv => kv.Key.Equals("slave_repl_offset")).Value);
            var priority = int.Parse(group.First(kv => kv.Key.Equals("slave_priority")).Value);

            LinkStatus = masterLinkStatus;
            REPLOffset = replOffset;
            Priority = priority;
        }

        public SlaveInstance(WatchGroup @group) : base(@group)
        {
        }
    }
}
