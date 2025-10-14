using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class ProgramViewModel
    {
        public PsProgram Program { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }
        public string Image { get; set; }
        public bool EnablePerformerLivestreamQuestions { get; set; }
        public bool Approve { get; set; }
    }
}
