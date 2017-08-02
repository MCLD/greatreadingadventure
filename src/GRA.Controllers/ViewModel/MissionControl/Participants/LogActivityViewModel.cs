using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class LogActivityViewModel
    {
        public int Id { get; set; }
        public bool HasPendingQuestionnaire { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public bool HasAccount { get; set; }
        [DisplayName("Minutes Read")]
        public int? MinutesRead { get; set; }
        [DisplayName("Secret Code")]
        public string SecretCode { get; set; }
        public bool IsSecretCode { get; set; }
        public int? VendorCodeTypeId { get; set; }
        public SelectList VendorCodeTypeList { get; set; }
    }
}
