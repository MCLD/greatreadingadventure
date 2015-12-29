using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP {
    public partial class BadgeGallery : BaseSRPPage {
        public string NoneAvailableText { get; set; }

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
                        Response.Redirect("~/Default.aspx");
                    }
                }
            }
            TranslateStrings(this);
            this.NoneAvailableText = GetResourceString("badges-none-available");
            myBadgesButton.Visible = this.IsLoggedIn;
            this.MetaDescription = string.Format("See the available badges and how to earn them! - {0}",
                                                 GetResourceString("system-name"));
        }

        protected void btnFilter_Click(object sender, EventArgs e) {
            odsBadges.DataBind();
            rptr.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e) {
            CategoryId.ClearSelection();
            AgeGroupId.ClearSelection();
            BranchId.ClearSelection();
            LocationID.ClearSelection();
            odsBadges.DataBind();
            rptr.DataBind();
        }

        protected void dd_DataBound(object sender, EventArgs e) {
            var dd = sender as DropDownList;
            if(dd.Items.Count < 2)
                dd.Visible = false;
        }
    }
}