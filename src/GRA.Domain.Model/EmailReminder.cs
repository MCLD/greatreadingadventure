using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class EmailReminder : Abstract.BaseDomainEntity
    {
        [Required]
        public string Email { get; set; }

        public int? LanguageId { get; set; }

        public string LanguageName { get; set; }

        public DateTime? SentAt { get; set; }

        [Required]
        public string SignUpSource { get; set; }
    }
}
