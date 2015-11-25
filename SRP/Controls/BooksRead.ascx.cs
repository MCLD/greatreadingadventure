using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class BooksRead : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                var currentPatron = (Patron)Session["Patron"];

                if(currentPatron.IsMasterAccount) {
                    familyMemberSelector.Visible = true;

                    var ds = Patron.GetSubAccountList(currentPatron.PID);
                    PID.Items.Add(new ListItem(DisplayHelper.FormatName(currentPatron.FirstName, 
                                                                        currentPatron.LastName,
                                                                        currentPatron.Username), 
                                               currentPatron.PID.ToString()));
                    for(int i = 0; i < ds.Tables[0].Rows.Count; i++) {
                        PID.Items.Add(new ListItem(DisplayHelper.FormatName(ds.Tables[0].Rows[i]["FirstName"].ToString(),
                                                                            ds.Tables[0].Rows[i]["LastName"].ToString(),
                                                                            ds.Tables[0].Rows[i]["Username"].ToString()),
                                                   ds.Tables[0].Rows[i]["PID"].ToString()));
                    }
                    PID.SelectedValue = currentPatron.PID.ToString();
                }
                PopulateList(currentPatron.PID.ToString());
            }
        }

        protected void Ddl_SelectedIndexChanged(object sender, EventArgs e) {
            PopulateList(PID.SelectedValue);
        }

        protected void PopulateList(string patronId) {
            int patronIdNumeric = int.Parse(patronId);
            var patron = Patron.FetchObject(patronIdNumeric);
            whoRead.Text = DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username);
            var ds = Patron.GetReadingList(patronIdNumeric);
            rptr.DataSource = ds;
            rptr.DataBind();
            noBooksLabel.Visible = ds.Tables[0].Rows.Count == 0;
            booksPanel.Visible = !noBooksLabel.Visible;
        }
    }
}