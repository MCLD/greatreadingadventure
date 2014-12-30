﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;


namespace STG.SRP.Classes
{
    public partial class ChangeFamilyPassword : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["SA"]) && (Session["SA"] == null || Session["SA"].ToString() == ""))
                {
                    Response.Redirect("~/FamilyAccountList.aspx");
                }
                if (!string.IsNullOrEmpty(Request["SA"]))
                {
                    SA.Text = Request["SA"];
                    Session["SA"] = SA.Text;
                }
                else
                {
                    SA.Text = Session["SA"].ToString();
                }

                // now validate user can change password for SA Sub Account

                var patron = (Patron) Session["Patron"];
                if (!patron.IsMasterAccount)
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }

                if (!Patron.CanManageSubAccount(patron.PID, int.Parse(SA.Text)))
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }
                var sa = Patron.FetchObject(int.Parse(SA.Text));
                lblAccount.Text = (sa.FirstName + " " + sa.LastName).Trim();
                if (lblAccount.Text.Length == 0) lblAccount.Text = sa.Username;

            }
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
                    var sa = Patron.FetchObject(int.Parse(SA.Text));
                    sa.Password = NPassword.Text.Trim();
                    sa.Update();

                   
                    lblError.Text =
                        "The new password has been activated.  <br><br> Next time when " + lblAccount.Text  +  " logs in, use the new password.<br><br> <br><br> <br>";
                    pnlfields.Visible = false;
                }
            }


        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FamilyAccountList.aspx");
        }
    }
}