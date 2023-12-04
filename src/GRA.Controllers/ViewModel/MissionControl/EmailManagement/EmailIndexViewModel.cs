using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model.Utility;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailIndexViewModel : PaginateViewModel
    {
        public ICollection<EmailTemplateListItem> EmailTemplates { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAnyoneSubscribed { get; set; }
        public IDictionary<int, string> LanguageNames { get; set; }
        public int WelcomeEmailTemplateId { get; set; }
    }
}
