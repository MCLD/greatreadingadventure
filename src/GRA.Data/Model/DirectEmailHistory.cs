using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DirectEmailHistory : Abstract.BaseDbEntity
    {
        public string BodyHtml { get; set; }

        public string BodyText { get; set; }

        public int DirectEmailTemplateId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FromEmailAddress { get; set; }

        [Required]
        [MaxLength(255)]
        public string FromName { get; set; }

        public bool IsBulk { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [MaxLength(255)]
        public string OverrideToEmailAddress { get; set; }

        [Required]
        public string SentResponse { get; set; }

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }

        public bool Successful { get; set; }

        [Required]
        [MaxLength(255)]
        public string ToEmailAddress { get; set; }

        [MaxLength(255)]
        public string ToName { get; set; }

        public User User { get; set; }

        public int? UserId { get; set; }
    }
}
