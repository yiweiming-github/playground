using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class NormalProducer : IProducer
    {
        public void Produce(Queue<long> queue, object queueLock)
        {
            lock (queueLock)
            {
                queue.Enqueue(_counter++);
            }
        }

        private long _counter;
    }
}
