using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Broadcast : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public DateTime? SendAt { get; set; }
        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Body { get; set; }
        [DisplayName("Send To New Participants")]
        public bool SendToNewUsers { get; set; }
    }
}
