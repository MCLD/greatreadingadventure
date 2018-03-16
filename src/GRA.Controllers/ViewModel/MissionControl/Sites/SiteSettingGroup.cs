using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteSettingGroup
    {
        public string Name { get; set; }
        public List<SiteSettingInformation> SettingInformations { get; set; }
    }
}
