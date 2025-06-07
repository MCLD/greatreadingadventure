using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class StoredReport
    {
        public StoredReport(string title, DateTime asOf)
        {
            AsOf = asOf;
            Title = title;
            Links = new Dictionary<string, string>();
        }

        public DateTime AsOf { get; }
        public IEnumerable<IEnumerable<object>> Data { get; set; }
        public IEnumerable<object> FooterRow { get; set; }
        public IEnumerable<object> FooterText { get; set; }
        public IEnumerable<object> HeaderRow { get; set; }
        public IDictionary<string, string> Links { get; }
        public string Title { get; }
    }
}
