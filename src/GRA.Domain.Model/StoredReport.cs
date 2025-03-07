using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class StoredReport
    {
        public DateTime AsOf { get; set; }
        public IEnumerable<IEnumerable<object>> Data { get; set; }
        public IEnumerable<object> FooterRow { get; set; }
        public IEnumerable<object> FooterText { get; set; }
        public IEnumerable<object> HeaderRow { get; set; }
        public IDictionary<string, string> Links { get; set; }
        public string Title { get; set; }
    }
}
