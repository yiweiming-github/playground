using RP.Strategy;
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
            FuturesSimpleStrateyTest();
            Console.ReadLine();            
        }

        static void RiskParityTest()
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
        }

        static void FuturesSimpleStrateyTest()
        {
            var bars = InputHelper.ReadFuturesDataFromSinaExportFile("RB00_market.txt");
            var strategy = new SimpleHighLowStrategy(10, 5, 20, 10, SimpleHighLowTradeType.Both, HighLowType.Abs);
            strategy.Run(5000, bars);

            strategy.OutputValueSequence("net_value.txt");
            strategy.OutputActions("buy_sell_actions.txt");
        }
    }
}
