using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Disruptor.Dsl;
using Disruptor.Scheduler;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            NormalModeTest();
            Console.ReadLine();
            DisruptorModeTest();
            Console.ReadLine();
        }

        private static void NormalModeTest()
        {
            var queue = new Queue<long>();
            var queueLock = new object();
            var producer = new NormalProducer();

            const long totalCount = 10000;
            const int threadCount = 4;
            var threads = new Thread[threadCount];

            for (var i = 0; i < threadCount; i++)
            {
                var consumer = new NormalConsumer(queue, queueLock, totalCount / threadCount);
                var thread = new Thread(consumer.Run);
                threads[i] = thread;
                thread.Start();
            }

            var start = DateTime.Now;
            for (var i = 0; i < totalCount; ++i)
            {
                producer.Produce(queue, queueLock);
                Thread.Sleep(1);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            var end = DateTime.Now;
            Console.WriteLine("total time of normal: {0}", end - start);
        }

        private static void DisruptorModeTest()
        {
            const long totalCount = 10000;
            const int bufferSize = 1024;

            var taskScheduler = new RoundRobinThreadAffinedTaskScheduler(4);

            var disruptor = new Disruptor<LongEvent>(DisruptorProducer.CreateEvent, bufferSize, taskScheduler);
            disruptor.HandleEventsWith(new LongEventHandler());
            disruptor.Start();

            var ringBuffer = disruptor.RingBuffer;
            var producer = new DisruptorProducer(ringBuffer);

            var start = DateTime.Now;
            for (var i = 0; i < totalCount; ++i)
            {
                producer.Produce();
                Thread.Sleep(1);
            }

            disruptor.Shutdown();
            var end = DateTime.Now;
            Console.WriteLine("total time of Disruptor: {0}", end - start);
        }
    }
}
