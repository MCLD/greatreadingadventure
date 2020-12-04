using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AvatarLayerText
    {
        [Required]
        public int AvatarLayerId { get; set; }
        public AvatarLayer AvatarLayer { get; set; }

        [Required]
        public int LanguageId { get; set; }
        public Language Language { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string RemoveLabel { get; set; }
    }
}
