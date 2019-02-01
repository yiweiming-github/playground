using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;

namespace RP
{
    public class RiskParityCalculator
    {
        public static List<double> CalculateRiskParityWeight(Portfolio portfolio, List<double> initialWeights = null, double resultStd = 0.0001, int maxIteration = 10000)
        {
            var weights = initialWeights;
            if (weights == null)
            {
                weights = portfolio.Assets.Select(x => 1.0 / portfolio.Assets.Count).ToList();                
            }

            var iteration = 0;
            var strBuilder = new StringBuilder();
            while (iteration < maxIteration)
            {
                Console.WriteLine($"start iteration {iteration}");

                portfolio.Weights = weights;
                portfolio.CalculateStatistics();
                var riskWeights = CalculateRiskWeights(portfolio.StandardDeviation, portfolio.Weights, portfolio.Covs);

                strBuilder.Clear();
                strBuilder.Append("Weight guess: ");
                weights.ForEach(x =>
                    strBuilder.Append($" {x}")
                );
                Console.WriteLine(strBuilder.ToString());

                strBuilder.Clear();
                strBuilder.Append("Risk weight: ");
                riskWeights.ForEach(x =>
                    strBuilder.Append($" {x}")
                );
                Console.WriteLine(strBuilder.ToString());

                if (IsGoodResult(riskWeights, resultStd))
                {
                    Console.WriteLine("Found result good enough.");
                    break;
                }

                weights = GenerateNewWeightGuess(weights, riskWeights);

                ++iteration;
            }
            
            if (iteration == maxIteration)
            {
                Console.WriteLine($"reach max iteration {maxIteration}");
            }

            return weights;
        }

        private static List<double> CalculateRiskWeights(double portfolioStd, List<double> weights, List<double> covs)
        {
            var riskWeights = new List<double>();
            for (var i = 0; i < weights.Count; ++i)
            {
                riskWeights.Add(weights[i] * covs[i] / portfolioStd);
            }
            return riskWeights;
        }

        private static bool IsGoodResult(List<double> riskWeights, double std)
        {
            return riskWeights.StandardDeviation() < std;
        }

        private static List<double> GenerateNewWeightGuess(List<double> weights, List<double> riskWeights)
        {
            var maxIndex = 0;
            var minIndex = 0;
            for (var i = 0; i < riskWeights.Count; ++i)
            {
                if (riskWeights[i] < riskWeights[minIndex])
                {
                    minIndex = i;
                }

                if (riskWeights[i] > riskWeights[maxIndex])
                {
                    maxIndex = i;
                }
            }

            weights[minIndex] += 0.0001;
            weights[maxIndex] -= 0.0001;
            return weights;
        }
    }
}
