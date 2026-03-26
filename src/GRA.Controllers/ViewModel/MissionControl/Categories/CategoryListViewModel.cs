using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Categories
{
    public class CategoryListViewModel
    {
        public IEnumerable<Domain.Model.Category> Categories { get; set; }
        public GRA.Domain.Model.Category Category { get; set; }
        public bool EnforceA11y { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
    }
}
