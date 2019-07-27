using System;
using System.Collections.Generic;
using System.Text;
using YieldChain.CTP;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace CTPMarketProvider
{
    class ZeroMqDataProcessor : IMarketDataProcessor, IDisposable
    {
        public ZeroMqDataProcessor(string connnection)
        {
            _pubSocket = new PublisherSocket();
            _pubSocket.Bind(connnection);
        }        

        public void Process(ThostFtdcDepthMarketDataField data)
        {
            Console.WriteLine($"Processing by ZeroMqDataProcessor: {data.UpdateTime}, {data.UpdateMillisec}, {data.InstrumentID}, {data.LastPrice}");
            _pubSocket.SendFrame(JsonConvert.SerializeObject(data));
        }

        public void Dispose()
        {
            _pubSocket?.Dispose();
        }

        readonly PublisherSocket _pubSocket;
    }
}
