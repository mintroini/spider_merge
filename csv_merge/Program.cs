using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace csv_merge
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<CrawlItem> preRenderItems = LoadWithHelper("/Users/martinintroini/Projects/csv_merge/csv_merge/prerender.csv", true);
            List<CrawlItem> defaultItems = LoadWithHelper("/Users/martinintroini/Projects/csv_merge/csv_merge/default.csv", false);
            //List<CrawlItem> defaultItems = LoadCSV("/Users/martinintroini/Projects/csv_merge/csv_merge/default.csv", false);
            //List<CrawlItem> preRenderItems = LoadCSV("/Users/martinintroini/Projects/csv_merge/csv_merge/prerender.csv", true);
            int countOk=-1;
            int countError=-1;
            List<ExportItem> exports = CheckIssues(defaultItems, preRenderItems, out countOk, out countError);
            //List<ExportItem> exports = CheckIssues(preRenderItems, defaultItems, out countOk, out countError);
            using (var csv = new CsvWriter(new StreamWriter("/Users/martinintroini/Projects/csv_merge/csv_merge/processed-equals-defINren.csv")))
            //using (var csv = new CsvWriter(new StreamWriter("/Users/martinintroini/Projects/csv_merge/csv_merge/processed-equals-renINdef.csv")))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ",";
                csv.WriteRecords(exports);
            }

            Console.WriteLine("FINISHED");
            Console.WriteLine("OK: " + countOk.ToString());
            Console.WriteLine("ERRORS: " + countError.ToString());
            Console.ReadLine();
        }

        public static List<ExportItem> CheckIssues(List<CrawlItem> defaultItems, List<CrawlItem> preRenderItems, out int countOk , out int countError)
        {
            countOk = 0;
            countError = 0;
            var count = 0;
            List<ExportItem> ret = new List<ExportItem>();
            foreach (CrawlItem item in defaultItems)
            {
                //if(count > 200){
                //    break;
                //}
                count++;
                ExportItem e = new ExportItem();

                var foundAux = preRenderItems.Where(x => CheckURL(x.RealURL, item.RealURL)).ToList();
                var found = preRenderItems.Where(x => CheckURL(x.RealURL, item.RealURL)).FirstOrDefault();
                if (found != null)
                {
                    e.FoundInPreRender = true;
                    e.IsSame = found.RealURL == item.RealURL;
                    e.DefaultURL = item.URL;
                    e.PreRenderURL = item.URL;
                    e.statusA = item.StatusCode;
                    e.statusB = found.StatusCode;
                    //Calc DeltaLoad
                    //int foundTime=-1;
                    //int itemTime=-1;

                    //bool itemNumeric= int.TryParse(item.ResponseTime, out itemTime);
                    //bool foundNumeric= int.TryParse(found.ResponseTime, out foundTime);
                    //if(itemNumeric && foundNumeric){
                    //    int delta = foundTime - itemTime;
                    //    e.DeltaLoadTime = delta;
                    //}

                    //Check PageTitle
                    if(item.PageTitle.Length > found.PageTitle.Length)
                    {
                        e.HasMissingInfo = true;
                        e.MergeInformation = e.MergeInformation + "\n" + "Smaller Page Title";
                    }
                    e.Pre_PageTitle = found.PageTitle;

                    // Check H1
                    if (item.H1.Length > found.H1.Length)
                    {
                        e.HasMissingInfo = true;
                        e.MergeInformation = e.MergeInformation + "\n" + "Smaller H1";
                    }
                    e.Pre_H1 = found.H1;

                    // Check H2
                    if (item.H2.Length > found.H2.Length)
                    {
                        e.HasMissingInfo = true;
                        e.MergeInformation = e.MergeInformation + "\n" + "Smaller H2";
                    }
                    e.Pre_H2 = found.H2;

                    //Check Meta description
                    if (item.MetaDescription1.Length > found.MetaDescription1.Length)
                    {
                        e.HasMissingInfo = true;
                        e.MergeInformation = e.MergeInformation + "\n" + "Smaller MetaDescription1";
                    }
                    e.Pre_MetaDescription = found.MetaDescription1;

                    //Check Meta KeyWord
                    if (item.MetaKeyword1.Length > found.MetaKeyword1.Length)
                    {
                        e.HasMissingInfo = true;
                        e.MergeInformation = e.MergeInformation + "\n" + "Smaller MetaKeyword1";
                    }
                    e.Pre_MetaKeyWord = found.MetaKeyword1;
                    countOk++;
                }
                else
                {
                    e.DefaultURL = item.RealURL;
                    e.FoundInPreRender = false;
                    e.HasMissingInfo = true;
                    e.MergeInformation = "Not Found";
                    countError++;

                }

                Console.WriteLine("Count: " + count + "\nAdding: " + e.DefaultURL);
                ret.Add(e);
                
            }
            return ret;
        }

        private static bool CheckURL(string renderReal, string itemReal)
        {
            if(renderReal == itemReal){
                return true;
            }
            //if(itemReal.Contains(renderReal)){
            //    return true;
            //}
            //if (renderReal.Contains(itemReal))
            //{
            //    return true;
            //}
            return false;
        }

        public static List<CrawlItem> LoadWithHelper (string path, bool fixPrerender){
            int count = 0;

            List<CrawlItem> ret = new List<CrawlItem>();
            using (TextReader fileReader = File.OpenText(path))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                while (csv.Read())
                {
                    CrawlItem crawlItem = null;

                    if(count != 1 && count != 0){
                        if(fixPrerender){
                            if (csv.GetField<string>(1) != "image/jpeg")
                            {
                                crawlItem = new CrawlItem
                                {
                                    URL = csv.GetField<string>(0),
                                    PageTitle = csv.GetField<string>(4),
                                    MetaDescription1 = csv.GetField<string>(7),
                                    MetaDescription2 = csv.GetField<string>(10),
                                    MetaKeyword1 = csv.GetField<string>(13),
                                    H1 = csv.GetField<string>(17),
                                    H2 = csv.GetField<string>(19),
                                    StatusCode = csv.GetField<string>(3),
                                    ResponseTime = csv.GetField<string>(40)
                                };
                                crawlItem.RealURL = crawlItem.FixURL();
                            }
                        }else{
                            if (csv.GetField<string>(1) != "image/jpeg")
                            {
                                crawlItem = new CrawlItem
                                {
                                    URL = csv.GetField<string>(0),
                                    PageTitle = csv.GetField<string>(4),
                                    MetaDescription1 = csv.GetField<string>(7),
                                    MetaDescription2 = csv.GetField<string>(10),
                                    MetaKeyword1 = csv.GetField<string>(16),
                                    H1 = csv.GetField<string>(22),
                                    H2 = csv.GetField<string>(26),
                                    StatusCode = csv.GetField<string>(3),
                                    ResponseTime = csv.GetField<string>(45)
                                };
                                crawlItem.RealURL = crawlItem.URL;
                            }
                        }
                        //if(control && fixPrerender){
                        //    crawlItem.RealURL = crawlItem.FixURL();
                        //}else{
                        //    crawlItem.RealURL = crawlItem.URL;
                        //}
                        if (crawlItem != null)
                        {
                            ret.Add(crawlItem);
                        }
                    }
                    count++;
                }
            }
            return ret;
        }

        public static List<CrawlItem> LoadCSV (string csvDir, bool checkPrerender)
        {
            int count = 0;
            string[] header = new string[4];
            List<CrawlItem> ret = new List<CrawlItem>();
            using (var reader = new StreamReader(csvDir))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (count == 0)
                    {

                    } 
                    else if( count == 1){
                        header = values;
                    }
                    else
                    {
                        CrawlItem crawlItem = new CrawlItem
                        {
                            URL = values[0],
                            PageTitle = values[4],
                            MetaDescription1 = values[7],
                            MetaDescription2 = values[10],
                            MetaKeyword1 = values[16],
                            H1 = values[22],
                            H2 = values[26],
                            StatusCode = values[3],
                            ResponseTime = values[45]
                        };
                        ret.Add(crawlItem);
                    }
                    count++;
                }

            }
            return null;
        }
    }
}
