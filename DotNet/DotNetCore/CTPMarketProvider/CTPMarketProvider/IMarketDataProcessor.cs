using System;
using System.Collections.Generic;
using System.Text;
using YieldChain.CTP;

namespace CTPMarketProvider
{
    interface IMarketDataProcessor
    {
        void Process(ThostFtdcDepthMarketDataField data);
    }
}
