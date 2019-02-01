using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RP
{
    public class FileDataReader
    {
        public static List<Asset> ReadAssetsFromFile(string fileName, bool ignoreFirstRow = true, bool ignoreFirstCol = false)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception("file not exist!");
            }

            List<Asset> assets = null;
            bool isFirstRow = true;
            using (var sr = new StreamReader(fileName, Encoding.Default))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (ignoreFirstRow && isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }

                    var parts = line.Split(new char[] { ',' });
                    var colCount = parts.Length;
                    if (ignoreFirstCol)
                    {
                        colCount -= 1;
                    }

                    if (assets == null)
                    {
                        assets = new List<Asset>();
                        for (var i = 0; i < colCount; ++i)
                        {
                            assets.Add(new Asset
                            {
                                Values = new List<double>()
                            });
                        }
                    }
                    else
                    {
                        if (colCount != assets.Count)
                        {
                            throw new Exception("Wrong data in file!");
                        }

                        var startIndex = ignoreFirstCol ? 1 : 0;
                        for(var i = startIndex; i < parts.Length; ++i)
                        {
                            if (ignoreFirstCol)
                            {
                                assets[i - 1].Values.Add(Double.Parse(parts[i]));
                            }
                            else
                            {
                                assets[i].Values.Add(Double.Parse(parts[i]));
                            }
                        }
                    }
                }
            }

            if (assets != null)
            {
                assets.ForEach(x => x.UpdateNetValue());
            }
            return assets;
        }
    }
}
