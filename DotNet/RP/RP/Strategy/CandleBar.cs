using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Strategy
{
    public class CandleBar
    {
        public DateTime Time { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
        public double Amount { get; set; }

        public double GetHigh(HighLowType type = HighLowType.Abs)
        {
            return type == HighLowType.Abs ? High : (Open > Close ? Open : Close);
        }

        public double GetLow(HighLowType type = HighLowType.Abs)
        {
            return type == HighLowType.Abs ? Low : (Open < Close ? Open : Close);
        }
    }

    public enum HighLowType
    {
        Abs,
        Body
    }
}
