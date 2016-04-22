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

                var systemName = GetResourceString("system-name");
                var description = GetResourceString("frontpage-description");
                var wt = new WebTools();
                var baseUrl = WebTools.GetBaseUrl(Request);
                var bannerPath = new GRA.Logic.Banner().FullMetadataBannerPath(baseUrl,
                    Session,
                    Server);

                // open graph & facebook

                wt.AddOgMetadata(Metadata,
                    systemName,
                    description,
                    bannerPath,
                    baseUrl,
                    facebookApp: GetResourceString("facebook-appid"));

                // dublin core
                Metadata.Controls.Add(new HtmlMeta { Name = "DC.Title", Content = systemName });
                Metadata.Controls.Add(new HtmlMeta
                {
                    Name = "DC.Description",
                    Content = description
                });
                Metadata.Controls.Add(new HtmlMeta { Name = "DC.Source", Content = baseUrl });
                Metadata.Controls.Add(new HtmlMeta
                {
                    Name = "DC.Type",
                    Content = "InteractiveResource"
                });

                //twitter
                wt.AddTwitterMetadata(Metadata,
                    systemName,
                    GetResourceString("twitter-description"),
                    bannerPath,
                    "summary_large_image",
                    GetResourceString("twitter-username"));
            }
            TranslateStrings(this);
        }
    }
}
