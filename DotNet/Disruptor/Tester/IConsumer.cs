using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    interface IConsumer
    {
        void Consume(Queue<long> queue, object queueLock);
    }
}
