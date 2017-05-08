using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Questionnaire : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int RelatedSystemId { get; set; }
        public int RelatedBranchId { get; set; }
        public bool IsDeleted { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public ICollection<Question> Questions { get; set; }
        public bool IsLocked { get; set; }
        public int? BadgeId { get; set; }
        [MaxLength(255)]
        public string BadgeName { get; set; }
        [MaxLength(1000)]
        public string BadgeNotificationMessage { get; set; }
    }
}
