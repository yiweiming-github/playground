using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;

namespace RP
{
    public class Asset
    {
        public List<double> Values
        {
            get
            {
                return _values;
            }

            set
            {
                _values = value;                
            }
        }

        public List<double> NetValues
        {
            get
            {
                return _netValues;
            }
        }
        
        public double StandardDeviation
        {
            get
            {
                return _standardDeviation;
            }
        }

        public void UpdateNetValue()
        {
            _netValues = CalculateNetValue(_values);
            _standardDeviation = _netValues.StandardDeviation();
        }

        protected List<double> _values;
        protected List<double> _netValues;
        protected double _standardDeviation;

        public static List<double> CalculateNetValue(List<double> values)
        {
            if (values == null)
            {
                return null;
            }

            if (values.Count == 0)
            {
                return values;
            }

            var benchmark = values[0];
            return values.Select(x => x / benchmark).ToList();
        }
    }
}
