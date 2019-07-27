using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Strategy
{
    public class SimpleHighLowStrategy : FutureStrategyBase
    {
        public SimpleHighLowStrategy(int longOpenCount, int longCloseCount, int shortOpenCount, int shortCloseCount, SimpleHighLowTradeType tradeType = SimpleHighLowTradeType.Both, HighLowType highLowType = HighLowType.Abs)
        {
            _longOpenCount = longOpenCount;
            _longCloseCount = longCloseCount;
            _shortOpenCount = shortOpenCount;
            _shortCloseCount = shortCloseCount;

            _tradeType = tradeType;
            _highLowType = highLowType;
        }

        protected override void ProcessBar(CandleBar bar)
        {
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
                _longOpenQueue.Dequeue();
                _longOpenQueue.Enqueue(bar.GetHigh(_highLowType));
                var longOpenHigh = _longOpenQueue.Max();

                _longCloseQueue.Dequeue();
                _longCloseQueue.Enqueue(bar.GetLow(_highLowType));
                var longCloseLow = _longCloseQueue.Min();

                _shortOpenQueue.Dequeue();
                _shortOpenQueue.Enqueue(bar.GetLow(_highLowType));
                var shortOpenLow = _shortOpenQueue.Min();

                _shortCloseQueue.Dequeue();
                _shortCloseQueue.Enqueue(bar.GetHigh(_highLowType));
                var shortCloseHigh = _shortCloseQueue.Max();

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

        private SimpleHighLowTradeType _tradeType;
        private HighLowType _highLowType;

        private double _longPreviousHigh = double.NegativeInfinity;
        private double _longPreviousLow = double.PositiveInfinity;

        private double _shortPreviousHigh = double.NegativeInfinity;
        private double _shortPreviousLow = double.PositiveInfinity;

        private readonly int _longOpenCount;
        private readonly int _longCloseCount;
        private readonly int _shortOpenCount;
        private readonly int _shortCloseCount;

        private Queue<double> _longOpenQueue = new Queue<double>();
        private Queue<double> _longCloseQueue = new Queue<double>();

        private Queue<double> _shortOpenQueue = new Queue<double>();
        private Queue<double> _shortCloseQueue = new Queue<double>();
    }

    public enum SimpleHighLowTradeType
    {
        Both,
        LongOnly,
        ShortOnly
    }
}
