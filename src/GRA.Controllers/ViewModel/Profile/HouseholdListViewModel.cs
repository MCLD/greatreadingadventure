using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HouseholdListViewModel
    {
        public IEnumerable<GRA.Domain.Model.User> Users { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        public GRA.Domain.Model.User Head { get; set; }
        public bool IsHead { get; set; }
    }
}
