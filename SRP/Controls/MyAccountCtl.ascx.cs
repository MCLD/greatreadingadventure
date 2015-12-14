using System;
using System.Data;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.Controls
{
    public partial class MyAccountCtl : System.Web.UI.UserControl
    {
        protected string SaveButtonText { get; set; }
        private CustomRegistrationFields customFields;
        protected CustomRegistrationFields CustomFields {
            get {
                if(customFields == null) {
                    customFields = CustomRegistrationFields.FetchObject();
                }
                return customFields;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var basePage = (BaseSRPPage)Page;
            if (!IsPostBack)
            {
                var patron = (Patron)Session["Patron"];
                rptr.DataSource = Patron.GetPatronForEdit(patron.PID);
                rptr.DataBind();
   
                basePage.TranslateStrings(rptr);
            }
            this.SaveButtonText = basePage.GetResourceString("family-member-add-save");
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Page.IsValid)
            {
                if(e.CommandName == "save")
                {

                    var p = Patron.FetchObject(((Patron) Session["Patron"]).PID);
                    DateTime _d;

                    var DOB = e.Item.FindControl("DOB") as TextBox;
                    if (DOB != null && !string.IsNullOrEmpty(DOB.Text))
                    {
                        if (DateTime.TryParse(DOB.Text, out _d)) p.DOB = _d;
                    }

                    p.Age = FormatHelper.SafeToInt(((TextBox)(e.Item).FindControl("Age")).Text);
                    //p.Custom2 = (((TextBox)(e.Item).FindControl("Custom2")).Text);

                    p.SchoolGrade = ((TextBox)(e.Item).FindControl("SchoolGrade")).Text;
                    p.FirstName = ((TextBox)(e.Item).FindControl("FirstName")).Text;
                    p.MiddleName = ((TextBox)(e.Item).FindControl("MiddleName")).Text;
                    p.LastName = ((TextBox)(e.Item).FindControl("LastName")).Text;
                    p.Gender = ((DropDownList)(e.Item).FindControl("Gender")).SelectedValue;
                    p.EmailAddress = ((TextBox)(e.Item).FindControl("EmailAddress")).Text;
                    p.PhoneNumber = ((TextBox)(e.Item).FindControl("PhoneNumber")).Text;
                        p.PhoneNumber = FormatHelper.FormatPhoneNumber(p.PhoneNumber);
                    p.StreetAddress1 = ((TextBox)(e.Item).FindControl("StreetAddress1")).Text;
                    p.StreetAddress2 = ((TextBox)(e.Item).FindControl("StreetAddress2")).Text;
                    p.City = ((TextBox)(e.Item).FindControl("City")).Text;
                    p.State = ((TextBox)(e.Item).FindControl("State")).Text;
                    p.ZipCode = ((TextBox)(e.Item).FindControl("ZipCode")).Text;
                    p.ZipCode = FormatHelper.FormatZipCode(p.ZipCode);

                    p.Country = ((TextBox)(e.Item).FindControl("Country")).Text;
                    p.County = ((TextBox)(e.Item).FindControl("County")).Text;
                    p.ParentGuardianFirstName = ((TextBox)(e.Item).FindControl("ParentGuardianFirstName")).Text;
                    p.ParentGuardianLastName = ((TextBox)(e.Item).FindControl("ParentGuardianLastName")).Text;
                    p.ParentGuardianMiddleName = ((TextBox)(e.Item).FindControl("ParentGuardianMiddleName")).Text;
                    p.LibraryCard = ((TextBox)(e.Item).FindControl("LibraryCard")).Text;

                    //p.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("PrimaryLibrary")).SelectedValue);
                    //p.SchoolName = ((DropDownList)(e.Item).FindControl("SchoolName")).SelectedValue;
                    //p.SchoolType = FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("SchoolType")).SelectedValue);

                    //p.District = ((DropDownList)(e.Item).FindControl("District")).SelectedValue;
                    //p.SDistrict = ((DropDownList)(e.Item).FindControl("SDistrict")).SelectedValue.SafeToInt();

                    p.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("PrimaryLibrary")).SelectedValue);
                    p.SchoolName = ((DropDownList)(e.Item).FindControl("SchoolName")).SelectedValue;
                    p.SchoolType = FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("SchoolType")).SelectedValue);

                    var lc = LibraryCrosswalk.FetchObjectByLibraryID(p.PrimaryLibrary);
                    if (lc != null)
                    {
                        p.District = lc.DistrictID == 0 ? ((DropDownList)(e.Item).FindControl("District")).SelectedValue : lc.DistrictID.ToString();
                    }
                    else
                    {
                        p.District = ((DropDownList)(e.Item).FindControl("District")).SelectedValue;
                    }
                    var sc = SchoolCrosswalk.FetchObjectBySchoolID(p.SchoolName.SafeToInt());
                    if (sc != null)
                    {
                        p.SDistrict = sc.DistrictID == 0 ? ((DropDownList)(e.Item).FindControl("SDistrict")).SelectedValue.SafeToInt() : sc.DistrictID;
                        p.SchoolType = sc.SchTypeID == 0 ? FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("SchoolType")).SelectedValue) : sc.SchTypeID;
                    }
                    else
                    {
                        p.SDistrict = ((DropDownList)(e.Item).FindControl("SDistrict")).SelectedValue.SafeToInt();
                    }


                    p.Teacher = ((TextBox)(e.Item).FindControl("Teacher")).Text;
                    p.GroupTeamName = ((TextBox)(e.Item).FindControl("GroupTeamName")).Text;
                    p.LiteracyLevel1 = FormatHelper.SafeToInt(((TextBox)(e.Item).FindControl("LiteracyLevel1")).Text);
                    p.LiteracyLevel2 = FormatHelper.SafeToInt(((TextBox)(e.Item).FindControl("LiteracyLevel2")).Text);

                    p.Custom1 = string.IsNullOrEmpty(this.CustomFields.DDValues1)
                        ? ((TextBox)(e.Item).FindControl("Custom1")).Text
                        : ((DropDownList)(e.Item).FindControl("Custom1DD")).SelectedValue;
                    p.Custom2 = string.IsNullOrEmpty(this.CustomFields.DDValues2)
                        ? ((TextBox)(e.Item).FindControl("Custom2")).Text 
                        : ((DropDownList)(e.Item).FindControl("Custom2DD")).SelectedValue;
                    p.Custom3 = string.IsNullOrEmpty(this.CustomFields.DDValues3)
                        ? ((TextBox)(e.Item).FindControl("Custom3")).Text 
                        : ((DropDownList)(e.Item).FindControl("Custom3DD")).SelectedValue;
                    p.Custom4 = string.IsNullOrEmpty(this.CustomFields.DDValues4)
                        ? ((TextBox)(e.Item).FindControl("Custom4")).Text 
                        : ((DropDownList)(e.Item).FindControl("Custom4DD")).SelectedValue;
                    p.Custom5 = string.IsNullOrEmpty(this.CustomFields.DDValues5)
                        ? ((TextBox)(e.Item).FindControl("Custom5")).Text 
                        : ((DropDownList)(e.Item).FindControl("Custom5DD")).SelectedValue;
                    
                    //p.AvatarID = FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("AvatarID")).SelectedValue);
                    p.AvatarID = FormatHelper.SafeToInt(((System.Web.UI.HtmlControls.HtmlInputText)e.Item.FindControl("AvatarID")).Value);
                    // do the save
                    p.Update();
                    Session["Patron"] = p;

                    rptr.DataSource = Patron.GetPatronForEdit(p.PID);
                    rptr.DataBind();

                    ((BaseSRPPage)Page).TranslateStrings(rptr);

                    Session[SessionKey.PatronMessage] = "Your account information has been updated!";
                    Session[SessionKey.PatronMessageGlyphicon] = "check";
                }
            }
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
        protected void rptr_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ctl = (DropDownList)e.Item.FindControl("Gender");
            var txt = (TextBox)e.Item.FindControl("GenderTxt");
            var i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("PrimaryLibrary"); 
            txt = (TextBox)e.Item.FindControl("PrimaryLibraryTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;


            ctl = (DropDownList)e.Item.FindControl("SchoolType"); 
            txt = (TextBox)e.Item.FindControl("SchoolTypeTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            //--
            ctl = (DropDownList)e.Item.FindControl("SchoolName");
            txt = (TextBox)e.Item.FindControl("SchoolNameTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("SDistrict");
            txt = (TextBox)e.Item.FindControl("SDistrictTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("District");
            txt = (TextBox)e.Item.FindControl("DistrictTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;
            //--

            var familyListButton = e.Item.FindControl("FamilyAccountList");
            var familyAddbutton = e.Item.FindControl("FamilyAccountAdd");
            var showFamilyList = Session[SessionKey.IsMasterAccount] as bool? == true;
            if(familyListButton != null) {
                familyListButton.Visible = showFamilyList;
            }
            if(familyAddbutton != null) {
                var patron = e.Item.DataItem as DataRowView;
                if(patron != null
                   && patron["Over18Flag"] as bool? == true
                   && patron["MasterAcctPID"] as int? == 0) {
                    familyAddbutton.Visible = !showFamilyList;
                } else {
                    familyAddbutton.Visible = false;
                }
            }


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

        protected bool CanAddFamilyAccounts(string dob, string age)
        {
            var actualAge = 0;
            int.TryParse(age, out actualAge);
            
            if (!string.IsNullOrEmpty(dob))
            {
                try
                {
                    var actualAge2 = DateTime.Now.Year - DateTime.Parse(dob).Year;
                    if (actualAge2 > actualAge) actualAge = actualAge2;
                }
                catch
                {
                    int.TryParse(age, out actualAge);
                }

            }
            return (actualAge >= 18);
        }
    }
}