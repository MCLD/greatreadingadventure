using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Classes
{
    public partial class ProgramCSS : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string CSSFile
        {
            get
            {
                
                if (Session["ProgramID"] == null ||  Session["ProgramID"].ToString() == "")
                {

                    return "<link rel=\"stylesheet\" type=\"text/css\" href=\"/CSS/Program/Default.css\">";
                }
                else
                {
                    return "<link rel=\"stylesheet\" type=\"text/css\" href=\"/CSS/Program/" + Session["ProgramID"].ToString()  + ".css\">";
                }
            }
        }
    }
}