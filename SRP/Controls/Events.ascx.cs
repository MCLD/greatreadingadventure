using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Text;

namespace GRA.SRP.Controls {
    public partial class Events : System.Web.UI.UserControl {
        public string FirstAvailableDate { get; set; }
        public string LastAvailableDate { get; set; }
        public string NoneAvailableText { get; set; }
        protected string DisplayEventDateTime(DateTime? eventDate,
                                              string eventTime,
                                              DateTime? endDate,
                                              string endTime) {
            DateTime nonNullEndDate;
            if(endDate == null) {
                nonNullEndDate = DateTime.MinValue;
            } else {
                nonNullEndDate = (DateTime)endDate;
            }
            return Event.DisplayEventDateTime(new Event {
                EventDate = (DateTime)eventDate,
                EventTime = eventTime,
                EndDate = nonNullEndDate,
                EndTime = endTime
            });
        }

        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                GetFilterSessionValues();
                GetData();
            }

            int programId;
            var sessionProgramId = Session["ProgramID"];
            if(sessionProgramId == null
               || !int.TryParse(sessionProgramId.ToString(), out programId)) {
                programId = Programs.GetDefaultProgramID();

            }
            var program = Programs.FetchObject(programId);

            this.FirstAvailableDate = program.StartDate.ToShortDateString();
            this.LastAvailableDate = program.EndDate.ToShortDateString();

            var basePage = (BaseSRPPage)Page;
            this.NoneAvailableText = basePage.GetResourceString("events-none-available");
        }

        protected void btnFilter_Click(object sender, EventArgs e) {
            StringBuilder sb = new StringBuilder();
            if(!string.IsNullOrEmpty(StartDate.Text)) {
                sb.Append("Start Date: ");
                sb.Append("<strong>");
                sb.Append(StartDate.Text);
                sb.Append("</strong>");
            }
            if(!string.IsNullOrEmpty(EndDate.Text)) {
                if(sb.Length > 0) {
                    sb.Append(" / ");
                }
                sb.Append("End date: ");
                sb.Append("<strong>");
                sb.Append(EndDate.Text);
                sb.Append("</strong>");
            }
            if(!string.IsNullOrEmpty(BranchId.SelectedItem.Text)) {
                if(sb.Length > 0) {
                    sb.Append(" / ");
                }
                sb.Append("Branch/library: ");
                sb.Append("<strong>");
                sb.Append(BranchId.SelectedItem.Text);
                sb.Append("</strong>");
            }
            whatsShowing.Text = sb.ToString();
            whatsShowing.Visible = !string.IsNullOrEmpty(whatsShowing.Text);
            SetFilterSessionValues();
            GetData();
        }

        protected void btnClear_Click(object sender, EventArgs e) {
            StartDate.Text = EndDate.Text = whatsShowing.Text= string.Empty;
            whatsShowing.Visible = false;
            BranchId.SelectedValue = "0";
            SetFilterSessionValues();
            GetData();
        }

        public void GetData() {
            var ds = DAL.Event.GetUpcomingDisplay(Session["UEL_Start"].ToString(),
                                                  Session["UEL_End"].ToString(),
                                                  int.Parse(Session["UEL_Branch"].ToString()));
            rptr.DataSource = ds;
            rptr.DataBind();
        }
        public void SetFilterSessionValues() {
            Session["UEL_Start"] = StartDate.Text;
            Session["UEL_End"] = EndDate.Text;
            Session["UEL_Branch"] = BranchId.SelectedValue;
            Session["UEL_Filtered"] = "1";
        }

        public void GetFilterSessionValues() {
            if(Session["UEL_Start"] != null) {
                StartDate.Text = Session["UEL_Start"].ToString();
            } else {
                Session["UEL_Start"]= string.Empty;
            }
            if(Session["UEL_End"] != null) {
                EndDate.Text = Session["UEL_End"].ToString();
            } else {
                Session["UEL_End"]= string.Empty;
            }
            if(Session["UEL_Branch"] != null) {
                try { BranchId.SelectedValue = Session["UEL_Branch"].ToString(); } catch(Exception) { }
            } else {
                Session["UEL_Branch"] = 0;
            }
        }

        public bool WasFiltered() {
            return (Session["UEL_Filtered"] != null);
        }



        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {
        }

        protected void btnList_Click(object sender, EventArgs e) {
            Response.Redirect("~/Events/");
        }
    }
}