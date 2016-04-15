using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.Logic
{
    public class Banner
    {
        private const string DefaultBanner = "~/images/meadow.jpg";
        private const string DefaultBanner2x = "~/images/meadow@2x.jpg";
        public Tuple<string, string> GetBannerPath(string programId, HttpServerUtility Server)
        {
            var banner = DefaultBanner;
            var banner2x = DefaultBanner2x;
            if (!string.IsNullOrEmpty(programId))
            {
                var programBanner = string.Format("~/images/Banners/{0}.png",
                                                  programId);
                var programBanner2x = string.Format("~/images/Banners/{0}@2x.png",
                                                    programId);
                if (File.Exists(Server.MapPath(programBanner)))
                {
                    banner = programBanner;
                    if (File.Exists(Server.MapPath(programBanner2x)))
                    {
                        banner2x = programBanner2x;
                    }
                    else
                    {
                        banner2x = null;
                    }
                }
                else
                {
                    programBanner = string.Format("~/images/Banners/{0}.jpg",
                                                  programId);
                    programBanner2x = string.Format("~/images/Banners/{0}@2x.jpg",
                                                    programId);
                    if (File.Exists(Server.MapPath(programBanner)))
                    {
                        banner = programBanner;
                        if (File.Exists(Server.MapPath(programBanner2x)))
                        {
                            banner2x = programBanner2x;
                        }
                        else
                        {
                            banner2x = null;
                        }
                    }
                }
            }
            return new Tuple<string, string>(banner, banner2x);
        }

        public string FullMetadataBannerPath(string baseUrl,
            HttpSessionState Session,
            HttpServerUtility Server)
        {
            string programId = Session["ProgramId"] != null
                ? Session["ProgramId"].ToString()
                : null;

            Tuple<string, string> bannerPaths = GetBannerPath(programId, Server);

            string bannerImage = !string.IsNullOrEmpty(bannerPaths.Item2)
                ? VirtualPathUtility.ToAbsolute(bannerPaths.Item2)
                : VirtualPathUtility.ToAbsolute(bannerPaths.Item1);
            return string.Format("{0}{1}", baseUrl, bannerImage);
        }

        public string FullMetadataBannerPath(string baseUrl,
            string programId,
            HttpServerUtility Server)
        {
            Tuple<string, string> bannerPaths = GetBannerPath(programId, Server);

            string bannerImage = !string.IsNullOrEmpty(bannerPaths.Item2)
                ? VirtualPathUtility.ToAbsolute(bannerPaths.Item2)
                : VirtualPathUtility.ToAbsolute(bannerPaths.Item1);
            return string.Format("{0}{1}", baseUrl, bannerImage);
        }

    }
}
