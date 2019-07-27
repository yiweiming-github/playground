using System;
using System.Collections.Generic;
using System.Text;

namespace CTPMarketProvider.Redis
{
    public interface IRedisHelper
    {
        bool Exists(string key);
        T Get<T>(string key) where T : class;
        void Set(string key, object value);
        void Remove(string key);
    }
}
