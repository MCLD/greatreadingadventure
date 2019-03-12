using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GRA
{
    public class Version
    {
        public string GetVersion()
        {
            var thisAssemblyVersion = Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            var fileVersion = Assembly
                .GetEntryAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                .Version;
            if (!string.IsNullOrEmpty(fileVersion)
                && fileVersion.Count(_ => _ == '.') > 2
                && fileVersion.Length > fileVersion.LastIndexOf('.'))
            {
                var revision = fileVersion.Substring(fileVersion.LastIndexOf('.') + 1);
                if (!string.IsNullOrEmpty(revision) && revision != "0")
                {
                    thisAssemblyVersion += " revision " + revision;
                }
            }
            return thisAssemblyVersion;
        }
    }
}
