using System;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP {
    public partial class EventsPage : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(Request["PID"])) {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if(!IsPostBack) {
                if(Session["ProgramID"] == null) {
                    try {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    } catch {
                        Response.Redirect("~/Default.aspx");
                    }
                }
            }
            TranslateStrings(this);
            this.MetaDescription = string.Format("Plan to attend exciting upcoming events! - {0}",
                                                 GetResourceString("system-name"));
        }
    }
}