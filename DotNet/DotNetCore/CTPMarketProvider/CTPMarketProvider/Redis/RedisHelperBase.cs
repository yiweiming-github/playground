using Microsoft.Extensions.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTPMarketProvider.Redis
{
    public class RedisHelperBase : IRedisHelper, IDisposable
    {
        public RedisHelperBase(RedisCacheOptions options)
        {
            _connection = ConnectionMultiplexer.Connect(options.Configuration);
            _cache = _connection.GetDatabase();
            _instanceName = options.InstanceName;
        }

        public virtual bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _cache.KeyExists(GetKey(key));
        }

        public virtual T Get<T>(string key) where T : class
        {
            throw new NotFiniteNumberException();
        }

        public virtual void Set(string key, object value)
        {
            throw new NotFiniteNumberException();
        }

        public virtual void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _cache.KeyDelete(GetKey(key));
        }

        public virtual void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        protected string GetKey(string key)
        {
            return _instanceName + key;
        }

        protected IDatabase _cache;
        protected ConnectionMultiplexer _connection;
        protected string _instanceName;
    }
}
