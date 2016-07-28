using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace DispatcherEmulator
{
    class DispatcherWithLock
    {
        public DispatcherWithLock()
        {
            _lock = new object();
            _dict = new Dictionary<int, int>();
        }

        public void Run()
        {
            var thread = new Thread(() => ResultListenerRun());
            thread.Start();

            var msg = new ZmqMessage();
            msg.Append(new Frame(new byte[128]));
            using (var context = ZmqContext.Create())
            using (var listener = context.CreateSocket(SocketType.SUB))
            using (var pusher = context.CreateSocket(SocketType.PUSH))
            {
                pusher.Bind("tcp://*:5556");
                listener.Connect("tcp://127.0.0.1:5555");
                listener.SubscribeAll();
                Console.WriteLine("Start run...");
                var count = 0;
                while (true)
                {
                    listener.ReceiveMessage();
                    if (count == 0)
                    {
                        _startTime = DateTime.Now;
                    }
                    //Console.WriteLine("Run received. {0}", count);
                    lock (_lock)
                    {
                        _dict[count] = 1;
                    }
                    pusher.SendMessage(msg);
                    ++count;
                }
            }
        }

        public void ResultListenerRun()
        {
            Console.WriteLine("ResultListenerRun");
            using (var context = ZmqContext.Create())
            using (var listener = context.CreateSocket(SocketType.PULL))
            {
                listener.Bind("tcp://*:5557");
                var count = 0;
                while (true)
                {
                    listener.ReceiveMessage();
                    //Console.WriteLine("ResultListenerRun received. {0}", count);
                    lock (_lock)
                    {
                        if (_dict.ContainsKey(count))
                        {
                            _dict[count] = 0;
                        }
                    }
                    ++count;
                    if (count == 3999)
                    {
                        Console.WriteLine("{0}", DateTime.Now - _startTime);
                    }
                }
            }
        }

        private object _lock;
        private Dictionary<int, int> _dict;
        private DateTime _startTime;
    }
}
