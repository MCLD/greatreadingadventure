using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GRA.Logic
{
    /// <summary>
    /// Provide unified logic for displaying information about Badges
    /// </summary>
    public class Badge
    {
        /// <summary>
        /// Path to the image to display if no badge image is provided or when a badge is created.
        /// </summary>
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        /// <summary>
        /// The badge name length to truncate at in the gallery
        /// </summary>
        private const int BadgeGalleryNameLength = 70;

        /// <summary>
        /// Return an object containing information about the badge, typically for when badge
        /// details are to be displayed.
        /// </summary>
        /// <param name="server">An <see cref="HttpServerUtility"/> for context information</param>
        /// <param name="badgeId">The ID number of the badge</param>
        /// <param name="patronId">The ID number of the patron - 0 or below if we don't know the
        /// patron's ID</param>
        /// <returns>A populated <see cref="BadgeDisplayDetails"/> with badge details</returns>
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
            badgeDetails.HideDefaultDescription = badge.HideDefaultDescriptionFlag;
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

            badgeDetails.HowToEarn = earn.OrderBy(x => x).ToArray();

            return badgeDetails;
        }

        /// <summary>
        /// Take a comma-separated length of text and create links from it
        /// </summary>
        /// <param name="server">An <see cref="HttpServerUtility"/> for context information</param>
        /// <param name="stringCsvList">The comma-separated list of strings</param>
        /// <param name="urlFormat">a string.Format where {0} will be replaced with the search
        /// terms</param>
        /// <returns>A string containing search links to each of the comma-separated search values
        /// </returns>
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

        /// <summary>
        /// Truncate badge names so that they are less than 70 characters for display in the badge
        /// gallery
        /// </summary>
        /// <param name="fullNameObject">The full name of the badge - we accept an object in case
        /// it needs to be done during data binding (in <see cref="Eval"/>)</param>
        /// <returns>The full text of the name if it is less than 71 characters, otherwise the name
        /// truncated to the last space prior to 70 characters with an elipsis appended.</returns>
        public static object GalleryLengthName(object fullNameObject)
        {
            string fullName = fullNameObject as string;
            if (!string.IsNullOrEmpty(fullName))
            {
                if (fullName.Length <= BadgeGalleryNameLength)
                {
                    return fullName;
                }
                else
                {
                    if (fullName.Contains(" "))
                    {
                        return string.Format("{0}...",
                            fullName.Substring(0, fullName.Substring(0, BadgeGalleryNameLength)
                            .LastIndexOf(' ')));
                    }
                    else
                    {
                        return string.Format("{0}...",
                            fullName.Substring(0, BadgeGalleryNameLength));
                    }
                }
            }
            return fullNameObject;
        }
    }
}
