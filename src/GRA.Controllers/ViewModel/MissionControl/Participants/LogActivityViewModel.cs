using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class LogActivityViewModel : ParticipantPartialViewModel
    {
        public bool HasPendingQuestionnaire { get; set; }
        [DisplayName("Activity Amount")]
        public int? ActivityAmount { get; set; }
        public bool DisableSecretCode { get; set; }
        [DisplayName("Secret Code")]
        public string SecretCode { get; set; }
        public bool IsSecretCode { get; set; }
        public int? VendorCodeTypeId { get; set; }
        public SelectList VendorCodeTypeList { get; set; }
        public PointTranslation PointTranslation { get; set; }
    }
}
