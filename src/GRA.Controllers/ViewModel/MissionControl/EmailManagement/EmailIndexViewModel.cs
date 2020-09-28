using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailIndexViewModel
    {
        public ICollection<EmailTemplate> EmailTemplates { get; set; }
        public PaginateViewModel PaginateModel { get; set; }

        [DisplayName("Email addresses for test email, comma separated")]
        public string SendTestRecipients { get; set; }

        public int SubscribedParticipants { get; set; }

        [DisplayName("Please enter YES in the field below to confirm")]
        public string SendValidation { get; set; }

        public int SendEmailTemplateId { get; set; }

        public int SendTestTemplateId { get; set; }

        public string DefaultTestEmail { get; set; }

        public bool IsAdmin { get; set; }

        public SelectList AddressTypes { get; set; }

        public string EmailList { get; set; }
        public string SendButtonDisabled
        {
            get
            {
                return AddressTypes?.Any() == true
                    ? null
                    : "disabled";
            }
        }

        [DisplayName("Send to subscribed participants as well?")]
        public bool SendToParticipantsToo { get; set; }
    }
}
