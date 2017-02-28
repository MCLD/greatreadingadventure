using System;
using System.Text.RegularExpressions;

namespace GRA
{
    public class CodeSanitizer : Abstract.ICodeSanitizer
    {
        public string Sanitize(string code, int length = 50)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            string resultString = code
                .Trim()
                .ToLower();

            if (resultString.Length > length)
            {
                resultString = resultString.Substring(0, length);
            }
            return new Regex("[^a-z0-9]").Replace(resultString, string.Empty);
        }
    }
}
