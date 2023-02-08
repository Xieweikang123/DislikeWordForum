using Microsoft.Extensions.Caching.Memory;
using System;

namespace BackendAPI.Web.Core.Helper
{

    /// <summary>
    /// 缓存
    /// </summary>
    public class MyCache
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 平滑过渡时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireIn"></param>
        public static void SetSliding<T>(string key, T value, TimeSpan expireIn)
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = expireIn
            };

            _cache.Set(key, value, options);
        }

        //public void SetAbsoluteExp<T>(string key, T value, TimeSpan expireIn)
        //{

        //    var options = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = 
        //    };

        //    _cache.Set(key, value, options);
        //}

        public static bool TryGetValue<T>(string key, out T value)
        {
            return _cache.TryGetValue<T>(key, out value);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
