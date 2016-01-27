using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GRA.Tools {
    public class DisplayHelper {
        public static string FormatName(string first, string last, string username) {
            if(string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(last)
               || string.IsNullOrWhiteSpace(first)) {
                return username.Trim();
            }

            StringBuilder formattedName = new StringBuilder(first.Trim());

            if(!string.IsNullOrWhiteSpace(last)) {
                formattedName.AppendFormat(" {0}", last.Trim());
            }
            if(!string.IsNullOrWhiteSpace(username)) {
                formattedName.AppendFormat(" ({0})", username);
            }
            return formattedName.ToString().Trim();
        }

        public static string FormatFirstName(string first, string username) {
            return string.IsNullOrWhiteSpace(first)
                ? username.Trim()
                : first.Trim();
        }

        public static string RemoveHtml(string htmlText) {
            return RemoveHtml(htmlText, 0);
        }

        public static string RemoveHtml(string htmlText, int maxLength) {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlText);
            string text = HttpUtility.HtmlDecode(htmlDoc.DocumentNode.InnerText)
                .Replace("<p>", string.Empty)
                .Replace("</p>", string.Empty);

            if(maxLength > 0) {
                return text.Length <= maxLength
                    ? text
                    : string.Format("{0}...", text.Substring(0, maxLength - 3));
            } else {
                return text;
            }
        }
    }
}
