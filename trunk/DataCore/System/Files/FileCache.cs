using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.EmbeddedWebServer;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files
{
    public class FileCache : IBackgroundOperationContainer
    {
        private const int ALLOWED_CACHE_MINUTES = 60;

        private static Dictionary<int, byte[]> _files;
        private static Dictionary<int, DateTime> _cachedTimes;
        public static object _lock = new object();
        private static MT19937 _rand = new MT19937();

        static FileCache()
        {
            _files = new Dictionary<int, byte[]>();
            _cachedTimes = new Dictionary<int, DateTime>();
        }

        public static int CacheFile(byte[] data)
        {
            Monitor.Enter(_lock);
            int tmp = _rand.Next();
            while (_files.ContainsKey(tmp))
            {
                tmp = _rand.Next();
            }
            _files.Add(tmp, data);
            _cachedTimes.Add(tmp, DateTime.Now);
            Monitor.Exit(_lock);
            return tmp;
        }

        public static byte[] GetFileFromCache(int id)
        {
            byte[] ret = null;
            Monitor.Enter(_lock);
            if (_files.ContainsKey(id))
                ret = _files[id];
            Monitor.Exit(_lock);
            return ret;
        }

        public static void RemoveFileFomCache(int id)
        {
            Monitor.Enter(_lock);
            _files.Remove(id);
            _cachedTimes.Remove(id);
            Monitor.Exit(_lock);
        }

        [BackgroundOperationCall(-1, -1, -1, -1, BackgroundOperationDaysOfWeek.All)]
        public static void Cleanup()
        {
            Monitor.Enter(_lock);
            try
            {
                int[] tmp = new int[_cachedTimes.Count];
                _cachedTimes.Keys.CopyTo(tmp, 0);
                foreach (int i in tmp)
                {
                    if (_cachedTimes.ContainsKey(i))
                    {
                        if (DateTime.Now.Subtract(_cachedTimes[i]).TotalMinutes > ALLOWED_CACHE_MINUTES)
                        {
                            _cachedTimes.Remove(i);
                            _files.Remove(i);
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}
