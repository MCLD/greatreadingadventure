using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Home
{
    public class ExitPageViewModel : BaseLandExitViewModel
    {
        public Branch Branch { get; set; }
        public string LeftHeader { get; set; }
    }
}
