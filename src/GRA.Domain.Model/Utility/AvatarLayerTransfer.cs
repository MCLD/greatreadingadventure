using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GRA.Domain.Model.Utility
{
    public class AvatarLayerTransfer
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<AvatarColorTransfer> AvatarColors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<AvatarItemTransfer> AvatarItems { get; set; }

        public bool CanBeEmpty { get; set; }

        public bool DefaultLayer { get; set; }

        public int GroupId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int Position { get; set; }

        [MaxLength(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RemoveLabel { get; set; }

        public bool ShowColorSelector { get; set; }

        public bool ShowItemSelector { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<AvatarTextTransfer> Texts { get; set; }

        public decimal ZoomScale { get; set; }
        public int ZoomYOffset { get; set; }
    }
}
