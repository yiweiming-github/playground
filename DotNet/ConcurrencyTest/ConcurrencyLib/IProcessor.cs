using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyLib
{
    interface IProcessor
    {
        void Process(int taskCount);
    }
}
