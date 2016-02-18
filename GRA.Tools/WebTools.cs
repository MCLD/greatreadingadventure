using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.Tools
{
    public class WebTools
    {
        /// <summary>
        /// Provided an <see cref="HttpRequest"/>, returns the URL to the current path minus the
        /// trailing slash.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> object</param>
        /// <returns>The URL to the current path with the trailing slash removed.</returns>
        public static string GetBaseUrl(HttpRequest request)
        {
            return string.Format("{0}://{1}{2}",
                                 request.Url.Scheme,
                                 request.Url.Authority,
                                 request.ApplicationPath.TrimEnd('/'));
        }

        public string CssEnsureClass(string css, string cssClass)
        {
            var cssStyleList = css.Split(' ').ToList();
            if (!string.IsNullOrWhiteSpace(css))
            {
                if (!cssStyleList.Contains(cssClass))
                {
                    cssStyleList.Add(cssClass);
                    return string.Join(" ", cssStyleList);
                }
            }
            return css;
        }

        public string CssRemoveClass(string css, string cssClass)
        {
            var cssStyleList = css.Split(' ').ToList();
            if (!string.IsNullOrWhiteSpace(css))
            {
                if (css.Contains(cssClass))
                {
                    cssStyleList.Remove(cssClass);
                    return string.Join(" ", cssStyleList);
                }
            }
            return css;
        }
    }
}
