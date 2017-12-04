using System.ComponentModel;

namespace GRA.Controllers.ViewModel.MissionControl.Pages
{
    public class PagesEditViewModel
    {
        public GRA.Domain.Model.Page Page { get; set; }
        public bool CanEdit { get; set; }
    }
}
