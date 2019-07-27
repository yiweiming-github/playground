using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTPMarketProvider.Redis
{
    public class JsonRedisHelper : RedisHelperBase, IDisposable
    {
        public JsonRedisHelper(RedisCacheOptions options) : base(options)
        {
        }

        public override T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _cache.StringGet(GetKey(key));
            if (!value.HasValue)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public override void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Exists(GetKey(key)))
            {
                Remove(GetKey(key));
            }

            _cache.StringSet(GetKey(key), JsonConvert.SerializeObject(value));
        }
    }
}
