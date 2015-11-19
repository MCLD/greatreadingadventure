using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Text;
using System.Data;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class ActivityHistCtl : System.Web.UI.UserControl {

        public string FilterButtonText { get; set; }
        protected void Page_Load(object sender, EventArgs e) {
            this.FilterButtonText = allActivitiesFilter.Text;
            if(!IsPostBack) {
                if(string.IsNullOrEmpty(Request["ActHistPID"]) && (Session["ActHistPID"] == null || Session["ActHistPID"].ToString() == "")) {
                    Response.Redirect("~/Account/");
                }

                lblPID.Text = Session["ActHistPID"].ToString();

                var LoggedInPatron = (Patron)Session["Patron"];

                if(LoggedInPatron.IsMasterAccount) {
                    pnlFilter.Visible = true;

                    var ds = Patron.GetSubAccountList(LoggedInPatron.PID);
                    PID.Items.Add(new ListItem(FormatName(LoggedInPatron.FirstName, LoggedInPatron.LastName, LoggedInPatron.Username), LoggedInPatron.PID.ToString()));
                    for(int i = 0; i < ds.Tables[0].Rows.Count; i++) {
                        PID.Items.Add(new ListItem(FormatName(ds.Tables[0].Rows[i]["FirstName"].ToString(),
                                                              ds.Tables[0].Rows[i]["LastName"].ToString(),
                                                              ds.Tables[0].Rows[i]["Username"].ToString()),
                                                   ds.Tables[0].Rows[i]["PID"].ToString()));
                    }
                    PID.SelectedValue = lblPID.Text;
                }

                //var patron = Patron.FetchObject(int.Parse(lblPID.Text));

                PopulateList();
                AdventureDropDownItem.Visible = Session[SessionKey.AdventuresActive] as bool? == true;
                ChallengeDropDownItem.Visible = Session[SessionKey.ChallengesActive] as bool? == true;
                EventsDropDownItem.Visible = Session[SessionKey.EventsActive] as bool? == true;

            }
        }

        public string FormatName(string first, string last, string username) {
            if(string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(last)
               || string.IsNullOrWhiteSpace(first)) {
                return username;
            }

            StringBuilder formattedName = new StringBuilder(first.Trim());

            if(!string.IsNullOrWhiteSpace(last)) {
                formattedName.AppendFormat(" {0}", last.Trim());
            }
            if(!string.IsNullOrWhiteSpace(username)) {
                formattedName.AppendFormat(" ({0})", username);
            }
            return formattedName.ToString();
        }

        protected string AwardClass(object awardReasonCd) {
            int? awardReasonCdValue = awardReasonCd as int?;
            if(awardReasonCdValue != null) {
                switch((int)awardReasonCdValue) {
                    case (int)PointAwardReason.Reading:
                        return "award-reading";
                    case (int)PointAwardReason.EventAttendance:
                        return "award-event";
                    case (int)PointAwardReason.BookListCompletion:
                        return "award-challenge";
                    case (int)PointAwardReason.GameCompletion:
                    case (int)PointAwardReason.MiniGameCompletion:
                        return "award-adventure";
                }
            }
            return string.Empty;
        }

        protected void FilterActivitiesClick(object sender, EventArgs e) {
            var senderButton = sender as LinkButton;
            if(senderButton != null) {
                string filterIds = null;
                switch(senderButton.Text.ToLower()) {
                    case "reading":
                        filterIds = (string.Format("'{0}'",
                                                   (int)PointAwardReason.Reading));
                        break;
                    case "adventures":
                        filterIds = string.Format("'{0}','{1}'",
                                                  (int)PointAwardReason.GameCompletion,
                                                  (int)PointAwardReason.MiniGameCompletion);
                        break;
                    case "challenges":
                        filterIds = (string.Format("'{0}'",
                                                   (int)PointAwardReason.BookListCompletion));
                        break;
                    case "events":
                        filterIds = (string.Format("'{0}'",
                                                   (int)PointAwardReason.EventAttendance));
                        break;
                }
                this.FilterButtonText = senderButton.Text;
                PopulateList(filterIds);
            }
        }

        protected void Ddl_SelectedIndexChanged(object sender, EventArgs e) {
            lblPID.Text = PID.SelectedValue;
            PopulateList();
        }

        public void PopulateList() {
            PopulateList(null);
        }

        public void PopulateList(string filterIds) {
            int pid = int.Parse(lblPID.Text);
            var ds = PatronPoints.GetAll(pid);
            int recordCount = 0;
            if(!string.IsNullOrEmpty(filterIds)) {
                DataView dv = new DataView(ds.Tables[0]);
                dv.RowFilter = string.Format("AwardReasonCd IN ({0})", filterIds);
                rptr.DataSource = dv;
                recordCount = dv.Count;
            } else {
                rptr.DataSource = ds;
                recordCount = ds.Tables[0].Rows.Count;
            }

            rptr.DataBind();
            
            if(recordCount == 0) {
                activitiesPanel.Visible = false;
                noActivitiesLabel.Visible = true;
            } else {
                activitiesPanel.Visible = true;
                noActivitiesLabel.Visible = false;
            }
        }

        public string FormatReading(string author, string title, string review, int PRID) {
            if(string.IsNullOrWhiteSpace(author) && string.IsNullOrWhiteSpace(title)) {
                return string.Empty;
            }

            StringBuilder response = new StringBuilder(" ");

            if(string.IsNullOrWhiteSpace(title)) {
                response.Append("a book");
            } else {
                response.AppendFormat("<strong>{0}</strong>", title);
            }

            if(!string.IsNullOrWhiteSpace(author)) {
                response.AppendFormat(" by <em>{0}</em>", author);
            }

            if(!string.IsNullOrWhiteSpace(review)) {
                response.AppendFormat(" ({0})", review);
            }
            return response.ToString();
        }
    }
}