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

                PatronNavigation.NavigateUrl = string.Format("~/ControlRoom/Modules/Patrons/PatronDetails.aspx?pid={0}", PID1);

                txtUsername.Text = p.Username;
                txtEmail.Text = p.EmailAddress;

                if (p.DOB == DateTime.MinValue)
                {
                    DobLabel.Visible = false;
                    txtDOB.Visible = false;
                }
                else
                {
                    DobLabel.Visible = true;
                    txtDOB.Visible = true;
                    txtDOB.Text = p.DOB.ToShortDateString();
                }
                txtFirstName.Text = p.FirstName;
                txtLastName.Text = p.LastName;
                if (string.IsNullOrWhiteSpace(p.Gender))
                {
                    GenderLabel.Visible = false;
                    txtGender.Visible = false;
                }
                else
                {
                    GenderLabel.Visible = true;
                    txtGender.Visible = true;
                    txtGender.Text = p.Gender;
                }

                Registered.Text = p.RegistrationDate.ToString();

                PointTotal.Text = PatronPoints.GetTotalPatronPoints(p.PID).ToString();

                if (p.ProgID != 0)
                {
                    txtProgram.Text = Programs.FetchObject(p.ProgID).AdminName;
                }
            }
        }
    }
}