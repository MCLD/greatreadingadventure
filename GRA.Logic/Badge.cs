using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GRA.Logic
{
    public class Badge
    {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        public BadgeDisplayDetails GetForDisplay(HttpServerUtility server,
            int badgeId,
            int patronId)
        {
            var badge = SRP.DAL.Badge.FetchObject(badgeId);
            if (badge == null)
            {
                throw new Exception("Badge not found.");
            }
            var badgeDetails = new BadgeDisplayDetails();

            badgeDetails.BadgeId = badge.BID;
            badgeDetails.DisplayName = badge.UserName;
            badgeDetails.Description = badge.CustomDescription;
            badgeDetails.Hidden = badge.HiddenFromPublic == true;

            // if the badge is flagged as hidden from the public, ensure the user has earned it
            if (patronId > 0)
            {
                var patronBadges = SRP.DAL.PatronBadges.GetAll(patronId);
                if (patronBadges != null && patronBadges.Tables.Count > 0)
                {
                    var filterExpression = string.Format("BadgeID = {0}", badge.BID);
                    var patronHasBadge = patronBadges.Tables[0].Select(filterExpression);
                    if (patronHasBadge.Count() > 0)
                    {
                        badgeDetails.Earned = true;
                        var earned = patronHasBadge[0]["DateEarned"] as DateTime?;
                        if (earned != null)
                        {
                            badgeDetails.DateEarned = ((DateTime)earned).ToShortDateString();
                        }
                    }
                }
            }

            string badgePath = NoBadgePath;
            string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                      badgeId);
            if (System.IO.File.Exists(server.MapPath(potentialBadgePath)))
            {
                badgePath = potentialBadgePath;
            }
            badgeDetails.ImageUrl = badgePath;
            badgeDetails.AlternateText = string.Format("Badge: {0}", badge.UserName);


            var earn = new HashSet<string>();

            if (badgeDetails.Description.Length < 1)
            {
                string earnText = SRP.DAL.Badge.GetBadgeReading(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Earn points by reading: {0}.", earnText));
                }

                earnText = SRP.DAL.Badge.GetBadgeGoal(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Achieve part of your personal reading goal: {0}.",
                        earnText));
                }

                earnText = SRP.DAL.Badge.GetEnrollmentPrograms(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Enroll in a reading program: {0}.", earnText));
                }

                earnText = SRP.DAL.Badge.GetBadgeBookLists(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Complete a Challenge: {0}.", earnText));
                }

                earnText = SRP.DAL.Badge.GetBadgeGames(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Unlock and complete an Adventure: {0}.", earnText));
                }

                earnText = SRP.DAL.Badge.GetBadgeEvents(badgeId);
                if (earnText.Length > 0)
                {
                    earn.Add(string.Format("Attend an Event: {0}.",
                        CreateSearchLinks(server, earnText, "~/Events/?Search={0}")));
                }

                if (earn.Count == 0)
                {
                    earn.Add("Learn the secret code to unlock it.");
                }
            }

            badgeDetails.HowToEarn = earn.OrderBy(x => x).ToArray();

            return badgeDetails;
        }

        private string CreateSearchLinks(HttpServerUtility server,
            string stringCsvList,
            string urlFormat)
        {
            string[] items;
            if (stringCsvList.Contains(','))
            {
                items = stringCsvList.Split(',');
            }
            else
            {
                items = new string[1] { stringCsvList };
            }

            StringBuilder stringWithLinks = null;
            foreach (string stringItem in items)
            {
                string link = string.Format(urlFormat, server.UrlEncode(stringItem.Trim()));
                if (stringWithLinks != null)
                {
                    stringWithLinks.Append(", ");
                }
                else
                {
                    stringWithLinks = new StringBuilder();
                }
                stringWithLinks.AppendFormat("<a href=\"{0}\" target=\"_blank\">{1} <small><span class=\"glyphicon glyphicon-new-window\"></span></small></a>",
                   VirtualPathUtility.ToAbsolute(link),
                   stringItem.Trim());
            }
            return stringWithLinks.ToString();
        }
    }
}
