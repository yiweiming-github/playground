using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Strategy
{
    public class SimpleHighLowStrategy : FutureStrategyBase
    {
        public SimpleHighLowStrategy(
            int longOpenCount, 
            int longCloseCount, 
            int shortOpenCount, 
            int shortCloseCount, 
            SimpleHighLowTradeType tradeType = SimpleHighLowTradeType.Both, 
            HighLowType highLowType = HighLowType.Abs,
            bool adjustByVol = false,
            int volLookBackDay = 5)
        {
            _longOpenCount = longOpenCount;
            _longCloseCount = longCloseCount;
            _shortOpenCount = shortOpenCount;
            _shortCloseCount = shortCloseCount;
            _adjustByVol = adjustByVol;
            _volLookBackDay = volLookBackDay;

            _tradeType = tradeType;
            _highLowType = highLowType;
        }

        protected override void ProcessBar(CandleBar bar)
        {
            var vol = 0.0;
            if (_volQueue.Count < _volLookBackDay - 1)
            {
                _volQueue.Enqueue(Math.Abs(bar.High - bar.Low) / bar.Open);
            }
            else
            {
                if (_volQueue.Count >= _volLookBackDay)
                {
                    _volQueue.Dequeue();
                }
                _volQueue.Enqueue(Math.Abs(bar.High - bar.Low) / bar.Open);
                vol = _volQueue.Average() * 50.0;
            }

            var longOpenCount = (_adjustByVol && vol > 0.8) ? (int)(_longOpenCount / 2) + 1 : _longOpenCount;
            var longCloseCount = (_adjustByVol && vol > 0.8) ? (int)(_longCloseCount / 2) + 1 : _longCloseCount;
            var shortOpenCount = (_adjustByVol && vol > 0.8) ? (int)(_shortOpenCount / 2) + 1 : _shortOpenCount;
            var shortCloseCount = (_adjustByVol && vol > 0.8) ? (int)(_shortCloseCount / 2) + 1 : _shortCloseCount;

            if (_longOpenQueue.Count < _longOpenCount || _longCloseQueue.Count < _longCloseCount ||
                _shortOpenQueue.Count < _shortOpenCount || _shortCloseQueue.Count < _shortCloseCount)
            {
                if (_longOpenQueue.Count < _longOpenCount)
                {
                    _longOpenQueue.Enqueue(bar.GetHigh(_highLowType));
                    if (bar.GetHigh(_highLowType) > _longPreviousHigh)
                    {
                        _longPreviousHigh = bar.GetHigh(_highLowType);
                    }
                }

                if (_longCloseQueue.Count < _longCloseCount)
                {
                    _longCloseQueue.Enqueue(bar.GetLow(_highLowType));
                    if (bar.GetLow(_highLowType) < _longPreviousLow)
                    {
                        _longPreviousLow = bar.GetLow(_highLowType);
                    }
                }

                if (_shortOpenQueue.Count < _shortOpenCount)
                {
                    _shortOpenQueue.Enqueue(bar.GetLow(_highLowType));
                    if (bar.GetLow(_highLowType) < _shortPreviousLow)
                    {
                        _shortPreviousLow = bar.GetLow(_highLowType);
                    }
                }

                if (_shortCloseQueue.Count < _shortCloseCount)
                {
                    _shortCloseQueue.Enqueue(bar.GetHigh(_highLowType));
                    if (bar.GetHigh(_highLowType) > _shortPreviousHigh)
                    {
                        _shortPreviousHigh = bar.GetHigh(_highLowType);
                    }
                }
            }
            else
            {   
                //_longOpenQueue.Dequeue();
                _longOpenQueue.Enqueue(bar.GetHigh(_highLowType));                
                var longOpenHigh = Max(_longOpenQueue, longOpenCount);                

                //_longCloseQueue.Dequeue();
                _longCloseQueue.Enqueue(bar.GetLow(_highLowType));
                var longCloseLow = Min(_longCloseQueue, longCloseCount);

                //_shortOpenQueue.Dequeue();
                _shortOpenQueue.Enqueue(bar.GetLow(_highLowType));
                var shortOpenLow = Min(_shortOpenQueue, shortOpenCount);

                //_shortCloseQueue.Dequeue();
                _shortCloseQueue.Enqueue(bar.GetHigh(_highLowType));
                var shortCloseHigh = Max(_shortCloseQueue, shortCloseCount);

                if (_tradeType != SimpleHighLowTradeType.ShortOnly)
                {
                    if (longOpenHigh > _longPreviousHigh)
                    {
                        if (_longPositionVolume <= 0)
                        {
                            Buy(bar.Time, bar.Close, 1.0, PositionType.Long);
                        }
                    }

                    if (longCloseLow < _longPreviousLow)
                    {
                        Sell(bar.Time, bar.Close, 1.0, PositionType.Long);
                    }
                }

                if (_tradeType != SimpleHighLowTradeType.LongOnly)
                {
                    if (shortOpenLow < _shortPreviousLow)
                    {
                        if (_shortPositionVolume <= 0)
                        {
                            Buy(bar.Time, bar.Close, 1.0, PositionType.Short);
                        }
                    }

                    if (shortCloseHigh > _shortPreviousHigh)
                    {
                        Sell(bar.Time, bar.Close, 1.0, PositionType.Short);
                    }
                }

                _longPreviousHigh = longOpenHigh;
                _longPreviousLow = longCloseLow;
                _shortPreviousLow = shortOpenLow;
                _shortPreviousHigh = shortCloseHigh;
            }           

            base.ProcessBar(bar);
        }

        private double Max(Queue<double> queue, int count = 0)
        {
            if (count <= 0 || queue.Count <= count)
            {
                return queue.Max();
            }

            var array = queue.ToArray();
            var result = new double[count];
            Array.Copy(array, queue.Count - count, result, 0, count);
            return result.Max();
        }

        private double Min(Queue<double> queue, int count = 0)
        {
            if (count <= 0 || queue.Count <= count)
            {
                return queue.Min();
            }

            var array = queue.ToArray();
            var result = new double[count];
            Array.Copy(array, queue.Count - count, result, 0, count);
            return result.Min();
        }

        private SimpleHighLowTradeType _tradeType;
        private HighLowType _highLowType;

        private bool _adjustByVol;
        private int _volLookBackDay;

        private double _longPreviousHigh = double.NegativeInfinity;
        private double _longPreviousLow = double.PositiveInfinity;

        private double _shortPreviousHigh = double.NegativeInfinity;
        private double _shortPreviousLow = double.PositiveInfinity;

        private int _longOpenCount;
        private int _longCloseCount;
        private int _shortOpenCount;
        private int _shortCloseCount;

        private Queue<double> _longOpenQueue = new Queue<double>();
        private Queue<double> _longCloseQueue = new Queue<double>();

        private Queue<double> _shortOpenQueue = new Queue<double>();
        private Queue<double> _shortCloseQueue = new Queue<double>();

        private Queue<double> _volQueue = new Queue<double>();
    }

    public enum SimpleHighLowTradeType
    {
        Both,
        LongOnly,
        ShortOnly
    }
}
