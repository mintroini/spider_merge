using System;
namespace csv_merge
{
    public class ExportItem
    {
        public string DefaultURL { get; set; }
        public string PreRenderURL { get; set; }
        public bool IsSame { get; set; }
        public bool FoundInPreRender { get; set; }
        public string statusA { get; set; }
        public string statusB { get; set; }


        //public int DeltaLoadTime { get; set; }
        public bool HasMissingInfo { get; set; }
        public string MergeInformation { get; set; }

        public string Pre_PageTitle { get; set; }
        public string Pre_H1 { get; set; }
        public string Pre_H2 { get; set; }
        public string Pre_MetaDescription { get; set; }
        public string Pre_MetaKeyWord { get; set; }



        public ExportItem()
        {
        }
    }
}
