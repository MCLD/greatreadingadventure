using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Program : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public int AchieverPointAmount { get; set; }
        public int? AchieverBadgeId { get; set; }
        public int? JoinBadgeId { get; set; }
        [Required]
        public bool AskAge { get; set; }
        [Required]
        public bool AgeRequired { get; set; }
        [Required]
        public bool AskSchool { get; set; }
        [Required]
        public bool SchoolRequired { get; set; }
        public int Position { get; set; }

        public string FromEmailName { get; set; }
        public string FromEmailAddress { get; set; }
        public int? AgeMaximumAllowed { get; set; }
        public int? AgeMinimumAllowed { get; set; }

    }
}
