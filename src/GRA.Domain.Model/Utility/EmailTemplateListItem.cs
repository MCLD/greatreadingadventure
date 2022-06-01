using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Utility
{
    public class EmailTemplateListItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int EmailsSent { get; set; }
        public bool IsSystem { get; set; }
        public bool IsBulkSent { get; set; }
        public IEnumerable<int> LanguageIds { get; set; }
    }
}
