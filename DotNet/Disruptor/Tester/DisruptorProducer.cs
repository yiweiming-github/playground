using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;

namespace Tester
{
    class DisruptorProducer
    {
        private readonly RingBuffer<LongEvent> _ringBuffer;

        public DisruptorProducer(RingBuffer<LongEvent> ringBuffer)
        {
            _ringBuffer = ringBuffer;
            _value = 0;
        }

        public void Produce()
        {
            var sequence = _ringBuffer.Next();
            try
            {
                _ringBuffer[sequence].Value = _value++;
            }
            finally
            {
                _ringBuffer.Publish(sequence);
            }
        }

        public static LongEvent CreateEvent()
        {
            return new LongEvent();
        }

        private long _value;
    }
}
