﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.GroupTypes
{
    public class GroupTypesListViewModel
    {
        public IEnumerable<GroupType> GroupTypes { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GroupType GroupType { get; set; }
        public int? MaximumHouseholdMembers { get; set; }
        public int SiteId { get; set; }
    }
}
