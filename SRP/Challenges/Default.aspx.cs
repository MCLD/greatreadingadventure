using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Challenges {
    public partial class Default : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            IsSecure = true;
            if(!IsPostBack)
                TranslateStrings(this);

        }
    }
}