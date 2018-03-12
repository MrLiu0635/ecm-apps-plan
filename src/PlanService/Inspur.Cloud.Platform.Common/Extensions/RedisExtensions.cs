using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Inspur.ECP.Rtf.Extensions
{
    public static class RedisExtensions
    {
        public static void Set<T>(this IDistributedCache cache, string key, T value)
        {
            cache.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var value = cache.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }


        [Obsolete("此方法仅作为验证功能使用，性能差，不要用。")]
        public static void BinarySet(this IDistributedCache cache, string key, object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(stream, value);
                byte[] bytes = stream.ToArray();
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                options.SlidingExpiration = TimeSpan.FromMinutes(10);
                cache.Set(key, bytes, options);
            }

        }

        [Obsolete("此方法仅作为验证功能使用，性能差，不要用。")]
        public static T BinaryGet<T>(this IDistributedCache cache, string key)
        {
            var value = cache.Get(key);
            if (value == null)
            {
                return default(T);
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream st = new MemoryStream(value))
                {
                    return (T)bf.Deserialize(st);
                }
            }
        }

    }
}
