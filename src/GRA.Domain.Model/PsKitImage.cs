using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsKitImage : Abstract.BaseDomainEntity
    {
        public int KitId { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }
    }
}
