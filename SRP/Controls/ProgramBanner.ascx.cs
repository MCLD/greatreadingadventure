using System;
using System.Web;
using System.IO;
namespace GRA.SRP.Classes {
    public partial class ProgramBanner : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                var banner = "~/images/meadow.jpg";
                var banner2x = "~/images/meadow@2x.jpg";
                if(Session["ProgramID"] != null) {
                    var programBanner = string.Format("~/images/Banners/{0}.png",
                                                      Session["ProgramID"]);
                    var programBanner2x = string.Format("~/images/Banners/{0}@2x.png",
                                                        Session["ProgramID"]);
                    if(File.Exists(Server.MapPath(programBanner))) {
                        banner = programBanner;
                        banner2x = programBanner2x;
                    } else {
                        programBanner = string.Format("~/images/Banners/{0}.jpg",
                                                      Session["ProgramID"]);
                        programBanner2x = string.Format("~/images/Banners/{0}@2x.jpg",
                                                        Session["ProgramID"]);
                        if(File.Exists(Server.MapPath(programBanner))) {
                            banner = programBanner;
                            banner2x = programBanner2x;
                        }
                    }
                }
                pgmBanner.Visible = true;
                pgmBanner.Src = banner;
                if(File.Exists(Server.MapPath(banner2x))) {
                    pgmBanner.Attributes.Add("srcset",
                                             string.Format("{0} 1x, {1} 2x",
                                                           VirtualPathUtility.ToAbsolute(banner),
                                                           VirtualPathUtility.ToAbsolute(banner2x)));
                }


            }
        }

    }
}