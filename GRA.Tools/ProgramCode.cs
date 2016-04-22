namespace GRA.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class ProgramCode : IDisposable
    {
        /// <summary>
        /// Default list of allowed characters to use for generating codes
        /// </summary>
        private const string AllowedCharacters = "ACEFHJKMNPRTUVWXY3479";

        private RNGCryptoServiceProvider rngProvider = null;
        private int characterCodeLength;

        /// <summary>
        /// The list of allowed characters to use for generating codes.
        /// </summary>
        public string CharacterSet { get; set; }
        /// <summary>
        /// Initialize the CodeGenerator
        /// </summary>
        /// <param name="codeLength">The length, in characters, of the code to generate.</param>
        public ProgramCode(int codeLength)
        {
            rngProvider = new RNGCryptoServiceProvider();
            characterCodeLength = codeLength;
            CharacterSet = AllowedCharacters;
        }

        /// <summary>
        /// Implement Dispose() as part of <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            if (rngProvider != null)
            {
                rngProvider.Dispose();
            }
        }

        /// <summary>
        /// Generate a random code
        /// </summary>
        /// <returns>A string containing the random code</returns>
        public string Generate()
        {
            StringBuilder result = new StringBuilder(characterCodeLength);
            var randomBytes = new byte[8];

            for (int i = 0; i < characterCodeLength; i++)
            {
                rngProvider.GetBytes(randomBytes);
                var selected = (int)Math.Abs(BitConverter.ToInt64(randomBytes, 0)
                    % AllowedCharacters.Length);
                result.Append(AllowedCharacters.Substring(selected, 1));
            }
            return FormatCode(result.ToString());
        }

        /// <summary>
        /// Format a string by placing hyphens in at appropriate spaces
        /// </summary>
        /// <param name="code">The code to format</param>
        /// <returns>A hyphenated code</returns>
        private string FormatCode(string code)
        {
            if (code.Length > 7)
            {
                if (code.Length % 2 == 0)
                {
                    return string.Format("{0}-{1}",
                    code.Substring(0, code.Length / 2),
                    code.Substring(code.Length / 2));
                }
                else if (code.Length % 3 == 0)
                {
                    return string.Format("{0}-{1}-{2}",
                        code.Substring(0, code.Length / 3),
                        code.Substring(code.Length / 3, code.Length / 3),
                        code.Substring((code.Length / 3) * 2));
                }
                else
                {
                    return code;
                }
            }
            else
            {
                return code;
            }
        }
    }

}
