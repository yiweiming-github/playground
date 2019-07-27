using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RP
{
    public class SinaApiReader
    {
        private const string HS300_URL = "http://money.finance.sina.com.cn/quotes_service/api/json_v2.php/CN_MarketData.getKLineData?symbol=sh000300&scale=240&ma=5&datalen=499";
        private const string H50_URL = "http://money.finance.sina.com.cn/quotes_service/api/json_v2.php/CN_MarketData.getKLineData?symbol=sh000016&scale=240&ma=5&datalen=499";
        private const string SZ399006_URL = "http://money.finance.sina.com.cn/quotes_service/api/json_v2.php/CN_MarketData.getKLineData?symbol=sz399006&scale=240&ma=5&datalen=499";
        //private const string T0_URL = "http://stock2.finance.sina.com.cn/futures/api/json.php/CffexFuturesService.getCffexFuturesDailyKLine?symbol=T0";
        //private const string AU0_URL = "http://stock2.finance.sina.com.cn/futures/api/json.php/IndexService.getInnerFuturesDailyKLine?symbol=AU0";
        //private const string RB0_URL = "http://stock2.finance.sina.com.cn/futures/api/json.php/IndexService.getInnerFuturesDailyKLine?symbol=RB0";

        public static List<Asset> ReadAssetsFromSina()
        {
            string[] urls = { HS300_URL, H50_URL, SZ399006_URL };
            //string[] urls = { H50_URL, SZ399006_URL };
            var assets = new List<Asset>();

            foreach (var url in urls)
            {
                var response = RequestSinaApi(url);
                var result = JsonConvert.DeserializeObject<List<SinaStockResult>>(response);
                var asset = CreateStockAsset(result);
                assets.Add(asset);
            }

            assets = AlignDataLength(assets);

            assets.ForEach(x => x.UpdateNetValue());
            return assets;
        }

        private static string RequestSinaApi(string url)
        {
            var ret = string.Empty;

            var webReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
            webReq.Method = "GET";
            //webReq.ContentType = "application/x-www-form-urlencoded";
            //webReq.Headers.Add("Accept-Language", "zh-cn");
            //webReq.Headers.Add("Accept-Encoding", "gzip,deflate");

            var response = (HttpWebResponse)webReq.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }

            return ret;
        }

        private static Asset CreateStockAsset(List<SinaStockResult> result)
        {
            return new Asset
            {
                Values = result.Select(x => x.close).ToList()
            };
        }

        private static Asset CreateT0Asset(string[][] result)
        {
            return new Asset
            {
                Values = result.Select(x => Double.Parse(x[4])).ToList()
            };
        }

        private static List<Asset> AlignDataLength(List<Asset> assets)
        {
            var days = assets.Min(x => x.Values.Count);
            assets.ForEach(asset =>
            {
                if (asset.Values.Count > days)
                {
                    asset.Values.RemoveRange(0, asset.Values.Count - days);
                }
            });
            return assets;
        }
    }

    [Serializable]
    public class SinaStockResult
    {
        public string day;
        public double open;
        public double high;
        public double low;
        public double close;        
    }
    
    [Serializable]
    public class SinaT0Result
    {
        public string[] data;
    }
}
