using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class NormalConsumer : IConsumer
    {
        public NormalConsumer(Queue<long> queue, object queueLock, long maxProcessCount)
        {
            _queue = queue;
            _queueLock = queueLock;
            _maxProcessCount = maxProcessCount;
        }

        public void Consume(Queue<long> queue, object queueLock)
        {
            long x = -1;
            lock (queueLock)
            {
                if (queue.Count > 0)
                {
                    x = queue.Dequeue();
                    ++_processedCount;
                }
            }

            if (x > 0)
            {
                Console.WriteLine("consumed {0}", x);
            }
        }

        public void Run()
        {
            while (_processedCount < _maxProcessCount)
            {
                Consume(_queue, _queueLock);
            }
        }

        private readonly Queue<long> _queue;
        private readonly object _queueLock;
        private readonly long _maxProcessCount;
        private long _processedCount;
    }
}
