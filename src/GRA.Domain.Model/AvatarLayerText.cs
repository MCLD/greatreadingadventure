using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarLayerText
    {
        [MaxLength(255)]
        public string Language { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string RemoveLabel { get; set; }
    }
}
