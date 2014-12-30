using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
namespace STG.SRP.Classes
{
    public partial class ProgramBanner : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) pgmBanner.Src = GetProgramBanner();
        }

        protected string GetProgramBanner()
        {
            var defaultBanner ="/images/Banners/default.png";
            if (Session["ProgramID"] == null)
            {
                pgmBanner.Visible = true;
                return defaultBanner;
            }
            var img = "/images/Banners/" + Session["ProgramID"] + ".png";
            if (!File.Exists(Server.MapPath("~" + img)))
            {
                pgmBanner.Visible = true;
                return defaultBanner;
            }
            pgmBanner.Visible = true;
            return img;
        }
    }
}