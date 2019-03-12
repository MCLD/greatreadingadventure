using System.Globalization;

namespace GRA
{
    public static class Culture
    {
        //public const string 
        public static readonly string EnglishUS = "en-US";
        public static readonly string EspanolUS = "es-US";

        public static readonly string DefaultName = EnglishUS;
        public static readonly CultureInfo DefaultCulture = new CultureInfo(DefaultName);

        public static readonly CultureInfo[] SupportedCultures = new[]
        {
            new CultureInfo(EnglishUS)
        };
    }
}
