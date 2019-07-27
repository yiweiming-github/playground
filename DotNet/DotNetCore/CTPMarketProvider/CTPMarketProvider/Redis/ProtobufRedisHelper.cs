using Microsoft.Extensions.Caching.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTPMarketProvider.Redis
{
    public class ProtobufRedisHelper : RedisHelperBase, IDisposable
    {
        public ProtobufRedisHelper(RedisCacheOptions options) : base(options)
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
            using (var stream = new MemoryStream(value))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
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

            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                _cache.StringSet(GetKey(key), stream.GetBuffer());
            }
        }
    }
}
