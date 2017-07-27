using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HouseholdListViewModel
    {
        public IEnumerable<GRA.Domain.Model.User> Users { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        public GRA.Domain.Model.User Head { get; set; }
        public bool AuthUserIsHead { get; set; }
        public int ActiveUser { get; set; }
        public string UserSelection { get; set; }
        public int MinutesRead { get; set; }
        public string MinutesReadMessage { get; set; }
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }
        public bool CanLogActivity { get; set; }
        public bool CanEditHousehold { get; set; }

        public Dictionary<int, DailyImageViewModel> DailyImageDictionary { get; set; }
    }
}
