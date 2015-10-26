using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Controls
{
    public partial class ActivityHistCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["ActHistPID"]) && (Session["ActHistPID"] == null || Session["ActHistPID"].ToString() == ""))
                {
                    Response.Redirect("~/Account/");
                }

                lblPID.Text = Session["ActHistPID"].ToString();

                var LoggedInPatron = (Patron) Session["Patron"];

                if (LoggedInPatron.IsMasterAccount)
                {
                    pnlFilter.Visible = true;

                    var ds = Patron.GetSubAccountList(LoggedInPatron.PID);
                    PID.Items.Add(new ListItem(FormatName(LoggedInPatron.FirstName, LoggedInPatron.LastName, LoggedInPatron.Username), LoggedInPatron.PID.ToString()));
                    for (int i = 0; i <ds.Tables[0].Rows.Count; i++)
                    {
                        PID.Items.Add(new ListItem(FormatName(ds.Tables[0].Rows[i]["FirstName"].ToString(), 
                                                              ds.Tables[0].Rows[i]["LastName"].ToString(),
                                                              ds.Tables[0].Rows[i]["Username"].ToString()), 
                                                   ds.Tables[0].Rows[i]["PID"].ToString()));
                    }
                    PID.SelectedValue = lblPID.Text;
                }
              
                //var patron = Patron.FetchObject(int.Parse(lblPID.Text));

                PopulateList();
            }
        }

        public string FormatName(string first, string last, string username)
        {
            if ((first + " " + last).Trim().Length == 0) return username;
            return (first + " " + last).Trim() + "(" + username + ")";
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            lblPID.Text = PID.SelectedValue;
            PopulateList();
        }
        
        public void PopulateList()
        {
            int pid = int.Parse(lblPID.Text);
            var ds = PatronPoints.GetAll(pid);

            rptr.DataSource = ds;
            rptr.DataBind();

        }

        public string FormatReading(string author, string title, string review, int PRID)
        {
            var ret = "";

            if (author != "" && title != "")
            {
                ret = string.Format("<b>{0}</b> by <i>{1}</i>", title, author);
            }
            if (author != "" && title == "")
            {
                ret = string.Format("a book by <i>{0}</i>", author);
            }
            if (author == "" && title != "")
            {
                ret = string.Format("<b>{0}</b>", title);
            }
            if (review.Trim() != "")
            {
                ret = string.Format("{0}<br/>{1}", ret, review);


                if ( SRPSettings.GetSettingValue("FBReviewOn").SafeToBool())
                {
                    var fbButton = string.Format("<div class=\"fb-share-button\" data-href='{0}://{1}{2}/ShareReview.aspx?ID={3}' data-type=\"button\"></div>",
                                Request.Url.Scheme, Request.Url.Authority, Request.ApplicationPath.TrimEnd('/'), PRID);

                    ret = string.Format("{0}<br/>{1}", ret, fbButton);
                }


            }
            return ret;
        }
    }
}