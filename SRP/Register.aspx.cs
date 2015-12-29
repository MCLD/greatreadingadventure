using System;
using SRPApp.Classes;

namespace GRA.SRP {
    public partial class Register : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack) { 
                if(IsLoggedIn) {
                    Response.Redirect("~");
                }

                TranslateStrings(this);
            }
            this.MetaDescription = string.Format("Register now to join the fun! - {0}",
                                                 GetResourceString("system-name"));
        }
    }
}