using System;

namespace GRA.Utility
{
    public static class ColorUtility
    {
        /// <summary>
        /// Provided two sRGB color space RGB hexadecimal strings, return the contrast ratio as
        /// computed by the WCAG 2.1 specification:
        /// https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html#dfn-contrast-ratio
        /// </summary>
        /// <param name="rgbHexadecimal1">First sRGB color space RGB hexadecimal string</param>
        /// <param name="rgbHexadecimal2">Second sRGB color space RGB hexadecimal string</param>
        /// <returns>The copntrast ratio per the WCAG 2.1 specification</returns>
        public static double? GetContrastRatio(string rgbHexadecimal1, string rgbHexadecimal2)
        {
            ArgumentNullException.ThrowIfNull(rgbHexadecimal1);
            ArgumentNullException.ThrowIfNull(rgbHexadecimal2);

            double la;
            double lb;

            try
            {
                la = GetRelativeLuminance(TryParseRgb(rgbHexadecimal1));
                lb = GetRelativeLuminance(TryParseRgb(rgbHexadecimal2));
            }
            catch (ArgumentException aex)
            {
                throw new GraException($"Unable to compute contrast ratio: {aex.Message}", aex);
            }

            return (Math.Max(la, lb) + ColorConstants.ContrastRatioAddTerm)
                / (Math.Min(la, lb) + ColorConstants.ContrastRatioAddTerm);
        }

        /// <summary>
        /// Convert R, G, or B element from sRGB color space to linear rgb color space based on:
        /// https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html#dfn-relative-luminance
        /// </summary>
        /// <param name="rgbElement">Single R, G, or B element, 0 to 255</param>
        /// <returns>sRGB value translated to linear RGB color space</returns>
        private static double ConvertToLinearColorspace(int rgbElement)
        {
            var c = rgbElement / ColorConstants.MaxRgbValue;
            return c <= ColorConstants.LuminanceCalculationCutoff
                ? c / ColorConstants.LuminanceLowDivisor
                : Math.Pow((c + ColorConstants.LuminanceHighAddTerm)
                           / ColorConstants.LuminanceHighDivisor,
                           ColorConstants.LuminanceHighPower);
        }

        /// <summary>
        /// Calculate relative luminance value from RGB values expressed in a linear RGB color, see:
        /// https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html#dfn-relative-luminance
        /// space.
        /// </summary>
        /// <param name="rgb">Struct containing R, G, and B values (linear RGB color space)</param>
        /// <returns>A relative luminance value</returns>
        private static double GetRelativeLuminance((int r, int g, int b) rgb)
        {
            return (ColorConstants.sRGBRedLuminanceMultiplier * ConvertToLinearColorspace(rgb.r))
                + (ColorConstants.sRGBGreenLuminanceMultiplier * ConvertToLinearColorspace(rgb.g))
                + (ColorConstants.sRGBBlueLuminanceMultiplier * ConvertToLinearColorspace(rgb.b));
        }

        /// <summary>
        /// Extract integer values for R, G, and B out of an RGB hexadecimal string.
        /// </summary>
        /// <param name="rgbHexadecimal">RGB hexadecimal color representation, optional leading #
        /// </param>
        /// <returns>Integer R, G, and B values.</returns>
        /// <exception cref="ArgumentException">This is thrown if the color could not be decoded
        /// from the string provided.</exception>
        private static (int r, int g, int b) TryParseRgb(string rgbHexadecimal)
        {
            ArgumentNullException.ThrowIfNull(rgbHexadecimal);

            var rgb = rgbHexadecimal.Trim().TrimStart('#');
            if (rgb.Length != 6)
            {
                throw new ArgumentException($"Invalid hex color value provided, must be six digits: {rgbHexadecimal}");
            }

            try
            {
                int intRgb = Convert.ToInt32(rgb, 16);
                return ((intRgb >> 16) & 0xFF, (intRgb >> 8) & 0xFF, intRgb & 0xFF);
            }
            catch (Exception ex) when (ex is ArgumentException
                                       || ex is FormatException
                                       || ex is OverflowException)
            {
                // for "TryParseRgb" any invalid response is going to mean the arguments were bad
                throw new ArgumentException($"Invalid hex color value provided: {rgbHexadecimal}",
                    ex);
            }
        }
    }
}
