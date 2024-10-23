using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Location : Abstract.BaseDomainEntity
    {
        [MaxLength(255)]
        [Required]
        public string Address { get; set; }

        public int EventCount { get; set; }

        [MaxLength(50)]
        public string Geolocation { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        public int SiteId { get; set; }

        [MaxLength(50)]
        public string Telephone { get; set; }

        [Display(Name = "Link to this location")]
        [MaxLength(255)]
        public string Url { get; set; }
    }
}
