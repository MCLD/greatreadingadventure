using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GRA.Logic
{
    public class Code
    {
        public static string SanitizeCode(string userEntry)
        {
            if (!string.IsNullOrEmpty(userEntry))
            {
                string resultString = userEntry
                    .Trim()
                    .ToLower();
                if (resultString.Length > 50)
                {
                    resultString = resultString.Substring(0, 50);

                }
                return new Regex("[^a-z0-9]").Replace(resultString, string.Empty);
            }
            else
            {
                return null;
            }
        }
    }
}
