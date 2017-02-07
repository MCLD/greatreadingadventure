using System;
using System.Security.Cryptography;
using System.Text;

namespace GRA
{
    public class CodeGenerator : Abstract.ICodeGenerator, IDisposable
    {
        private const string DefaultAllowedCharacters = "ACEFHJKMNPRTUVWXY3479";

        private RandomNumberGenerator _rng;
        private string _allowedCharacters;

        public CodeGenerator()
        {
            _allowedCharacters = DefaultAllowedCharacters;
            _rng = RandomNumberGenerator.Create();
        }

        public void Dispose()
        {
            if (_rng != null)
            {
                _rng.Dispose();
            }
        }

        public string FormatCode(string code)
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

        public string Generate(int codeLength, bool formatCode = true)
        {
            var result = new StringBuilder(codeLength);
            var randomBytes = new byte[8];

            for (int i = 0; i < codeLength; i++)
            {
                _rng.GetBytes(randomBytes);
                var selected = (int)Math.Abs(BitConverter.ToInt64(randomBytes, 0)
                    % _allowedCharacters.Length);
                result.Append(_allowedCharacters.Substring(selected, 1));
            }
            return formatCode ? FormatCode(result.ToString()) : result.ToString();
        }

        public void SetAllowedCharacters(string allowedCharacters)
        {
            _allowedCharacters = allowedCharacters;
        }
    }
}
