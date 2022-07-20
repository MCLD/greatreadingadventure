using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Data.Model
{
    public class DirectEmailTemplate : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [NotMapped]
        public DirectEmailTemplateText DirectEmailTemplateText { get; set; }

        public EmailBase EmailBase { get; set; }

        [Required]
        public int EmailBaseId { get; set; }

        public int EmailsSent { get; set; }
        public bool SentBulk { get; set; }

        [MaxLength(25)]
        public string SystemEmailId { get; set; }
    }
}
