using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;

namespace GRA.Extensions
{
    public static class IStringLocalizerExtensions
    {
        public static LocalizedString GetString(this IStringLocalizer stringLocalizer,
            string cultureName,
            string name) => GetString(stringLocalizer, cultureName, name, []);

        public static LocalizedString GetString(this IStringLocalizer stringLocalizer,
            string cultureName,
            string name,
            params object[] arguments)
        {
            var cultureInfo = string.IsNullOrEmpty(cultureName)
                ? CultureInfo.CurrentUICulture
                : CultureInfo.GetCultureInfo(cultureName);

            var cultureInfoOriginal = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentUICulture = cultureInfo;
                CultureInfo.CurrentCulture = cultureInfo;
                return stringLocalizer.GetString(name, arguments);
            }
            finally
            {
                CultureInfo.CurrentUICulture = cultureInfoOriginal;
                CultureInfo.CurrentCulture = cultureInfoOriginal;
            }
        }
    }
}
