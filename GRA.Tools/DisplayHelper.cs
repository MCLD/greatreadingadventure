using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
