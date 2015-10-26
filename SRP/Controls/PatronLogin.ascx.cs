using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Classes
{
    public partial class PatronLogin : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!(string.IsNullOrEmpty(PUserName.Text.Trim()) || string.IsNullOrEmpty(PPassword.Text.Trim())))
            {
                var patron = new Patron();
                if (Patron.Login(PUserName.Text.Trim(), PPassword.Text.Trim()))
                {
                    var bp = Patron.GetObjectByUsername(PUserName.Text.Trim());
                    
                    var pgm = DAL.Programs.FetchObject(bp.ProgID);
                    if (pgm == null)
                    {
                        var progID = Programs.GetDefaultProgramForAgeAndGrade(bp.Age, bp.SchoolGrade.SafeToInt()); //Programs.FetchObject(Programs.GetDefaultProgramID());
                        bp.ProgID = progID;
                        bp.Update();
                    }
                    new PatronSession(Session).Establish(bp);

                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    Response.Redirect("~/Dashboard.aspx");
                }
                else
                {
                    lblError.Text = "Invalid username or password.";
                    Session["PatronLoggedIn"] = false;
                    Session["Patron"] = null;
                }
            }

        }
    }
}