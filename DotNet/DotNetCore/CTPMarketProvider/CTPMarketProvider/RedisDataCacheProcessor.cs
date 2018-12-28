using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using YieldChain.CTP;

namespace CTPMarketProvider
{
    class RedisDataCacheProcessor : IMarketDataProcessor
    {
        public RedisDataCacheProcessor(string connection)
        {
            _connectionStr = connection;
            var redisManager = new RedisManagerPool(_connectionStr);
            _redisClient = redisManager.GetClient();
            _typedClient = _redisClient.As<ThostFtdcDepthMarketDataField>();
            _typedClient.GetNextSequence();
        }

        public void Process(ThostFtdcDepthMarketDataField data)
        {
            Console.WriteLine($"Processing by RedisDataCacheProcessor: {data.UpdateTime}, {data.UpdateMillisec}, {data.InstrumentID}, {data.LastPrice}");
            _typedClient[data.InstrumentID] = data;
        }

        readonly string _connectionStr;
        readonly IRedisClient _redisClient;
        readonly IRedisTypedClient<ThostFtdcDepthMarketDataField> _typedClient;
    }
}
