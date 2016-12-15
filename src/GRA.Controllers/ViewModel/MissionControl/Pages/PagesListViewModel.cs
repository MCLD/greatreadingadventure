using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Pages
{
    public class PagesListViewModel
    {
        public List<GRA.Domain.Model.Page> Pages { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool CanAddPage { get; set; }
        public bool CanDeletePage { get; set; }   
    }
}
