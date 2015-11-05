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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var patron = (Patron)Session["Patron"];
                rptr.DataSource = Patron.GetPatronForEdit(patron.PID);
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);
            }
        }

        protected void dv_DataBound(object sender, EventArgs e)
        {
            //if (dv.CurrentMode == DetailsViewMode.Edit)
            //{
            //    //var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
            //    //if (control!=null) control.ProcessRender();
            //}

            var cr = CustomRegistrationFields.FetchObject();
 


        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/PatronList.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }

            if (e.CommandName.ToLower() == "childadd")
            {
                Response.Redirect(returnURL);
            }

            //if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            //{
            //    try
            //    {
            //        var obj = new Patron();
            //        int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
            //        obj.Fetch(pk);

            //        obj.IsMasterAccount = ((CheckBox)((DetailsView)sender).FindControl("IsMasterAccount")).Checked;
            //        obj.MasterAcctPID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("MasterAcctPID")).SelectedValue);
            //        obj.MasterAcctPID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MasterAcctPID")).Text);
            //        obj.Username = ((TextBox)((DetailsView)sender).FindControl("Username")).Text;
            //        obj.Password = ((TextBox)((DetailsView)sender).FindControl("Password")).Text;
            //        obj.DOB = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("DOB")).Text);
            //        obj.Age = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("Age")).SelectedValue);
            //        obj.Age = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Age")).Text);
            //        obj.SchoolGrade = ((TextBox)((DetailsView)sender).FindControl("SchoolGrade")).Text;
            //        obj.ProgID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("ProgID")).SelectedValue);
            //        obj.ProgID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("ProgID")).Text);
            //        obj.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
            //        obj.MiddleName = ((TextBox)((DetailsView)sender).FindControl("MiddleName")).Text;
            //        obj.LastName = ((TextBox)((DetailsView)sender).FindControl("LastName")).Text;
            //        obj.Gender = ((TextBox)((DetailsView)sender).FindControl("Gender")).Text;
            //        obj.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("EmailAddress")).Text;
            //        obj.PhoneNumber = ((TextBox)((DetailsView)sender).FindControl("PhoneNumber")).Text;
            //        obj.StreetAddress1 = ((TextBox)((DetailsView)sender).FindControl("StreetAddress1")).Text;
            //        obj.StreetAddress2 = ((TextBox)((DetailsView)sender).FindControl("StreetAddress2")).Text;
            //        obj.City = ((TextBox)((DetailsView)sender).FindControl("City")).Text;
            //        obj.State = ((TextBox)((DetailsView)sender).FindControl("State")).Text;
            //        obj.ZipCode = ((TextBox)((DetailsView)sender).FindControl("ZipCode")).Text;
            //        obj.Country = ((TextBox)((DetailsView)sender).FindControl("Country")).Text;
            //        obj.County = ((TextBox)((DetailsView)sender).FindControl("County")).Text;
            //        obj.ParentGuardianFirstName = ((TextBox)((DetailsView)sender).FindControl("ParentGuardianFirstName")).Text;
            //        obj.ParentGuardianLastName = ((TextBox)((DetailsView)sender).FindControl("ParentGuardianLastName")).Text;
            //        obj.ParentGuardianMiddleName = ((TextBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName")).Text;
            //        obj.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PrimaryLibrary")).SelectedValue);
            //        obj.PrimaryLibrary = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PrimaryLibrary")).Text);
            //        obj.LibraryCard = ((TextBox)((DetailsView)sender).FindControl("LibraryCard")).Text;
            //        obj.SchoolName = ((TextBox)((DetailsView)sender).FindControl("SchoolName")).Text;
            //        obj.District = ((TextBox)((DetailsView)sender).FindControl("District")).Text;
            //        obj.Teacher = ((TextBox)((DetailsView)sender).FindControl("Teacher")).Text;
            //        obj.GroupTeamName = ((TextBox)((DetailsView)sender).FindControl("GroupTeamName")).Text;
            //        obj.SchoolType = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("SchoolType")).SelectedValue);
            //        obj.SchoolType = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("SchoolType")).Text);
            //        obj.LiteracyLevel1 = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("LiteracyLevel1")).SelectedValue);
            //        obj.LiteracyLevel1 = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LiteracyLevel1")).Text);
            //        obj.LiteracyLevel2 = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("LiteracyLevel2")).SelectedValue);
            //        obj.LiteracyLevel2 = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LiteracyLevel2")).Text);
            //        obj.ParentPermFlag = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag")).Checked;
            //        obj.Over18Flag = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag")).Checked;
            //        obj.ShareFlag = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag")).Checked;
            //        obj.TermsOfUseflag = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag")).Checked;
            //        obj.Custom1 = ((TextBox)((DetailsView)sender).FindControl("Custom1")).Text;
            //        obj.Custom2 = ((TextBox)((DetailsView)sender).FindControl("Custom2")).Text;
            //        obj.Custom3 = ((TextBox)((DetailsView)sender).FindControl("Custom3")).Text;
            //        obj.Custom4 = ((TextBox)((DetailsView)sender).FindControl("Custom4")).Text;
            //        obj.Custom5 = ((TextBox)((DetailsView)sender).FindControl("Custom5")).Text;
            //        obj.AvatarID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("AvatarID")).SelectedValue);
            //        obj.AvatarID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AvatarID")).Text);

            //        obj.LastModDate = DateTime.Now;
            //        obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

            //        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
            //        {
            //            obj.Update();
            //            if (e.CommandName.ToLower() == "saveandback")
            //            {
            //                Response.Redirect(returnURL);
            //            }

            //            odsData.DataBind();
            //            dv.DataBind();
            //            dv.ChangeMode(DetailsViewMode.Edit);

            //            var masterPage = (IControlRoomMaster)Master;
            //            masterPage.PageMessage = SRPResources.SaveOK;
            //        }
            //        else
            //        {
            //            var masterPage = (IControlRoomMaster)Master;
            //            string message = String.Format(SRPResources.ApplicationError1, "<ul>");
            //            foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
            //            {
            //                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
            //            }
            //            message = string.Format("{0}</ul>", message);
            //            masterPage.PageError = message;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        var masterPage = (IControlRoomMaster)Master;
            //        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            //    }
            //}
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "childadd")
            {
                Response.Redirect("~/AddChildAccount.aspx");
            }

            lblError.Text = "";
            if (Page.IsValid)
            {
                if(e.CommandName == "save")
                {

                    var p = Patron.FetchObject(((Patron) Session["Patron"]).PID);
                    DateTime _d;

                    var DOB = e.Item.FindControl("DOB") as TextBox;
                    if (DOB != null && DOB.Text != "")
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
                    
                    var cr = CustomRegistrationFields.FetchObject();
                    p.Custom1 = cr.DDValues1 == "" ? ((TextBox)(e.Item).FindControl("Custom1")).Text : ((DropDownList)(e.Item).FindControl("Custom1DD")).SelectedValue;
                    p.Custom2 = cr.DDValues2 == "" ? ((TextBox)(e.Item).FindControl("Custom2")).Text : ((DropDownList)(e.Item).FindControl("Custom2DD")).SelectedValue;
                    p.Custom3 = cr.DDValues3 == "" ? ((TextBox)(e.Item).FindControl("Custom3")).Text : ((DropDownList)(e.Item).FindControl("Custom3DD")).SelectedValue;
                    p.Custom4 = cr.DDValues4 == "" ? ((TextBox)(e.Item).FindControl("Custom4")).Text : ((DropDownList)(e.Item).FindControl("Custom4DD")).SelectedValue;
                    p.Custom5 = cr.DDValues5 == "" ? ((TextBox)(e.Item).FindControl("Custom5")).Text : ((DropDownList)(e.Item).FindControl("Custom5DD")).SelectedValue;
                    
                    //p.AvatarID = FormatHelper.SafeToInt(((DropDownList)(e.Item).FindControl("AvatarID")).SelectedValue);
                    p.AvatarID = FormatHelper.SafeToInt(((System.Web.UI.HtmlControls.HtmlInputText)e.Item.FindControl("AvatarID")).Value);
                    // do the save
                    p.Update();
                    Session["Patron"] = p;

                    rptr.DataSource = Patron.GetPatronForEdit(p.PID);
                    rptr.DataBind();

                    ((BaseSRPPage)Page).TranslateStrings(rptr);
                    

                    lblError.Text = "Your information has been saved.<br><br>";
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
            if(familyListButton != null) {
                familyListButton.Visible = Session[SessionKey.IsMasterAccount] as bool? == true;
            }


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

        protected bool CanAddChildAccounts(string dob, string age)
        {
            var actualAge = 0;
            int.TryParse(age, out actualAge);
            
            if (dob!="")
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