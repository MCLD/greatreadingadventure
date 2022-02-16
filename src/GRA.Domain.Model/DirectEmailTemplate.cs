using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DirectEmailTemplate : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public DirectEmailTemplateText DirectEmailTemplateText { get; set; }

        public EmailBase EmailBase { get; set; }

        [Required]
        public int EmailBaseId { get; set; }

        public bool SentBulk { get; set; }

        [MaxLength(25)]
        public string SystemEmailId { get; set; }
    }
}
