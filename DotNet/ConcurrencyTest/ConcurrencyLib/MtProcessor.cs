using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyLib
{
    public class MtProcessor : IProcessor
    {
        public MtProcessor()
        {
            
        }

        public void Process(int taskCount)
        {
            for (int i = 0; i < taskCount; ++i)
            {
                Start();
            }
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Finish));
            ++QueueLength;
        }

        public void Finish(object obj)
        {
            Console.WriteLine("Thread {0} working. length {1}", Thread.CurrentThread.GetHashCode(), QueueLength);
            for (int i = 0; i < 10000; i++)
            {
                var rnd = new Random();
                rnd.Next(1, 100000);
            }
            --QueueLength;
        }

        public bool AllDone()
        {
            return QueueLength == 0;
        }

        public int QueueLength = 0;
    }

    
}
