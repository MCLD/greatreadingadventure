using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class EmailTemplate : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Description { get; set; }

        [MaxLength(255)]
        [Required]
        [DisplayName("Email subject")]
        public string Subject { get; set; }

        [MaxLength(255)]
        [Required]
        [DisplayName("From Name")]
        public string FromName { get; set; }

        [MaxLength(255)]
        [Required]
        [DisplayName("From Address")]
        public string FromAddress { get; set; }

        [DisplayName("Email body in text format")]
        public string BodyText { get; set; }

        [DisplayName("Email body in HTML format")]
        public string BodyHtml { get; set; }

        public int EmailsSent { get; set; }
    }
}
