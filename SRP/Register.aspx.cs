using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.DAL;

namespace STG.SRP
{
    public partial class Register : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) TranslateStrings(this);
        }
    }
}