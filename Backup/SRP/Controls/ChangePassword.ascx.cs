using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.Controls;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;


namespace STG.SRP.Classes
{
    public partial class ChangePassword : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uxNewPasswordStrengthValidator.ValidationExpression = STGOnlyUtilities.PasswordStrengthRE();
            uxNewPasswordStrengthValidator.ErrorMessage = STGOnlyUtilities.PasswordStrengthError();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Page.IsValid) 
            {
                if (!(string.IsNullOrEmpty(NPassword.Text.Trim())))
                {
                    var patron = (Patron)Session["Patron"];
                    if (patron.Password != CPass.Text.Trim())
                    {
                        lblError.Text =
                            "You entered an incorrect password.";

                             CPass.Attributes.Add("Value", CPass.Text);
                             NPassword.Attributes.Add("Value", NPassword.Text);
                             NPasswordR.Attributes.Add("Value", NPasswordR.Text);
                        return;
                    }

                    if (NPassword.Text.Trim() != NPasswordR.Text.Trim())
                    {
                        lblError.Text =
                            "The new password and new password re-entry do not match.";
                        CPass.Attributes.Add("Value", CPass.Text);
                        NPassword.Attributes.Add("Value", NPassword.Text);
                        NPasswordR.Attributes.Add("Value", NPasswordR.Text);
                        return;
                    }
                    patron.Password = NPassword.Text.Trim();
                    patron.Update();

                        Session["PatronLoggedIn"] = true;
                        Session["Patron"] = patron;
                        Session["ProgramID"] = patron.ProgID;
                        Session["PatronProgramID"] = patron.ProgID;
                        Session["CurrentProgramID"] = patron.ProgID;


                    lblError.Text =
                        "Your new password has been activated.  <br><br> Next time you need to log in, please use your new password.<br><br> <br><br> <br>";
                    pnlfields.Visible = false;
                }
            }


        }
    }
}