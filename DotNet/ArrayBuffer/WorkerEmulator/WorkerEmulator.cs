using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;


namespace WorkerEmulator
{
    class WorkerEmulator
    {
        static void Main(string[] args)
        {
            using(var context = ZmqContext.Create())
            using (var listener = context.CreateSocket(SocketType.PULL))
            using(var responder = context.CreateSocket(SocketType.PUSH))
            {
                responder.Connect("tcp://127.0.0.1:5557");
                listener.Connect("tcp://127.0.0.1:5556");
                //listener.SubscribeAll();
                var count = 0;
                while (true)
                {
                    var msg = listener.ReceiveMessage();
                    //Console.WriteLine("received a message {0}", count);
                    responder.SendMessage(msg);
                    Thread.Sleep(10);
                    ++count;
                }
            }
        }
    }
}
