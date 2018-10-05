namespace GRA.Data.Model
{
    public class PsProgramAgeGroup
    {
        public int ProgramId { get; set; }
        public PsProgram Program { get; set; }
        public int AgeGroupId { get; set; }
        public PsAgeGroup AgeGroup { get; set; }
    }
}
