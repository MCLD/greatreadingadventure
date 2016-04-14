using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;

namespace GRA.Tools
{
    public class WebTools
    {
        public const string BranchLinkStub = " <a href=\"{0}\" target=\"_blank\" title=\"Show me this location's Web page in a new window\">{1} <span class=\"glyphicon glyphicon-new-window hidden-print\"></span></a>";
        public const string BranchMapStub = " <a href=\"http://maps.google.com/?q={0}\" target=\"_blank\" class=\"event-branch-detail-glyphicon hidden-print\" title=\"Show me a map of this location in a new window\"><span class=\"glyphicon glyphicon glyphicon-map-marker\"></span></a>";
        public const string BranchMapLinkStub = "http://maps.google.com/?q={0}";

        /// <summary>
        /// Provided an <see cref="HttpRequest"/>, returns the URL to the current path minus the
        /// trailing slash.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> object</param>
        /// <returns>The URL to the current path with the trailing slash removed.</returns>
        public static string GetBaseUrl(HttpRequest request)
        {
            // authority includes port, host does not
            string configHostname = WebConfigurationManager.AppSettings["ReverseProxyHostname"];
            if (!string.IsNullOrWhiteSpace(configHostname))
            {
                return string.Format("{0}://{1}{2}",
                     request.Url.Scheme,
                     configHostname,
                     request.ApplicationPath.TrimEnd('/'));
            }
            else
            {
                return string.Format("{0}://{1}{2}",
                    request.Url.Scheme,
                    request.Url.Authority,
                    request.ApplicationPath.TrimEnd('/'));
            }
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
            return css.Trim();
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
            return css.Trim();
        }

        public string BuildEventJsonld(SchemaOrgEvent evt)
        {
            // check requied fields per https://developers.google.com/structured-data/rich-snippets/events
            if (evt == null
                || evt.Location == null
                || string.IsNullOrEmpty(evt.Name)
                || evt.StartDate == DateTime.MinValue
                || string.IsNullOrEmpty(evt.Location.Name)
                || string.IsNullOrEmpty(evt.Location.Address))
            {
                return string.Empty;
            }

            JObject location = null;
            if (!string.IsNullOrEmpty(evt.Location.Telephone))
            {
                location = new JObject(
                    new JProperty("@type", "Library"),
                    new JProperty("name", evt.Location.Name),
                    new JProperty("address", evt.Location.Address),
                    new JProperty("telephone", evt.Location.Telephone),
                    new JProperty("url", evt.Location.Url)
                    );
            }
            else
            {
                location = new JObject(
                    new JProperty("@type", "Library"),
                    new JProperty("name", evt.Location.Name),
                    new JProperty("address", evt.Location.Address),
                    new JProperty("url", evt.Location.Url)
                    );
            }

            JObject jsonld = null;
            if (!string.IsNullOrEmpty(evt.Url))
            {
                jsonld = new JObject(
                    new JProperty("@context", "http://schema.org"),
                    new JProperty("@type", "Event"),
                    new JProperty("name", evt.Name),
                    new JProperty("url", evt.Url),
                    new JProperty("location", location),
                    new JProperty("startDate", evt.StartDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                    );
            }
            else
            {
                jsonld = new JObject(
                    new JProperty("@context", "http://schema.org"),
                    new JProperty("@type", "Event"),
                    new JProperty("name", evt.Name),
                    new JProperty("location", location),
                    new JProperty("startDate", evt.StartDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                    );
            }
            return string.Format("<script type=\"application/ld+json\">{0}</script>",
                jsonld.ToString());
        }

        public HtmlControl OgMetadataTag(string property, string content)
        {
            var tag = new HtmlMeta();
            tag.Attributes.Add("property", property);
            tag.Content = content;
            return tag;
        }
    }

    public class SchemaOrgLibrary
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Url { get; set; }
    }

    public class SchemaOrgEvent
    {
        public SchemaOrgLibrary Location { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime StartDate { get; set; }
    }
}
