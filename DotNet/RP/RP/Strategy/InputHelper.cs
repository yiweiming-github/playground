using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Strategy
{
    public class InputHelper
    {
        public static List<CandleBar> ReadFuturesDataFromSinaExportFile(string fileName)
        {
            var content = File.ReadAllText(fileName);
            var data = JsonConvert.DeserializeObject<List<List<string>>>(content);
            var bars = new List<CandleBar>();
            data.ForEach(x =>
            {
                bars.Add(new CandleBar()
                {
                    Time = DateTime.Parse(x[0]),
                    Open = Double.Parse(x[1]),
                    High = Double.Parse(x[2]),
                    Low = Double.Parse(x[3]),
                    Close = Double.Parse(x[4]),
                    Volume = Double.Parse(x[5])
                });
            });

            return bars;
        }
    }
}
