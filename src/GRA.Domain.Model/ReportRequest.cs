using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ReportRequest : Abstract.BaseDomainEntity
    {
        public int? SiteId { get; set; }
        public int ReportCriteriaId { get; set; }
        public int ReportId { get; set; }
        [MaxLength(255)]
        public string InstanceName { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }
        public bool? Success { get; set; }
        public string ResultJson { get; set; }
        public bool Favorite { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
