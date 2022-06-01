using System.Collections.Generic;
using System.ComponentModel;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailIndexViewModel : PaginateViewModel
    {
        public ICollection<EmailTemplateListItem> EmailTemplates { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAnyoneSubscribed { get; set; }
        public IDictionary<int, string> LanguageNames { get; set; }
    }
}
