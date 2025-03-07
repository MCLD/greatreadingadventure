using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ReportRequest : Abstract.BaseDbEntity
    {
        public bool Favorite { get; set; }
        public DateTime? Finished { get; set; }

        [MaxLength(255)]
        public string InstanceName { get; set; }

        [MaxLength(255)]
        public string Message { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public int ReportCriteriaId { get; set; }
        public int ReportId { get; set; }
        public string ResultJson { get; set; }
        public DateTime? Started { get; set; }
        public bool? Success { get; set; }
    }
}
