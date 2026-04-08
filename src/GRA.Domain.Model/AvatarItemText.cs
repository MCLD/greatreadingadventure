using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarItemText
    {
        [MaxLength(130)]
        public string AltText { get; set; }

        public int AvatarItemId { get; set; }

        public int LanguageId { get; set; }

        /// <summary>
        /// This variable does not map back to the database and is only used to deserialize items
        /// during avatar import.
        /// </summary>
        public string LanguageName { get; set; }
    }
}
