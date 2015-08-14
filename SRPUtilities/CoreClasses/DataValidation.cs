using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GRA.SRP.Core.Utilities
{
    public class DataValidation
    {
        public const string VALID_EMAIL_EXPRESSION = @"^[A-Za-z0-9_\-\.]+@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+$";

        public const string VALID_GUID_EXPRESSION =
            @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";

        public const string VALID_ZIPCODE_EXPRESSION = @"^\d{5}(-\d{4})?$";
        private static Regex _isEmailRegEx;

        private static Regex _isGuidRegEx;

        private static Regex _isZipCodeRegEx;

        public static Regex IsGuidRegEx
        {
            get
            {
                if (_isGuidRegEx == null)
                    _isGuidRegEx = new Regex(VALID_GUID_EXPRESSION, RegexOptions.Compiled);
                return _isGuidRegEx;
            }
        }

        public static Regex IsZipCodeRegEx
        {
            get
            {
                if (_isZipCodeRegEx == null)
                    _isZipCodeRegEx = new Regex(VALID_ZIPCODE_EXPRESSION, RegexOptions.Compiled);
                return _isZipCodeRegEx;
            }
        }

        public static Regex IsEmailRegEx
        {
            get
            {
                if (_isEmailRegEx == null)
                    _isEmailRegEx = new Regex(VALID_EMAIL_EXPRESSION, RegexOptions.Compiled);
                return _isEmailRegEx;
            }
        }

        public static bool IsValidDateTimeOrBlank(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            value = value.Trim();
            if (value.Length == 0)
                return true;
            return IsValidDateTime(value);
        }

        public static bool IsValidDateTime(string value)
        {
            DateTime testdate;
            return DateTime.TryParse(value, out testdate);
        }

        public static bool IsValidGuid(string value)
        {
            return value != null
                       ?
                           IsGuidRegEx.IsMatch(value)
                       :
                           false;
        }

        public static bool IsValidGuid(string candidate, out Guid output)
        {
            bool isValid = false;
            output = Guid.Empty;
            if (candidate != null)
            {
                if (IsGuidRegEx.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }
            return isValid;
        }

        public static bool IsValidUSZipcode(string value)
        {
            return IsZipCodeRegEx.IsMatch(value);
        }

        public static bool IsValidEmailAddress(string value)
        {
            return IsEmailRegEx.IsMatch(value);
        }


        public static bool IsValidUnsignedInt(string value, int length)
        {
            if (
                value.StartsWith("-") ||
                value.StartsWith("+") ||
                value.Length != length)
                return false;
            int i;
            return int.TryParse(value, out i);
        }


        /// <summary>
        /// Determines whether the string value represents a valid it of the specified length
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// 	<c>true</c> if the string passed int/lenth test; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidInt(string value, int length)
        {
            if (value.Length != length)
                return false;
            int i;
            return int.TryParse(value, out i);
        }

        public static bool IsValidInt(string value)
        {
            int i;
            return int.TryParse(value, out i);
        }

        public static bool FixUsPhone(ref string phone)
        {
            string[] parts = phone.Split(new[] { '(', '-', '/', ')', ' ' },
                                         StringSplitOptions.RemoveEmptyEntries);
            var phoneBuffer = new StringBuilder();
            int partCount = 0;
            foreach (string part in parts)
            {
                if (part != "1")
                {
                    ++partCount;
                    if (phoneBuffer.ToString().Length > 0)
                        phoneBuffer.Append('-');
                    phoneBuffer.Append(part);
                }
            }

            if (
                // First part must be 3 digit numaric
                phoneBuffer.ToString().IndexOf('-') == 3 &&
                IsValidUnsignedInt(phoneBuffer.ToString().Substring(0, 3), 3) &&
                // no more than three sections
                partCount < 4 &&
                // only the two sizes
                (phoneBuffer.ToString().Length == 8 ||
                 phoneBuffer.ToString().Length == 12)
                )
            {
                phone = phoneBuffer.ToString();
                return true;
            }
            return false;
        }
    }
}