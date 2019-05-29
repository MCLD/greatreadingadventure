using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class EmailUserLog
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int EmailTemplateId { get; set; }
        public virtual EmailTemplate EmailTemplate { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        [Required]
        public string EmailAddress { get; set; }
    }
}
