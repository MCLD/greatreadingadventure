using System;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace SRP {
    public partial class _Default : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            if(!String.IsNullOrEmpty(Request["PID"])) {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if(!IsPostBack) {

                if(Session["ProgramID"] == null) {
                    try {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    } catch {
                        Response.Redirect("~/ControlRoom/Setup.aspx");
                    }
                    // pgmID.Text = Session["ProgramID"].ToString();
                } else {
                    //pgmID.Text = Session["ProgramID"].ToString();
                }


            }
            TranslateStrings(this);
        }

        protected void Button1_Click(object sender, EventArgs e) {
            //Session["ProgramID"] = pgmID.Text;
            //TranslateStrings(this);
            Response.Redirect("/");
        }
    }
}
