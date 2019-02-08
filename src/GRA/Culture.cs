using System.Globalization;

namespace GRA
{
    public static class Culture
    {
        //public const string 
        public const string EnglishUS = "en-US";
        public const string EspanolUS = "es-US";

        public const string DefaultName = EnglishUS;
        public static readonly CultureInfo DefaultCulture = new CultureInfo(DefaultName);

        public static readonly CultureInfo[] SupportedCultures = new[]
        {
            new CultureInfo(EnglishUS),
            new CultureInfo(EspanolUS)
        };
    }
}
