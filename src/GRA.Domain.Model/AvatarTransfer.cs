using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Domain.Model
{
    public class AvatarTransfer : Abstract.BaseDomainEntity
    {
        [NotMapped]
        public string CreatedByName { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }

        public long? FileKBytes { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DataTransferType TransferType { get; set; }
    }
}
