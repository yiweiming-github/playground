using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace ApEmulator
{
    class ApEmulator
    {
        static void Main(string[] args)
        {
            var msg = new ZmqMessage();
            msg.Append(new Frame(new byte[128]));
            var msgCount = 10000;
            var msgCountPerSec = msgCount/1000;
            using(var context = ZmqContext.Create())
            using (var publisher = context.CreateSocket(SocketType.PUB))
            {
                publisher.Bind("tcp://*:5555");

                Console.WriteLine("Start sending {0} requests...", msgCount);
                while (msgCount > 0)
                {
                    //for (int i = 0; i < msgCountPerSec; i++)
                    //{
                    //    publisher.SendMessage(msg);
                    //    --msgCount;
                    //}
                    //Thread.Sleep(2);

                    publisher.SendMessage(msg);
                    --msgCount;
                    Thread.Sleep(1);
                }
                
                Console.WriteLine("Done.");
            }
            Console.ReadLine();
        }
    }
}
