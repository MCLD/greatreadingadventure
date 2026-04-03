using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GRA.Domain.Model.Utility
{
    public class AvatarTextTransfer
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [MaxLength(130)]
        public string AltText { get; set; }

        public string LanguageName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [MaxLength(255)]
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [MaxLength(255)]
        public string RemoveLabel { get; set; }
    }
}
