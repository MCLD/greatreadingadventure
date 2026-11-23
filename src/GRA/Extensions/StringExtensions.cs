using System.Collections.Generic;
using System.Text;

namespace GRA.Extensions
{
    public static class StringExtensions
    {
        public static string AddTitleCaseSpaces(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder();

            foreach (var c in value)
            {
                if (char.IsUpper(c))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            return sb.ToString().Trim();
        }

        public static IEnumerable<int> ToIntList(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                yield break;
            }

            foreach (var stringValue in value.Split(','))
            {
                if (int.TryParse(stringValue, out int intValue))
                {
                    yield return intValue;
                }
            }
        }
    }
}
