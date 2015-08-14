using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP
{
    public partial class MyLogEntry : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IsSecure = true;
            if (!IsPostBack) TranslateStrings(this);

        }
    }
}

