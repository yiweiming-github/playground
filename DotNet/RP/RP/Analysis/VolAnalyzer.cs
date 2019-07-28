using RP.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Analysis
{
    public class VolAnalyzer
    {
        public static List<double> IntradayDiffSequence(IEnumerable<CandleBar> bars, int avgLength = 5)
        {
            var queue = new Queue<double>();
            var sequence = new List<double>();
            var i = 0;
            foreach (var bar in bars)
            {
                if (i < avgLength - 1)
                {
                    queue.Enqueue(Math.Abs(bar.High - bar.Low) / bar.Open);
                    sequence.Add(0);
                    ++i;
                    continue;
                }
                if (i >= avgLength)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(Math.Abs(bar.High - bar.Low) / bar.Open);
                sequence.Add(queue.Average());
            }
            return sequence;
        }
    }
}
