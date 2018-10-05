using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsKitImage : Abstract.BaseDbEntity
    {
        public int KitId { get; set; }
        public PsKit Kit { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }
    }
}
