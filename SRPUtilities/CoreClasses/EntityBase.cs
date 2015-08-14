using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace GRA.SRP.Core.Utilities
{
    [Serializable]
    public abstract class EntityBase //: DataEntity 
    {
        public virtual string Version { get { return "1.0"; } }

            public EntityBase()
        {
            ErrorCodes = new List<BusinessRulesValidationMessage>();
        }


        public List<BusinessRulesValidationMessage> ErrorCodes
        {
            get;
            set;
        }

        public virtual bool IsValid(BusinessRulesValidationMode validationMode)
        {
            return CheckBusinessRules(validationMode);
        }

        public void AddErrorCode(BusinessRulesValidationMessage errorCode)
        {

            ErrorCodes.Add(errorCode);
        }

        public void AddErrorCode(string fieldName, string fieldNameDisplay, string errorMessage, BusinessRulesValidationCode code)
        {

            ErrorCodes.Add(new BusinessRulesValidationMessage(fieldName, fieldNameDisplay, errorMessage, code));// BusinessRulesValidationMessage(fieldName, code));
        }


        public void ClearErrorCodes()
        {

            ErrorCodes.Clear();
        }

        protected virtual bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            return true;
        }

        public bool ValidateLength(string propertyName, string fieldNameDisplay, string value, int maxLength)
        {
            if (!String.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                String newMessage = String.Format("{0} has a maximum size of {1}", propertyName, maxLength);
                // Add the message to the validation list
                AddErrorCode(propertyName, fieldNameDisplay, newMessage, BusinessRulesValidationCode.FIELD_VALIDATION);
                return false;
            }
            return true;
        }

        public bool ValidateEmail(string propertyName, string fieldNameDisplay, string value)
        {
            if (!String.IsNullOrEmpty(value) && !IsValidEmailAddress(value))
            {
                String newMessage = String.Format("{0} is not a valid email.", propertyName);
                // Add the message to the validation list
                AddErrorCode(propertyName, fieldNameDisplay, newMessage, BusinessRulesValidationCode.FIELD_VALIDATION);
                return false;
            }
            return true;
        }


        public static string ProperCase(string stringInput) 
        {
            //Get the culture property of the thread.
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            //Create TextInfo object.
            TextInfo textInfo = cultureInfo.TextInfo;
            //Convert to title case.
            return textInfo.ToTitleCase(stringInput);
        }


        /// <summary>
        /// true, if is valid email address
        /// from http://www.davidhayden.com/blog/dave/
        /// archive/2006/11/30/ExtensionMethodsCSharp.aspx
        /// </summary>
        /// <param name="s">email address to test</param>
        /// <returns>true, if is valid email address</returns>
        public static bool IsValidEmailAddress(string s)
        {
            return new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$").IsMatch(s);
        }

        /// <summary>
        /// Checks if url is valid. 
        /// from http://www.osix.net/modules/article/?id=586
        /// and changed to match http://localhost
        /// 
        /// complete (not only http) url regex can be found 
        /// at http://internet.ls-la.net/folklore/url-regexpr.html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsValidUrl(string url)
        {
            string strRegex = "^(https?://)"
                    + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@
                    + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                    + "|" // allows either IP or domain
                    + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                    + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                    + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                    + "(:[0-9]{1,5})?" // port number- :80
                    + "((/?)|" // a slash isn't required if there is no file name
                    + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
            return new Regex(strRegex).IsMatch(url);
        }

        /// <summary>
        /// Check if url (http) is available.
        /// </summary>
        /// <param name="httpUri">url to check</param>
        /// <param name="httpUrl"></param>
        /// <example>
        /// string url = "www.codeproject.com;
        /// if( !url.UrlAvailable())
        ///     ...codeproject is not available
        /// </example>
        /// <returns>true if available</returns>
        public static bool UrlAvailable(string httpUrl)
        {
            if (!httpUrl.StartsWith("http://") || !httpUrl.StartsWith("https://"))
                httpUrl = "http://" + httpUrl;
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(httpUrl);
                myRequest.Method = "GET";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse myHttpWebResponse =
                   (HttpWebResponse)myRequest.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>

        /// Reverse the string
        /// from http://en.wikipedia.org/wiki/Extension_method
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Reverse(string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new String(chars);
        }

        /// <summary>

        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="s">string to reduce</param>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>

        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public static string Reduce(string s, int count, string endings)
        {
            if (count < endings.Length)
                throw new Exception("Failed to reduce to less then endings length.");
            int sLength = s.Length;
            int len = sLength;
            if (endings != null)
                len += endings.Length;
            if (count > sLength)
                return s; //it's too short to reduce
            s = s.Substring(0, sLength - len + count);
            if (endings != null)
                s += endings;
            return s;
        }

        /// <summary>
        /// true, if the string can be parse as Double respective Int32
        /// Spaces are not considred.
        /// </summary>
        /// <param name="s">input string</param>

        /// <param name="floatpoint">true, if Double is considered,
        /// otherwhise Int32 is considered.</param>
        /// <returns>true, if the string contains only digits or float-point</returns>
        public static bool IsNumber(string s, bool floatpoint)
        {
            int i;
            double d;
            string withoutWhiteSpace = s.Replace(" ", string.Empty);
            if (floatpoint)
                return double.TryParse(withoutWhiteSpace, NumberStyles.Any,
                    Thread.CurrentThread.CurrentUICulture, out d);
            else
                return int.TryParse(withoutWhiteSpace, out i);
        }

        /// <summary>
        /// Remove accent from strings 
        /// </summary>
        /// <example>
        ///  input:  "Příliš žluťoučký kůň úpěl ďábelské ódy."
        ///  result: "Prilis zlutoucky kun upel dabelske ody."
        /// </example>
        /// <param name="s"></param>
        /// <remarks>founded at http://stackoverflow.com/questions/249087/
        /// how-do-i-remove-diacritics-accents-from-a-string-in-net</remarks>
        /// <returns>string without accents</returns>

        public static string RemoveDiacritics(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }


        /// <summary>
        /// Replace \r\n or \n by <br />
        /// from http://weblogs.asp.net/gunnarpeipman/archive/2007/11/18/c-extension-methods.aspx
        /// </summary>

        /// <param name="s"></param>
        /// <returns></returns>
        public static string Nl2Br(string s)
        {
            return s.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        /// <summary>
        static MD5CryptoServiceProvider s_md5 = null;

        /// from http://weblogs.asp.net/gunnarpeipman/archive/2007/11/18/c-extension-methods.aspx
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5(string s)
        {
            if (s_md5 == null) //creating only when needed
                s_md5 = new MD5CryptoServiceProvider();
            Byte[] newdata = Encoding.Default.GetBytes(s);
            Byte[] encrypted = s_md5.ComputeHash(newdata);
            return BitConverter.ToString(encrypted).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// Gets whether the specified path is a valid absolute file path.
        /// </summary>
        /// <param name="path">Any path. OK if null or empty.</param>
        static public bool IsValidPath(string path)
        {
            Regex r = new Regex(@"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$");
            return r.IsMatch(path);
        }

    }
}