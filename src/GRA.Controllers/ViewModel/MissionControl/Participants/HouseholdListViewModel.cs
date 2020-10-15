﻿using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HouseholdListViewModel : ParticipantPartialViewModel
    {
        public IEnumerable<GRA.Domain.Model.User> Users { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool CanRedeemBulkVendorCodes { get; set; }
        public bool CanEditDetails { get; set; }
        public bool CanImportNewMembers { get; set; }
        public bool CanLogActivity { get; set; }
        public bool CanReadMail { get; set; }
        public bool CanViewPrizes { get; set; }
        public GRA.Domain.Model.User Head { get; set; }
        public string UserSelection { get; set; }
        public int ActivityAmount { get; set; }
        public string ActivityMessage { get; set; }
        public bool DisableSecretCode { get; set; }
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }
        public bool ShowVendorCodes { get; set; }
        public PointTranslation PointTranslation { get; set; }

        public List<SelectListItem> HouseholdPrizeList { get; set; }
        public string Prize { get; set; }

        public int SystemId { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
        public bool UpgradeToGroup { get; set; }
        public bool UseGroups { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }

        public EmailAwardViewModel EmailAwardModel { get; set; }
    }
}
