﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HistoryListViewModel
    {
        public List<HistoryItemViewModel> Historys { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        public int TotalPoints { get; set; }
    }
}
