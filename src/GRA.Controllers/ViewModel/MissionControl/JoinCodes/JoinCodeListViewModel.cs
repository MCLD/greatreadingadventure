using System.Collections.Generic;
using System.ComponentModel;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.JoinCodes
{
    public class JoinCodeListViewModel : PaginateViewModel
    {
        public IEnumerable<JoinCode> JoinCodes { get; set; }
        public bool CanManageJoinCodes { get; set; }
        public int DeleteId { get; set; }
        public string JoinUrl { get; set; }

        [DisplayName("Branch (Optional)")]
        public int? BranchId { get; set; }

        [DisplayName("Code Type")]
        public bool IsQRCode { get; set; }

        public SelectList BranchList { get; set; }

    }
}
