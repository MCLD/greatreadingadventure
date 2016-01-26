using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using System.Text;

namespace GRA.SRP.Controls {
    public partial class PatronRegistration : System.Web.UI.UserControl {
        private CustomRegistrationFields customFields;
        protected CustomRegistrationFields CustomFields
        {
            get
            {
                if(customFields == null) {
                    customFields = CustomRegistrationFields.FetchObject();
                }
                return customFields;
            }
        }
        protected string CurrentStep
        {
            get
            {
                return Step.Text;
            }
        }
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                var dataSource = RegistrationSettings.GetAll();
                rptr.DataSource = dataSource;
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);
            }
        }

        protected void rptr_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            var registrationHelper = new RegistrationHelper();
            if(!string.IsNullOrEmpty(this.CustomFields.DDValues1)) {
                var codes = Codes.GetAlByTypeID(int.Parse(this.CustomFields.DDValues1));
                registrationHelper.BindCustomDDL(e, codes, "Custom1DD", "Custom1DDTXT");
            }
            if(!string.IsNullOrEmpty(this.CustomFields.DDValues2)) {
                var codes = Codes.GetAlByTypeID(int.Parse(this.CustomFields.DDValues2));
                registrationHelper.BindCustomDDL(e, codes, "Custom2DD", "Custom2DDTXT");
            }
            if(!string.IsNullOrEmpty(this.CustomFields.DDValues3)) {
                var codes = Codes.GetAlByTypeID(int.Parse(this.CustomFields.DDValues3));
                registrationHelper.BindCustomDDL(e, codes, "Custom3DD", "Custom3DDTXT");
            }
            if(!string.IsNullOrEmpty(this.CustomFields.DDValues4)) {
                var codes = Codes.GetAlByTypeID(int.Parse(this.CustomFields.DDValues4));
                registrationHelper.BindCustomDDL(e, codes, "Custom4DD", "Custom4DDTXT");
            }
            if(!string.IsNullOrEmpty(this.CustomFields.DDValues5)) {
                var codes = Codes.GetAlByTypeID(int.Parse(this.CustomFields.DDValues5));
                registrationHelper.BindCustomDDL(e, codes, "Custom5DD", "Custom5DDTXT");
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e) {
            var step = int.Parse(Step.Text);

            if(Page.IsValid)
                DoBusinessRulesPrev(step);
            btnPrev.Enabled = true;
            if(int.Parse(Step.Text) == 1 || int.Parse(Step.Text) >= 7)
                btnPrev.Enabled = false;

            var p = (TextBox)rptr.Items[0].FindControl("Password");
            p.Attributes.Add("Value", (string.IsNullOrEmpty(p.Text) ? p.Attributes["Value"] : p.Text));
            p = (TextBox)rptr.Items[0].FindControl("Password2");
            p.Attributes.Add("Value", (string.IsNullOrEmpty(p.Text) ? p.Attributes["Value"] : p.Text));
        }

        protected void btnNext_Click(object sender, EventArgs e) {
            var step = int.Parse(Step.Text);

            if(Page.IsValid)
                DoBusinessRulesNext(step);
            btnPrev.Enabled = true;
            if(int.Parse(Step.Text) == 1 || int.Parse(Step.Text) >= 7)
                btnPrev.Enabled = false;

            var p = (TextBox)rptr.Items[0].FindControl("Password");
            p.Attributes.Add("Value", (string.IsNullOrEmpty(p.Text) ? p.Attributes["Value"] : p.Text));
            p = (TextBox)rptr.Items[0].FindControl("Password2");
            p.Attributes.Add("Value", (string.IsNullOrEmpty(p.Text) ? p.Attributes["Value"] : p.Text));
        }

        public void DoBusinessRulesNext(int curStep) {
            // code needs to have the steps in order for the ifs to flow properly on panels with now fields showing

            if(curStep == 1) {
                //get Age

                var sDOB = ((TextBox)rptr.Items[0].FindControl("DOB")).Text;
                var sAge = ((TextBox)rptr.Items[0].FindControl("Age")).Text;
                var sGrade = ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text;

                var age = -1;
                if(!string.IsNullOrEmpty(sDOB)) {
                    var DOB = DateTime.Parse(sDOB);
                    age = DateTime.Now.Year - DOB.Year;
                } else {
                    int.TryParse(sAge, out age);
                }

                RegistrationAge.Text = age.ToString();

                // Get Default Program for the Age
                // Set Program to that
                var grade = -1;
                if(sGrade.Length > 0)
                    int.TryParse(sGrade, out grade);

                var pgmDD = (DropDownList)rptr.Items[0].FindControl("ProgID");
                if(pgmDD.Items.Count == 2) {
                    // single program - just select the program
                    pgmDD.SelectedIndex = 1;
                } else if(pgmDD.SelectedValue == "0" || string.IsNullOrEmpty(pgmDD.SelectedValue)) {
                    var defaultProgram = Programs.GetDefaultProgramForAgeAndGrade(age, grade).ToString();
                    if(pgmDD.Items.FindByValue(defaultProgram) != null) {
                        pgmDD.SelectedValue = defaultProgram;
                    }
                }


                if(MasterPID.Text.Length > 0)    // Already registered the master account and now looping for family accounts
                {
                    var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                    var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 2).ToString());

                    curPanel.Visible = false;
                    newPanel.Visible = true;

                    Step.Text = (curStep + 2).ToString();
                } else {
                    if(age > 17 && SRPSettings.GetSettingValue("AllowFamilyAccounts").SafeToBoolYes()) {
                        // Ask about adult
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep + 1).ToString();
                    } else {
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 2).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep + 2).ToString();
                    }
                }


            }
            // Finished Current Step = 1

            if(curStep == 2) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

            }
            // Finished Current Step = 2 

            if(curStep == 3) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString() + "Visibility")).Text;
                if(newPanelVisibility == "0")
                    curStep = curStep + 1;  // If not, move to the next panel
            }
            // Finished Current Step = 3 

            if(curStep == 4) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString() + "Visibility")).Text;
                if(newPanelVisibility == "0")
                    curStep = curStep + 1;  // If not, move to the next panel

            }
            // Finished Current Step = 4 

            if(curStep == 5) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                // deal with parental consent, by program
                var PID = int.Parse(((DropDownList)rptr.Items[0].FindControl("ProgID")).SelectedValue);
                var prog = new Programs();
                prog.Fetch(PID);
                ((Label)rptr.Items[0].FindControl("lblConsent")).Text = prog.ParentalConsentText;

                ((Panel)rptr.Items[0].FindControl("pnlConsent")).Visible = prog.ParentalConsentFlag;
                //

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString() + "Visibility")).Text;
                if(newPanelVisibility == "0" && !prog.ParentalConsentFlag)
                    curStep = curStep + 1;  // If not, move to the next panel

            }
            // Finished Current Step = 5 

            if(curStep == 6) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();
            }
            // Finished Current Step = 6 

            if(curStep == 7) {
                if(!SaveAccount()) {
                    return;
                }

                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                //newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                var famAcct = (DropDownList)rptr.Items[0].FindControl("FamilyAccount");
                if(famAcct.SelectedValue == "Yes") {
                    curStep = 9;  // Move to the next panel
                    Step.Text = "9";
                    curPanel = FindControl("Panel" + curStep.ToString());
                    curPanel.Visible = true;
                    btnPrev.Enabled = false;
                    btnDone.Visible = true;
                    return;
                } else {
                    // we're done with registration, we can just jump right in
                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    Session[SessionKey.PatronMessage] = ((BaseSRPPage)Page).GetResourceString("registration-success");
                    Session[SessionKey.PatronMessageGlyphicon] = "thumbs-up";
                    Response.Redirect("~");
                }

                newPanel.Visible = true;
                btnPrev.Enabled = false;


            }
            // Finished Current Step = 7 

            if(curStep == 8) {
                var curPanel = FindControl("Panel" + curStep.ToString());
                var newPanel = FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();
                btnPrev.Enabled = false;

                // log them in and take them home

                Response.Redirect(GoToUrl);
            }
            // Finished Current Step = 8 

            if(curStep == 9) {
                // Reset Steps, flag as family members, restart the wizard

                var curPanel = FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel1");

                curPanel.Visible = false;
                newPanel.Visible = true;

                btnPrev.Enabled = false;
                btnDone.Visible = false;
                Step.Text = "1";
                Panel0.Visible = true;
                RegisteringFamily.Text = "1";
                RegistrationAge.Text = "0";

                ((TextBox)rptr.Items[0].FindControl("ParentGuardianFirstName")).Text = parentGuardianFirst.Text;
                ((TextBox)rptr.Items[0].FindControl("ParentGuardianMiddleName")).Text = parentGuardianMiddle.Text;
                ((TextBox)rptr.Items[0].FindControl("ParentGuardianLastName")).Text = parentGuardianLast.Text;

                ((TextBox)rptr.Items[0].FindControl("Username")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("Password")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("Password")).Attributes.Add("Value", string.Empty);
                ((TextBox)rptr.Items[0].FindControl("Password2")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("Password2")).Attributes.Add("Value", string.Empty);
                ((TextBox)rptr.Items[0].FindControl("Age")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("DOB")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text = string.Empty;
                ((DropDownList)rptr.Items[0].FindControl("ProgID")).SelectedValue = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("FirstName")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("MiddleName")).Text = string.Empty;
                ((DropDownList)rptr.Items[0].FindControl("Gender")).SelectedValue = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("LiteracyLevel1")).Text = string.Empty;
                ((TextBox)rptr.Items[0].FindControl("LiteracyLevel2")).Text = string.Empty;
            }
            // Finished Current Step = 9 

        }


        public void DoBusinessRulesPrev(int curStep) {
            // code needs to have the steps in reverse order for the ifs to flow properly on panels with now fields showing
            if(curStep == 7) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();

                // deal with parental consent, by program
                var PID = int.Parse(((DropDownList)rptr.Items[0].FindControl("ProgID")).SelectedValue);
                var prog = new Programs();
                prog.Fetch(PID);
                ((Label)rptr.Items[0].FindControl("lblConsent")).Text = prog.ParentalConsentText;

                ((Panel)rptr.Items[0].FindControl("pnlConsent")).Visible = prog.ParentalConsentFlag;


                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString() + "Visibility")).Text;
                if(newPanelVisibility == "0")
                    curStep = curStep - 1;  // If not, move to the prev panel

            }
            // Finished Current Step = 7

            if(curStep == 6) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString() + "Visibility")).Text;
                if(newPanelVisibility == "0")
                    curStep = curStep - 1;  // If not, move to the prev panel

            }
            // Finished Current Step = 6


            if(curStep == 5) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString() + "Visibility")).Text;
                if(newPanelVisibility == "0")
                    curStep = curStep - 1;  // If not, move to the prev panel
            }
            // Finished Current Step = 5


            if(curStep == 4) {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();

            }
            // Finished Current Step = 4


            if(curStep == 3) {
                //get Age

                var Age = int.Parse(RegistrationAge.Text);

                if(MasterPID.Text.Length > 0)    // Already registered the master account and now looping for family accounts
                {
                    var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                    var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 2).ToString());

                    curPanel.Visible = false;
                    newPanel.Visible = true;

                    Step.Text = (curStep - 2).ToString();
                } else {
                    if(Age > 17) {
                        // Ask about adult
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep - 1).ToString();
                    } else {
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 2).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep - 2).ToString();
                    }
                }


            }
            // Finished Current Step = 3


            if(curStep == 2) {
                var sDOB = ((TextBox)rptr.Items[0].FindControl("DOB")).Text;
                var sAge = ((TextBox)rptr.Items[0].FindControl("Age")).Text;
                var sGrade = ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text;

                var age = -1;
                if(!string.IsNullOrEmpty(sDOB)) {
                    var DOB = DateTime.Parse(sDOB);
                    age = DateTime.Now.Year - DOB.Year;
                } else {
                    int.TryParse(sAge, out age);
                }

                RegistrationAge.Text = age.ToString();

                // Get Default Program for the Age
                // Set Program to that
                var grade = -1;
                if(sGrade.Length > 0)
                    int.TryParse(sGrade, out grade);

                var pgmDD = (DropDownList)rptr.Items[0].FindControl("ProgID");
                if(pgmDD.SelectedValue == "0" || string.IsNullOrEmpty(pgmDD.SelectedValue)) {
                    pgmDD.SelectedValue = Programs.GetDefaultProgramForAgeAndGrade(age, grade).ToString();
                }


                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();
            }
            // Finished Current Step = 2
        }

        public bool SaveAccount() {
            try {
                var p = new Patron();
                DateTime _d;
                var DOB = rptr.Items[0].FindControl("DOB") as TextBox;
                if(DOB != null && !string.IsNullOrEmpty(DOB.Text)) {
                    if(DateTime.TryParse(DOB.Text, out _d))
                        p.DOB = _d;
                }

                p.Age = FormatHelper.SafeToInt(((TextBox)(rptr.Items[0]).FindControl("Age")).Text);

                p.ProgID = FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("ProgID")).SelectedValue);
                p.Username = ((TextBox)(rptr.Items[0]).FindControl("Username")).Text;
                p.NewPassword = ((TextBox)(rptr.Items[0]).FindControl("Password")).Text;

                var famAcct = (DropDownList)rptr.Items[0].FindControl("FamilyAccount");
                p.IsMasterAccount = (famAcct.SelectedValue == "Yes" && MasterPID.Text.Length == 0);

                p.SchoolGrade = ((TextBox)(rptr.Items[0]).FindControl("SchoolGrade")).Text;
                p.FirstName = ((TextBox)(rptr.Items[0]).FindControl("FirstName")).Text;
                p.MiddleName = ((TextBox)(rptr.Items[0]).FindControl("MiddleName")).Text;
                p.LastName = ((TextBox)(rptr.Items[0]).FindControl("LastName")).Text;
                p.Gender = ((DropDownList)(rptr.Items[0]).FindControl("Gender")).SelectedValue;
                p.EmailAddress = ((TextBox)(rptr.Items[0]).FindControl("EmailAddress")).Text;
                p.PhoneNumber = ((TextBox)(rptr.Items[0]).FindControl("PhoneNumber")).Text;
                p.PhoneNumber = FormatHelper.FormatPhoneNumber(p.PhoneNumber);
                p.StreetAddress1 = ((TextBox)(rptr.Items[0]).FindControl("StreetAddress1")).Text;
                p.StreetAddress2 = ((TextBox)(rptr.Items[0]).FindControl("StreetAddress2")).Text;
                p.City = ((TextBox)(rptr.Items[0]).FindControl("City")).Text;
                p.State = ((TextBox)(rptr.Items[0]).FindControl("State")).Text;
                p.ZipCode = ((TextBox)(rptr.Items[0]).FindControl("ZipCode")).Text;
                p.ZipCode = FormatHelper.FormatZipCode(p.ZipCode);

                p.Country = ((TextBox)(rptr.Items[0]).FindControl("Country")).Text;
                p.County = ((TextBox)(rptr.Items[0]).FindControl("County")).Text;
                p.ParentGuardianFirstName = ((TextBox)(rptr.Items[0]).FindControl("ParentGuardianFirstName")).Text;
                p.ParentGuardianLastName = ((TextBox)(rptr.Items[0]).FindControl("ParentGuardianLastName")).Text;
                p.ParentGuardianMiddleName = ((TextBox)(rptr.Items[0]).FindControl("ParentGuardianMiddleName")).Text;
                p.LibraryCard = ((TextBox)(rptr.Items[0]).FindControl("LibraryCard")).Text;

                p.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("PrimaryLibrary")).SelectedValue);
                p.SchoolName = ((DropDownList)(rptr.Items[0]).FindControl("SchoolName")).SelectedValue;
                p.SchoolType = FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("SchoolType")).SelectedValue);

                var lc = LibraryCrosswalk.FetchObjectByLibraryID(p.PrimaryLibrary);
                if(lc != null) {
                    p.District = lc.DistrictID == 0
                        ? ((DropDownList)(rptr.Items[0]).FindControl("District")).SelectedValue
                        : lc.DistrictID.ToString();
                } else {
                    p.District = ((DropDownList)(rptr.Items[0]).FindControl("District")).SelectedValue;
                }
                var sc = SchoolCrosswalk.FetchObjectBySchoolID(p.SchoolName.SafeToInt());
                if(sc != null) {
                    p.SDistrict = sc.DistrictID == 0
                        ? ((DropDownList)(rptr.Items[0]).FindControl("SDistrict")).SelectedValue.SafeToInt()
                        : sc.DistrictID;

                    p.SchoolType = sc.SchTypeID == 0
                        ? FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("SchoolType")).SelectedValue)
                        : sc.SchTypeID;
                } else {
                    p.SDistrict = ((DropDownList)(rptr.Items[0]).FindControl("SDistrict")).SelectedValue.SafeToInt();
                }


                p.Teacher = ((TextBox)(rptr.Items[0]).FindControl("Teacher")).Text;
                p.GroupTeamName = ((TextBox)(rptr.Items[0]).FindControl("GroupTeamName")).Text;
                p.LiteracyLevel1 = FormatHelper.SafeToInt(((TextBox)(rptr.Items[0]).FindControl("LiteracyLevel1")).Text);
                p.LiteracyLevel2 = FormatHelper.SafeToInt(((TextBox)(rptr.Items[0]).FindControl("LiteracyLevel2")).Text);

                p.ParentPermFlag = ((CheckBox)(rptr.Items[0]).FindControl("ParentPermFlag")).Checked;
                p.Over18Flag = int.Parse(RegistrationAge.Text) >= 18;
                p.ShareFlag = ((CheckBox)(rptr.Items[0]).FindControl("ShareFlag")).Checked;
                p.TermsOfUseflag = ((CheckBox)(rptr.Items[0]).FindControl("TermsOfUseflag")).Checked;

                var cr = this.CustomFields;
                p.Custom1 = string.IsNullOrEmpty(cr.DDValues1)
                    ? ((TextBox)(rptr.Items[0]).FindControl("Custom1")).Text
                    : ((DropDownList)(rptr.Items[0]).FindControl("Custom1DD")).SelectedValue;

                p.Custom2 = string.IsNullOrEmpty(cr.DDValues2)
                    ? ((TextBox)(rptr.Items[0]).FindControl("Custom2")).Text
                    : ((DropDownList)(rptr.Items[0]).FindControl("Custom2DD")).SelectedValue;

                p.Custom3 = string.IsNullOrEmpty(cr.DDValues3)
                    ? ((TextBox)(rptr.Items[0]).FindControl("Custom3")).Text
                    : ((DropDownList)(rptr.Items[0]).FindControl("Custom3DD")).SelectedValue;

                p.Custom4 = string.IsNullOrEmpty(cr.DDValues4)
                    ? ((TextBox)(rptr.Items[0]).FindControl("Custom4")).Text
                    : ((DropDownList)(rptr.Items[0]).FindControl("Custom4DD")).SelectedValue;

                p.Custom5 = string.IsNullOrEmpty(cr.DDValues5)
                    ? ((TextBox)(rptr.Items[0]).FindControl("Custom5")).Text
                    : ((DropDownList)(rptr.Items[0]).FindControl("Custom5DD")).SelectedValue;

                p.AvatarID = FormatHelper.SafeToInt(((System.Web.UI.HtmlControls.HtmlInputText)rptr.Items[0].FindControl("AvatarID")).Value);

                var registeringMasterAccount = true;
                if(!RegisteringFamily.Text.Equals("0")) {
                    registeringMasterAccount = false;
                    p.MasterAcctPID = int.Parse(MasterPID.Text);
                }

                if(p.IsValid(BusinessRulesValidationMode.INSERT)) {
                    p.Insert();

                    var prog = Programs.FetchObject(p.ProgID);
                    var earnedBadgesList = new List<Badge>();
                    if(prog.RegistrationBadgeID != 0) {
                        AwardPoints.AwardBadgeToPatron(prog.RegistrationBadgeID,
                                                       p,
                                                       ref earnedBadgesList);
                    }
                    AwardPoints.AwardBadgeToPatronViaMatchingAwards(p, ref earnedBadgesList);

                    var earnedBadges = string.Join("|", earnedBadgesList.Select(b => b.BID).Distinct());

                    if(p.IsMasterAccount && earnedBadges.Length > 0) {
                        // if family account and is master, and has badges, rememebr to show them
                        new SessionTools(Session).EarnedBadges(earnedBadges);
                    }
                    if(!p.IsMasterAccount && p.MasterAcctPID == 0 && earnedBadges.Length > 0) {
                        // if not family master or not family at all and badges, rememebr to show ...
                        new SessionTools(Session).EarnedBadges(earnedBadges);
                    }

                    if(p.IsMasterAccount) {
                        parentGuardianFirst.Text = p.FirstName;
                        parentGuardianMiddle.Text = p.MiddleName;
                        parentGuardianLast.Text = p.LastName;
                    }

                    if(registeringMasterAccount) {
                        MasterPID.Text = p.PID.ToString();
                        new SessionTools(Session).EstablishPatron(p);
                    }
                } else {
                    StringBuilder message = new StringBuilder("<strong>");
                    message.AppendFormat(SRPResources.ApplicationError1, "<ul>");
                    foreach(BusinessRulesValidationMessage m in p.ErrorCodes) {
                        message.AppendFormat("<li>{0}</li>", m.ErrorMessage);
                    }
                    message.Append("</ul></strong>");
                    Session[SessionKey.PatronMessage] = message.ToString();
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                    return false;
                }
            } catch(Exception ex) {
                this.Log().Error(string.Format("A problem occurred during registration: {0}",
                                               ex.Message));
                Session[SessionKey.PatronMessage] = string.Format("<strong>{0}</strong>",
                                                                  ex.Message);
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                return false;
            }
            return true;
        }

        protected void City_TextChanged(object sender, EventArgs e) {
            ReloadLibraryDistrict();
            ReloadSchoolDistrict();
        }

        protected void District_SelectedIndexChanged(object sender, EventArgs e) {
            ReloadLibraryDistrict();
        }

        protected void SchoolType_SelectedIndexChanged(object sender, EventArgs e) {
            ReloadSchoolDistrict();
        }

        protected void SDistrict_SelectedIndexChanged(object sender, EventArgs e) {
            ReloadSchoolDistrict();
        }

        protected void Age_TextChanged(object sender, EventArgs e) {
            ReloadSchoolDistrict();
        }

        protected void SchoolGrade_TextChanged(object sender, EventArgs e) {
            ReloadSchoolDistrict();
        }

        protected void ReloadSchoolDistrict() {
            var sc = (DropDownList)(rptr.Items[0]).FindControl("SchoolName");
            var st = (DropDownList)(rptr.Items[0]).FindControl("SchoolType");
            var sd = (DropDownList)(rptr.Items[0]).FindControl("SDistrict");
            var ag = (TextBox)(rptr.Items[0]).FindControl("Age");
            var gr = (TextBox)(rptr.Items[0]).FindControl("SchoolGrade");

            var scVal = sc.SelectedValue;
            sc.Items.Clear();
            sc.Items.Add(new ListItem("[Select a Value]", "0"));
            var ds = SchoolCrosswalk.GetFilteredSchoolDDValues(st.SelectedValue.SafeToInt(),
                                                               sd.SelectedValue.SafeToInt(),
                                                               city.Text,
                                                               ag.Text.SafeToInt(),
                                                               gr.Text.SafeToInt());
            foreach(DataRow r in ds.Tables[0].Rows) {
                sc.Items.Add(new ListItem(r["Description"].ToString(), r["CID"].ToString()));
            }

            var si = sc.Items.FindByValue(scVal);
            sc.SelectedValue = si != null ? scVal : "0";
        }

        protected void ReloadLibraryDistrict() {
            //*
            var pl = (DropDownList)(rptr.Items[0]).FindControl("PrimaryLibrary");
            var dt = (DropDownList)(rptr.Items[0]).FindControl("District");
            var plVal = pl.SelectedValue;
            pl.Items.Clear();
            pl.Items.Add(new ListItem("[Select a Value]", "0"));
            var ds = LibraryCrosswalk.GetFilteredBranchDDValues(int.Parse(dt.SelectedValue),
                                                                city.Text);
            foreach(DataRow r in ds.Tables[0].Rows) {
                pl.Items.Add(new ListItem(r["Description"].ToString(), r["CID"].ToString()));
            }
            var il = pl.Items.FindByValue(plVal);
            pl.SelectedValue = il != null ? plVal : "0";
            //*            
        }


        protected void btnDone_Click(object sender, EventArgs e) {
            Response.Redirect(GoToUrl);
        }

        protected void TermsOfUseflag_ServerValidate(object source, ServerValidateEventArgs args) {
            args.IsValid = ((CheckBox)(rptr.Items[0]).FindControl("TermsOfUseflag")).Checked;
        }

        public string GoToUrl
        {
            get
            {
                if(ViewState["gotourl"] == null || ViewState["gotourl"].ToString().Length == 0) {
                    ViewState["gotourl"] = "~";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }
    }
}