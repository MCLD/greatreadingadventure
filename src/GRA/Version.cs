using System.Reflection;

namespace GRA
{
    public static class Version
    {
        public static string GetVersion()
        {
            var fileVersion = GetShortVersion();

            return !string.IsNullOrEmpty(fileVersion)
                ? fileVersion
                : Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
        }

        public static string GetShortVersion()
        {
            return Assembly
                 .GetEntryAssembly()
                 .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                 .Version;
        }
    }
}
