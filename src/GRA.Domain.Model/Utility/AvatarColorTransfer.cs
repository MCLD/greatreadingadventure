using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GRA.Domain.Model.Utility
{
    public class AvatarColorTransfer
    {
        [JsonIgnore]
        public int Id { get; set; }
        [MaxLength(15)]
        [Required]
        public string Color { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<AvatarTextTransfer> Texts { get; set; }
    }
}
