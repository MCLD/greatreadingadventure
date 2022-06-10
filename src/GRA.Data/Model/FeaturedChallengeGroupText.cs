using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class FeaturedChallengeGroupText
    {
        public int FeaturedChallengeGroupId { get; set; }
        public FeaturedChallengeGroup FeaturedChallengeGroup { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        [MaxLength(255)]
        [Required]
        public string ImagePath { get; set; }

        [MaxLength(255)]
        [Required]
        public string AltText { get; set; }
    }
}
