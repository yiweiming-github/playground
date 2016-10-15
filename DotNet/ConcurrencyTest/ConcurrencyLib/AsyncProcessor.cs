using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyLib
{
    public class AsyncProcessor : IProcessor
    {
        public async void Process(int taskCount)
        {
            Total = taskCount;
            for (int i = 0; i < taskCount; i++)
            {
                int d = await Work();
                --Total;
            }
        }

        async Task<int> Work()
        {
            Console.WriteLine("Total {0}", Total);
            for (int i = 0; i < 10000; i++)
            {
                var rnd = new Random();
                rnd.Next(1, 100000);
            }
            return 1;
        }

        public bool AllDone()
        {
            return Total == 0;
        }

        public int Total = 0;
    }
}
