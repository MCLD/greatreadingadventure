﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Pages
{
    public class PagesListViewModel
    {
        public List<PageHeader> PageHeaders { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public PageHeader PageHeader { get; set; }
    }
}
