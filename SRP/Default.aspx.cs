using System;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP;
using GRA.Tools;
using System.Web.UI.HtmlControls;
using System.Web;

namespace SRP
{
    public partial class _Default : BaseSRPPage
    {
        public string SystemName { get; set; }
        public string ImageType { get; set; }
        public string ImageUrl { get; set; }
        public string CanonicalUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (((BaseSRPPage)Page).IsLoggedIn)
            {
                Server.Transfer("~/Dashboard.aspx");
            }
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
                        Response.Redirect("~/ControlRoom/Configure.aspx");
                    }
                    // pgmID.Text = Session["ProgramID"].ToString();
                }
                else
                {
                    //pgmID.Text = Session["ProgramID"].ToString();
                }

                string systemName = GetResourceString("system-name");
                string description = GetResourceString("frontpage-description");
                var baseUrl = WebTools.GetBaseUrl(Request);
                var banner = new GRA.Logic.Banner();
                string programId = Session["ProgramId"] != null
                    ? Session["ProgramId"].ToString()
                    : null;
                Tuple<string, string> bannerPaths = banner.GetBannerPath(programId, Server);

                string bannerImage = !string.IsNullOrEmpty(bannerPaths.Item2)
                    ? VirtualPathUtility.ToAbsolute(bannerPaths.Item2)
                    : VirtualPathUtility.ToAbsolute(bannerPaths.Item1);
                string bannerPath = string.Format("{0}/{1}", baseUrl, bannerImage);

                var wt = new WebTools();

                // open graph & facebook
                string facebookApp = GetResourceString("facebook-appid");
                if (!string.IsNullOrEmpty(facebookApp) && facebookApp != "facebook-appid")
                {
                    Metadata.Controls.Add(wt.OgMetadataTag("fb:app_id", facebookApp));
                }
                Metadata.Controls.Add(wt.OgMetadataTag("og:title", systemName));
                Metadata.Controls.Add(wt.OgMetadataTag("og:type", "website"));
                Metadata.Controls.Add(wt.OgMetadataTag("og:url", baseUrl));
                Metadata.Controls.Add(wt.OgMetadataTag("og:image", bannerPath));
                Metadata.Controls.Add(wt.OgMetadataTag("og:description", description));

                // dublin core
                Metadata.Controls.Add(new HtmlMeta { Name = "DC.Title", Content = systemName });
                Metadata.Controls.Add(new HtmlMeta { Name = "DC.Description", Content = description });
                Metadata.Controls.Add(new HtmlMeta { Name = "DC.Source", Content = baseUrl });
                Metadata.Controls.Add(new HtmlMeta { Name = "DC.Type", Content = "InteractiveResource" });

                //twitter
                string twitterUsername = GetResourceString("twitter-username");
                string twitterDescription = GetResourceString("twitter-description");
                Metadata.Controls.Add(new HtmlMeta { Name = "twitter:card", Content = "summary_large_image" });
                if (!string.IsNullOrEmpty(twitterUsername) && twitterUsername != "twitter-username")
                {
                    Metadata.Controls.Add(wt.OgMetadataTag("twitter:site", twitterUsername));
                }
                Metadata.Controls.Add(new HtmlMeta { Name = "twitter:title", Content = systemName });
                if (!string.IsNullOrEmpty(twitterDescription) && twitterDescription != "twitter-description")
                {
                    Metadata.Controls.Add(wt.OgMetadataTag("twitter:description", twitterDescription));
                }
                Metadata.Controls.Add(new HtmlMeta { Name = "twitter:image", Content = bannerPath });
            }
            TranslateStrings(this);
        }
    }
}
