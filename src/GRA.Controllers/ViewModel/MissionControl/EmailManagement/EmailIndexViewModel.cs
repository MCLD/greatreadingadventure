using System.Collections.Generic;
using DocumentFormat.OpenXml.EMMA;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model.Utility;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailIndexViewModel : PaginateViewModel
    {
        public EmailIndexViewModel()
        {
            EmailTemplates = new List<EmailTemplateListItem>();
            LanguageNames = new Dictionary<int, string>();
        }

        public ICollection<EmailTemplateListItem> EmailTemplates { get; }
        public bool IsAdmin { get; set; }
        public bool IsAnyoneSubscribed { get; set; }
        public IDictionary<int, string> LanguageNames { get; }
        public int WelcomeEmailsTotal { get; set; }
        public int WelcomeEmailTemplateId { get; set; }

        public static string DisplayPercent(int first, int second)
        {
            return second > 0
                ? $" ({first * 100 / second}%)"
                : null;
        }
    }
}
