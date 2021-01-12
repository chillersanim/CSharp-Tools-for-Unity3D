using System;

namespace UnityTools.Core
{
    public static class SystemUtil
    {
        public static readonly int CacheLineSize;

        static SystemUtil()
        {
            CacheLineSize = Environment.Is64BitOperatingSystem ? 64 : 32;
        }
    }
}
