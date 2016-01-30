using System;

namespace ServerUtils
{
    public static class TimeUtils
    {
        static DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        public static DateTime UnixTimeToDateTime(uint sec)
        {
            return start.AddSeconds(sec);
        }

        public static uint DateTimeToUnixTime(DateTime time)
        {
            return (uint)(time - start).TotalSeconds;
        }
        public static double DateTimeToUnixTimeDouble(DateTime time)
        {
            return (time - start).TotalSeconds;
        }
        public static DateTime UnixTimeDoubleToDateTime(double sec)
        {
            return start.AddSeconds(sec);
        }

        public static DateTime GetNowDateTime()
        {
            return DateTime.Now;
        }

        public static uint GetNowUnixTime()
        {
            return DateTimeToUnixTime(DateTime.Now);
        }

        public static double GetNowUnixTimeDouble()
        {
            return (DateTime.Now - start).TotalSeconds;
        }
    }
}
