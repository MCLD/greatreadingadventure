using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Social
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImageAlt { get; set; }

        public int ImageHeight { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImageLink { get; set; }

        public int ImageWidth { get; set; }

        public Language Language { get; set; }

        [Key]
        [Required]
        public int LanguageId { get; set; }

        [Key]
        [Required]
        public int SocialHeaderId { get; set; }

        [Required]
        [MaxLength(70)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string TwitterUsername { get; set; }
    }
}
