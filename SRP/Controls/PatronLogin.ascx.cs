using System;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.Classes {
    public partial class PatronLogin : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected string LoginErrorMessage { get; set; }

        protected void loginClick(object sender, EventArgs e) {
            if(!(string.IsNullOrEmpty(loginUsername.Text.Trim()) || string.IsNullOrEmpty(loginPassword.Text.Trim()))) {
                var patron = new Patron();
                if(Patron.Login(loginUsername.Text.Trim(), loginPassword.Text.Trim())) {
                    var bp = Patron.GetObjectByUsername(loginUsername.Text.Trim());

                    var pgm = DAL.Programs.FetchObject(bp.ProgID);
                    if(pgm == null) {
                        var progID = Programs.GetDefaultProgramForAgeAndGrade(bp.Age, bp.SchoolGrade.SafeToInt()); //Programs.FetchObject(Programs.GetDefaultProgramID());
                        bp.ProgID = progID;
                        bp.Update();
                    }
                    new PatronSession(Session).Establish(bp);

                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    if(ViewState[SessionKey.RequestedPath] != null) {
                        string requestedPath = ViewState[SessionKey.RequestedPath].ToString();
                        Response.Redirect(requestedPath);
                    } else {
                        Response.Redirect("~/Dashboard.aspx");
                    }
                } else {
                    Session[SessionKey.PatronMessage] = "Invalid username or password.";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                    Session[SessionKey.PatronMessageGlyphicon] = "remove";
                    Session["PatronLoggedIn"] = false;
                    Session["Patron"] = null;
                }
            }

        }
    }
}