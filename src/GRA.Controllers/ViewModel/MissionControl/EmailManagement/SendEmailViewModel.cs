using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class SendEmailViewModel
    {
        public bool IsForSubscribers { get; set; }
        public bool IsMixedFooter { get; set; }
        public IDictionary<string, int> RegisteredLanguages { get; set; }
        public string SelectedList { get; set; }
        public SelectList SubscriptionLists { get; set; }
        public DirectEmailTemplate Template { get; set; }
        public IEnumerable<string> TemplateLanguages { get; set; }
    }
}
