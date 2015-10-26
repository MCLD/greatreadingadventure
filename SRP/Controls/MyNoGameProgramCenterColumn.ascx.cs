using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class MyNoGameProgramCenterColumn : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pgm = DAL.Programs.FetchObject(int.Parse(Session["PatronProgramID"].ToString()));
            if(pgm != null) {
                lblSponsor.Text = pgm.HTML2;    //sponsor
                lblFooter.Text = pgm.HTML5;     //footer
            }
        }
    }
}