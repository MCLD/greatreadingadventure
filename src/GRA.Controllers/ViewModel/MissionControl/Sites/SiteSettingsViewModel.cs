using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteSettingsViewModel
    {
        public int Id { get; set; }
        public List<SiteSettingGroup> SiteSettingGroups { get; set; }
    }
}
