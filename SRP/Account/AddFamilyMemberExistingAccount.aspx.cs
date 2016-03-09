namespace GRA.SRP.Account
{
    using DAL;
    using SRPApp.Classes;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Tools;
    public partial class AddFamilyMemberExistingAccount : BaseSRPPage
    {
        protected string SaveButtonText { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsSecure = true;
            if (!IsPostBack)
            {
                TranslateStrings(this);
            }
            this.SaveButtonText = GetResourceString("myaccount-save");
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            var st = new SessionTools(Session);
            var sessionPatron = (Patron)Session[SessionKey.Patron];
            if (Username.Text.Trim().Equals(sessionPatron.Username))
            {
                st.AlertPatron("You cannot add yourself as a family member to your own family.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
                return;
            }

            if (string.IsNullOrEmpty(Username.Text))
            {
                st.AlertPatron("In order to link an existing account to yours as a family member you must supply their username.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
                return;
            }

            if (string.IsNullOrEmpty(Password.Text))
            {
                st.AlertPatron("In order to link an existing account to yours as a family member you must supply their password.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
                return;
            }

            // validate password
            if (!Patron.VerifyPassword(Username.Text, Password.Text))
            {
                // log invalid attempts
                this.Log().Error("Invalid credentials provided by {0}/{1} to try to link a family member.",
                    sessionPatron.Username,
                    sessionPatron.PID);
                st.AlertPatron("The username and password you supplied are not valid.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
                return;
            }

            var familyMemberPatron = Patron.GetObjectByUsername(Username.Text);

            if(familyMemberPatron.MasterAcctPID == sessionPatron.PID)
            {
                this.Log().Error("Attempt by {0}/{1} to add family member {2}/{3} who is already linked.",
                    familyMemberPatron.Username,
                    familyMemberPatron.PID,
                    sessionPatron.Username,
                    sessionPatron.PID);
                st.AlertPatron(string.Format("{0} is already a member of your family!",
                        familyMemberPatron.Username),
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
                return;

            }

            this.Log().Info("Correct credentials to make {0}/{1} a family member under {2}/{3}.",
                familyMemberPatron.Username,
                familyMemberPatron.PID,
                sessionPatron.Username,
                sessionPatron.PID);

            var currentPatron = Patron.GetObjectByUsername(sessionPatron.Username);
            if (!currentPatron.IsMasterAccount)
            {
                currentPatron.IsMasterAccount = true;
                this.Log().Info("Updating {0}/{1} to mark as head of family",
                    sessionPatron.Username,
                    sessionPatron.PID);
                currentPatron.Update();
                st.EstablishPatron(currentPatron);
            }

            var alreadyFamilyMembers = Patron.GetSubAccountList(familyMemberPatron.PID);
            int alreadyFamilyMemberCount = alreadyFamilyMembers.Tables[0].Rows.Count;

            if (alreadyFamilyMemberCount > 0)
            {
                this.Log().Info("{0}/{1} already had {2} family members, moving them all to {3}/{4}",
                    familyMemberPatron.Username,
                    familyMemberPatron.PID,
                    alreadyFamilyMemberCount,
                    sessionPatron.Username,
                    sessionPatron.PID);
                foreach (DataRow row in alreadyFamilyMembers.Tables[0].Rows)
                {
                    var alreadyFamilyPatron = Patron.GetObjectByUsername(row["Username"].ToString());
                    this.Log().Info("Marking {0}/{1} as a family member of {2}/{3} and saving...",
                        alreadyFamilyPatron.Username,
                        alreadyFamilyPatron.PID,
                        sessionPatron.Username,
                        sessionPatron.PID);
                    alreadyFamilyPatron.MasterAcctPID = currentPatron.PID;
                    alreadyFamilyPatron.IsMasterAccount = false;
                    alreadyFamilyPatron.Update();
                }
            }

            if (familyMemberPatron.IsMasterAccount)
            {
                this.Log().Info("{0}/{1} was already marked as a family head, marking as regular user",
                    familyMemberPatron.Username,
                    familyMemberPatron.PID);
                familyMemberPatron.IsMasterAccount = false;
            }

            familyMemberPatron.MasterAcctPID = sessionPatron.PID;

            this.Log().Info("Saving update to {0}/{1}...",
                familyMemberPatron.Username,
                familyMemberPatron.PID);
            familyMemberPatron.Update();

            if (alreadyFamilyMemberCount> 0)
            {
                st.AlertPatron(
                    string.Format("<strong>{0}</strong> and {1} other users are now members of your family!",
                        familyMemberPatron.Username,
                        alreadyFamilyMemberCount),
                    PatronMessageLevels.Success,
                    "thumbs-up");
            } else
            {
                st.AlertPatron(
                    string.Format("<strong>{0}</strong> is now a member of your family!",
                        familyMemberPatron.Username),
                    PatronMessageLevels.Success,
                    "thumbs-up");
            }
            Response.Redirect("~/Account/FamilyAccountList.aspx");
        }
    }
}