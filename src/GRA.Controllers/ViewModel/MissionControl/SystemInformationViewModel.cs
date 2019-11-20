using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl
{
    public class SystemInformationViewModel
    {
        public string Assembly { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Assemblies { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public Dictionary<string, string> RuntimeSettings { get; set; }
    }
}
