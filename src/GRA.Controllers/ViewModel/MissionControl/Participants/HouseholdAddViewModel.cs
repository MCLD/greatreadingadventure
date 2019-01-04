using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
﻿using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HouseholdAddViewModel : SchoolSelectionViewModel
    {
        public Domain.Model.User User { get; set; }
        public int Id { get; set; }
        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public string ProgramJson { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList AskFirstTime { get; set; }

        [DisplayName("Is this your first time participating?")]
        [Required(ErrorMessage = "Please let us know if this is your first time participating in the program")]
        public string IsFirstTime { get; set; }

        public SelectList AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }
        [Required(ErrorMessage = "Please let us know if you would like to receive emails throughout the program")]
        public string EmailSubscriptionRequested { get; set; }

        public string TranslationDescriptionPastTense { get; set; }
        public string ActivityDescriptionPlural { get; set; }
    }
}
