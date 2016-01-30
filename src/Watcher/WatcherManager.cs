using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Auto.Server;
using StackExchange.Redis;

namespace WatcherService
{
    internal class WatcherManager
    {
        private static readonly object muxerSyncLock = new object();
        private static ConnectionMultiplexer muxerInstance;

        private readonly Dictionary<string, WatchGroup> groups = new Dictionary<string, WatchGroup>();
        private readonly Dictionary<string, RedisInstanceBase> redisInstancesDict = new Dictionary<string, RedisInstanceBase>();
        private readonly Dictionary<Guid, Watcher> watchersDict = new Dictionary<Guid, Watcher>();

        public WatcherManager(IEnumerable<WatchGroup> groups)
        {
            var groupList = groups.ToList();
            var config = new ConfigurationOptions()
            {
                AllowAdmin = true,
            };

            foreach (var group in groupList)
            {
                config.EndPoints.Add(group.Master.EndPoint);
            }

            muxerInstance = ConnectionMultiplexer.Connect(config);
            muxerInstance.ConnectionRestored += MuxerInstanceOnConnectionRestored;

            foreach (var group in groupList)
            {
                var server = muxerInstance.GetServer(group.Master.EndPoint);
                var epStr = server.EndPoint.ToString();

                group.Master.Server = server;
                group.Master.OnPing(TimeSpan.Zero);

                Program.zkAdaptor.Identity(group.Master.EndPoint.ToString());
                this.groups.Add(epStr, group);
                redisInstancesDict.Add(epStr, group.Master);
            }
        }

        #region Periodic Check
        private int tick = 0;
        public async Task OneLoop()
        {
            try
            {
                SyncStatesInMemory();
                if (tick%10 == 0)
                {
                    await RefreshAllMasters();
                    await RefreshAllSlaves();
                }

                if (tick%2 == 0)
                {
                    await NotifyOtherWatchers();
                }

                await PingAll();
                CheckAlive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                tick++;
            }
        }

        // every 10s
        private async Task RefreshAllMasters()
        {
            var masters = groups.Values.Select(g => g.Master);
            var toConnectSlaves = new List<RedisInstanceBase>();

            foreach (var master in masters)
            {
                try
                {
                    if (!master.Server.IsConnected)
                    {
                        continue;
                    }

                    if (master.Server.IsSlave)
                    {
                        Console.WriteLine("master {0} state hasn't been sync, still slave", master.EndPoint);
                        //todo
                        continue;
                    }

                    var val = await master.Server.InfoAsync();

                    foreach (var group in val)
                    {
                        if (group.Key.Equals("Replication"))
                        {
                            var slavesNum = int.Parse(group.First(kv => kv.Key.Equals("connected_slaves")).Value);
                            for (int i = 0; i < slavesNum; i++)
                            {
                                var slaveId = string.Format("slave{0}", i);
                                var slaveInfo = group.First(kv => kv.Key.Equals(slaveId)).Value;
                                var infos = slaveInfo.Split(',');

                                var ip = infos[0].Split('=')[1];
                                if (ip.Equals("127.0.0.1"))
                                {
                                    ip = master.EndPoint.ToString().Split(':')[0];
                                }

                                var ep = new IPEndPoint(IPAddress.Parse(ip),
                                    int.Parse(infos[1].Split('=')[1]));
                                var epStr = ep.ToString();

                                RedisInstanceBase ins = null;
                                if (!redisInstancesDict.TryGetValue(epStr, out ins))
                                {
                                    ins = redisInstancesDict[epStr] = new SlaveInstance(master.Group);
                                    toConnectSlaves.Add(ins);
                                }

                                var slaveIns = ins as SlaveInstance;

                                if (slaveIns == null)
                                {
                                    Console.WriteLine("slave {0} state hasn't been sync, still master", ins.EndPoint);
                                    continue;
                                }

                                slaveIns.OnMasterInfoRefresh(ep, slaveInfo);
                                master.Group.AddSlave(slaveIns);
                            }
                        }
                        else if (group.Key.Equals("Server"))
                        {
                            var runId = group.First(kv => kv.Key.Equals("run_id")).Value;

                            master.RunId = runId;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (toConnectSlaves.Count > 0)
            {
                ReconfigureConnection(toConnectSlaves);
            }
        }

        // every 10s
        private async Task RefreshAllSlaves()
        {
            var slaves = groups.Values.Select(g => g.Slaves.Values).Aggregate(new List<SlaveInstance>(),
                (list, collection) =>
                {
                    list.AddRange(collection);
                    return list;
                });

            foreach (var slave in slaves)
            {
                try
                {
                    if (!slave.Server.IsConnected)
                        continue;

                    if (!slave.Server.IsSlave)
                    {
                        Console.WriteLine("slave {0} state hasn't been sync, still master", slave.EndPoint);
                        // todo
                        continue;
                    }

                    var val = await slave.Server.InfoAsync();

                    foreach (var group in val)
                    {
                        if (group.Key.Equals("Replication"))
                        {
                            // intranet
                            //var masterHost = group.First(kv => kv.Key.Equals("master_host")).Value;
                            //var masterPort = int.Parse(group.First(kv => kv.Key.Equals("master_port")).Value);

                            slave.OnSlaveInfoRefresh(group);
                        }
                        else if (group.Key.Equals("Server"))
                        {
                            var runId = group.First(kv => kv.Key.Equals("run_id")).Value;

                            slave.RunId = runId;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // every 2s
        private async Task NotifyOtherWatchers()
        {
            foreach (var master in groups.Values.Select(v=>v.Master))
            {
                Program.watcherServiceNotifyDelegate.NotifyWatchingMaster(Program.amqpAdaptor.Uuid.ToByteArray(), master.EndPoint.ToString());
            }
        }

        // every 1s
        private async Task PingAll()
        {
            await Task.WhenAll(redisInstancesDict.Values.Where(v=>v.Server.IsConnected).Select(v=>v.Server.PingAsync().ContinueWith(
                ((task) =>
                {
                    v.OnPing(task.Result);
                }))));
        }

        // every 1s
        private void CheckAlive()
        {
            var now = DateTime.Now;
            var toFailover = new List<WatchGroup>();
            foreach (var value in redisInstancesDict.Values)
            {
                if ((now - value.LastPing).TotalMilliseconds > value.Group.DownAfterPeriod)
                {
                    //Console.WriteLine("{0} has been lost for {1}ms", value.EndPoint, (now - value.LastPing).TotalMilliseconds);

                    if (value.State != InstanceState.ObjectiveDown)
                    {
                        value.SubjectiveDown();
                    }
                    else if (value is MasterInstance 
                        && value.Group.Master == value
                        && Program.zkAdaptor.IsLeader(value.EndPoint.ToString()))
                    {
                        toFailover.Add(value.Group);
                    }
                }
            }

            foreach (var watchGroup in toFailover)
            {
                Failover(watchGroup);
            }
        }
        #endregion

        #region Inter-Watcher Notify

        // notify thread entrance
        public void NotifyWatchingMaster(Guid uuid, string addr)
        {
            RedisInstanceBase ins = null;
            if (!redisInstancesDict.TryGetValue(addr, out ins))
            {
                return;
            }

            if (Program.amqpAdaptor.Uuid.Equals(uuid))
            {
                //Console.WriteLine("NotifyWatchingMaster ingnore self");
                return;
            }

            Watcher watcher = null;

            lock (watchersDict)
            {
                if (!watchersDict.TryGetValue(uuid, out watcher))
                {
                    watcher = watchersDict[uuid] = new Watcher()
                    {
                        Uuid = uuid,
                    };
                }
            }

            //Console.WriteLine("NotifyWatchingMaster {0} => {1}", uuid, addr);

            ins.Group.AddWatcher(watcher);
        }

        // notify thread entrance
        public void NotifyInstanceSubjectiveDown(Guid uuid, string addr)
        {
            RedisInstanceBase ins = null;
            if (!redisInstancesDict.TryGetValue(addr, out ins))
            {
                return;
            }

            if (Program.amqpAdaptor.Uuid.Equals(uuid))
            {
                //Console.WriteLine("NotifyWatchingMaster ingnore self");
                return;
            }

            Watcher watcher = null;
            lock (watchersDict)
            {
                if (!watchersDict.TryGetValue(uuid, out watcher))
                {
                    watcher = watchersDict[uuid] = new Watcher()
                    {
                        Uuid = uuid,
                    };
                }
            }

            Console.WriteLine("NotifyInstanceSubjectiveDown {0} => {1}", uuid, addr);

            lock (watcher.SubjectiveDownSet)
            {
                watcher.SubjectiveDownSet.Add(addr);
            }
        }

        #endregion

        private void Failover(WatchGroup group)
        {
            var toLift = SelectOneSlave(group.Slaves.Values);

            if (toLift == null)
            {
                throw new Exception("no slave selected");
            }

            Console.WriteLine("failover lift {0} to master", toLift.EndPoint);

            toLift.Server.MakeMaster(ReplicationChangeOptions.Broadcast);

            foreach (var slave in group.Slaves.Values)
            {
                if (slave != toLift)
                {
                    slave.Server.SlaveOf(toLift.EndPoint);
                }
            }
        }

        private void SyncStatesInMemory()
        {
            var toLift = new List<SlaveInstance>();
            var toDown = new List<MasterInstance>();

            foreach (var ins in redisInstancesDict.Values)
            {
                if (!ins.Server.IsConnected)
                    continue;

                if (ins.Server.IsSlave && ins is MasterInstance)
                {
                    // informed that one instance changed from master to slave
                    toDown.Add((MasterInstance)ins);
                }
                else if (!ins.Server.IsSlave && ins is SlaveInstance)
                {
                    // informed that one instance changed from slave to master
                    toLift.Add((SlaveInstance)ins);
                }
            }

            foreach (var slaveInstance in toLift)
            {
                SlaveToMaster(slaveInstance);
                Program.zkAdaptor.Identity(slaveInstance.EndPoint.ToString());
            }

            foreach (var masterInstance in toDown)
            {
                MasterToSlave(masterInstance);
            }
        }

        #region for hack...
        private void RefreshAll()
        {
            foreach (var value in redisInstancesDict.Values)
            {
                value.Server = muxerInstance.GetServer(value.EndPoint);
            }
        }

        private void ReconfigureConnection(List<RedisInstanceBase> toConnect)
        {
            var config = ConfigurationOptions.Parse(muxerInstance.Configuration);

            foreach (var ins in toConnect)
            {
                config.EndPoints.Add(ins.EndPoint);
            }

            lock (muxerSyncLock)
            {
                muxerInstance.Dispose();
                muxerInstance = ConnectionMultiplexer.Connect(config);

                RefreshAll();

                muxerInstance.ConnectionRestored += MuxerInstanceOnConnectionRestored;
            }

            foreach (var ins in toConnect)
            {
                ins.Server = muxerInstance.GetServer(ins.EndPoint);
            }
        }
        #endregion

        private MasterInstance SlaveToMaster(SlaveInstance slave)
        {
            var group = slave.Group;
            var newMaster = new MasterInstance(group)
            {
                EndPoint = slave.EndPoint,
            };

            newMaster.CopyFrom(slave);

            redisInstancesDict[newMaster.EndPoint.ToString()] = newMaster;
            group.SetCurrentMaster(newMaster);
            group.RemoveSlave(slave);

            Console.WriteLine("modify {0} from slave to master in momery", slave.EndPoint);

            return newMaster;
        }

        private SlaveInstance MasterToSlave(MasterInstance master)
        {
            var group = master.Group;
            var newSlave = new SlaveInstance(group)
            {
                EndPoint = master.EndPoint,
            };
            newSlave.CopyFrom(master);

            redisInstancesDict[newSlave.EndPoint.ToString()] = newSlave;
            group.AddSlave(newSlave);

            Console.WriteLine("modify {0} from master to slave in momery", master.EndPoint);

            return newSlave;
        }

        #region event callback
        private void MuxerInstanceOnConnectionRestored(object sender, ConnectionFailedEventArgs connectionFailedEventArgs)
        {
            var epStr = connectionFailedEventArgs.EndPoint.ToString();
            RedisInstanceBase ins = null;
            if (!redisInstancesDict.TryGetValue(epStr, out ins))
            {
                return;
            }

            var master = ins as MasterInstance;
            if (master == null)
            {
                return;
            }

            if (ins.Group.Master != ins && Program.zkAdaptor.IsLeader(epStr))
            {
                Console.WriteLine("down {0} to slave of {1}", ins.EndPoint, ins.Group.Master.EndPoint);
                ins.Server.SlaveOf(ins.Group.Master.EndPoint);
                MasterToSlave(master);
            }
        }
        #endregion

        private static SlaveInstance SelectOneSlave(IEnumerable<SlaveInstance> slaves)
        {
            var now = DateTime.Now;
            var toSelect = slaves
                // exclude slaves that has been lost for this watcher
                .Where(s => s.Server.IsConnected)
                // exclude slaves that has not responded to this watcher for 5s
                .Where(s => (now - s.LastPing).TotalSeconds < 5)
                .OrderByDescending(s => s.Priority).ToArray();

            if (toSelect.Length > 1)
            {
                return toSelect.OrderByDescending(s => s.REPLOffset).FirstOrDefault();
            }

            return toSelect.FirstOrDefault();
        }
    }
}
