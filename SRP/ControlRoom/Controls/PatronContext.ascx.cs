using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class PatronContext : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var PID1 = int.Parse(Session["CURR_PATRON_ID"].ToString());
                var p = Patron.FetchObject(PID1);

                txtUsername.Text = p.Username;
                txtEmail.Text = p.EmailAddress;
                txtDOB.Text = p.DOB.ToShortDateString();
                txtFirstName.Text = p.FirstName;
                txtLastName.Text = p.LastName;
                txtGender.Text = p.Gender;
                if (p.ProgID != 0) txtProgram.Text = Programs.FetchObject(p.ProgID).AdminName;
            }
        }
    }
}