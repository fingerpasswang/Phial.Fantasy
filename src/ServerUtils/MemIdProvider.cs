using System;
using System.Collections.Generic;
using System.Globalization;
namespace ServerUtils
{
    public enum IdSegTypePersistence
    {
        Player,
        Session,
        All,
    }
    public enum IdSegTypeMemory
    {
        All,
    }

    public class MemIdProvider
    {
        private readonly int groupId;
        private readonly int processId;
        private uint startTime;

        public MemIdProvider(int number, int sid)
        {
            groupId = number;
            processId = sid;

            InitProviders();
        }

        public ulong GenId<T>(T type) where T : IConvertible
        {
            return Helper<T>.GenId(type);
        }

        public void SetTime(uint time)
        {
        }
                #region helper
        static class Helper<T> where T : IConvertible
        {
            private static readonly IFormatProvider formater = new NumberFormatInfo();
            private static readonly List<IIdProvider<T>> idProviders = new List<IIdProvider<T>>();
            public static ulong GenId(T type)
            {
                return idProviders[type.ToInt32(formater)].GenId(type);
            }
            public static void AddProvider(IIdProvider<T> provider)
            {
                idProviders.Add(provider);
            }
        }

        private void InitProviders()
        {
            for (int i = 0; i < (int)IdSegTypeMemory.All; i++)
            {
                Helper<IdSegTypeMemory>.AddProvider(new MemoryIdProvider(processId));
            }

            var timeStamp = TimeUtils.DateTimeToUnixTime(new DateTime(2014, 1, 1));

            startTime = TimeUtils.GetNowUnixTime() - timeStamp;

            for (int i = 0; i < (int)IdSegTypePersistence.All; i++)
            {
                Helper<IdSegTypePersistence>.AddProvider(new PersistenceIdProvider(groupId, processId, startTime));
            }
        }
        #endregion
    }

    internal interface IIdProvider<T>
    {
        ulong GenId(T type);
    }

    internal class TrivialIdProvider<T> : IIdProvider<T>
    {
        public ulong GenId(T type)
        {
            return 0;
        }
    }

    /// <summary>
    /// id generation rule in-memory
    /// </summary>
    internal class MemoryIdProvider : IIdProvider<IdSegTypeMemory>
    {
        private ulong index = 1;
        private readonly int processId;
        public MemoryIdProvider(int processId)
        {
            this.processId = processId;
        }
        public ulong GenId(IdSegTypeMemory type)
        {
            var genId = index * 1000 + (ulong)processId * 10 + (ulong)type;

            ++index;

            return genId;
        }
    }

    /// <summary>
    /// id generation rule in-persistence
    /// </summary>
    internal class PersistenceIdProvider : IIdProvider<IdSegTypePersistence>
    {
        private ulong index = 1;
        private readonly int groupId;
        private readonly int processId;
        private readonly uint startTime;
        public PersistenceIdProvider(int groupId, int processId, uint startTime)
        {
            //UXHelper.UXAssert(groupId >= 0 && groupId <= 999, "groupId range");
            //UXHelper.UXAssert(processId >= 0 && processId <= 99, "processId range");

            this.groupId = groupId;
            this.processId = processId;
            this.startTime = startTime;
        }

        // hexadecimal-based generation rule
        // (persistence,1)(idType,5)(reserve,1)(time,29)(index,13)(groupId,10)(processId,5)
        //       63         62-58       57       56-28     27-15      14-5         4-0

        // decimalism-based generation rule
        //type time offset group processid
        // 1    9     4     3        2
        public ulong GenId(IdSegTypePersistence type)
        {
            ulong genId = (ulong)type * 1000000000000000000 + (ulong)startTime * 1000000000 + index * 100000 + (ulong)groupId * 100 +
                          (ulong)processId;

            // todo change to Interlocked.Exchange
            index++;
            return genId;
        }
    }
}
