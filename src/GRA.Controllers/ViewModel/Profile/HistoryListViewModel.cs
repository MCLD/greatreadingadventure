using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HistoryListViewModel
    {
        public IEnumerable<GRA.Domain.Model.UserLog> Historys { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
    }
}
