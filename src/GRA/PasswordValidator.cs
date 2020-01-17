using System;
using System.Linq;
using System.Text;
using GRA.Abstract;
using Microsoft.Extensions.Localization;

namespace GRA
{
    public class PasswordValidator : IPasswordValidator
    {
        private readonly int _minimumLength;
        private readonly bool _requireDigit;
        private readonly bool _requireSymbol;
        private readonly bool _requireLower;
        private readonly bool _requireUpper;

        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;

        public PasswordValidator(IStringLocalizer<Resources.Shared> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));

            // TODO move password validation configuration items to software configuration
            _minimumLength = 6;
            _requireDigit = true;
            _requireSymbol = false;
            _requireLower = false;
            _requireUpper = false;
        }

        public void Validate(string password)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(password) || password.Length < _minimumLength)
            {
                errors.AppendLine(
                    _sharedLocalizer[Annotations.Validate.PasswordLength, _minimumLength]);
            }
            if (!string.IsNullOrEmpty(password))
            {
                if (_requireDigit && !password.Any(IsDigit))
                {
                    errors
                        .AppendLine(_sharedLocalizer[Annotations.Validate.PasswordRequiresDigit]);
                }
                if (_requireLower && !password.Any(IsLower))
                {
                    errors.AppendLine(_sharedLocalizer[Annotations
                        .Validate
                        .PasswordRequiresLowercase]);
                }
                if (_requireUpper && !password.Any(IsUpper))
                {
                    errors.AppendLine(_sharedLocalizer[Annotations
                        .Validate
                        .PasswordRequiresUppercase]);
                }
                if (_requireSymbol && password.All(IsLetterOrDigit))
                {
                    errors
                        .AppendLine(_sharedLocalizer[Annotations.Validate.PasswordRequiresSymbol]);
                }
            }
            if (errors.Length > 0)
            {
                throw new GraPasswordValidationException(errors.ToString());
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
