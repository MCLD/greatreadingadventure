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
    }
}
