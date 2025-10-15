using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AvatarItemText
    {
        [Required]
        public int AvatarItemId { get; set; }
        public AvatarItem AvatarItem { get; set; }

        [Required]
        public int LanguageId { get; set; }
        public Language Language { get; set; }

        [Required]
        [MaxLength(130)]
        public string AltText { get; set; }
    }
}
