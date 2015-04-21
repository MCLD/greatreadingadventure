using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.DAL;

namespace STG.SRP.Layout
{
    public partial class TSelection : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var cssFile = "<link rel=\"stylesheet\" type=\"text/css\" href=\"CSS/Program/Default.css\">";
            if (((Select)Page).DefPID != null)
            {
                cssFile = "<link rel=\"stylesheet\" type=\"text/css\" href=\"CSS/Program/" + ((Select)Page).DefPID + ".css\">";
            }
            var plc = FindControl("ProgramCSS");
            Control ctl = new LiteralControl(cssFile);
            plc.Controls.Add(ctl);

            if (!IsPostBack)
            {
                lnkRegister.Visible = false;
                lnkLogin.Visible = false;
                lnkLogout.Visible = false;
                n.Visible = b.Visible = v.Visible = o.Visible = a.Visible = p.Visible = f.Visible = false;
                home1.Visible = true;
                home2.Visible = false;
            }
        }
    }
}