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

            badgeBackLink.NavigateUrl = "~/Badges/";

            TwitterShare.Visible = false;
            FacebookShare.Visible = false;

            try
            {
                int badgeId = 0;
                string displayBadge = Request.QueryString["BadgeId"];
                if (!int.TryParse(displayBadge, out badgeId))
                {
                    throw new Exception("Invalid badge id provided.");
                }

                int patronId = 0;
                var patron = Session[SessionKey.Patron] as DAL.Patron;
                if (patron != null)
                {
                    patronId = patron.PID;
                }

                var badgeDetailData = new Logic.Badge().GetForDisplay(Server, badgeId, patronId);


                if (badgeDetailData.Hidden == true && badgeDetailData.Earned == false)
                {
                    throw new Exception("Secret badge must be earned to be revealed.");
                }

                badgeTitle.Text = badgeDetailData.DisplayName;
                this.Title = string.Format("Badge: {0}", badgeTitle.Text);
                this.MetaDescription = string.Format("All about the {0} badge - {1}",
                                                     badgeTitle.Text,
                                                     GetResourceString("system-name"));
                badgeImage.ImageUrl = badgeDetailData.ImageUrl;
                badgeImage.AlternateText = badgeDetailData.AlternateText;

                if (!string.IsNullOrEmpty(badgeDetailData.DateEarned))
                {
                    badgeEarnWhen.Text = string.Format("<p><strong>You earned this badge on {0}!</strong></p>",
                        badgeDetailData.DateEarned);
                    badgeEarnWhen.Visible = true;
                }
                else
                {
                    badgeEarnWhen.Visible = false;
                }

                badgeDetails.Visible = true;


                if (badgeDetailData.HowToEarn.Length > 0)
                {
                    badgeDesriptionLabel.Visible = true;
                    badgeDesriptionLabel.Text = this.Server.HtmlDecode(badgeDetailData.Description);
                }
                else
                {
                    badgeDesriptionLabel.Visible = false;
                }

                if (!badgeDetailData.HideDefaultDescription)
                {
                    badgeEarnPanel.Visible = true;

                    StringBuilder sb = new StringBuilder();
                    foreach (var line in badgeDetailData.HowToEarn)
                    {
                        sb.AppendFormat("<li>{0}</li>", line);
                    }
                    badgeEarnLabel.Text = sb.ToString();
                }
                else
                {
                    badgeEarnPanel.Visible = false;
                }

                /* metadata */
                string systemName = GetResourceString("system-name");
                var fbDescription = StringResources.getStringOrNull("facebook-description");
                var hashtags = StringResources.getStringOrNull("socialmedia-hashtags");

                string title = string.Format("{0} badge: {1}",
                    systemName,
                    badgeDetailData.DisplayName);
                string description = null;
                string twitDescrip = null;

                if (badgeDetailData.Earned)
                {
                    description = string.Format("By participating in {0} I earned this badge: {1}!",
                        systemName,
                        badgeDetailData.DisplayName);
                    twitDescrip = string.Format("I earned this {0} badge: {1}!",
                        systemName,
                        badgeDetailData.DisplayName);
                    if (twitDescrip.Length > 118)
                    {
                        // if it's longer than this it won't fit with the url, shorten it
                        twitDescrip = string.Format("I earned this badge: {0}!",
                            badgeDetailData.DisplayName);
                    }
                }
                else
                {
                    description = string.Format("By participating in {0} you can earn this badge: {1}!",
                        systemName,
                        badgeDetailData.DisplayName);
                    twitDescrip = string.Format("Check out this {0} badge: {1}!",
                        systemName,
                        badgeDetailData.DisplayName);
                    if (twitDescrip.Length > 118)
                    {
                        // if it's longer than this it won't fit with the url, shorten it
                        twitDescrip = string.Format("Check out this badge: {0}!",
                            badgeDetailData.DisplayName);
                    }
                }

                var wt = new WebTools();
                var baseUrl = WebTools.GetBaseUrl(Request);
                var badgeDetailsUrl = string.Format("{0}/Badges/Details.aspx?BadgeId={1}",
                    baseUrl,
                    badgeDetailData.BadgeId);
                var badgeImagePath = string.Format("{0}{1}", baseUrl,
                    VirtualPathUtility.ToAbsolute(badgeDetailData.ImageUrl));

                wt.AddOgMetadata(Metadata,
                    title,
                    wt.BuildFacebookDescription(description, hashtags, fbDescription),
                    badgeImagePath,
                    badgeDetailsUrl,
                    GetResourceString("facebook-appid"));

                wt.AddTwitterMetadata(Metadata,
                    title,
                    twitDescrip,
                    badgeImagePath,
                    twitterUsername: GetResourceString("twitter-username"));

                TwitterShare.NavigateUrl = wt.GetTwitterLink(twitDescrip,
                    Server.UrlEncode(badgeDetailsUrl),
                    hashtags);

                FacebookShare.NavigateUrl = wt.GetFacebookLink(Server.UrlEncode(badgeDetailsUrl));

                if (!badgeDetailData.Hidden)
                {
                    TwitterShare.Visible = true;
                    FacebookShare.Visible = true;
                }
                // end social
                badgeDetails.Visible = true;
            }
            catch (Exception)
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
