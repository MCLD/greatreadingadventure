using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class PatronNotificationsCtl : System.Web.UI.UserControl {
        public string SubjectHasError { get; set; }
        public string BodyHasError { get; set; }

        public bool UserHasMessages { get; set; }

        public string SuccessIfTrue(object isTrue) {
            var boolIsTrue = isTrue as bool?;
            if(boolIsTrue != null && boolIsTrue == true) {
                return "class=\"success\"";
            }
            return string.Empty;
        }
        protected void Page_Load(object sender, EventArgs e) {

            if(!IsPostBack) {
                var ds = DAL.Notifications.GetAllToPatron(((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();

                this.UserHasMessages = ds.Tables[0].Rows.Count > 0;
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {
            pnlList.Visible = false;
            lblFrom.Text = ((BaseSRPPage)this.Page).GetResourceString("notifications-from");

            var o = new Notifications();
            o.Fetch(int.Parse(e.CommandArgument.ToString()));

            lblTitle.Text = o.Subject;
            lblBody.Text = Server.HtmlDecode(o.Body);
            NID.Text = o.NID.ToString();
            lblReceived.Text = o.AddedDate.ToString();

            o.isUnread = false;
            o.Update();

            pnlDetail.Visible = true;
        }

        protected void btnList_Click(object sender, EventArgs e) {
            Response.Redirect("~/Mail/");
        }

        protected void btnDelete_Click(object sender, EventArgs e) {
            if(!string.IsNullOrEmpty(NID.Text)) {
                this.Log().Debug("Deleting message: {0}", NID.Text);
                int nid;
                if(int.TryParse(NID.Text, out nid)) {
                    Notifications notification = null;
                    try {
                        notification = Notifications.FetchObject(nid);
                        if(notification != null) {
                            notification.Delete();
                        }
                    } catch (Exception ex) {
                        this.Log().Error("Unable to delete message, nid text = {0}, nid = {1}, notification = {2}: {3} - {4}",
                                         NID.Text,
                                         nid,
                                         notification,
                                         ex.Message,
                                         ex.StackTrace);
                    }
                } else {
                    this.Log().Error("Unable to convert message ID to number: {0}", NID.Text);
                }
            }
            Response.Redirect("~/Mail/");
        }

        protected void btnAsk_Click(object sender, EventArgs e) {
            pnlList.Visible = pnlDetail.Visible = false;
            pnlAsk.Visible = true;
        }

        //protected void btnUnread_Click(object sender, EventArgs e)
        //{
        //    var o = Notifications.FetchObject(int.Parse(NID.Text));
        //    o.isUnread = true;
        //    o.Update();
        //    Response.Redirect("~/Mail/");
        //}

        protected void btnAskSubmit_Click(object sender, EventArgs e) {
            bool somethingHasError = false;
            if(string.IsNullOrWhiteSpace(txtSubject.Text)) {
                somethingHasError = true;
                this.SubjectHasError = "has-error";
            }

            if(string.IsNullOrWhiteSpace(txtBody.Text)) {
                somethingHasError = true;
                this.BodyHasError = "has-error";
            }

            if(!somethingHasError) {
                var o = new Notifications();
                o.PID_From = ((Patron)Session["Patron"]).PID;
                o.PID_To = 0;
                o.isQuestion = true;
                o.Subject = txtSubject.Text;
                o.Body = txtBody.Text;
                o.AddedDate = o.LastModDate = DateTime.Now;
                o.LastModUser = o.AddedUser = ((Patron)Session["Patron"]).Username;
                o.Insert();
                txtSubject.Text = txtBody.Text= string.Empty;

                pnlAsk.Visible = false;
                pnlList.Visible = true;
                new SessionTools(Session).AlertPatron(((BaseSRPPage)this.Page).GetResourceString("notifications-message-sent"),
                                                      glyphicon: "send");
                Response.Redirect("~/Mail/");
            }
        }
    }
}