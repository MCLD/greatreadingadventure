using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GRA.Tools {
    public class WebTools {
        /// <summary>
        /// Provided an <see cref="HttpRequest"/>, returns the URL to the current path minus the
        /// trailing slash.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> object</param>
        /// <returns>The URL to the current path with the trailing slash removed.</returns>
        public static string GetBaseUrl(HttpRequest request) {
            return "{Scheme}://{Authority}{ApplicationPath}"
                   .FormatWith(new {
                       Scheme = request.Url.Scheme,
                       Authority = request.Url.Authority,
                       ApplicationPath = request.ApplicationPath.TrimEnd('/')
                   });
        }
    }
}
