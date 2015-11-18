using System;
using SRPApp.Classes;

namespace GRA.SRP {
    public partial class Register : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack) { 
                if(IsLoggedIn) {
                    Response.Redirect("~/Dashboard.aspx");
                }

                TranslateStrings(this);
            }
        }
    }
}