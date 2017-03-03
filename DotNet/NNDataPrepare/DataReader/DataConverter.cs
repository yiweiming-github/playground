using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Citics.Trading.Strategy.Data;

namespace DataReader
{
    public class DataConverter
    {
        public enum GridType
        {
            OnlyHighLow,
            ClassicKBar
        }

        public enum PixelType
        {
            OutBar = 0,
            InBar = 9,
            LongBody = 1,
            LongTail = 2,
            ShortBody = -1,
            ShortTail = -2
        }

        public void GenerateDataForClassification(List<string> codes, int startDate, int endDate, string gridFilePath, string predictFilePath, string preProcessResultFilePath)
        {
            //PreProcess
            var data = ReadFromDb(codes[0], startDate, endDate);
            var preProcessResults = PreProcess(data);

            for (var i = 1; i < codes.Count; ++i)
            {
                data = ReadFromDb(codes[i], startDate, endDate);
                preProcessResults.AddRange(PreProcess(data));
            }

            SavePreProcessResult(preProcessResults, preProcessResultFilePath);

            var thresholds = FindThreshold(preProcessResults, 10);

            for (var i = 1; i <= thresholds.Count; ++i)
            {
                Console.WriteLine("threshold {0} : {1}", i, thresholds[i - 1]);
            }

            // Process
            data = ReadFromDb(codes[0], startDate, endDate);
            var sample = ProcessDataForDL(data, thresholds);

            //converter.VisualizeData(sample.Item1.Last(), 50, 20);

            var itemCount = sample.Item1.Count;
            SavePlainToFile(sample, gridFilePath, predictFilePath, append: false);
            Console.WriteLine("Finished {0} of {1}", 1, codes.Count);

            for (var i = 1; i < codes.Count; ++i)
            {
                data = ReadFromDb(codes[i], startDate, endDate);
                sample = ProcessDataForDL(data, thresholds);
                SavePlainToFile(sample, gridFilePath, predictFilePath, append: true);

                if (sample != null)
                {
                    itemCount += sample.Item1.Count;
                }

                Console.WriteLine("Finished {0} of {1}", i + 1, codes.Count);
            }
        }

        public void GenerateDataForRL(List<string> codes, int startDate, int endDate, string outputFileFolder, int window = 20, int predict = 5)
        {
            var indexFilePath = outputFileFolder + @"\\index.txt";
            var codesFilePath = outputFileFolder + @"\\codes.txt";
            const string chartFilePathFormat = "{0}\\{1}_charts.txt";
            const string priceFilePathFormat = "{0}\\{1}_prices.txt";
            const string datesFilePathFormat = "{0}\\{1}_dates.txt";

            var index = 1;
            var data = ReadFromDb(codes[0], startDate, endDate, shuffle: false);
            var dummyThresholds = new List<double>() { 1 };
            var sample = ProcessDataForDL(data, dummyThresholds);        
            if (data != null && sample != null && data.Count - sample.Item1.Count >= window + predict - 2)
            {
                //SaveIndexFile(string.Format("{0}", index), indexFilePath, append: false);
                SaveChartFile(sample, string.Format(chartFilePathFormat, outputFileFolder, index), append: false);
                SavePriceFile(data.GetRange(window - 1, sample.Item1.Count + 1), string.Format(priceFilePathFormat, outputFileFolder, index), append: false);
                SaveDateFile(sample, string.Format(datesFilePathFormat, outputFileFolder, index), append: false);
                SaveCodeFile(codes[0], codesFilePath, append: false);
            }
            Console.WriteLine("Finished {0} of {1}", 1, codes.Count);
            ++index;

            for (var i = 1; i < codes.Count; ++i, ++index)
            {
                data = ReadFromDb(codes[i], startDate, endDate, shuffle: false);
                sample = ProcessDataForDL(data, dummyThresholds);
                if (data != null && sample != null && data.Count - sample.Item1.Count >= window + predict - 2)
                {
                    //SaveIndexFile(string.Format("{0}", index), indexFilePath, append: true);
                    SaveChartFile(sample, string.Format(chartFilePathFormat, outputFileFolder, index), append: false);
                    SavePriceFile(data.GetRange(window - 1, sample.Item1.Count + 1), string.Format(priceFilePathFormat, outputFileFolder, index), append: false);
                    SaveDateFile(sample, string.Format(datesFilePathFormat, outputFileFolder, index), append: false);
                    SaveCodeFile(codes[i], codesFilePath, append: true);
                }
                Console.WriteLine("Finished {0} of {1}", i + 1, codes.Count);
            }

            SaveIndexFile(string.Format("{0}", index - 1), indexFilePath, append: false);
        }
        
        #region private functions

        private static void SavePreProcessResult(List<double> results, string filePath)
        {
            if (results != null)
            {
                using (var file = new StreamWriter(filePath, append: false))
                {
                    foreach (var digit in results)
                    {
                        file.WriteLine(digit);
                    }                    
                }
            }
        }

        private List<double> PreProcess(List<Tuple<double, double, double, double, int>> raw, int window = 20, int gridCount = 50, int predict = 5)
        {
            var results = new List<double>();
            var gridMatrix = new List<int[]>();
            var processingQueue = new Queue<Tuple<double, double, double, double,int>>();
            var i = 0;
            for (; i < window; ++i)
            {
                processingQueue.Enqueue(raw[i]);
            }

            var max = processingQueue.Max(x => x.Item1);
            var min = processingQueue.Min(x => x.Item2);
            results.Add(CalculateTargetValue(raw, 0, window, predict, max - min));
            //var forwardMax = raw.GetRange(window, predict).Max(x => x.Item1);
            //results.Add((forwardMax - raw[window - 1].Item3) / (max - min));

            for (i = 1; i < raw.Count - window - predict + 1; ++i)
            {
                processingQueue.Dequeue();
                processingQueue.Enqueue(raw[i + window]);

                max = processingQueue.Max(x => x.Item1);
                min = processingQueue.Min(x => x.Item2);

                results.Add(CalculateTargetValue(raw, i, window, predict, max - min));
                //forwardMax = raw.GetRange(i + window, predict).Max(x => x.Item1);
                //results.Add((forwardMax - raw[i + window - 1].Item3) / (max - min));
            }

            return results;
        }

        private double CalculateTargetValue(List<Tuple<double, double, double, double,int>> raw, int currentIndex, int window, int predict, double windowRange)
        {
            return ForwardClose(raw, currentIndex, window, predict, windowRange);
        }

        private double ForwardHigh(List<Tuple<double, double, double, double, int>> raw, int currentIndex, int window, int predict, double windowRange)
        {
            var forwardHigh = raw.GetRange(currentIndex + window, predict).Max(x => x.Item1);
            return (forwardHigh - raw[currentIndex + window - 1].Item3) / windowRange;
        }

        private double ForwardClose(List<Tuple<double, double, double, double, int>> raw, int currentIndex, int window, int predict, double windowRange)
        {
            var forwardClose = raw[currentIndex + window + predict - 1].Item3;
            return (forwardClose - raw[currentIndex + window - 1].Item3) / windowRange;
        }

        private List<Tuple<double, double, double, double, int>> ReadFromDb(string code, int startDate, int endDate, bool shuffle = true)
        {
            var db = DBFactory.DATAENGINE;
            var sql = new StringBuilder();
            sql.AppendFormat(@"SELECT T1.HIGH_PRICE, T1.LOW_PRICE, T1.CLOSE_PRICE, T1.OPEN_PRICE, T1.PRICE_DATE FROM STOCK_PRICE_BY_DATE T1, STOCK T2 
WHERE T2.STOCK_CODE= '{0}' AND T1.INSTRUMENT_ID ＝ T2.INSTRUMENT_ID 
AND T1.PRICE_DATE >= {1} AND T1.PRICE_DATE <= {2} ORDER BY PRICE_DATE", code, startDate, endDate);

            var dbCommand = db.GetSqlStringCommand(sql.ToString());
            var rawData = new List<Tuple<double, double, double, double,int>>();
            using (var reader = db.ExecuteReader(dbCommand))
            {                
                while (reader.Read())
                {                    
                    var high = DataOperator.GetDouble(reader, "HIGH_PRICE");
                    var low = DataOperator.GetDouble(reader, "LOW_PRICE");
                    var close = DataOperator.GetDouble(reader, "CLOSE_PRICE");
                    var open = DataOperator.GetDouble(reader, "OPEN_PRICE");
                    var date = DataOperator.GetInt32(reader, "PRICE_DATE");
                    rawData.Add(new Tuple<double, double, double, double, int>(high, low, close, open, date));
                }
            }            

            if (shuffle)
            {
                var ran = new Random();
                for (var i = 0; i < rawData.Count; ++i)
                {
                    var k = ran.Next(0, rawData.Count - 1);
                    var temp = rawData[i];
                    rawData[i] = rawData[k];
                    rawData[k] = temp;
                }
            }

            return rawData;
        }

        //private List<Tuple<double, double, double, double, int>> ReadFromDb(List<string> codes, int startDate, int endDate)
        //{
        //    var rawData = new List<Tuple<double, double, double, double, int>>();            
        //    foreach(var code in codes)
        //    {
        //        rawData.AddRange(ReadFromDb(code, startDate, endDate));                
        //    }           

        //    return rawData;
        //}

        private Tuple<List<int[]>, int[], int[]> ProcessDataForDL(List<Tuple<double, double, double, double, int>> raw, List<double> thresholds, int window = 20, int gridCount = 50, int predict = 5)
        {
            if (raw.Count < window + predict)
            {
                return null;
            }            

            var gridMatrix = new List<int[]>();
            var predictList = new List<int>();
            var dates = new List<int>();
            var processingQueue = new Queue<Tuple<double, double, double, double, int>>();

            var i = 0;
            for(; i < window; ++i)
            {
                processingQueue.Enqueue(raw[i]);
            }

            var max = processingQueue.Max(x => x.Item1);
            var min = processingQueue.Min(x => x.Item2);

            var gridArray = Gridize(processingQueue, gridCount);
            gridMatrix.Add(gridArray);

            predictList.Add(Categorize(CalculateTargetValue(raw, 0, window, predict, max - min), thresholds));
            dates.Add(raw[i - 1].Item5);
            //var forwardMax = raw.GetRange(window, predict).Max(x => x.Item1);            
            //predictList.Add(Categorize((forwardMax - raw[window - 1].Item3) / (max - min), thresholds));

            for (i = 0; i < raw.Count - window - predict + 1; ++i)
            {
                processingQueue.Dequeue();
                processingQueue.Enqueue(raw[i + window]);

                max = processingQueue.Max(x => x.Item1);
                min = processingQueue.Min(x => x.Item2);

                gridArray = Gridize(processingQueue, gridCount);
                gridMatrix.Add(gridArray);

                predictList.Add(Categorize(CalculateTargetValue(raw, i, window, predict, max - min), thresholds));
                dates.Add(raw[i + window].Item5);
                //forwardMax = raw.GetRange(i + window, predict).Max(x => x.Item1);
                //predictList.Add(Categorize((forwardMax - raw[i + window - 1].Item3) / (max - min), thresholds));
            }

            return new Tuple<List<int[]>, int[], int[]>(gridMatrix, predictList.ToArray(), dates.ToArray());
        }

        private List<double> FindThreshold(List<double> results, int bucketCount)
        {
            results.Sort();

            var step = (int)results.Count / bucketCount;
            var thresholds = new List<double>();
            for(var j = 0; j < bucketCount; ++j)
            {
                if ((j + 1) * step < results.Count)
                {
                    thresholds.Add(results[(j + 1) * step]);
                }
                else
                {
                    //thresholds.Add(results.Last());
                    break;
                }

                if (thresholds.Count == bucketCount - 1)
                {
                    break;
                }
            }

            return thresholds;
        }

        private int Categorize(double x, List<double> thresholds)
        {
            var category = 0;
            for (var i = 0; i < thresholds.Count; ++i)
            {
                if (x > thresholds[i])
                {
                    category = i;
                }
                else
                {
                    category = i;
                    return category;
                }            
            }

            return thresholds.Count;
        }

        private int[] Gridize(Queue<Tuple<double, double, double, double, int>> data, int gridCount, GridType gridType = GridType.ClassicKBar)
        {
            var max = data.Max(x => x.Item1);
            var min = data.Min(x => x.Item2);
            var gridUnit = (max - min) / gridCount;

            var array = new int[data.Count * gridCount];
            var listToProcess = data.ToList();
            for (var i = 0; i < listToProcess.Count; ++i)
            {
                var current = min;
                var row = gridCount - 1;
                while (row >= 0)
                {
                    var index = row * listToProcess.Count + i;
                    if (gridType == GridType.OnlyHighLow)
                    {
                        if (current >= listToProcess[i].Item2 && current <= listToProcess[i].Item1)
                        {
                            array[index] = (int)PixelType.InBar;
                        }
                        else
                        {
                            array[index] = (int)PixelType.OutBar;
                        }
                    }
                    else if (gridType == GridType.ClassicKBar)
                    {
                        if (current > listToProcess[i].Item1 || current < listToProcess[i].Item2)
                        {
                            array[index] = (int)PixelType.OutBar;
                        }
                        else
                        {
                            if (listToProcess[i].Item3 >= listToProcess[i].Item4) //Close >= Open means Long
                            {
                                if (current >= listToProcess[i].Item4 && current <= listToProcess[i].Item3)
                                {
                                    array[index] = (int)PixelType.LongBody;
                                }
                                else
                                {
                                    array[index] = (int)PixelType.LongTail;
                                }
                            }
                            else // Short
                            {
                                if (current >= listToProcess[i].Item3 && current <= listToProcess[i].Item4)
                                {
                                    array[index] = (int)PixelType.ShortBody;
                                }
                                else
                                {
                                    array[index] = (int)PixelType.ShortTail;
                                }
                            }
                        }
                    }

                    current += gridUnit;
                    --row;
                }
            }

            return array;
        }

        private void VisualizeData(int[] array, int row, int col)
        {
            const int bar = 0xA1F6;
            for (var i = 0; i < row; ++i)
            {
                for (var j = 0; j < col; ++j)
                {
                    if (array[i * col + j] > 0)
                    {
                        Console.Write(array[i * col + j]);
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.Write("\n");
            }
        }

        /// <summary>
        /// The grid data will be written in byte format to save space.
        /// The first line will be a integer number indicating the array length of each row,
        /// for example, original data:
        /// 1 0 0 0 1 1 0 0 0 1 0 1 0 0 0 1 0 1 0 1 0 1 1 0 1 1 1 0 1
        /// 
        /// the digit count is 29
        /// 
        /// STEP 1:
        /// group as every 8 digits, and padding with 0 for the last segment
        /// 10001100, 01010001, 01010110, 11101 000
        /// 
        /// STEP 2:
        /// convert each byte to int value:
        /// 140,81,86,232
        /// 
        /// So the file will be:
        /// 
        /// 29
        /// 140,81,86,232
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gridFile"></param>
        /// <param name="predictFile"></param>
        private void SaveEncodedToFile(Tuple<List<int[]>, int[]> data, string gridFile, string predictFile)
        {

        }

        private void SavePlainToFile(Tuple<List<int[]>, int[], int[]> data, string gridFile, string predictFile, bool append = false)
        {
            SaveChartFile(data, gridFile, append);
            SavePredictFile(data, predictFile, append);
        }

        private void SaveChartFile(Tuple<List<int[]>, int[], int[]> data, string gridFile, bool append = false)
        {
            if (data != null)
            {
                using (var file = new StreamWriter(gridFile, append))
                {
                    foreach (var array in data.Item1)
                    {
                        foreach (var digit in array)
                        {
                            file.Write("{0} ", digit);
                        }
                        file.WriteLine();
                    }
                }
            }
        }

        private void SavePredictFile(Tuple<List<int[]>, int[], int[]> data, string predictFile, bool append = false)
        {
            if (data != null)
            {
                using (var file = new StreamWriter(predictFile, append))
                {
                    foreach (var predict in data.Item2)
                    {
                        file.WriteLine(predict);
                    }
                }
            }
        }

        private void SaveIndexFile(string content, string indexFile, bool append = false)
        {
            using (var file = new StreamWriter(indexFile, append))
            {
                file.WriteLine(content);                
            }
        }

        private void SavePriceFile(List<Tuple<double, double, double, double, int>> data, string filePath, bool append = false)
        {
            if (data != null)
            {
                using (var file = new StreamWriter(filePath, append))
                {
                    foreach (var prices in data)
                    {
                        file.WriteLine("{0} {1} {2} {3}", prices.Item1, prices.Item2, prices.Item3, prices.Item4);
                    }
                }
            }
        }

        private void SaveDateFile(Tuple<List<int[]>, int[], int[]> data, string filePath, bool append = false)
        {
            if (data != null)
            {
                using (var file = new StreamWriter(filePath, append))
                {
                    foreach(var date in data.Item3)
                    {
                        file.WriteLine(date);
                    }
                }
            }
        }

        private void SaveCodeFile(string code, string filePath, bool append = false)
        {
            if (!string.IsNullOrEmpty(code))
            {
                using (var file = new StreamWriter(filePath, append))
                {
                    file.WriteLine(code);
                }
            }
        }
        #endregion
    }
}
