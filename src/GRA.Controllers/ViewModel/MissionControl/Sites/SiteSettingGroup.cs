using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteSettingGroup
    {
        public string Name { get; set; }
        public List<SiteSettingInformation> SettingInformations { get; set; }
    }
}
