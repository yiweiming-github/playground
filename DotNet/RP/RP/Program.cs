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
            //var assets = FileDataReader.ReadAssetsFromFile("assets.csv");
            var assets = SinaApiReader.ReadAssetsFromSina();

            var portfolio = new Portfolio
            {
                Assets = assets
            };

            var weights = RiskParityCalculator.CalculateRiskParityWeight(portfolio);
            portfolio.Weights = weights;
            portfolio.CalculateStatistics();

            portfolio.ExportToCsv("exported_portfolio.csv");
            Console.ReadLine();            
        }
    }
}
