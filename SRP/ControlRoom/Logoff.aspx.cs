using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom
{
    public partial class Logoff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SRPUser u = (SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()];
                if (u != null) u.Logoff();
            }
            finally
            {
                Response.Redirect("~/ControlRoom/Login.aspx");
            }
        }
    }
}