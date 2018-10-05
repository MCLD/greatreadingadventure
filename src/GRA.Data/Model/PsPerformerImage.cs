using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsPerformerImage : Abstract.BaseDbEntity
    {
        public int PerformerId { get; set; }
        public PsPerformer Performer { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }
    }
}
