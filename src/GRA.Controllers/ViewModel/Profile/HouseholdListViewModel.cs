using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

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
        public int ActivityAmount{ get; set; }
        public string ActivityMessage { get; set; }
        public bool DisableSecretCode { get; set; }
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }
        public bool CanLogActivity { get; set; }
        public bool CanEditHousehold { get; set; }
        public bool ShowVendorCodes { get; set; }
        public PointTranslation PointTranslation { get; set; }

        public Dictionary<int, DailyImageViewModel> DailyImageDictionary { get; set; }
    }
}
