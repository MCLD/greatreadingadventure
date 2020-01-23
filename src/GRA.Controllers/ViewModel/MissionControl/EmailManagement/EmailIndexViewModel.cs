using System.Collections.Generic;
using System.ComponentModel;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

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
    }
}
