using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class ProgramDetailsViewModel
    {
        public PsProgram Program { get; set; }
        public string Image { get; set; }
        public bool IsEditable { get; set; }
    }
}
