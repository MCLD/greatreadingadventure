using GRA.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRA
{
    public class PasswordValidator : IPasswordValidator
    {
        public void Validate(string password)
        {
            // TODO move these items to configuration
            int minimumLength = 6;
            bool requireDigit = true;
            bool requireSymbol = true;
            bool requireLower = false;
            bool requireUpper = false;

            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(password) || password.Length <= minimumLength)
            {
                errors.AppendLine($"The password you've provided is too short, it must be at least {minimumLength} characters long.");
            }
            if (requireDigit && !password.Any(IsDigit))
            {
                errors.AppendLine("You must include a number in your password.");
            }
            if (requireLower && !password.Any(IsLower))
            {
                errors.AppendLine("You must include a lower-case letter in your password.");
            }
            if (requireUpper && !password.Any(IsUpper))
            {
                errors.AppendLine("You must include an upper-case letter in your password.");
            }
            if (requireSymbol && password.All(IsLetterOrDigit))
            {
                errors.AppendLine("You must include a special character in your password.");
            }
            if (errors.Length > 0)
            {
                throw new GraException(errors.ToString());
            }
        }

        public bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        public bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }
        public bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }
        public bool IsLetterOrDigit(char c)
        {
            return IsUpper(c) || IsLower(c) || IsDigit(c);
        }
    }
}
