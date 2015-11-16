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

namespace GRA.SRP.Controls
{
    public partial class PatronRegistration : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                rptr.DataSource = RegistrationSettings.GetAll();
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);

            }


        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void rptr_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //var ctl = (DropDownList)rptr.Items[0].FindControl("Gender");
            //var txt = (TextBox)rptr.Items[0].FindControl("GenderTxt");
            //var i = ctl.Items.FindByValue(txt.Text);
            //if (i != null) ctl.SelectedValue = txt.Text;


            //ctl = (DropDownList)rptr.Items[0].FindControl("PrimaryLibrary");
            //txt = (TextBox)rptr.Items[0].FindControl("PrimaryLibraryTxt");
            //i = ctl.Items.FindByValue(txt.Text);
            //if (i != null) ctl.SelectedValue = txt.Text;


            //ctl = (DropDownList)rptr.Items[0].FindControl("SchoolType");
            //txt = (TextBox)rptr.Items[0].FindControl("SchoolTypeTxt");
            //i = ctl.Items.FindByValue(txt.Text);
            //if (i != null) ctl.SelectedValue = txt.Text;

            var uxNewPasswordStrengthValidator = (RegularExpressionValidator)e.Item.FindControl("uxNewPasswordStrengthValidator");
            uxNewPasswordStrengthValidator.ValidationExpression = STGOnlyUtilities.PasswordStrengthRE();
            uxNewPasswordStrengthValidator.ErrorMessage = STGOnlyUtilities.PasswordStrengthError();
            uxNewPasswordStrengthValidator.Text = string.Format("<font color='red'>{0} </font>",
                                                                uxNewPasswordStrengthValidator.ErrorMessage);


            var ctl = (DropDownList)e.Item.FindControl("Custom1DD");
            var txt = (TextBox)e.Item.FindControl("Custom1DDTXT");
            var i = ctl.Items.FindByValue("");
            var cr = CustomRegistrationFields.FetchObject();
            if (cr.DDValues1 != "")
            {
                var ds = Codes.GetAlByTypeID(int.Parse(cr.DDValues1));
                ctl = (DropDownList)e.Item.FindControl("Custom1DD");
                txt = (TextBox)e.Item.FindControl("Custom1DDTXT");
                ctl.Items.Clear();
                ctl.Items.Add(new ListItem("[Select a Value]", ""));
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    ctl.Items.Add(new ListItem(ds.Tables[0].Rows[j]["Code"].ToString()));
                }

                i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;
            }
            if (cr.DDValues2 != "")
            {
                var ds = Codes.GetAlByTypeID(int.Parse(cr.DDValues2));
                ctl = (DropDownList)e.Item.FindControl("Custom2DD");
                txt = (TextBox)e.Item.FindControl("Custom2DDTXT");
                ctl.Items.Clear();
                ctl.Items.Add(new ListItem("[Select a Value]", ""));
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    ctl.Items.Add(new ListItem(ds.Tables[0].Rows[j]["Code"].ToString()));
                }

                i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;
            }
            if (cr.DDValues3 != "")
            {
                var ds = Codes.GetAlByTypeID(int.Parse(cr.DDValues3));
                ctl = (DropDownList)e.Item.FindControl("Custom3DD");
                txt = (TextBox)e.Item.FindControl("Custom3DDTXT");
                ctl.Items.Clear();
                ctl.Items.Add(new ListItem("[Select a Value]", ""));
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    ctl.Items.Add(new ListItem(ds.Tables[0].Rows[j]["Code"].ToString()));
                }

                i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;
            }
            if (cr.DDValues4 != "")
            {
                var ds = Codes.GetAlByTypeID(int.Parse(cr.DDValues4));
                ctl = (DropDownList)e.Item.FindControl("Custom4DD");
                txt = (TextBox)e.Item.FindControl("Custom4DDTXT");
                ctl.Items.Clear();
                ctl.Items.Add(new ListItem("[Select a Value]", ""));
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    ctl.Items.Add(new ListItem(ds.Tables[0].Rows[j]["Code"].ToString()));
                }

                i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;
            }
            if (cr.DDValues5 != "")
            {
                var ds = Codes.GetAlByTypeID(int.Parse(cr.DDValues5));
                ctl = (DropDownList)e.Item.FindControl("Custom5DD");
                txt = (TextBox)e.Item.FindControl("Custom5DDTXT");
                ctl.Items.Clear();
                ctl.Items.Add(new ListItem("[Select a Value]", ""));
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    ctl.Items.Add(new ListItem(ds.Tables[0].Rows[j]["Code"].ToString()));
                }

                i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;
            }

        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            var step = int.Parse(Step.Text);
            //var curPanel = rptr.Items[0].FindControl("Panel" + step.ToString());
            //var newPanel = rptr.Items[0].FindControl("Panel" + (step - 1).ToString());

            //curPanel.Visible = false;
            //newPanel.Visible = true;

            //Step.Text = (step - 1).ToString();
            //btnPrev.Enabled = (step != 2);

            if (Page.IsValid) DoBusinessRulesPrev(step);
            btnPrev.Enabled = true;
            if (int.Parse(Step.Text) == 1 || int.Parse(Step.Text) >= 7) btnPrev.Enabled = false; 

            var p = (TextBox)rptr.Items[0].FindControl("Password");
            p.Attributes.Add("Value", (p.Text == "" ? p.Attributes["Value"]: p.Text));
            p = (TextBox)rptr.Items[0].FindControl("Password2");
            p.Attributes.Add("Value", (p.Text == "" ? p.Attributes["Value"] : p.Text));            
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            var step = int.Parse(Step.Text);
            //var curPanel = rptr.Items[0].FindControl("Panel" + step.ToString());
            //var newPanel = rptr.Items[0].FindControl("Panel" + (step + 1).ToString());

            if (Page.IsValid) DoBusinessRulesNext(step);
            btnPrev.Enabled = true;
            if (int.Parse(Step.Text) == 1 || int.Parse(Step.Text) >= 7 ) btnPrev.Enabled = false; 

            var p = (TextBox)rptr.Items[0].FindControl("Password");
            p.Attributes.Add("Value", (p.Text == "" ? p.Attributes["Value"] : p.Text)); 
            p = (TextBox)rptr.Items[0].FindControl("Password2");
            p.Attributes.Add("Value", (p.Text == "" ? p.Attributes["Value"] : p.Text));            
        }


        public void DoBusinessRulesNext(int curStep)
        {
            // code needs to have the steps in order for the ifs to flow properly on panels with now fields showing

            if (curStep == 1)
            {
                //get Age

                var sDOB = ((TextBox) rptr.Items[0].FindControl("DOB")).Text;
                var sAge = ((TextBox)rptr.Items[0].FindControl("Age")).Text;
                var sGrade = ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text;

                var age = -1;
                if (sDOB != "")
                {
                    var DOB = DateTime.Parse(sDOB);
                    age = DateTime.Now.Year - DOB.Year;
                }
                else
                {
                    int.TryParse(sAge, out age);
                }

                RegistrationAge.Text = age.ToString();

                // Get Default Program for the Age
                // Set Program to that
                var grade = -1;
                if (sGrade.Length > 0) int.TryParse(sGrade, out grade);

                var pgmDD = (DropDownList)rptr.Items[0].FindControl("ProgID");
                if (pgmDD.SelectedValue == "0" || pgmDD.SelectedValue == "")
                {
                    pgmDD.SelectedValue = Programs.GetDefaultProgramForAgeAndGrade(age, grade).ToString();
                }


                if (MasterPID.Text.Length>0)    // Already registered the master account and now looping for family accounts
                {
                    var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                    var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 2).ToString());

                    curPanel.Visible = false;
                    newPanel.Visible = true;

                    Step.Text = (curStep + 2).ToString();                    
                }
                else
                {
                    if (age > 17 && SRPSettings.GetSettingValue("AllowFamilyAccounts").SafeToBoolYes())
                    {
                        // Ask about adult
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep + 1).ToString();
                    }
                    else
                    {
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 2).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep + 2).ToString();
                    }
                }
                

            }
            // Finished Current Step = 1

            if (curStep == 2)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

            }
            // Finished Current Step = 2 

            if (curStep == 3)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString() + "Visibility")).Text;
                if (newPanelVisibility == "0") curStep = curStep + 1;  // If not, move to the next panel
            }
            // Finished Current Step = 3 

            if (curStep == 4)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString() + "Visibility")).Text;
                if (newPanelVisibility == "0") curStep = curStep + 1;  // If not, move to the next panel

            }
            // Finished Current Step = 4 

            if (curStep == 5)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();

                // deal with parental consent, by program
                var PID = int.Parse(((DropDownList) rptr.Items[0].FindControl("ProgID")).SelectedValue);
                var prog = new Programs();
                prog.Fetch(PID);
                ((Label) rptr.Items[0].FindControl("lblConsent")).Text = prog.ParentalConsentText;

                ((Panel)rptr.Items[0].FindControl("pnlConsent")).Visible = prog.ParentalConsentFlag;
                //

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString() + "Visibility")).Text;
                if (newPanelVisibility == "0" && !prog.ParentalConsentFlag) curStep = curStep + 1;  // If not, move to the next panel

            }
            // Finished Current Step = 5 

            if (curStep == 6)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();
                lblError.Text = "";
            }
            // Finished Current Step = 6 


            if (curStep == 7)
            {

                lblError.Text = "";

                if (!SaveAccount()) return;
                
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = FindControl("Panel" + (curStep + 1).ToString());

                curPanel.Visible = false;
                //newPanel.Visible = true;

                Step.Text = (curStep + 1).ToString();


                var famAcct = (RadioButtonList)rptr.Items[0].FindControl("FamilyAccount");
                if (famAcct.SelectedValue == "Yes")
                {
                    curStep = 9;  // Move to the next panel
                    Step.Text = "9";
                    curPanel = FindControl("Panel" + curStep.ToString());
                    curPanel.Visible = true;
                    btnPrev.Enabled = false;
                    btnDone.Visible = true;
                    return;
                }
                
                newPanel.Visible = true;
                btnPrev.Enabled = false;


            }
            // Finished Current Step = 7 

            if (curStep == 8)
            {
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

            if (curStep == 9)
            {
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

                ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("DOB")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("Age")).Text = "";
                ((DropDownList)rptr.Items[0].FindControl("ProgID")).SelectedValue = "";
                ((TextBox)rptr.Items[0].FindControl("FirstName")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("MiddleName")).Text = "";
                ((DropDownList)rptr.Items[0].FindControl("Gender")).SelectedValue = "";
                //((TextBox)rptr.Items[0].FindControl("SchoolName")).Text = "";
                //((TextBox)rptr.Items[0].FindControl("District")).Text = "";
                //((TextBox)rptr.Items[0].FindControl("Teacher")).Text = "";
                //((TextBox)rptr.Items[0].FindControl("GroupTeamName")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("LiteracyLevel1")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("LiteracyLevel2")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("Username")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("Password")).Text = "";
                ((TextBox) rptr.Items[0].FindControl("Password")).Attributes.Add("Value", "");
                ((TextBox)rptr.Items[0].FindControl("Password2")).Text = "";
                ((TextBox)rptr.Items[0].FindControl("Password2")).Attributes.Add("Value", "");
                //((TextBox)rptr.Items[0].FindControl("AvatarID")).Text = "1";
            }
            // Finished Current Step = 9 

        }


        public void DoBusinessRulesPrev(int curStep)
        {
            // code needs to have the steps in reverse order for the ifs to flow properly on panels with now fields showing
            if (curStep == 7)
            {
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
                if (newPanelVisibility == "0") curStep = curStep - 1;  // If not, move to the prev panel

            }
            // Finished Current Step = 7
            
            if (curStep == 6)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();

                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString() + "Visibility")).Text;
                if (newPanelVisibility == "0") curStep = curStep - 1;  // If not, move to the prev panel

            }
            // Finished Current Step = 6

            
            if (curStep == 5)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();
            
                // do we show this next panel?
                var newPanelVisibility = ((TextBox)rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString() + "Visibility")).Text;
                if (newPanelVisibility == "0") curStep = curStep - 1;  // If not, move to the prev panel
            }
            // Finished Current Step = 5

            
            if (curStep == 4)
            {
                var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                curPanel.Visible = false;
                newPanel.Visible = true;

                Step.Text = (curStep - 1).ToString();

            }
            // Finished Current Step = 4

            
            if (curStep == 3)
            {
                //get Age

                var Age = int.Parse(RegistrationAge.Text);

                if (MasterPID.Text.Length > 0)    // Already registered the master account and now looping for family accounts
                {
                    var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                    var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 2).ToString());

                    curPanel.Visible = false;
                    newPanel.Visible = true;

                    Step.Text = (curStep - 2).ToString();
                }
                else
                {
                    if (Age > 17)
                    {
                        // Ask about adult
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 1).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep - 1).ToString();
                    }
                    else
                    {
                        var curPanel = rptr.Items[0].FindControl("Panel" + curStep.ToString());
                        var newPanel = rptr.Items[0].FindControl("Panel" + (curStep - 2).ToString());

                        curPanel.Visible = false;
                        newPanel.Visible = true;

                        Step.Text = (curStep - 2).ToString();
                    }   
                }


            }
            // Finished Current Step = 3

            
            if (curStep == 2)
            {

                var sDOB = ((TextBox)rptr.Items[0].FindControl("DOB")).Text;
                var sAge = ((TextBox)rptr.Items[0].FindControl("Age")).Text;
                var sGrade = ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text;

                var age = -1;
                if (sDOB != "")
                {
                    var DOB = DateTime.Parse(sDOB);
                    age = DateTime.Now.Year - DOB.Year;
                }
                else
                {
                    int.TryParse(sAge, out age);
                }

                RegistrationAge.Text = age.ToString();

                // Get Default Program for the Age
                // Set Program to that
                var grade = -1;
                if (sGrade.Length > 0) int.TryParse(sGrade, out grade);

                var pgmDD = (DropDownList)rptr.Items[0].FindControl("ProgID");
                if (pgmDD.SelectedValue == "0" || pgmDD.SelectedValue == "")
                {
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

        public bool SaveAccount()
        {        
            try
            {
                var p = new Patron();
                DateTime _d;
                var DOB = rptr.Items[0].FindControl("DOB") as TextBox;
                if (DOB != null && DOB.Text != "")
                {
                    if (DateTime.TryParse(DOB.Text, out _d)) p.DOB = _d;
                }

                p.Age = FormatHelper.SafeToInt(((TextBox)(rptr.Items[0]).FindControl("Age")).Text);

                p.ProgID = FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("ProgID")).SelectedValue);
                p.Username = ((TextBox)(rptr.Items[0]).FindControl("Username")).Text;
                p.NewPassword = ((TextBox)(rptr.Items[0]).FindControl("Password")).Text;

                var famAcct = (RadioButtonList)rptr.Items[0].FindControl("FamilyAccount");
                p.IsMasterAccount = (famAcct.SelectedValue == "Yes" && MasterPID.Text.Length==0);

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

                //p.District = ((DropDownList)(rptr.Items[0]).FindControl("District")).SelectedValue;
                //p.SDistrict = ((DropDownList)(rptr.Items[0]).FindControl("SDistrict")).SelectedValue.SafeToInt();

                p.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("PrimaryLibrary")).SelectedValue);
                p.SchoolName = ((DropDownList)(rptr.Items[0]).FindControl("SchoolName")).SelectedValue;
                p.SchoolType = FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("SchoolType")).SelectedValue);

                var lc = LibraryCrosswalk.FetchObjectByLibraryID(p.PrimaryLibrary);
                if (lc != null)
                {
                    p.District = lc.DistrictID == 0 ? ((DropDownList)(rptr.Items[0]).FindControl("District")).SelectedValue : lc.DistrictID.ToString();
                }
                else
                {
                    p.District = ((DropDownList)(rptr.Items[0]).FindControl("District")).SelectedValue;
                }
                var sc = SchoolCrosswalk.FetchObjectBySchoolID(p.SchoolName.SafeToInt());
                if (sc != null)
                {
                    p.SDistrict = sc.DistrictID == 0 ? ((DropDownList)(rptr.Items[0]).FindControl("SDistrict")).SelectedValue.SafeToInt() : sc.DistrictID;
                    p.SchoolType = sc.SchTypeID == 0 ? FormatHelper.SafeToInt(((DropDownList)(rptr.Items[0]).FindControl("SchoolType")).SelectedValue) : sc.SchTypeID;
                }
                else
                {
                    p.SDistrict = ((DropDownList)(rptr.Items[0]).FindControl("SDistrict")).SelectedValue.SafeToInt();
                }


                p.Teacher = ((TextBox)(rptr.Items[0]).FindControl("Teacher")).Text;
                p.GroupTeamName = ((TextBox)(rptr.Items[0]).FindControl("GroupTeamName")).Text;
                p.LiteracyLevel1 = FormatHelper.SafeToInt(((TextBox)(rptr.Items[0]).FindControl("LiteracyLevel1")).Text);
                p.LiteracyLevel2 = FormatHelper.SafeToInt(((TextBox)(rptr.Items[0]).FindControl("LiteracyLevel2")).Text);
 
                p.ParentPermFlag = ((CheckBox)(rptr.Items[0]).FindControl("ParentPermFlag")).Checked;
                p.Over18Flag = int.Parse(RegistrationAge.Text) > 18;
                p.ShareFlag = ((CheckBox)(rptr.Items[0]).FindControl("ShareFlag")).Checked;
                p.TermsOfUseflag = ((CheckBox)(rptr.Items[0]).FindControl("TermsOfUseflag")).Checked;

                var cr = CustomRegistrationFields.FetchObject();
                p.Custom1 = cr.DDValues1 == "" ? ((TextBox)(rptr.Items[0]).FindControl("Custom1")).Text : ((DropDownList)(rptr.Items[0]).FindControl("Custom1DD")).SelectedValue;
                p.Custom2 = cr.DDValues2 == "" ? ((TextBox)(rptr.Items[0]).FindControl("Custom2")).Text : ((DropDownList)(rptr.Items[0]).FindControl("Custom2DD")).SelectedValue;
                p.Custom3 = cr.DDValues3 == "" ? ((TextBox)(rptr.Items[0]).FindControl("Custom3")).Text : ((DropDownList)(rptr.Items[0]).FindControl("Custom3DD")).SelectedValue;
                p.Custom4 = cr.DDValues4 == "" ? ((TextBox)(rptr.Items[0]).FindControl("Custom4")).Text : ((DropDownList)(rptr.Items[0]).FindControl("Custom4DD")).SelectedValue;
                p.Custom5 = cr.DDValues5 == "" ? ((TextBox)(rptr.Items[0]).FindControl("Custom5")).Text : ((DropDownList)(rptr.Items[0]).FindControl("Custom5DD")).SelectedValue;
                    
                //p.Custom1 = ((TextBox)(rptr.Items[0]).FindControl("Custom1")).Text;
                //p.Custom2 = ((TextBox)(rptr.Items[0]).FindControl("Custom2")).Text;
                //p.Custom3 = ((TextBox)(rptr.Items[0]).FindControl("Custom3")).Text;
                //p.Custom4 = ((TextBox)(rptr.Items[0]).FindControl("Custom4")).Text;
                //p.Custom5 = ((TextBox)(rptr.Items[0]).FindControl("Custom5")).Text;
                    
                p.AvatarID = FormatHelper.SafeToInt(((System.Web.UI.HtmlControls.HtmlInputText)rptr.Items[0].FindControl("AvatarID")).Value);

                var registeringMasterAccount = true;
                if (RegisteringFamily.Text != "0")
                {
                    registeringMasterAccount = false;
                    p.MasterAcctPID = int.Parse(MasterPID.Text);
                }
                if (p.IsValid(BusinessRulesValidationMode.INSERT))
                {
                    p.Insert();

                    var prog = Programs.FetchObject(p.ProgID);
                    var list = new List<Badge>();
                    if (prog.RegistrationBadgeID !=0)
                    {
                        
                        AwardPoints.AwardBadgeToPatron(prog.RegistrationBadgeID, p, ref list);

                        #region replaced by call above

                        //    var now = DateTime.Now;
                        //    var pb = new PatronBadges { BadgeID = prog.RegistrationBadgeID, DateEarned = now, PID = p.PID };
                        //    pb.Insert();

                        //    var EarnedBadge = Badge.GetBadge(prog.RegistrationBadgeID);

                        //    //if badge generates notification, then generate the notification
                        //    if (EarnedBadge.GenNotificationFlag)
                        //    {
                        //        var not = new Notifications
                        //        {
                        //            PID_To = p.PID,
                        //            PID_From = 0,  //0 == System Notification
                        //            Subject = EarnedBadge.NotificationSubject,
                        //            Body = EarnedBadge.NotificationBody,
                        //            isQuestion = false,
                        //            AddedDate = now,
                        //            LastModDate = now,
                        //            AddedUser = p.Username,
                        //            LastModUser = "N/A"
                        //        };
                        //        not.Insert();
                        //    }

                        //    //if badge generates prize, then generate the prize
                        //    if (EarnedBadge.IncludesPhysicalPrizeFlag)
                        //    {
                        //        var ppp = new DAL.PatronPrizes
                        //        {
                        //            PID = p.PID,
                        //            PrizeSource = 1,
                        //            PrizeName = EarnedBadge.PhysicalPrizeName,
                        //            RedeemedFlag = false,
                        //            AddedUser = p.Username,
                        //            LastModUser = "N/A",
                        //            AddedDate = now,
                        //            LastModDate = now
                        //        };

                        //        ppp.Insert();
                        //    }



                        //    // if badge generates award code, then generate the code
                        //    if (EarnedBadge.AssignProgramPrizeCode)
                        //    {
                        //        var RewardCode = "";
                        //        // get the Code value
                        //        // save the code value for the patron
                        //        RewardCode = ProgramCodes.AssignCodeForPatron(p.ProgID, p.ProgID);

                        //        // generate the notification
                        //        var not = new Notifications
                        //        {
                        //            PID_To = p.PID,
                        //            PID_From = 0,  //0 == System Notification
                        //            Subject = EarnedBadge.PCNotificationSubject,
                        //            Body = EarnedBadge.PCNotificationBody.Replace("{ProgramRewardCode}", RewardCode),
                        //            isQuestion = false,
                        //            AddedDate = now,
                        //            LastModDate = now,
                        //            AddedUser = p.Username,
                        //            LastModUser = "N/A"
                        //        };
                        //        not.Insert();
                        //    }

                        #endregion
                    }
                    AwardPoints.AwardBadgeToPatronViaMatchingAwards(p, ref list);

                    var sBadges = "";
                    sBadges = list.Aggregate(sBadges, (current, b) => current + "|" + b.BID.ToString());
                    if(p.IsMasterAccount && sBadges.Length > 0) {
                        // if family account and is master, and has badges, rememebr to show them
                        Session[SessionKey.EarnedBadges] = sBadges;
                    } 
                    if(!p.IsMasterAccount && p.MasterAcctPID == 0 && sBadges.Length > 0) {
                        // if not family master or not family at all and badges, rememebr to show ...
                        Session[SessionKey.EarnedBadges] = sBadges;
                    }

                    if (registeringMasterAccount)
                    {
                        MasterPID.Text = p.PID.ToString();
                        Session["PatronLoggedIn"] = true;
                        Session["Patron"] = p;
                        Session["ProgramID"] = p.ProgID;
                        Session["PatronProgramID"] = p.ProgID;
                        Session["CurrentProgramID"] = p.ProgID;
                        Session["TenantID"] = p.TenID;
                        Session[SessionKey.IsMasterAccount] = p.IsMasterAccount;
                        if (p.IsMasterAccount)
                        {
                            Session["MasterAcctPID"] = p.PID;
                        }
                        else
                        {
                            Session["MasterAcctPID"] = 0;
                        }
                    }
                }
                else
                {
                    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                    foreach (BusinessRulesValidationMessage m in p.ErrorCodes)
                    {
                        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                    }
                    message = string.Format("{0}</ul>", message);
                    lblError.Text = message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = String.Format(SRPResources.ApplicationError1, ex.Message);

                return false;
            }
            return true;
        }


        protected void City_TextChanged(object sender, EventArgs e)
        {
            ReloadLibraryDistrict();
            ReloadSchoolDistrict();
        }

        protected void District_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadLibraryDistrict();
        }

        protected void SchoolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadSchoolDistrict();
        }

        protected void SDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadSchoolDistrict();
        }

        protected void Age_TextChanged(object sender, EventArgs e)
        {
            ReloadSchoolDistrict();
        }

        protected void SchoolGrade_TextChanged(object sender, EventArgs e)
        {
            ReloadSchoolDistrict();
        }

        protected void ReloadSchoolDistrict()
        {
            //*
            var sc = (DropDownList)(rptr.Items[0]).FindControl("SchoolName");
            var st = (DropDownList)(rptr.Items[0]).FindControl("SchoolType");
            var sd = (DropDownList)(rptr.Items[0]).FindControl("SDistrict");
            var ag = (TextBox)(rptr.Items[0]).FindControl("Age");
            var gr = (TextBox)(rptr.Items[0]).FindControl("SchoolGrade");

            var scVal = sc.SelectedValue;
            sc.Items.Clear();
            sc.Items.Add(new ListItem("[Select a Value]", "0"));
            var ds = SchoolCrosswalk.GetFilteredSchoolDDValues(st.SelectedValue.SafeToInt(), sd.SelectedValue.SafeToInt(), city.Text, ag.Text.SafeToInt(), gr.Text.SafeToInt());
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                sc.Items.Add(new ListItem(r["Code"].ToString(), r["CID"].ToString()));
            }

            var si = sc.Items.FindByValue(scVal);
            sc.SelectedValue = si != null ? scVal : "0";
            //*            
        }

        protected void ReloadLibraryDistrict()
        {
            //*
            var pl = (DropDownList)(rptr.Items[0]).FindControl("PrimaryLibrary");
            var dt = (DropDownList)(rptr.Items[0]).FindControl("District");
            var plVal = pl.SelectedValue;
            pl.Items.Clear();
            pl.Items.Add(new ListItem("[Select a Value]", "0"));
            var ds = LibraryCrosswalk.GetFilteredBranchDDValues(int.Parse(dt.SelectedValue), city.Text);
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                pl.Items.Add(new ListItem(r["Code"].ToString(), r["CID"].ToString()));
            }
            var il = pl.Items.FindByValue(plVal);
            pl.SelectedValue = il != null ? plVal : "0";
            //*            
        }


        protected void btnDone_Click(object sender, EventArgs e)
        {
            Response.Redirect(GoToUrl);
        }

        protected void TermsOfUseflag_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((CheckBox)(rptr.Items[0]).FindControl("TermsOfUseflag")).Checked;
        }

        public string GoToUrl
        {
            get
            {
                if (ViewState["gotourl"] == null || ViewState["gotourl"].ToString().Length == 0)
                {
                    ViewState["gotourl"] = "~/Dashboard.aspx";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }




    }
}