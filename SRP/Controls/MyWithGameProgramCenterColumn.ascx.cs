using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class MyWithGameProgramCenterColumn : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PatronLoggedIn"] == null || !(bool)Session["PatronLoggedIn"]) Response.Redirect("~/");

            var patron = (Patron)(Session["Patron"]);
            var pgm = DAL.Programs.FetchObject(int.Parse(Session["PatronProgramID"].ToString()));

            imgAvatar.ImageUrl = "~/images/Avatars/" + patron.AvatarID + ".png";
            lblSponsor.Text = pgm.HTML2;    //sponsor
            lblFooter.Text = pgm.HTML5;     //footer
        }
    }
}