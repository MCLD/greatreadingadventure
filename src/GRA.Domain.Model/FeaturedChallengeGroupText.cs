using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class FeaturedChallengeGroupText
    {
        [MaxLength(255)]
        public string Filename { get; set; }

        [MaxLength(255)]
        [DisplayName("Alternative Text")]
        [Required]
        public string AltText { get; set; }

        public string ImagePath { get; set; }
    }
}
