using Newtonsoft.Json;
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
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        public class JsonEarnedBadge {
            public string UserName { get; set; }
            public int BadgeId { get; set; }
            public string ImageUrl { get; set; }
            public string EarnedMessage { get; set; }
        }

        public class JsonEarnedBadges : JsonBase {
            public JsonEarnedBadge[] EarnedBadges { get; set; }
        }

        public void ProcessRequest(HttpContext context) {
            var jsonResponse = new JsonEarnedBadges();
            var earnedBadges = new List<JsonEarnedBadge>();
            try {
                if(context.Request.QueryString["BadgeIds"] == null) {
                    throw new ArgumentNullException("BadgeIds", "No badge ids provided.");
                }

                string badgeIdsQueryString = context.Request.QueryString["BadgeIds"];
                string[] badgeIdsStringArray = null;
                if(badgeIdsQueryString.Contains(',')) {
                    badgeIdsStringArray = badgeIdsQueryString.Split(',');
                } else {
                    badgeIdsStringArray = new string[] { badgeIdsQueryString };
                }

                // string array 
                if(badgeIdsStringArray.Count() == 0) {
                    throw new ArgumentException("No properly-formatted badge ids provided", "BadgeIds");
                }

                foreach(var badgeIdString in badgeIdsStringArray) {
                    int badgeId;
                    if(!int.TryParse(badgeIdString, out badgeId)) {
                        this.Log().Error(string.Format("Badge {0} requested from {1}?{2} is not numeric.",
                                                       badgeIdString,
                                                       context.Request.Url,
                                                       context.Request.QueryString));

                    } else {
                        var badge = DAL.Badge.FetchObject(badgeId);
                        if(badge == null) {
                            this.Log().Error(string.Format("Requested badge {0} from {1}?{2} - not found",
                                                           badgeId,
                                                           context.Request.Url,
                                                           context.Request.QueryString));
                        } else {
                            string badgePath = NoBadgePath;
                            string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                                      badgeId);
                            if(System.IO.File.Exists(context.Server.MapPath(potentialBadgePath))) {
                                badgePath = potentialBadgePath;
                            }

                            earnedBadges.Add(new JsonEarnedBadge {
                                UserName = badge.UserName,
                                BadgeId = badge.BID,
                                ImageUrl = VirtualPathUtility.ToAbsolute(badgePath),
                                EarnedMessage = context.Server.HtmlDecode(badge.CustomEarnedMessage)
                            });
                        }
                    }
                }

                if(earnedBadges.Count() > 0) {
                    jsonResponse.Success = true;
                    jsonResponse.EarnedBadges = earnedBadges.ToArray();
                } else {
                    this.Log().Error(string.Format("Patron should have earned a badge but no badges were awarded: {0}?{1}",
                                                   context.Request.Url,
                                                   context.Request.QueryString));
                }
            } catch(ArgumentException aex) {
                this.Log().Error(string.Format("Error with badges requested from {0}?{1}: {2}.",
                                 context.Request.Url,
                                 context.Request.QueryString,
                                 aex.Message));
                jsonResponse.Success = false;
                jsonResponse.ErrorMessage = aex.Message;
            }
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(jsonResponse));
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}