using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace CTPMarketWebSocket
{
    public class CTPMarketReceiver : IDisposable
    {
        private CTPMarketReceiver() { }
        public static CTPMarketReceiver Instance => _instance ?? (_instance = new CTPMarketReceiver());

        public void Start(string connection, IDataProcessor processor)
        {
            _processor = processor;
            _subSocket = new SubscriberSocket();
            _subSocket.Connect(connection);
            _subSocket.SubscribeToAnyTopic();
            while (true)
            {
                var str = _subSocket.ReceiveFrameString();
                Console.WriteLine(str);
            }
        }

        public void Dispose()
        {
            _subSocket?.Dispose();
        }

        static CTPMarketReceiver _instance;
        IDataProcessor _processor;
        SubscriberSocket _subSocket;
    }
}
