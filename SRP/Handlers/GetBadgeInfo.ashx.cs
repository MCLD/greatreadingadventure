using GRA.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers
{
    public class JsonBadge : JsonBase
    {
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string[] Earn { get; set; }
        public string DateEarned { get; set; }
    }

    /// <summary>
    /// Return a badge's information via JSON
    /// </summary>
    public class GetBadgeInfo : IHttpHandler, IRequiresSessionState
    {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        public void ProcessRequest(HttpContext context)
        {
            var jsonResponse = new JsonBadge();
            try
            {
                if (context.Request.QueryString["BadgeId"] == null)
                {
                    throw new Exception("No badge id provided.");
                }

                int badgeId = 0;
                if (!int.TryParse(context.Request["BadgeId"].ToString(), out badgeId))
                {
                    throw new Exception("Invalid badge id provided.");
                }

                var badge = DAL.Badge.FetchObject(badgeId);
                if (badge == null)
                {
                    throw new Exception("Badge not found.");
                }

                // if the badge is flagged as hidden from the public, ensure the user has earned it
                bool hideBadge = badge.HiddenFromPublic == true;
                bool earnedBadge = false;

                var patron = context.Session[SessionKey.Patron] as DAL.Patron;
                if (patron != null)
                {
                    var patronBadges = DAL.PatronBadges.GetAll(patron.PID);
                    if (patronBadges != null && patronBadges.Tables.Count > 0)
                    {
                        var filterExpression = string.Format("BadgeID = {0}", badge.BID);
                        var patronHasBadge = patronBadges.Tables[0].Select(filterExpression);
                        if (patronHasBadge.Count() > 0)
                        {
                            earnedBadge = true;
                            var earned = patronHasBadge[0]["DateEarned"] as DateTime?;
                            if (earned != null)
                            {
                                jsonResponse.DateEarned = ((DateTime)earned).ToShortDateString();
                            }
                        }
                    }
                }


                if (hideBadge == true && earnedBadge == true)
                {
                    hideBadge = false;
                }

                if (hideBadge == true)
                {
                    throw new Exception("Secret badge must be earned to be revealed.");
                }

                jsonResponse.UserName = badge.UserName;

                string badgePath = NoBadgePath;
                string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                          badgeId);
                if (System.IO.File.Exists(context.Server.MapPath(potentialBadgePath)))
                {
                    badgePath = potentialBadgePath;
                }

                jsonResponse.ImageUrl = VirtualPathUtility.ToAbsolute(badgePath);

                List<string> earn = new List<string>();

                string earnText = DAL.Badge.GetBadgeReading(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Earn points by reading: {0}.", earnText));
                }

                        earnText = DAL.Badge.GetBadgeGoal(badgeId);
                        if (earnText.Length > 0)
                        {
                            earn.Add(string.Format("Achieve part of your personal reading goal: {0}.", earnText));
                        }

                        earnText = DAL.Badge.GetEnrollmentPrograms(badgeId);
                        if (earnText.Length > 0)
                        {
                            earn.Add(string.Format("Enroll in a reading program: {0}.", earnText));
                        }

                earnText = DAL.Badge.GetBadgeBookLists(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Complete a Challenge: {0}.", earnText));
                }

                earnText = DAL.Badge.GetBadgeGames(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Unlock and complete an Adventure: {0}.",
                                           earnText));
                }

                earnText = DAL.Badge.GetBadgeEvents(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Attend an Event: {0}.", earnText));
                }

                if (earn.Count == 0)
                {
                    earn.Add("Learn the secret code to unlock it.");
                }

                jsonResponse.Earn = earn.ToArray();
                jsonResponse.Success = true;
            }
            catch (Exception ex)
            {
                string safeBadgeId = context.Request["BadgeId"] == null
                    ? "<none requested>"
                    : context.Request["BadgeId"].ToString();
                this.Log().Error("Requested badge {0} from {1}?{2} - {3}",
                    safeBadgeId,
                    context.Request.Url,
                    context.Request.QueryString,
                    ex.Message);
                jsonResponse.Success = false;
                jsonResponse.ErrorMessage = ex.Message;
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
