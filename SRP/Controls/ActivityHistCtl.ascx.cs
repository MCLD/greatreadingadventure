using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class ActivityHistCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["ActHistPID"]) && (Session["ActHistPID"] == null || Session["ActHistPID"].ToString() == ""))
                {
                    Response.Redirect("~/MyAccount.aspx");
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


    }
}