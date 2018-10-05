using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsProgramImage : Abstract.BaseDbEntity
    {
        public int ProgramId { get; set; }
        public PsProgram Program { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }
    }
}
