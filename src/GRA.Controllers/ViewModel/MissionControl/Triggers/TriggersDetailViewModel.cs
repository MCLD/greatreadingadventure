using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace GRA.Controllers.ViewModel.MissionControl.Triggers
{
    public class TriggersDetailViewModel
    {
        public GRA.Domain.Model.Trigger Trigger { get; set; }
        public string Action { get; set; }
        public bool IsSecretCode { get; set; }

        public ICollection<Trigger> DependentTriggers { get; set; }

        [DisplayName("Challenges/Triggers")]
        public ICollection<TriggerRequirement> TriggerRequirements { get; set; }
        public string BadgeRequiredList { get; set; }
        public string ChallengeRequiredList { get; set; }

        public string BadgePath { get; set; }
        public IFormFile BadgeImage { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList VendorCodeTypeList { get; set; }
    }
}
