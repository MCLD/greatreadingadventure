using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers {
    /// <summary>
    /// Summary description for GetEarnedBadgesInfo
    /// </summary>
    public class GetEarnedBadgesInfo : IHttpHandler, IRequiresSessionState {
        public class JsonEarnedBadge {
            string UserName { get; set; }
            string BadgeId { get; set; }
            string EarnedMessage { get; set; }
        }

        public class JsonEarnedBadges : JsonBase {
            public JsonEarnedBadge[] EarnedBadges { get; set; }
        }

        public void ProcessRequest(HttpContext context) {
            var jsonResponse = new JsonEarnedBadges();
            if(context.Request.QueryString["BadgeIds"] == null) {
                jsonResponse.Success = false;
                jsonResponse.ErrorMessage = "No badge ids provided.";
                this.Log().Error(string.Format("Badges requested from {0}?{1} with no ids.",
                                               context.Request.Url,
                                               context.Request.QueryString));


            } else {
                string badgeIdsString = context.Request.QueryString["BadgeIds"];
                string[] badgeIdsStrings = null;
                if(badgeIdsString.Contains(',')) {
                    badgeIdsStrings = badgeIdsString.Split(',');
                } else {
                    badgeIdsStrings = new string[] { badgeIdsString };
                }

                // string array 
                if(badgeIdsStrings.Count() == 0) {
                    jsonResponse.Success = false;
                    jsonResponse.ErrorMessage = "No properly-formatted badge ids provided.";
                    this.Log().Error(string.Format("Badges requested from {0}?{1} in the wrong format.",
                                                   context.Request.Url,
                                                   context.Request.QueryString));
                }

            }

        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}