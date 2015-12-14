using System;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using System.Web;

namespace GRA.SRP.Classes {
    public partial class PatronLogin : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!Page.IsPostBack) {
                if(Request.Cookies[CookieKey.Username] != null) {
                    loginUsername.Text = Request.Cookies[CookieKey.Username].Value;
                    loginRememberMe.Checked = true;
                }
            }
        }

        protected string LoginErrorMessage { get; set; }

        protected void loginClick(object sender, EventArgs e) {
            if(!(string.IsNullOrEmpty(loginUsername.Text.Trim()) || string.IsNullOrEmpty(loginPassword.Text.Trim()))) {
                var patron = new Patron();
                if(Patron.Login(loginUsername.Text.Trim(), loginPassword.Text)) {
                    var bp = Patron.GetObjectByUsername(loginUsername.Text.Trim());

                    var pgm = DAL.Programs.FetchObject(bp.ProgID);
                    if(pgm == null) {
                        var progID = Programs.GetDefaultProgramForAgeAndGrade(bp.Age, bp.SchoolGrade.SafeToInt());
                        bp.ProgID = progID;
                        bp.Update();
                    }
                    new SessionTools(Session).EstablishPatron(bp);

                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    if(loginRememberMe.Checked) {
                        var loginUsernameCookie = new HttpCookie(CookieKey.Username);
                        loginUsernameCookie.Expires = DateTime.Now.AddDays(14);
                        loginUsernameCookie.Value = loginUsername.Text.Trim();
                        Response.SetCookie(loginUsernameCookie);
                    } else {
                        if(Request.Cookies[CookieKey.Username] != null) {
                            Response.Cookies[CookieKey.Username].Expires = DateTime.Now.AddDays(-1);
                        }
                    }


                    if(ViewState[SessionKey.RequestedPath] != null) {
                        string requestedPath = ViewState[SessionKey.RequestedPath].ToString();
                        Response.Redirect(requestedPath);
                    } else {
                        Response.Redirect("~");
                    }
                } else {
                    Session[SessionKey.PatronMessage] = "Invalid username or password.";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                    Session[SessionKey.PatronMessageGlyphicon] = "remove";
                    Session[SessionKey.Patron] = null;
                }
            }

        }
    }
}