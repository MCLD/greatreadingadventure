using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DirectEmailTemplate : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public EmailBase EmailBase { get; set; }

        [Required]
        public int EmailBaseId { get; set; }

        public int? SystemEmail { get; set; }
    }
}
