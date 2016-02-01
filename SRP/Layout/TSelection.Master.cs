using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP.Layout {
    public partial class TSelection : System.Web.UI.MasterPage {
        public string SystemNameText {
            get {
                return StringResources.getString("system-name");
            }
        }

        public string SloganText {
            get {
                return StringResources.getString("slogan");
            }
        }

        public string UpsellText {
            get {
                return StringResources.getString("upsell");
            }
        }

        public string CopyrightStatementText {
            get {
                return StringResources.getString("footer-copyright");
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            var cssFile = "<link rel=\"stylesheet\" type=\"text/css\" href=\"CSS/Program/Default.css\">";
            if(((Select)Page).DefPID != null) {
                cssFile = "<link rel=\"stylesheet\" type=\"text/css\" href=\"CSS/Program/" + ((Select)Page).DefPID + ".css\">";
            }
            var plc = FindControl("ProgramCSS");
            Control ctl = new LiteralControl(cssFile);
            plc.Controls.Add(ctl);
        }
    }
}