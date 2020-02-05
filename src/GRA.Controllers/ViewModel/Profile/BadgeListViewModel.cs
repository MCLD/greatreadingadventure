using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.Profile
{
    public class BadgeListViewModel
    {
        public IEnumerable<Domain.Model.Badge> Badges { get; set; }
        public List<Domain.Model.UserLog> UserLogs { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
    }
}
