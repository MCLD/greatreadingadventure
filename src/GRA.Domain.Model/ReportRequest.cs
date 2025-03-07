using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ReportRequest : Abstract.BaseDomainEntity
    {
        public ReportRequest()
        {
            Properties = new Dictionary<JobDetailsPropertyName, string>();
        }

        public bool Favorite { get; set; }
        public DateTime? Finished { get; set; }

        [MaxLength(255)]
        public string InstanceName { get; set; }

        [MaxLength(255)]
        public string Message { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public IDictionary<JobDetailsPropertyName, string> Properties { get; }
        public int ReportCriteriaId { get; set; }
        public int ReportId { get; set; }
        public string ResultJson { get; set; }
        public int? SiteId { get; set; }
        public DateTime? Started { get; set; }
        public bool? Success { get; set; }
    }
}
