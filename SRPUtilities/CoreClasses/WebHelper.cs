using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities
{
    /// <summary>Summary description for WebHelper.</summary>
    public sealed class WebHelper
    {
        #region Constant(s)
        public static int kDefCookieExpireInDays = 365;
        #endregion

        #region Enum(s)
        public enum RelType
        {
            /// <summary></summary>
            Dev = 1,
            /// <summary></summary>
            Demo = 2,
            /// <summary></summary>
            QA = 3,
            /// <summary></summary>
            Prd = 4
        }
        #endregion

        #region Member(s)
        private static string _appURL = String.Empty;
        #endregion

        #region Properties
        public static string AppPath
        {
            get
            {
                if (_appURL == string.Empty)
                {
                    _appURL = HttpContext.Current.Request.ApplicationPath;
                }
                return (_appURL);
            }
        }

        public static string AppURL
        {
            get { return (HttpContext.Current.Request.Url.AbsoluteUri); }
        }

        public static bool IsLocalhost
        {
            //# get { return (HttpContext.Current.Server.MachineName == "localhost"); }
            get { return (HttpContext.Current.Request.Url.Host == "localhost"); }
        }
        #endregion

        #region URL Method(s)
        public static void Redirect(string url, bool endResponse, string msg)
        {
            Redirect(HttpContext.Current.Response, url, endResponse, msg);
        }

        public static void Redirect(HttpResponse httpRes, string url, bool endResponse, string msg)
        {
            string urlMsg;

            if (msg.Length > 0)
                urlMsg = string.Format("{0}?Msg={1}", url, msg);
            else
                urlMsg = url;

            httpRes.Redirect(urlMsg, endResponse);
        }

        public static string RootUrl
        {
            get
            {
                string _rootUrl = string.Empty;
                string _secureRootUrl = string.Empty;
                HttpContext context = HttpContext.Current;

                if (_rootUrl.Length == 0 || _secureRootUrl.Length == 0)
                {

                    string executionPath = context.Request.ApplicationPath;
                    _rootUrl = string.Format("http://{0}{1}", context.Request.Url.Authority,
                                             executionPath.Length == 1 ? string.Empty : executionPath);

                    _secureRootUrl = string.Format("https://{0}{1}", context.Request.Url.Authority,
                                                   executionPath.Length == 1 ? string.Empty : executionPath);

                }
                return String.Equals(context.Request.Url.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? _secureRootUrl : _rootUrl;
            }
        }
        #endregion

        #region Cookie Method(s)
        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        ///----------------------------------------------------------------
        public static string GetCookie(string name, string defVal)
        {
            HttpCookie myCookie;
            string val;

            myCookie = HttpContext.Current.Request.Cookies[name];
            if (myCookie == null)
                myCookie = new HttpCookie(name);
            if (myCookie.Value == null)
                myCookie.Value = defVal;

            val = myCookie.Value;

            return (val);
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        ///----------------------------------------------------------------
        public static int GetCookie(string name, int defVal)
        {
            string val;
            int nVal;

            val = GetCookie(name, defVal.ToString());
            nVal = Convert.ToInt32(val);

            return (nVal);
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        ///----------------------------------------------------------------
        public static long GetCookie(string name, long defVal)
        {
            string val;
            long lVal;

            val = GetCookie(name, defVal.ToString());
            lVal = Convert.ToInt32(val);

            return (lVal);
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="dtExpires"></param>
        ///----------------------------------------------------------------
        public static void SetCookie(string name, long val, DateTime dtExpires)
        {
            SetCookie(name, val.ToString(), dtExpires);
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        ///----------------------------------------------------------------
        public static void SetCookie(string name, long val)
        {
            SetCookie(name, val.ToString(), DateTime.Now.AddDays(kDefCookieExpireInDays));
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="dtExpires"></param>
        ///----------------------------------------------------------------
        public static void SetCookie(string name, int val, DateTime dtExpires)
        {
            SetCookie(name, val.ToString(), dtExpires);
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        ///----------------------------------------------------------------
        public static void SetCookie(string name, int val)
        {
            SetCookie(name, val.ToString(), DateTime.Now.AddDays(kDefCookieExpireInDays));
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        ///----------------------------------------------------------------
        public static void SetCookie(string name, string val)
        {
            SetCookie(name, val, DateTime.Now.AddDays(kDefCookieExpireInDays));
        }

        ///----------------------------------------------------------------
        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="dtExpires"></param>
        ///----------------------------------------------------------------
        public static void SetCookie(string name, string val, DateTime dtExpires)
        {
            HttpCookie myCookie;

            // Set the cookie value & expiration.
            myCookie = new HttpCookie(name);
            myCookie.Value = val;
            myCookie.Expires = dtExpires;

            // Add the cookie.
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
        #endregion

        #region Web UI Method(s)
        public static string UIFormatToShortDate(DateTime dateTime)
        {
            return (dateTime.ToShortDateString());
        }
        #endregion
    }
}
