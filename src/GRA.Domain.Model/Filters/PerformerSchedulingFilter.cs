using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class PerformerSchedulingFilter : BaseFilter
    {
        public int? AgeGroupId { get; set; }
        public bool? IsApproved { get; set; }

        public PerformerSchedulingFilter(int? page = null, int? take = null) : base(page) { }
    }
}
