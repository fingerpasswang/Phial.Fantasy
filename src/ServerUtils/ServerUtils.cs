using System;

namespace ServerUtils
{
    public static class ServerUtils
    {
        public static bool SessionIdEquals(this byte[] sid1, byte[] sid2)
        {
            return BitConverter.ToUInt64(sid1, 0) == BitConverter.ToUInt64(sid2, 0)
                   && BitConverter.ToUInt64(sid1, 8) == BitConverter.ToUInt64(sid2, 8);
        }
    }
}
