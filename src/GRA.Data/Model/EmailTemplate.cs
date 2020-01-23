using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class EmailTemplate : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Description { get; set; }

        [MaxLength(255)]
        [Required]
        public string Subject { get; set; }

        [MaxLength(255)]
        [Required]
        public string FromName { get; set; }

        [MaxLength(255)]
        [Required]
        public string FromAddress { get; set; }

        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public int EmailsSent { get; set; }
    }
}
