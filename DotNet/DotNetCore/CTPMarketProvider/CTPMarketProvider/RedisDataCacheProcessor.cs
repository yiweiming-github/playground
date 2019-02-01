using CTPMarketProvider.Redis;
using Microsoft.Extensions.Caching.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YieldChain.CTP;

namespace CTPMarketProvider
{
    class RedisDataCacheProcessor : IMarketDataProcessor
    {
        public RedisDataCacheProcessor(string connection)
        {
            _connectionStr = connection;
            _redisOptions = new RedisCacheOptions
            {
                Configuration = _connectionStr
            };
            _redisHelper = new JsonRedisHelper(_redisOptions);
        }

        public void Process(ThostFtdcDepthMarketDataField data)
        {
            Console.WriteLine($"Processing by RedisDataCacheProcessor: {data.UpdateTime}, {data.UpdateMillisec}, {data.InstrumentID}, {data.LastPrice}");
            _redisHelper.Set(data.InstrumentID, data);
        }

        readonly string _connectionStr;
        readonly RedisCacheOptions _redisOptions;
        readonly IRedisHelper _redisHelper;
    }

    [Serializable]
    public class SerializableDataField
    {
        public string TradingDay;
        public double AskPrice1;
        public int AskVolume1;
        public double BidPrice2;
        public int BidVolume2;
        public double AskPrice2;
        public int AskVolume2;
        public double BidPrice3;
        public int BidVolume3;
        public int BidVolume1;
        public double AskPrice3;
        public double BidPrice4;
        public int BidVolume4;
        public double AskPrice4;
        public int AskVolume4;
        public double BidPrice5;
        public int BidVolume5;
        public double AskPrice5;
        public int AskVolume5;
        public int AskVolume3;
        public double BidPrice1;
        public int UpdateMillisec;
        public string UpdateTime;
        public string InstrumentID;
        public string ExchangeID;
        public string ExchangeInstID;
        public double LastPrice;
        public double PreSettlementPrice;
        public double PreClosePrice;
        public double PreOpenInterest;
        public double OpenPrice;
        public double HighestPrice;
        public double LowestPrice;
        public int Volume;
        public double Turnover;
        public double OpenInterest;
        public double ClosePrice;
        public double SettlementPrice;
        public double UpperLimitPrice;
        public double LowerLimitPrice;
        public double PreDelta;
        public double CurrDelta;
        public double AveragePrice;
        public string ActionDay;
    }
}
