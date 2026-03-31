using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Category : Abstract.BaseDomainEntity
    {
        [MaxLength(10)]
        public string Color { get; set; }

        public double? ContrastRatio { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool MeetsWCAG21AAContrastRequirement { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int SiteId { get; set; }
    }
}
