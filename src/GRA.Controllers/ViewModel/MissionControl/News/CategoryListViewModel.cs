using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.News
{
    public class CategoryListViewModel
    {
        public IEnumerable<NewsCategory> Categories { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public NewsCategory Category { get; set; }
    }
}
