using System;

namespace GRA.Domain.Model.Report
{
    public class VendorTitlesOnOrder
    {
        public int Count { get; set; }
        public string Details { get; set; }
        public DateTime EarliestDate { get; set; }
        public DateTime LatestDate { get; set; }
    }
}
