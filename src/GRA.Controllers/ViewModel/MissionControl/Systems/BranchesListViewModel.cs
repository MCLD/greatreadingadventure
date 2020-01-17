﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Systems
{
    public class BranchesListViewModel
    {
        public List<GRA.Domain.Model.Branch> Branches { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.Branch Branch { get; set; }
        public string Search { get; set; }
        public SelectList SystemList { get; set; }
        public bool ShowGeolocation { get; set; }
        public string GoogleMapsAPIKey { get; set; }
    }
}