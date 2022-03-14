using System.Collections.Generic;
using System.ComponentModel;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailIndexViewModel : PaginateViewModel
    {
        public SelectList AddressTypes { get; set; }
        public string EmailList { get; set; }
        public ICollection<EmailTemplateListItem> EmailTemplates { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAnyoneSubscribed { get; set; }
        public Dictionary<int, string> LanguageNames { get; set; }
        public int SendEmailTemplateId { get; set; }
        public int SendTestTemplateId { get; set; }

        [DisplayName("Send to subscribed participants as well?")]
        public bool SendToParticipantsToo { get; set; }

        [DisplayName("Please enter YES in the field below to confirm")]
        public string SendValidation { get; set; }

        public int SubscribedParticipants { get; set; }
    }
}
