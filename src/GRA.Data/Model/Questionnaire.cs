using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Questionnaire : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public ICollection<Question> Questions { get; set; }

        public bool IsLocked { get; set; }
        public int? BadgeId { get; set; }
        [MaxLength(255)]
        public string BadgeName { get; set; }
        [MaxLength(1000)]
        public string BadgeNotificiationMessage { get; set; }
    }
}
