using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTPMarketWebSocket
{
    public interface IDataProcessor
    {
        void Process(string data);
    }
}
