using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Systems
{
    public class RemoveBranchViewModel
    {
        private Branch _branch;

        public Branch Branch
        {
            get
            {
                return _branch;
            }
            set
            {
                _branch = value;
                if (_branch != null)
                {
                    BranchId = _branch.Id;
                }
            }
        }

        public int BranchId { get; set; }
        public SelectList BranchList { get; set; }
        public int InUseCount { get; set; }
        public int? ReassignBranch { get; set; }
    }
}
