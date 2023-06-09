using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HouseholdListViewModel
    {
        public HouseholdListViewModel()
        {
            DailyImageDictionary = new Dictionary<int, DailyImageViewModel>();
        }

        public int ActiveUser { get; set; }
        public int ActivityAmount { get; set; }
        public string ActivityMessage { get; set; }
        public bool AuthUserIsHead { get; set; }
        public bool CanEditHousehold { get; set; }
        public bool CanLogActivity { get; set; }
        public IDictionary<int, DailyImageViewModel> DailyImageDictionary { get; }
        public bool DisableSecretCode { get; set; }
        public EmailAwardViewModel EmailAwardModel { get; set; }
        public bool GroupLeader { get; set; }
        public string GroupName { get; set; }
        public bool HasAccount { get; set; }
        public GRA.Domain.Model.User Head { get; set; }
        public int HouseholdCount { get; set; }
        public string LocalizedHouseholdTitle { get; set; }
        public PointTranslation PointTranslation { get; set; }
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }
        public bool ShowVendorCodes { get; set; }
        public IEnumerable<User> Users { get; set; }
        public string UserSelection { get; set; }
    }
}
