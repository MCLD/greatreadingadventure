using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Branch : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int SystemId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }
        public string Address { get; set; }
        [MaxLength(50)]
        public string Telephone { get; set; }
    }
}
