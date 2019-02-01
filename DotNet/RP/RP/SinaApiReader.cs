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
        private const string T0_URL = "http://stock2.finance.sina.com.cn/futures/api/json.php/CffexFuturesService.getCffexFuturesDailyKLine?symbol=T0";
        private const string AU0_URL = "http://stock2.finance.sina.com.cn/futures/api/json.php/IndexService.getInnerFuturesDailyKLine?symbol=AU0";
        private const string RB0_URL = "http://stock2.finance.sina.com.cn/futures/api/json.php/IndexService.getInnerFuturesDailyKLine?symbol=RB0";

        public static List<Asset> ReadAssetsFromSina()
        {
            var response = RequestSinaApi(HS300_URL);
            var hs300Result = JsonConvert.DeserializeObject<List<SinaHs300Result>>(response);
            var assetHs300 = CreateHs300Asset(hs300Result);
            

            response = RequestSinaApi(T0_URL);
            var t0Result = JsonConvert.DeserializeObject<string[][]>(response);
            var assetT0 = CreateT0Asset(t0Result);            

            response = RequestSinaApi(AU0_URL);
            var au0Result = JsonConvert.DeserializeObject<string[][]>(response);
            var assetAU0 = CreateT0Asset(au0Result);

            response = RequestSinaApi(RB0_URL);
            var rb0Result = JsonConvert.DeserializeObject<string[][]>(response);
            var assetRB0 = CreateT0Asset(rb0Result);

            var assets = new List<Asset>
            {
                assetHs300,
                assetT0,
                assetAU0,
                assetRB0
            };

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

        private static Asset CreateHs300Asset(List<SinaHs300Result> result)
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
    public class SinaHs300Result
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
