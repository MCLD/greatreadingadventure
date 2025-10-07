using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AvatarColorText
    {
        [Required]
        public int AvatarColorId { get; set; }
        public AvatarColor AvatarColor { get; set; }

        [Required]
        public int LanguageId { get; set; }
        public Language Language { get; set; }

        [Required]
        [MaxLength(130)]
        public string AltText { get; set; }
    }
}
