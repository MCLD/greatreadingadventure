using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Text;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class AddFamilyMemberControl : System.Web.UI.UserControl {
        private CustomRegistrationFields customFields;
        protected CustomRegistrationFields CustomFields {
            get {
                if(customFields == null) {
                    customFields = CustomRegistrationFields.FetchObject();
                }
                return customFields;
            }
        }
        protected string SaveButtonText { get; set; }

        protected void Page_Load(object sender, EventArgs e) {
            var basePage = (BaseSRPPage)Page;
            if(!IsPostBack) {
                if(string.IsNullOrEmpty(Request["SA"])
                   && (Session["SA"] == null || string.IsNullOrEmpty(Session["SA"].ToString()))) {
                    Session["SA"] = -1;
                }

                if(!string.IsNullOrEmpty(Request["SA"])) {
                    SA.Text = Request["SA"];
                    Session["SA"] = SA.Text;
                } else {
                    SA.Text = Session["SA"].ToString();
                }

                //var patron = (Patron)Session["Patron"];
                Patron patron = null;
                if((int)Session["MasterAcctPID"] == 0) {
                    // we are the parent
                    patron = (Patron)Session["Patron"];
                } else {
                    patron = Patron.FetchObject((int)Session["MasterAcctPID"]);
                }
                var ds = Patron.GetPatronForEdit(patron.PID);
                ds.Tables[0].Rows[0]["ParentGuardianFirstName"] = ds.Tables[0].Rows[0]["FirstName"];
                ds.Tables[0].Rows[0]["ParentGuardianMiddleName"] = ds.Tables[0].Rows[0]["MiddleName"];
                ds.Tables[0].Rows[0]["ParentGuardianLastName"] = ds.Tables[0].Rows[0]["LastName"];

                ds.Tables[0].Rows[0]["Username"] = string.Empty;
                ds.Tables[0].Rows[0]["Password"] = string.Empty;
                ds.Tables[0].Rows[0]["Age"] = 0;
                ds.Tables[0].Rows[0]["DOB"] = DBNull.Value;
                ds.Tables[0].Rows[0]["SchoolGrade"] = string.Empty;
                ds.Tables[0].Rows[0]["ProgID"] = 0;
                ds.Tables[0].Rows[0]["FirstName"] = string.Empty;
                ds.Tables[0].Rows[0]["MiddleName"] = string.Empty;
                ds.Tables[0].Rows[0]["Gender"] = string.Empty;
                ds.Tables[0].Rows[0]["LiteracyLevel1"] = 0;
                ds.Tables[0].Rows[0]["LiteracyLevel2"] = 0;

                rptr.DataSource = ds;
                rptr.DataBind();

                basePage.TranslateStrings(rptr);
            }
            this.SaveButtonText = basePage.GetResourceString("myaccount-save");
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {
            if(Page.IsValid && e.CommandName.Equals("save", StringComparison.OrdinalIgnoreCase)) {
                if(SaveAccount()) {
                    Response.Redirect("~/Account/FamilyAccountList.aspx");
                }
            }
        }

        protected void rptr_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            DropDownList ctl = null;
            TextBox txt = null;
            ListItem i = null;

            ctl = (DropDownList)e.Item.FindControl("PrimaryLibrary");
            txt = (TextBox)e.Item.FindControl("PrimaryLibraryTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if(i != null)
                ctl.SelectedValue = txt.Text;


            ctl = (DropDownList)e.Item.FindControl("SchoolType");
            txt = (TextBox)e.Item.FindControl("SchoolTypeTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if(i != null)
                ctl.SelectedValue = txt.Text;

            //--
            ctl = (DropDownList)e.Item.FindControl("SchoolName");
            txt = (TextBox)e.Item.FindControl("SchoolNameTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if(i != null)
                ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("SDistrict");
            txt = (TextBox)e.Item.FindControl("SDistrictTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if(i != null)
                ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("District");
            txt = (TextBox)e.Item.FindControl("DistrictTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if(i != null)
                ctl.SelectedValue = txt.Text;

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


        public bool SaveAccount() {
            try {
                //var patron = (Patron)Session["Patron"];
                Patron patron = null;
                if((int)Session["MasterAcctPID"] == 0) {
                    // we are the parent
                    patron = (Patron)Session["Patron"];
                } else {
                    patron = Patron.FetchObject((int)Session["MasterAcctPID"]);
                }
                var p = new Patron();
                DateTime _d;
                var DOB = rptr.Items[0].FindControl("DOB") as TextBox;
                if(DOB != null && !string.IsNullOrEmpty(DOB.Text)) {
                    if(DateTime.TryParse(DOB.Text, out _d))
                        p.DOB = _d;
                }

                p.Age = FormatHelper.SafeToInt(((TextBox)rptr.Items[0].FindControl("Age")).Text);

                p.ProgID = FormatHelper.SafeToInt(((DropDownList)rptr.Items[0].FindControl("ProgID")).SelectedValue);
                p.Username = ((TextBox)rptr.Items[0].FindControl("Username")).Text;
                p.NewPassword = ((TextBox)rptr.Items[0].FindControl("Password")).Text;

                p.IsMasterAccount = false;
                p.MasterAcctPID = patron.PID;

                p.SchoolGrade = ((TextBox)rptr.Items[0].FindControl("SchoolGrade")).Text;
                p.FirstName = ((TextBox)rptr.Items[0].FindControl("FirstName")).Text;
                p.MiddleName = ((TextBox)rptr.Items[0].FindControl("MiddleName")).Text;
                p.LastName = ((TextBox)rptr.Items[0].FindControl("LastName")).Text;
                p.Gender = ((DropDownList)rptr.Items[0].FindControl("Gender")).SelectedValue;
                p.EmailAddress = ((TextBox)rptr.Items[0].FindControl("EmailAddress")).Text;
                p.PhoneNumber = ((TextBox)rptr.Items[0].FindControl("PhoneNumber")).Text;
                p.PhoneNumber = FormatHelper.FormatPhoneNumber(p.PhoneNumber);
                p.StreetAddress1 = ((TextBox)rptr.Items[0].FindControl("StreetAddress1")).Text;
                p.StreetAddress2 = ((TextBox)rptr.Items[0].FindControl("StreetAddress2")).Text;
                p.City = ((TextBox)rptr.Items[0].FindControl("City")).Text;
                p.State = ((TextBox)rptr.Items[0].FindControl("State")).Text;
                p.ZipCode = ((TextBox)rptr.Items[0].FindControl("ZipCode")).Text;
                p.ZipCode = FormatHelper.FormatZipCode(p.ZipCode);

                p.Country = ((TextBox)rptr.Items[0].FindControl("Country")).Text;
                p.County = ((TextBox)rptr.Items[0].FindControl("County")).Text;
                p.ParentGuardianFirstName = ((TextBox)rptr.Items[0].FindControl("ParentGuardianFirstName")).Text;
                p.ParentGuardianLastName = ((TextBox)rptr.Items[0].FindControl("ParentGuardianLastName")).Text;
                p.ParentGuardianMiddleName = ((TextBox)rptr.Items[0].FindControl("ParentGuardianMiddleName")).Text;
                p.LibraryCard = ((TextBox)rptr.Items[0].FindControl("LibraryCard")).Text;

                //p.District = ((DropDownList)rptr.Items[0].FindControl("District")).SelectedValue;
                //p.SDistrict = ((DropDownList)rptr.Items[0].FindControl("SDistrict")).SelectedValue.SafeToInt();

                p.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList)rptr.Items[0].FindControl("PrimaryLibrary")).SelectedValue);
                p.SchoolName = ((DropDownList)rptr.Items[0].FindControl("SchoolName")).SelectedValue;
                p.SchoolType = FormatHelper.SafeToInt(((DropDownList)rptr.Items[0].FindControl("SchoolType")).SelectedValue);

                var lc = LibraryCrosswalk.FetchObjectByLibraryID(p.PrimaryLibrary);
                if(lc != null) {
                    p.District = lc.DistrictID == 0 ? ((DropDownList)rptr.Items[0].FindControl("District")).SelectedValue : lc.DistrictID.ToString();
                } else {
                    p.District = ((DropDownList)rptr.Items[0].FindControl("District")).SelectedValue;
                }
                var sc = SchoolCrosswalk.FetchObjectBySchoolID(p.SchoolName.SafeToInt());
                if(sc != null) {
                    p.SDistrict = sc.DistrictID == 0 ? ((DropDownList)rptr.Items[0].FindControl("SDistrict")).SelectedValue.SafeToInt() : sc.DistrictID;
                    p.SchoolType = sc.SchTypeID == 0 ? FormatHelper.SafeToInt(((DropDownList)rptr.Items[0].FindControl("SchoolType")).SelectedValue) : sc.SchTypeID;
                } else {
                    p.SDistrict = ((DropDownList)rptr.Items[0].FindControl("SDistrict")).SelectedValue.SafeToInt();
                }

                p.Teacher = ((TextBox)rptr.Items[0].FindControl("Teacher")).Text;
                p.GroupTeamName = ((TextBox)rptr.Items[0].FindControl("GroupTeamName")).Text;
                p.LiteracyLevel1 = FormatHelper.SafeToInt(((TextBox)rptr.Items[0].FindControl("LiteracyLevel1")).Text);
                p.LiteracyLevel2 = FormatHelper.SafeToInt(((TextBox)rptr.Items[0].FindControl("LiteracyLevel2")).Text);

                p.ParentPermFlag = true;
                p.Over18Flag = p.Age >= 18;
                p.ShareFlag = ((CheckBox)rptr.Items[0].FindControl("ShareFlag")).Checked;
                p.TermsOfUseflag = true;

                p.Custom1 = string.IsNullOrEmpty(this.CustomFields.DDValues1)
                    ? ((TextBox)rptr.Items[0].FindControl("Custom1")).Text
                    : ((DropDownList)rptr.Items[0].FindControl("Custom1DD")).SelectedValue;
                p.Custom2 = string.IsNullOrEmpty(this.CustomFields.DDValues2)
                    ? ((TextBox)rptr.Items[0].FindControl("Custom2")).Text
                    : ((DropDownList)rptr.Items[0].FindControl("Custom2DD")).SelectedValue;
                p.Custom3 = string.IsNullOrEmpty(this.CustomFields.DDValues3)
                    ? ((TextBox)rptr.Items[0].FindControl("Custom3")).Text
                    : ((DropDownList)rptr.Items[0].FindControl("Custom3DD")).SelectedValue;
                p.Custom4 = string.IsNullOrEmpty(this.CustomFields.DDValues4)
                    ? ((TextBox)rptr.Items[0].FindControl("Custom4")).Text
                    : ((DropDownList)rptr.Items[0].FindControl("Custom4DD")).SelectedValue;
                p.Custom5 = string.IsNullOrEmpty(this.CustomFields.DDValues5)
                    ? ((TextBox)rptr.Items[0].FindControl("Custom5")).Text
                    : ((DropDownList)rptr.Items[0].FindControl("Custom5DD")).SelectedValue;

                p.AvatarID = FormatHelper.SafeToInt(((System.Web.UI.HtmlControls.HtmlInputText)rptr.Items[0].FindControl("AvatarID")).Value);

                if(p.IsValid(BusinessRulesValidationMode.INSERT)) {
                    p.Insert();

                    var prog = Programs.FetchObject(p.ProgID);
                    var list = new List<Badge>();
                    if(prog.RegistrationBadgeID != 0) {
                        AwardPoints.AwardBadgeToPatron(prog.RegistrationBadgeID, p, ref list);
                    }
                    try {
                        this.Log().Debug("Running AwardBadgeToPatronViaMatchingAwards on patron ", p.PID);
                    } catch(Exception) { }

                    try {
                        AwardPoints.AwardBadgeToPatronViaMatchingAwards(p, ref list);
                    } catch (Exception ex) {
                        this.Log().Error("Unable to run AwardBadgeToPatronViaMatchingAwards: {0} - {1}",
                                         ex.Message,
                                         ex.StackTrace);
                    }

                    patron.IsMasterAccount = true;
                    patron.Update();

                    // update patron session for things like MasterAcctPID and IsMasterAccount
                    new SessionTools(Session).EstablishPatron(patron);
                   
                    new SessionTools(Session).AlertPatron("Your family member has been added!",
                        glyphicon: "check");

                } else {
                    StringBuilder message = new StringBuilder("<strong>");
                    message.AppendFormat(SRPResources.ApplicationError1, "<ul>");
                    foreach(BusinessRulesValidationMessage m in p.ErrorCodes) {
                        message.AppendFormat("<li>{0}</li>", m.ErrorMessage);
                    }
                    message.Append("</ul></strong>");
                    new SessionTools(Session).AlertPatron(message.ToString(),
                        PatronMessageLevels.Warning,
                        "exclamation-sign");
                    return false;
                }
            } catch(Exception ex) {
                this.Log().Error(string.Format("An exception was thrown adding a family member: {0}",
                                               ex.Message));
                this.Log().Error(string.Format("Stack trace: {0}", ex.StackTrace));
                new SessionTools(Session).AlertPatron(string.Format("<strong>{0}</strong>",
                                                      ex.Message),
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
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
            //*
            var sc = (DropDownList)rptr.Items[0].FindControl("SchoolName");
            var st = (DropDownList)rptr.Items[0].FindControl("SchoolType");
            var sd = (DropDownList)rptr.Items[0].FindControl("SDistrict");
            var ag = (TextBox)rptr.Items[0].FindControl("Age");
            var gr = (TextBox)rptr.Items[0].FindControl("SchoolGrade");
            var city = (TextBox)rptr.Items[0].FindControl("City");

            var scVal = sc.SelectedValue;
            sc.Items.Clear();
            sc.Items.Add(new ListItem("[Select a Value]", "0"));
            var ds = SchoolCrosswalk.GetFilteredSchoolDDValues(st.SelectedValue.SafeToInt(), sd.SelectedValue.SafeToInt(), city.Text, ag.Text.SafeToInt(), gr.Text.SafeToInt());
            foreach(DataRow r in ds.Tables[0].Rows) {
                sc.Items.Add(new ListItem(r["Description"].ToString(), r["CID"].ToString()));
            }

            var si = sc.Items.FindByValue(scVal);
            sc.SelectedValue = si != null ? scVal : "0";
            //*            
        }

        protected void ReloadLibraryDistrict() {
            //*
            var pl = (DropDownList)rptr.Items[0].FindControl("PrimaryLibrary");
            var dt = (DropDownList)rptr.Items[0].FindControl("District");
            var city = (TextBox)rptr.Items[0].FindControl("City");

            var plVal = pl.SelectedValue;
            pl.Items.Clear();
            pl.Items.Add(new ListItem("[Select a Value]", "0"));
            var ds = LibraryCrosswalk.GetFilteredBranchDDValues(int.Parse(dt.SelectedValue), city.Text);
            foreach(DataRow r in ds.Tables[0].Rows) {
                pl.Items.Add(new ListItem(r["Description"].ToString(), r["CID"].ToString()));
            }
            var il = pl.Items.FindByValue(plVal);
            pl.SelectedValue = il != null ? plVal : "0";
            //*            
        }

    }
}