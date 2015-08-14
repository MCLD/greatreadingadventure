using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Classes
{
    public partial class ProgramTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        protected void rpt_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Session["ProgramID"] = e.CommandArgument.ToString();
            Response.Redirect("~/");
        }
    }
}