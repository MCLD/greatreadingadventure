using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class IndexViewModel
    {
        public PsSettings Settings { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }
        public string AuthorizationCode { get; set; }
        public bool HasPermission { get; set; }
    }
}
