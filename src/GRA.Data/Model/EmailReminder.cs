using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class EmailReminder : Abstract.BaseDbEntity
    {
        [Required]
        public string Email { get; set; }

        public int? LanguageId { get; set; }

        public DateTime? SentAt { get; set; }

        [Required]
        public string SignUpSource { get; set; }
    }
}
