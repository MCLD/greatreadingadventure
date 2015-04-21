using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;

namespace STG.SRP.Classes
{
    public partial class PublicProgramView : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Programs p = new Programs();
                p = Programs.FetchObject(Session["ProgramID"].ToString().SafeToInt());
                Session["Program"] = p;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Register.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx");
        }


    }
}