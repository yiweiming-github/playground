using System;
using System.Threading.Tasks;
using YieldChain.CTP;

namespace CTPMarketProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            //var ctpMarket = new CTPMarketAdapter("tcp://180.168.146.187:10010", "9999", "120846", "2")
            //{
            //    DataProcesser = new RedisDataCacheProcessor("127.0.0.1:6379")
            //};

            //ctpMarket.Start();

            var emulator = new DataEmulator(new ZeroMqDataProcessor("tcp://*:5556"));
            emulator.Run();

            Console.Read();
        }
    }
}
