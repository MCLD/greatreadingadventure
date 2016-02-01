using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GRA.SRP.Utilities.CoreClasses {
    public static class FormatHelper {
        public static string ToNormalDate(this DateTime dt) {
            if(dt != null) {
                if(dt.ToString().StartsWith("1/1/0001")) {
                    return "N/A";
                }
                return dt.ToShortDateString();
            }
            return "N/A";
        }

        public static string ToWidgetDisplayDate(this DateTime dt) {
            if(dt != null) {
                if(dt.ToString().StartsWith("1/1/0001")) {
                    return string.Empty;
                }
                return dt.ToShortDateString();
            }
            return string.Empty;
        }

        public static string ToYesNo(this bool b) {
            return b ? "Yes" : "No";
        }

        public static string ToInt(this int i) {
            return String.Format("{0:#,##0}", i);
        }

        public static string ToWidgetDisplayInt(this int i) {
            if(i == 0) {
                return "";
            }
            return String.Format("{0:#,##0}", i);
        }

        public static string ToDecimal(this decimal i) {
            return String.Format("{0:#,##0.00}", i);
        }
        public static string ToMoney(this decimal d) {
            return String.Format("{0:C}", d);
        }

        public static int SafeToInt(this string s) {
            int _int = 0;
            int.TryParse(s, out _int);
            return _int;
        }


        public static bool SafeToBool(this string s) {
            if(s == "1") {
                return true;
            }
            if(s.Equals("yes", StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            if(s.Equals("on", StringComparison.OrdinalIgnoreCase)) {
                return true;
            }

            return false;
        }

        public static bool SafeToBoolYes(this string s) {
            if(s == "1") {
                return true;
            }
            if(s.Equals("yes", StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            if(s.Equals("on", StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            if(string.IsNullOrEmpty(s)) {
                return true;
            }

            return false;
        }

        public static decimal SafeToDecimal(this string s) {
            decimal _dec = (decimal)0.00;
            decimal.TryParse(s, out _dec);
            return _dec;
        }

        public static DateTime SafeToDateTime(this string s) {
            DateTime _date = DateTime.MinValue;
            DateTime.TryParse(s, out _date);
            return _date;
        }

        public static string FormatPhoneNumber(this string s) {
            var phoneFixed = string.Empty;
            if(s.Length == 0) {
                return phoneFixed;
            }
            phoneFixed = s.Replace(" ", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty);
            return string.Format("({0}) {1}-{2}", phoneFixed.Substring(0, 3), phoneFixed.Substring(3, 3), phoneFixed.Substring(6));
        }

        public static string FormatZipCode(string s) {
            var zipFixed = string.Empty;
            if(s.Length == 0) {
                return zipFixed;
            }
            zipFixed = s.Replace(" ", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty);
            if(zipFixed.Length == 5) {
                return zipFixed;
            }
            return string.Format("{0}-{1}", zipFixed.Substring(0, 5), zipFixed.Substring(5));
        }

        public static string HtmlStrip(this string input) {
            input = Regex.Replace(input, "<style>(.|\n)*?</style>", string.Empty);
            input = Regex.Replace(input, @"<xml>(.|\n)*?</xml>", string.Empty); // remove all <xml></xml> tags and anything inbetween.  
            return Regex.Replace(input, @"<(.|\n)*?>", string.Empty); // remove any tags but not there content "<p>test<span> drive</span></p>" becomes "test drive"
        }
    }
}