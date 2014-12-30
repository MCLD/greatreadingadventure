using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.SRP.Utilities.CoreClasses
{
    public static class FormatHelper
    {

        public static string ToNormalDate(this DateTime dt)
        {
            if (dt != null) {
                if (dt.ToString().StartsWith("1/1/0001")) return "N/A";
                return dt.ToString("MM/dd/yyyy");
            }
            return "N/A";
        }

        public static string ToWidgetDisplayDate(this DateTime dt)
        {
            if (dt != null)
            {
                if (dt.ToString().StartsWith("1/1/0001")) return "";
                return dt.ToString("MM/dd/yyyy");
            }
            return "";
        }

        public static string ToYesNo(this bool b)
        {
            if (b)
                return "Yes";
            return "No";
        }

        public static string ToInt(this int i)
        {
            return String.Format("{0:#,##0}", i);
        }

        public static string ToWidgetDisplayInt(this int i)
        {
            if (i == 0) return "";
            return String.Format("{0:#,##0}", i);
        }

        public static string ToDecimal(this decimal i)
        {
            return String.Format("{0:#,##0.00}", i);
        }
        public static string ToMoney(this decimal d)
        {
            return String.Format("{0:C}", d);
        }

        public static int SafeToInt(this string s)
        {
            int _int = 0;
            int.TryParse(s, out _int);
            return _int;
        }

        public static decimal SafeToDecimal(this string s)
        {
            decimal _dec = (decimal) 0.00;
            decimal.TryParse(s, out _dec);
            return _dec;
        }

        public static DateTime SafeToDateTime(this string s)
        {
            DateTime _date = DateTime.MinValue;
            DateTime.TryParse(s, out _date);
            return _date;
        }

        public static string FormatPhoneNumber(this string s)
        {
            var s1 = "";
            if (s.Length == 0) return "";
            s1 = s.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "");
            return string.Format("({0}) {1}-{2}", s1.Substring(0, 3), s1.Substring(3, 3), s1.Substring(6));
        }

        public static string FormatZipCode(string s)
        {
            var s1 = "";
            if (s.Length == 0) return "";
            s1 = s.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "");
            if (s1.Length == 5) return s1;
            return string.Format("{0}-{1}", s1.Substring(0, 5), s1.Substring(5));
        }
    }
}