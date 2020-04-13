using GRA.Domain.Model.Abstract;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Badge : BaseDomainEntity
    {
        public int SiteId { get; set; }
        public string Filename { get; set; }

        [Required]
        [MaxLength(255)]
        public string AltText { get; set; }
    }
}
