using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AvatarTransfer : Abstract.BaseDbEntity
    {
        [MaxLength(255)]
        public string Filename { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DataTransferType TransferType { get; set; }
    }
}
