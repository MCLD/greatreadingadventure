using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteListViewModel
    {
        public IEnumerable<Site> Sites { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
