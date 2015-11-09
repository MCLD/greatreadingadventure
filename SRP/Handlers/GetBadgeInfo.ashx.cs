using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers {
    public class JsonBadge : JsonBase {
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string[] Earn { get; set; }
    }

    /// <summary>
    /// Return a badge's information via JSON
    /// </summary>
    public class GetBadgeInfo : IHttpHandler, IRequiresSessionState {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        public void ProcessRequest(HttpContext context) {
            var jsonResponse = new JsonBadge();
            if(context.Request.QueryString["BadgeId"] == null) {
                jsonResponse.Success = false;
                jsonResponse.ErrorMessage = "No badge id provided.";
                this.Log().Error(string.Format("Badge requested from {0}?{1} with no id.",
                                               context.Request.Url,
                                               context.Request.QueryString));
            } else {
                int badgeId = 0;
                if(!int.TryParse(context.Request["BadgeId"].ToString(), out badgeId)) {
                    jsonResponse.Success = false;
                    jsonResponse.ErrorMessage = "Invalid badge id provided.";
                    this.Log().Error(string.Format("Requested badge {0} from {1}?{2} - invalid id",
                                                   context.Request["BadgeId"].ToString(),
                                                   context.Request.Url,
                                                   context.Request.QueryString));
                } else {
                    var badge = DAL.Badge.FetchObject(badgeId);
                    if(badge == null) {
                        jsonResponse.Success = false;
                        jsonResponse.ErrorMessage = "Badge not found.";
                        this.Log().Error(string.Format("Requested badge {0} from {1}?{2} - not found",
                                                       badgeId,
                                                       context.Request.Url,
                                                       context.Request.QueryString));
                    } else {
                        jsonResponse.UserName = badge.UserName;

                        string badgePath = NoBadgePath;
                        string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                                  badgeId);
                        if(System.IO.File.Exists(context.Server.MapPath(potentialBadgePath))) {
                            badgePath = potentialBadgePath;
                        }

                        jsonResponse.ImageUrl = VirtualPathUtility.ToAbsolute(badgePath);

                        List<string> earn = new List<string>();

                        string earnText = DAL.Badge.GetBadgeReading(badgeId);
                        if(earnText.Length > 0) {
                            earn.Add(string.Format("Earn points by reading: {0}.", earnText));
                        }

                        earnText = DAL.Badge.GetEnrollmentPrograms(badgeId);
                        if(earnText.Length > 0) {
                            earn.Add(string.Format("Enroll in a reading program: {0}.", earnText));
                        }

                        earnText = DAL.Badge.GetBadgeBookLists(badgeId);
                        if(earnText.Length > 0) {
                            earn.Add(string.Format("Complete a Challenge: {0}.", earnText));
                        }

                        earnText = DAL.Badge.GetBadgeGames(badgeId);
                        if(earnText.Length > 0) {
                            earn.Add(string.Format("Unlock and complete an Adventure: {0}.",
                                                   earnText));
                        }

                        earnText = DAL.Badge.GetBadgeEvents(badgeId);
                        if(earnText.Length > 0) {
                            earn.Add(string.Format("Attend an Event: {0}.", earnText));
                        }

                        jsonResponse.Earn = earn.ToArray();
                        jsonResponse.Success = true;
                    }
                }
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