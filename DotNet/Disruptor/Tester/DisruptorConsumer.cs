using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;

namespace Tester
{
    public class LongEvent
    {
        public long Value { get; set; }
    }

    public class LongEventHandler : IEventHandler<LongEvent>
    {
        public void OnNext(LongEvent longEvent, long sequence, bool endOfBatch)
        {
            Console.WriteLine("consumed {0}", longEvent.Value);
        }
    }
}
