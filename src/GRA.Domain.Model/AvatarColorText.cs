using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarColorText
    {
        [MaxLength(130)]
        public string AltText { get; set; }
        public int AvatarColorId { get; set; }
        public int LanguageId { get; set; }
    }
}
