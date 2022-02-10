using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DirectEmailTemplate : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public EmailBase EmailBase { get; set; }

        [Required]
        public int EmailBaseId { get; set; }

        public int? SystemEmailId { get; set; }
    }
}
