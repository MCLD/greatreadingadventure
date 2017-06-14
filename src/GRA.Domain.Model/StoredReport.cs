using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class StoredReport
    {
        public string Title { get; set; }
        public IEnumerable<IEnumerable<object>> Data { get; set; }
        public IEnumerable<object> HeaderRow { get; set; }
        public IEnumerable<object> FooterRow { get; set; }
        public IEnumerable<object> FooterText { get; set; }
        public DateTime AsOf { get; set; }
    }
}
