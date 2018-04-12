using System;
namespace csv_merge
{
    public class CrawlItem
    {
        public string URL { get; set; }
        public string PageTitle { get; set; }
        public string MetaDescription1 { get; set; }
        public string MetaDescription2 { get; set; }
        public string MetaKeyword1 { get; set; }
        public string H1 { get; set; }
        public string H2 { get; set; }
        public string StatusCode { get; set; }
        public string ResponseTime { get; set; }
        public string RealURL { get; set; }

        public string FixURL(){
            var aux =  this.URL.Split(new string[] { "seoprerender=T" }, StringSplitOptions.None);
            string auxURL = aux[0];
            string output = auxURL.Substring(auxURL.Length - 1, 1);
            if(output == "?")
            {
                return auxURL.Substring(0, auxURL.Length - 1);
            }
            else
            {
                return auxURL;
            }
        }

        public CrawlItem(){}
    }
}
