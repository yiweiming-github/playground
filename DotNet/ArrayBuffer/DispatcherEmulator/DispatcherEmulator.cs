using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherEmulator
{
    class DispatcherEmulator
    {
        static void Main(string[] args)
        {
            var dispatcher = new DispatcherWithLock();
            dispatcher.Run();
        }
    }
}
