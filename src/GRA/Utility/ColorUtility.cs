using System;

namespace GRA.Utility
{
    public static class ColorUtility
    {
        /* See https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html#dfn-relative-luminance
         and https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html#dfn-contrast-ratio
        for information on specific numbers used and reasoning*/
        public static double? GetContrastRatio(string hexA, string hexB)
        {
            var a = TryParseRgb(hexA);
            var b = TryParseRgb(hexB);
            if (a is null || b is null)
            {
                return null;
            }

            var la = GetRelativeLuminance(a.Value);
            var lb = GetRelativeLuminance(b.Value);

            var lighter = Math.Max(la, lb);
            var darker = Math.Min(la, lb);

            return (lighter + 0.05) / (darker + 0.05);
        }

        private static (int r, int g, int b)? TryParseRgb(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return null;
            }

            hex = hex.Trim().TrimStart('#');
            if (hex.Length != 6)
            {
                return null;
            }

            try
            {
                return (
                    Convert.ToInt32(hex.Substring(0, 2), 16),
                    Convert.ToInt32(hex.Substring(2, 2), 16),
                    Convert.ToInt32(hex.Substring(4, 2), 16)
                );
            }
            catch
            {
                return null;
            }
        }

        private static double GetRelativeLuminance((int r, int g, int b) rgb)
        {
            var r = Srgb8ToLinear(rgb.r);
            var g = Srgb8ToLinear(rgb.g);
            var b = Srgb8ToLinear(rgb.b);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static double Srgb8ToLinear(int c8)
        {
            var c = c8 / 255.0;
            return c <= 0.04045
                ? c / 12.92
                : Math.Pow((c + 0.055) / 1.055, 2.4);
        }
    }
}
