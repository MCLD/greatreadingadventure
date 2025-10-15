using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarItemText
    {
        [MaxLength(130)]
        public string AltText { get; set; }
        public int AvatarItemId { get; set; }
        public int LanguageId { get; set; }
    }
}
