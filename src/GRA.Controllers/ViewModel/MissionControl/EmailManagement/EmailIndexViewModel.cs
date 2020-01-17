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

        [DisplayName("Test Recipients")]
        public string SendTestRecipients { get; set; }
    }
}
