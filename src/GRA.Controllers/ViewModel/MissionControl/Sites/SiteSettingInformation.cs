using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteSettingInformation
    {
        public SiteSetting SiteSetting { get; set; }
        public SiteSettingDefinition Definition { get; set; }
        public string Key { get; set; }
    }
}
