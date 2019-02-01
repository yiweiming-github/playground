using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;

namespace RP
{
    public class Portfolio : Asset
    {
        public void CalculateStatistics()
        {
            if (!CheckWeights())
            {
                throw new Exception("Invalid weights!");
            }

            if (!CheckAssets())
            {
                throw new Exception("Invalid assets!");
            }

            var days = Assets[0].NetValues.Count;
            _values = new List<double>();
            for (var i = 0; i < days; ++i)
            {
                var sum = 0.0;
                for (var j = 0; j < Assets.Count; ++j)
                {
                    sum += Assets[j].NetValues[i] * Weights[j];
                }
                _values.Add(sum);
            }            
            
            _netValues = CalculateNetValue(_values);
            _standardDeviation = CalculatePortfolioStd();
            Covs = CalculateAssetCovs();
        }

        public List<Asset> Assets { get; set; }
        public List<double> Weights { get; set; }
        public List<double> Covs { get; private set; }

        private bool CheckWeights()
        {
            return Weights != null
                && Weights.Count > 0
                && Weights.Count == Assets.Count
                && Weights.Count(x => x <= 0) == 0;
        }

        private bool CheckAssets()
        {
            if (Assets == null || Assets.Count == 0)
            {
                return false;
            }

            var days = Assets[0].NetValues.Count;
            return Assets.Count(x => x.NetValues.Count != days) == 0;
        }

        private List<double> CalculateAssetCovs()
        {
            return Assets.Select(x => x.NetValues.Covariance(NetValues)).ToList();
        }

        /// <summary>
        /// STDp = SQRT(w1*w1*STD1*STD1 + w2*w2*STD2*STD2 + 2*w1*w2*COV(1, 2))
        /// </summary>
        /// <returns></returns>
        private double CalculatePortfolioStd()
        {
            var sum = 0.0;
            for (var i = 0; i < Assets.Count; ++i)
            {
                for (var j = 0; j < Assets.Count; ++j)
                {
                    sum += (Weights[i] * Weights[j] * Assets[i].NetValues.Covariance(Assets[j].NetValues) * Assets[j].NetValues.Covariance(Assets[i].NetValues));
                }
            }
            return Math.Sqrt(sum);
        }
    }
}
