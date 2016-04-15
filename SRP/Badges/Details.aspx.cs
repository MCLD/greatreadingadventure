using GRA.SRP.DAL;
using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GRA.SRP.Badges
{
    public partial class Details : BaseSRPPage
    {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool badgeEarned = false;
            string dateEarned = null;
            if (!String.IsNullOrEmpty(Request["PID"]))
            {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if (!IsPostBack)
            {
                if (Session["ProgramID"] == null)
                {
                    try
                    {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    }
                    catch
                    {
                        Response.Redirect("~/Badges/");
                    }
                }
            }
            TranslateStrings(this);

            if (Request.UrlReferrer == null)
            {
                badgeBackLink.NavigateUrl = "~/Badges/";
            }
            else
            {
                badgeBackLink.NavigateUrl = Request.UrlReferrer.AbsolutePath;
            }

            Badge badge = null;
            TwitterShare.Visible = false;
            FacebookShare.Visible = false;

            int badgeId = 0;
            string displayBadge = Request.QueryString["BadgeId"];
            if (!string.IsNullOrEmpty(displayBadge)
                && int.TryParse(displayBadge.ToString(), out badgeId))
            {
                badge = DAL.Badge.FetchObject(badgeId);
                if (badge != null)
                {
                    var patron = Session[SessionKey.Patron] as DAL.Patron;
                    DataSet patronBadges = null;
                    if (patron != null)
                    {
                        patronBadges = DAL.PatronBadges.GetAll(patron.PID);
                        if (patronBadges.Tables.Count > 0)
                        {
                            var filterExpression = string.Format("BadgeID = {0}", badge.BID);
                            var patronHasBadge = patronBadges.Tables[0].Select(filterExpression);
                            if (patronHasBadge.Count() > 0)
                            {
                                badgeEarned = true;
                                var earned = patronHasBadge[0]["DateEarned"] as DateTime?;
                                if (earned != null)
                                {
                                    dateEarned = ((DateTime)earned).ToShortDateString();
                                }
                            }
                        }
                    }

                    if (badge.HiddenFromPublic && !badgeEarned)
                    {
                        badge = null;
                    }
                }

                if (badge != null)
                {
                    badgeTitle.Text = badge.UserName;
                    this.Title = string.Format("Badge: {0}", badgeTitle.Text);
                    this.MetaDescription = string.Format("All about the {0} badge - {1}",
                                                         badgeTitle.Text,
                                                         GetResourceString("system-name"));
                    string badgePath = NoBadgePath;
                    string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                              badgeId);
                    if (System.IO.File.Exists(Server.MapPath(potentialBadgePath)))
                    {
                        badgePath = potentialBadgePath;
                    }

                    this.badgeImage.ImageUrl = badgePath;
                    this.badgeImage.AlternateText = string.Format("Badge: {0}", badge.UserName);

                    if (!string.IsNullOrEmpty(dateEarned))
                    {
                        badgeEarnWhen.Text = string.Format("<p><strong>You earned this badge on {0}!</strong></p>",
                            dateEarned);
                        badgeEarnWhen.Visible = true;
                    }
                    else
                    {
                        badgeEarnWhen.Visible = false;
                    }

                    StringBuilder earn = new StringBuilder();

                    string earnText = Badge.GetBadgeReading(badgeId);
                    if (earnText.Length > 0)
                    {
                        earn.AppendFormat("<li>Earn points by reading: {0}.</li>", earnText);
                    }

                    earnText = Badge.GetBadgeGoal(badgeId);
                    if (earnText.Length > 0)
                    {
                        earn.AppendFormat("<li>Achieve part of your personal reading goal: {0}.</li>", earnText);
                    }

                    earnText = Badge.GetEnrollmentPrograms(badgeId);
                    if (earnText.Length > 0)
                    {
                        earn.AppendFormat("<li>Enroll in a reading program: {0}</li>", earnText);
                    }

                    earnText = Badge.GetBadgeBookLists(badgeId);
                    if (earnText.Length > 0)
                    {
                        earn.AppendFormat("<li>Complete a Challenge: {0}</li>", earnText);
                    }

                    earnText = Badge.GetBadgeGames(badgeId);
                    if (earnText.Length > 0)
                    {
                        earn.AppendFormat("<li>Unlock and complete an Adventure: {0}</li>", earnText);
                    }

                    earnText = Badge.GetBadgeEvents(badgeId);
                    if (earnText.Length > 0)
                    {
                        earn.AppendFormat("<li>Attend an Event: {0}</li>", earnText);
                    }

                    if (earn.Length > 0)
                    {
                        badgeEarnLabel.Text = earn.ToString();
                    }
                    else
                    {
                        badgeEarnLabel.Text = "<li>Learn the secret code to unlock it.</li>";
                    }
                    badgeEarnPanel.Visible = true;
                    badgeDetails.Visible = true;

                    /* metadata */
                    string systemName = GetResourceString("system-name");
                    string title = string.Format("{0} badge: {1}",
                        systemName,
                        badge.UserName);
                    string description = null;
                    string twitDescrip = null;

                    if (badgeEarned)
                    {
                        description = string.Format("By participating in {0} I earned this badge: {1}!",
                            systemName,
                            badge.UserName);
                        twitDescrip = string.Format("I earned this {0} badge: {1}!",
                            systemName,
                            badge.UserName);
                        if (twitDescrip.Length > 118)
                        {
                            // if it's longer than this it won't fit with the url, shorten it
                            twitDescrip = string.Format("I earned this badge: {0}!",
                                badge.UserName);
                        }
                    }
                    else
                    {
                        description = string.Format("By participating in {0} you can earn this badge: {1}!",
                            systemName,
                            badge.UserName);
                        twitDescrip = string.Format("Check out this {0} badge: {1}!",
                            systemName,
                            badge.UserName);
                        if (twitDescrip.Length > 118)
                        {
                            // if it's longer than this it won't fit with the url, shorten it
                            twitDescrip = string.Format("Check out this badge: {0}!",
                                badge.UserName);
                        }
                    }

                    var wt = new WebTools();
                    var baseUrl = WebTools.GetBaseUrl(Request);
                    var badgeDetailsUrl = string.Format("{0}/Badges/Details.aspx?BadgeId={1}",
                        baseUrl,
                        badge.BID);
                    var badgeImagePath = string.Format("{0}{1}", baseUrl,
                        VirtualPathUtility.ToAbsolute(badgePath));

                    wt.AddOgMetadata(Metadata,
                        title,
                        description,
                        badgeImagePath,
                        badgeDetailsUrl,
                        "game.achievement",
                        GetResourceString("facebook-appid"));

                    wt.AddTwitterMetadata(Metadata,
                        title, 
                        twitDescrip, 
                        badgeImagePath, 
                        twitterUsername: GetResourceString("twitter-username"));
                    /* end metadata */

                    string twitterHashtags = GetResourceString("twitter-hashtags");
                    if (!string.IsNullOrEmpty(twitterHashtags)
                        && twitterHashtags != "twitter-hashtags")
                    {
                        TwitterShare.NavigateUrl = string.Format("http://twitter.com/share?text={0}&url={1}&hashtags={2}",
                            twitDescrip,
                            Server.UrlEncode(badgeDetailsUrl),
                            twitterHashtags);
                    }
                    else
                    {
                        TwitterShare.NavigateUrl = string.Format("http://twitter.com/share?text={0}&url={1}",
                            twitDescrip,
                            Server.UrlEncode(badgeDetailsUrl));
                    }
                    TwitterShare.Visible = true;
                    FacebookShare.NavigateUrl = string.Format("http://www.facebook.com/sharer.php?u={0}",
                        Server.UrlEncode(badgeDetailsUrl));
                    FacebookShare.Visible = true;
                }
                badgeDetails.Visible = true;
            }
            if (badge == null)
            {
                badgeDetails.Visible = false;
                var cph = Page.Master.FindControl("HeaderContent") as ContentPlaceHolder;
                if (cph != null)
                {
                    cph.Controls.Add(new HtmlMeta
                    {
                        Name = "robots",
                        Content = "noindex"
                    });
                }
                new SessionTools(Session).AlertPatron("Could not find details on that badge.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
            }
        }
    }
}
