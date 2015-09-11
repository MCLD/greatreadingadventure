using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.Tools {
    public class WebTools {
        /// <summary>
        /// Provided an <see cref="HttpRequest"/>, returns the URL to the current path minus the
        /// trailing slash.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> object</param>
        /// <returns>The URL to the current path with the trailing slash removed.</returns>
        public static string GetBaseUrl(HttpRequest request) {
            return string.Format("{0}://{1}{2}",
                                 request.Url.Scheme,
                                 request.Url.Authority,
                                 request.ApplicationPath.TrimEnd('/'));
        }
    }
}
