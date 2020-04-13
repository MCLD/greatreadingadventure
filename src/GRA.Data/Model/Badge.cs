using System.ComponentModel.DataAnnotations;
using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class Badge : BaseDbEntity
    {
        public int SiteId { get; set; }
        public string Filename { get; set; }

        [Required]
        [MaxLength(255)]
        public string AltText { get; set; }
    }
}
