using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YieldChain.CTP;

namespace CTPMarketProvider
{
    class DataEmulator
    {
        public DataEmulator(IMarketDataProcessor processor)
        {
            _processor = processor;
        }

        public void Run()
        {
            var codeList = new List<string>
            {
                "rb1901"
            };

            var rand = new Random();

            while (true)
            {
                codeList.ForEach(x =>
                {
                    _processor.Process(new ThostFtdcDepthMarketDataField
                    {
                        InstrumentID = x,
                        LastPrice = rand.NextDouble()
                    });
                });

                Thread.Sleep(500);
            }
        }

        IMarketDataProcessor _processor;
    }
}
