using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP
{
    class Program
    {
        static void Main(string[] args)
        {
            var assets = FileDataReader.ReadAssetsFromFile("assets.csv");

            var portfolio = new Portfolio
            {
                Assets = assets
            };

            RiskParityCalculator.CalculateRiskParityWeight(portfolio);

            Console.ReadLine();
        }
    }
}
