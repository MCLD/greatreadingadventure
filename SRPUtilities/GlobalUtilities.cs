using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

namespace GRA.SRP.Core.Utilities
{
    public class GlobalUtilities
    {
        public static string SRPDBConnectionStringName = "SRPDBConn";
        public static string SRPDB = ConfigurationManager.ConnectionStrings[SRPDBConnectionStringName].ToString();

        // This is a helper method used to determine the index of the
        // column being sorted. If no column is being sorted, -1 is returned.
        public static int GetSortColumnIndex(String strCol, GridView _gv)
        {
            foreach (DataControlField field in _gv.Columns)
            {
                if (field.SortExpression == strCol)
                {
                    return _gv.Columns.IndexOf(field);
                }
            }

            return -1;
        }

        public static void AddSortImage(GridViewRow headerRow, GridView _gv, string m_strSortExp, SortDirection m_SortDirection)
        {
            Int32 iCol = GetSortColumnIndex(m_strSortExp, _gv);
            if (-1 == iCol)
            {
                return;
            }
            // Create the sorting image based on the sort direction.
            Image sortImage = new Image();
            if (SortDirection.Ascending == m_SortDirection)
            {
                sortImage.ImageUrl = "~/ControlRoom/Images/dwnSort.gif";
                sortImage.AlternateText = "Ascending Order";
            }
            else
            {
                sortImage.ImageUrl = "~/ControlRoom/Images/upSort.gif";
                sortImage.AlternateText = "Descending Order";
            }

            // Add the image to the appropriate header cell.
            headerRow.Cells[iCol].Controls.Add(sortImage);
        }

        public static DataTable ConvertArrayToTable(Array myList)
        {
            DataTable dt = new DataTable();
            if (myList.Length > 0)
            {
                PropertyInfo[] propInfos = myList.GetValue(0).GetType().GetProperties();

                foreach (PropertyInfo propInfo in propInfos)
                {
                    dt.Columns.Add(propInfo.Name, propInfo.PropertyType);
                }

                foreach (object tempObject in myList)
                {
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < propInfos.Length; i++)
                    {
                        dr[i] = propInfos[i].GetValue(tempObject, null);
                    }

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        public static void gv_RowCreated(object sender, GridViewRowEventArgs e, string m_strSortExp, SortDirection m_SortDirection)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (String.Empty != m_strSortExp)
                {
                    AddSortImage(e.Row, (GridView)sender, m_strSortExp, m_SortDirection);
                }
            }
        }


        public static object DBSafeDate(string sDate)
        {
            //pass this method a string
            //if the string contains a date which is both valid
            //and greater than the min date for SQL Server
            //it will return an object containing a date
            //otherwise it will return an object containing DBNull
            object oRet = DBNull.Value;
            DateTime ldate;
            if (DateTime.TryParse(sDate, out ldate))
            {
                if (ldate >= DateTime.Parse(System.Data.SqlTypes.SqlDateTime.MinValue.ToString()))
                {
                    //Safe enough 
                    oRet = (object)ldate;
                }
            }
            return oRet;
        }
        public static object DBSafeValue(object oval, System.TypeCode theType)
        {

            object oRet = DBNull.Value;

            switch (theType)
            {
                case TypeCode.DateTime:
                    oRet = DBSafeDate(oval.ToString());
                    break;
                case TypeCode.Decimal:
                    decimal _decimal;
                    if (decimal.TryParse(oval.ToString(), out _decimal)) oRet = (object)_decimal;
                    break;
                case TypeCode.Int32:
                    int _int;
                    if (int.TryParse(oval.ToString(), out _int)) oRet = (object)_int;
                    break;
                case TypeCode.Boolean:
                    bool _bool;
                    if (bool.TryParse(oval.ToString(), out _bool)) oRet = (object)_bool;
                    break;
                case TypeCode.String:
                    if ((string)oval != string.Empty) 
                        oRet = oval;
                    else
                    {
                        oRet = "";
                    }
                    break;
                default:
                    throw new Exception("Type not supported by method");
            }
            return oRet;
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


    public static class OrderByHelper
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            foreach (OrderByInfo orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy<T>(collection, orderByInfo);

            return collection;
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            try
            {
                string[] props = orderByInfo.PropertyName.Split('.');
                Type type = typeof(T);

                ParameterExpression arg = Expression.Parameter(type, "x");
                Expression expr = arg;
                foreach (string prop in props)
                {
                    // use reflection (not ComponentModel) to mirror LINQ
                    PropertyInfo pi = type.GetProperty(prop);
                    expr = Expression.Property(expr, pi);
                    type = pi.PropertyType;
                }
                Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
                LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
                string methodName = String.Empty;

                if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
                {
                    if (orderByInfo.Direction == SortDirection.Ascending)
                        methodName = "ThenBy";
                    else
                        methodName = "ThenByDescending";
                }
                else
                {
                    if (orderByInfo.Direction == SortDirection.Ascending)
                        methodName = "OrderBy";
                    else
                        methodName = "OrderByDescending";
                }

                
                return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                                                  .MakeGenericMethod(typeof(T), type)
                                                  .Invoke(null, new object[] { collection, lambda });
            }
            catch (Exception ex)
            {
                String x = ex.Message;
                x = "";
            }

            return null;
        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (String.IsNullOrEmpty(orderBy))
                yield break;

            string[] items = orderBy.Split(',');
            bool initial = true;
            foreach (string item in items)
            {
                string[] pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(String.Format("Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC", item));

                string prop = pair[0].Trim();

                if (String.IsNullOrEmpty(prop))
                    throw new ArgumentException("Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                SortDirection dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = ("desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending);

                yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

                initial = false;
            }

        }

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }

        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }
    }

}