using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class ExcludedBranchListViewModel : PerformerManagementPartialViewModel
    {
        public ICollection<Branch> ExcludedBranches { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public SelectList UnexcludedBranches { get; set; }
        [Required]
        [DisplayName("Branch")]
        public int? BranchId { get; set; }
    }
}
