using System;
using System.Web;
using System.IO;
namespace GRA.SRP.Classes {
    public partial class ProgramBanner : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                var banner = new Logic.Banner();
                string programId = Session["ProgramId"] != null
                    ? Session["ProgramId"].ToString()
                    : null;
                Tuple<string,string> bannerPaths = banner.GetBannerPath(programId, Server);
                pgmBanner.Visible = true;
                pgmBanner.Src = bannerPaths.Item1;
                if(!string.IsNullOrEmpty(bannerPaths.Item2)) {
                    pgmBanner.Attributes.Add("srcset",
                                             string.Format("{0} 1x, {1} 2x",
                                                           VirtualPathUtility.ToAbsolute(bannerPaths.Item1),
                                                           VirtualPathUtility.ToAbsolute(bannerPaths.Item2)));
                }
            }
        }

    }
}