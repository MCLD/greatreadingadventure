using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;

namespace GRA.SRP
{
    public partial class Recover : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) TranslateStrings(this);

            this.MetaDescription = string.Format("Reset your password - {0}",
                                                 GetResourceString("system-name"));
        }
    }
}