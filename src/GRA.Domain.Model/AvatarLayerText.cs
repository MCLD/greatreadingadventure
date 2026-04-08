using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarLayerText
    {
        [MaxLength(255)]
        public string Language { get; set; }

        public int LanguageId { get; set; }

        /// <summary>
        /// This variable does not map back to the database and is only used to deserialize items
        /// during avatar import.
        /// </summary>
        public string LanguageName { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string RemoveLabel { get; set; }
    }
}
