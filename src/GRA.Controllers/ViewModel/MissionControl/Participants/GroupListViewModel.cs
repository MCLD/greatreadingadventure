using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class GroupListViewModel
    {
        public IEnumerable<GroupInfo> Groups { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public int? Type { get; set; }
        public GroupType GroupType { get; set; }

        public IEnumerable<GroupType> GroupTypeList { get; set; }
    }
}
