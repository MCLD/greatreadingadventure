using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl
{
    public class SystemInformationViewModel
    {
        public SystemInformationViewModel()
        {
            Assemblies = new Dictionary<string, string>();
            RuntimeSettings = new Dictionary<string, string>();
            Settings = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Assemblies { get; }
        public string Assembly { get; set; }
        public IDictionary<string, string> RuntimeSettings { get; }
        public IDictionary<string, string> Settings { get; }
        public string Version { get; set; }

        public static string FormatVersion(string version)
        {
            if (version?.IndexOf('+', System.StringComparison.Ordinal) > 0)
            {
                return version.Replace("+", "<small>+", System.StringComparison.Ordinal)
                    + "</small>";
            }
            return version;
        }
    }
}
