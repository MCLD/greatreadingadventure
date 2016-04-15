using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Avatar
{
    public partial class View : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer == null)
            {
                AvatarBackLink.NavigateUrl = "~/";
            }
            else
            {
                AvatarBackLink.NavigateUrl = Request.UrlReferrer.AbsolutePath;
            }

            SystemName.Text = SystemNamePrint.Text = StringResources.getString("system-name");
            SystemSlogan.Text = StringResources.getString("slogan");
            SystemSlogan.Visible = SystemSlogan.Text != "slogan";
            string programId = null;

            AvatarBackLink.Text = StringResources.getString("avatar-return");
            if (AvatarBackLink.Text == "avatar-return")
            {
                AvatarBackLink.Text = "Back";
            }

            var patron = Session[SessionKey.Patron] as DAL.Patron;
            if (patron != null)
            {
                programId = patron.ProgID.ToString();
                MyAvatarPrint.Text = Tools.DisplayHelper.FormatName(patron.FirstName,
                    patron.LastName,
                    patron.Username);
            }
            else
            {
                MyAvatarPrint.Text = "My Avatar";
            }

            if (string.IsNullOrEmpty(programId))
            {
                var sessionProgId = Session["ProgramId"];
                if (sessionProgId != null)
                {
                    programId = sessionProgId.ToString();
                }
            }

            if (string.IsNullOrEmpty(programId))
            {
                try
                {
                    programId = DAL.Programs.GetDefaultProgramID().ToString();
                }
                catch (Exception) { }
            }

            string bannerPath = new Logic.Banner().FullMetadataBannerPath(
                WebTools.GetBaseUrl(Request),
                programId,
                Server);

            if (!string.IsNullOrEmpty(bannerPath))
            {
                BannerImagePrint.ImageUrl = bannerPath;
            }
            else
            {
                BannerImagePrint.Visible = false;
            }

            string avatarPath = null;
            bool validAvatar = false;
            string avatarId = Request.QueryString["AvatarId"];
            if (!string.IsNullOrEmpty(avatarId) && avatarId.Length <= 24)
            {
                char[] avatarIdArray = avatarId.ToCharArray();

                avatarIdArray = Array.FindAll<char>(avatarIdArray, (c => (char.IsLetterOrDigit(c))));
                avatarId = new string(avatarIdArray);

                avatarPath = string.Format("~/Images/AvatarCache/{0}.png", avatarId);
                if (File.Exists(Server.MapPath(avatarPath)))
                {
                    validAvatar = true;
                    AvatarImage.ImageUrl = AvatarImagePrint.ImageUrl = avatarPath;
                    AvatarPanel.Visible = true;
                }
            }

            if (!validAvatar)
            {
                AvatarPanel.Visible = false;
            }
            else
            {
                var title = string.Format("{0} avatar", SystemName.Text);
                var description = string.Format("Check out this awesome avatar in {0}!", SystemName.Text);
                var wt = new WebTools();
                var baseUrl = WebTools.GetBaseUrl(Request);
                var avatarDetailPath = string.Format("{0}/Avatar/View.aspx?AvatarId={1}",
                    baseUrl,
                    avatarId);
                var fullAvatarPath = string.Format("{0}{1}",
                    baseUrl,
                    VirtualPathUtility.ToAbsolute(avatarPath));

                if (patron != null)
                {
                    title = string.Format("My {0} avatar", SystemName.Text);
                    description = string.Format("Check out my awesome avatar in {0}!",
                        SystemName.Text);
                }

                // open graph & facebook

                wt.AddOgMetadata(Metadata,
                    title,
                    description,
                    fullAvatarPath,
                    avatarDetailPath,
                    facebookApp: StringResources.getString("facebook-appid"));

                wt.AddTwitterMetadata(Metadata,
                    title,
                    description,
                    fullAvatarPath,
                    "summary_large_image",
                    StringResources.getString("twitter-username"));

                string twitterHashtags = StringResources.getString("twitter-hashtags");
                if (!string.IsNullOrEmpty(twitterHashtags) && twitterHashtags
                    != "twitter-hashtags")
                {
                    TwitterShare.NavigateUrl = string.Format("http://twitter.com/share?text={0}&url={1}&hashtags={2}",
                        description,
                        Server.UrlEncode(avatarDetailPath),
                        twitterHashtags);
                }
                else
                {
                    TwitterShare.NavigateUrl = string.Format("http://twitter.com/share?text={0}&url={1}",
                        description,
                        Server.UrlEncode(avatarDetailPath));
                }
                TwitterShare.Visible = true;
                FacebookShare.NavigateUrl = string.Format("http://www.facebook.com/sharer.php?u={0}",
                    Server.UrlEncode(avatarDetailPath));
                FacebookShare.Visible = true;
            }

            AvatarPrintPanel.Visible = AvatarPanel.Visible;
            AvatarAlert.Visible = !AvatarPanel.Visible;
        }
    }
}