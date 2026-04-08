using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GRA.Domain.Model.Utility
{
    public class AvatarItemTransfer
    {
        [JsonIgnore]
        public int Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<AvatarTextTransfer> Texts { get; set; }

        public bool Unlockable { get; set; }
    }
}
