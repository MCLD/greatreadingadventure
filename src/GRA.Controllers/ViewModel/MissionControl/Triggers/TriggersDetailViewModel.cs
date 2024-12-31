using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Triggers
{
    public class TriggersDetailViewModel
    {
        public static string NoCache
        {
            get
            {
                return DateTime.Now.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);
            }
        }

        public string Action { get; set; }
        public IFormFile AttachmentUploadFile { get; set; }
        public bool AwardsAttachment { get; set; }
        public bool AwardsMail { get; set; }
        public bool AwardsPrize { get; set; }

        [DisplayName("Badge Alternative Text")]
        [MaxLength(255)]
        public string BadgeAltText { get; set; }

        public string BadgeMakerImage { get; set; }
        public string BadgePath { get; set; }
        public string BadgeRequiredList { get; set; }

        [DisplayName("Upload a badge image. Badge images must be square.")]
        public IFormFile BadgeUploadImage { get; set; }
        public SelectList BranchList { get; set; }
        public bool CanViewParticipants { get; set; }
        public string ChallengeRequiredList { get; set; }
        public string CreatedByName { get; set; }
        public ICollection<Trigger> DependentTriggers { get; set; }
        public bool EditAttachment { get; set; }
        public bool EditAvatarBundle { get; set; }
        public bool EditMail { get; set; }
        public bool EditVendorCode { get; set; }
        public bool IgnorePointLimits { get; set; }
        public bool IsSecretCode { get; set; }
        public int? LowPointThreshold { get; set; }
        public int? MaxPointLimit { get; set; }
        public int? MinAllowedPoints { get; set; }
        public string MaxPointsMessage { get; set; }
        public string MaxPointsWarningMessage { get; set; }
        public SelectList ProgramList { get; set; }
        public ICollection<Event> RelatedEvents { get; set; }
        public bool RemoveAttachment { get; set; }
        public SelectList SystemList { get; set; }
        public Trigger Trigger { get; set; }

        [DisplayName("Challenges and triggers the participant must have earned")]
        public ICollection<TriggerRequirement> TriggerRequirements { get; set; }

        public string UnlockableAvatarBundle { get; set; }
        public SelectList UnlockableAvatarBundleList { get; set; }
        public bool UseBadgeMaker { get; set; }
        public string VendorCodeType { get; set; }
        public SelectList VendorCodeTypeList { get; set; }
    }
}
