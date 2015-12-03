using System;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace SRP {
    public partial class _Default : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            if(((BaseSRPPage)Page).IsLoggedIn) {
                Server.Transfer("~/Dashboard.aspx");
            }
            if(!String.IsNullOrEmpty(Request["PID"])) {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if(!IsPostBack) {

                if(Session["ProgramID"] == null) {
                    try {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    } catch {
                        Response.Redirect("~/ControlRoom/Configure.aspx");
                    }
                    // pgmID.Text = Session["ProgramID"].ToString();
                } else {
                    //pgmID.Text = Session["ProgramID"].ToString();
                }


            }
            TranslateStrings(this);
        }
    }
}
