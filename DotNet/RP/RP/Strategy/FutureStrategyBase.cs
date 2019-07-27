using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Strategy
{
    public class FutureStrategyBase
    {
        public virtual void Run(double initialCash, IEnumerable<CandleBar> bars, double slippage = 0.0)
        {
            _cash = _initialValue = initialCash;
            foreach (var bar in bars)
            {
                ProcessBar(bar);
            }
        }

        protected virtual void ProcessBar(CandleBar bar)
        {
            UpdateNetValue(bar);
        }

        protected void UpdateNetValue(CandleBar bar)
        {
            var value = (bar.Close * _longPositionVolume + (2 * _shortPositionAveragePrice - bar.Close) * _shortPositionVolume + _cash) / _initialValue;
            _valueSequence.Add($"{bar.Time},{value},{bar.Close},{_cash},{_longPositionVolume},{_shortPositionVolume}");
        }

        protected void RecordAction(DateTime time, string buySell, double price, double volume, PositionType positionType)
        {
            _actions.Add($"{time} {buySell} {positionType} {volume} @ {price}");
        }

        protected virtual bool Buy(DateTime time, double price, double volume, PositionType positionType = PositionType.Long)
        {
            if (_cash < price * volume)
            {
                return false;
            }

            if (positionType == PositionType.Long)
            {
                _longPositionAveragePrice = (_longPositionAveragePrice * _longPositionVolume + price * volume) / (_longPositionVolume + volume);
                _longPositionVolume += volume;
                _cash -= price * volume;
            }
            else
            {
                _shortPositionAveragePrice = (_shortPositionAveragePrice * _shortPositionVolume + price * volume) / (_shortPositionVolume + volume);
                _shortPositionVolume += volume;
                _cash -= price * volume;
            }
            RecordAction(time, "Buy", price, volume, positionType);
            return true;
        }

        protected virtual bool Sell(DateTime time, double price, double volume, PositionType positionType = PositionType.Long)
        {
            if (positionType == PositionType.Long)
            {
                if (volume > _longPositionVolume)
                {
                    return false;
                }

                _longPositionAveragePrice = (_longPositionVolume * _longPositionAveragePrice - price * volume) / (_longPositionVolume - volume);
                _longPositionVolume -= volume;
                _cash += price * volume;

                if (_longPositionVolume <= 0)
                {
                    _longPositionAveragePrice = 0;
                }

                RecordAction(time, "Sell", price, volume, positionType);
                return true;
            }
            else
            {
                if (volume > _shortPositionVolume)
                {
                    return false;
                }

                _cash += (2 * _shortPositionAveragePrice - price) * volume;
                _shortPositionAveragePrice = (_shortPositionVolume * _shortPositionAveragePrice - price * volume) / (_shortPositionVolume - volume);
                _shortPositionVolume -= volume;                

                if (_shortPositionVolume <= 0)
                {
                    _shortPositionAveragePrice = 0;
                }

                RecordAction(time, "Sell", price, volume, positionType);
                return true;
            }
        }

        public void OutputValueSequence(string fileName)
        {
            File.WriteAllLines(fileName, _valueSequence);            
        }

        public void OutputActions(string fileName)
        {
            File.WriteAllLines(fileName, _actions);
        }

        protected double _initialValue = 0;
        protected double _cash = 0;
        protected double _longPositionVolume = 0;
        protected double _shortPositionVolume = 0;
        protected double _longPositionAveragePrice = 0;
        protected double _shortPositionAveragePrice = 0;
        protected List<string> _valueSequence = new List<string>();
        protected List<string> _actions = new List<string>();
    }

    public enum PositionType
    {
        Long,
        Short
    }
}
