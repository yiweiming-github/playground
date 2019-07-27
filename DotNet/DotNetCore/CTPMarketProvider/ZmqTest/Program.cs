using System;
using NetMQ;
using NetMQ.Sockets;

namespace ZmqTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var socket = new SubscriberSocket())
            {
                socket.Connect("tcp://127.0.0.1:5556");
                socket.SubscribeToAnyTopic();
                while (true)
                {
                    var msg = socket.ReceiveFrameString();
                    Console.WriteLine(msg);
                }

                //var msg = new Msg();

                //socket.Receive(ref msg);
                //Console.WriteLine(msg.ToString());
            }

            Console.Read();
        }
    }
}
