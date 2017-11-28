using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Categories
{
    public class CategoryListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Category> Categories { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.Category Category { get; set; }
        public string Search { get; set; }
    }
}
