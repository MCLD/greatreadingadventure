using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Modules.Patrons;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class PatronCtl : System.Web.UI.UserControl
    {

        public string PatronID
        {
            get { return ViewState["PatronID"].ToString(); }
            set { ViewState["PatronID"] = value; }
        }

        public string MasterPatronID
        {
            get { return (ViewState["MasterPatronID"].ToString()=="" ? "0" : ViewState["MasterPatronID"].ToString()) ; }
            set { ViewState["MasterPatronID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadControl()
        {
            //Session["CURR_PATRON_ID"] = key;
            //Session["CURR_PATRON"] = Patron.FetchObject(key);

            if (Session["CURR_PATRON_MODE"].ToString() == "ADDSUB")
            {
                // Need to adda sub

                var PID1 = int.Parse(Session["CURR_PATRON_ID"].ToString());
                var p1 = Patron.FetchObject(PID1);
                //if (p1.IsMasterAccount)
                //{
                //    PatronID= string.Empty;
                //    MasterPatronID = PID1.ToString();
                //}
                //else
                //{
                //    var p2 = Patron.FetchObject(p1.MasterAcctPID);
                //    PatronID= string.Empty;
                //    MasterPatronID = p2.PID.ToString();
                //}

                if (!p1.IsMasterAccount && p1.MasterAcctPID > 0)
                {
                    var p2 = Patron.FetchObject(p1.MasterAcctPID);
                    PatronID= string.Empty;
                    MasterPatronID = p2.PID.ToString();
                }
                else
                {
                    PatronID= string.Empty;
                    MasterPatronID = PID1.ToString();                    
                }

            }


            if (PatronID != "")
            {
                SA.Text = PatronID;
            }
            else
            {
                SA.Text = "0";
            }

            var key = int.Parse(SA.Text);
            rptr.DataSource = Patron.GetPatronForEdit(key);
            rptr.DataBind();
            if (!IsAdd())
            {
                ((TextBox) rptr.Items[0].FindControl("Username")).Enabled = false;
                ((RequiredFieldValidator)rptr.Items[0].FindControl("rfvUsername")).Enabled = false;
            }
            if (Session["CURR_PATRON_MODE"].ToString() == "ADDSUB")
            {
                var ima = ((CheckBox) (rptr.Items[0]).FindControl("IsMasterAccount"));
                ima.Enabled = false;
                ima.Checked = false;
            }

        }

        public bool IsAdd()
        {
            return SA.Text == "0";
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var masterPage = (IControlRoomMaster)((BaseControlRoomPage)Page).Master; 
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect("Default.aspx");
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                LoadControl();
                if (masterPage != null) masterPage.PageMessage = SRPResources.RefreshOK;
                return;
            }

            lblError.Text= string.Empty;
            Page.Validate();
            if (Page.IsValid)
            {
                if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
                {
                    var p = new Patron();
                    if (!IsAdd())
                    {
                        p = Patron.FetchObject(int.Parse(SA.Text));
                    }

                    DateTime _d;

                    p.Username = ((TextBox)(e.Item).FindControl("Username")).Text;
                    p.NewPassword = ((TextBox)(e.Item).FindControl("Password")).Text;

                    var DOB = e.Item.FindControl("DOB") as TextBox;
                    if (DOB != null && DOB.Text != "")
                    {
                        if (DateTime.TryParse(DOB.Text, out _d)) p.DOB = _d;
                    }

                    

                    p.Age = FormatHelper.SafeToInt(((TextBox)(e.Item).FindControl("Age")).Text);
                    //p.Custom2 = (((TextBox)(e.Item).FindControl("Custom2")).Text);

                    p.MasterAcctPID = int.Parse(MasterPatronID);

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

                    //p.PrimaryLibrary = ((DropDownList)(e.Item).FindControl("PrimaryLibrary")).SelectedValue.SafeToInt();
                    //p.SchoolName = ((DropDownList)(e.Item).FindControl("SchoolName")).SelectedValue;
                    //p.District = ((DropDownList)(e.Item).FindControl("District")).SelectedValue;
                    //p.SDistrict = ((DropDownList)(e.Item).FindControl("SDistrict")).SelectedValue.SafeToInt();
                    //p.SchoolType = ((DropDownList)(e.Item).FindControl("SchoolType")).SelectedValue.SafeToInt();
                   
                    p.PrimaryLibrary = ((DropDownList)(e.Item).FindControl("PrimaryLibrary")).SelectedValue.SafeToInt();
                    p.SchoolName = ((DropDownList)(e.Item).FindControl("SchoolName")).SelectedValue;
                    p.SchoolType = ((DropDownList)(e.Item).FindControl("SchoolType")).SelectedValue.SafeToInt();

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
                    p.LiteracyLevel1 = ((TextBox)(e.Item).FindControl("LiteracyLevel1")).Text.SafeToInt();
                    p.LiteracyLevel2 = ((TextBox)(e.Item).FindControl("LiteracyLevel2")).Text.SafeToInt();
                    try {
                        p.ParentPermFlag = ((CheckBox)(e.Item).FindControl("ParentPermFlag")).Checked;
                        p.Over18Flag = ((CheckBox)(e.Item).FindControl("Over18Flag")).Checked;
                        p.ShareFlag = ((CheckBox)(e.Item).FindControl("ShareFlag")).Checked;
                        p.TermsOfUseflag = ((CheckBox)(e.Item).FindControl("TermsOfUseflag")).Checked;
                    }
                    catch { }



                    var cr = CustomRegistrationFields.FetchObject();
                    p.Custom1 = cr.DDValues1 == "" ? ((TextBox)(e.Item).FindControl("Custom1")).Text : ((DropDownList)(e.Item).FindControl("Custom1DD")).SelectedValue;
                    p.Custom2 = cr.DDValues2 == "" ? ((TextBox)(e.Item).FindControl("Custom2")).Text : ((DropDownList)(e.Item).FindControl("Custom2DD")).SelectedValue;
                    p.Custom3 = cr.DDValues3 == "" ? ((TextBox)(e.Item).FindControl("Custom3")).Text : ((DropDownList)(e.Item).FindControl("Custom3DD")).SelectedValue;
                    p.Custom4 = cr.DDValues4 == "" ? ((TextBox)(e.Item).FindControl("Custom4")).Text : ((DropDownList)(e.Item).FindControl("Custom4DD")).SelectedValue;
                    p.Custom5 = cr.DDValues5 == "" ? ((TextBox)(e.Item).FindControl("Custom5")).Text : ((DropDownList)(e.Item).FindControl("Custom5DD")).SelectedValue;

                    p.AvatarID = FormatHelper.SafeToInt(((DropDownList)e.Item.FindControl("AvatarID")).SelectedValue);
                    p.ProgID = FormatHelper.SafeToInt(((DropDownList)e.Item.FindControl("ProgID")).SelectedValue);
                    p.IsMasterAccount = ((CheckBox) (e.Item).FindControl("IsMasterAccount")).Checked;
                    // do the save

                    
                        
                    if (IsAdd())                 
                    {
                        if (p.IsValid(BusinessRulesValidationMode.INSERT))
                        {
                            p.Insert();
                            PatronID = p.PID.ToString();
                            SA.Text = PatronID;
                            Session["CURR_PATRON_MODE"] = "EDIT";
                            LoadControl();
                            
                            masterPage.PageMessage = SRPResources.AddedOK;

                            if (e.CommandName.ToLower() == "saveandback")
                            {
                                Response.Redirect("Default.aspx");
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
                            masterPage.PageError = message;                            
                        }

                        var parent = Patron.FetchObject(p.MasterAcctPID);
                        if (parent != null && !parent.IsMasterAccount)
                        {
                            parent.IsMasterAccount = true;
                            parent.Update();
                            Session["CURR_PATRON"] = parent;
                            PatronsRibbon.GetByAppContext((BaseControlRoomPage)Page);
                        }
                    }
                    else
                    {
                        if (p.IsValid(BusinessRulesValidationMode.UPDATE))
                        {
                            p.Update();
                            masterPage.PageMessage = SRPResources.SaveOK;

                            if (e.CommandName.ToLower() == "saveandback")
                            {
                                Response.Redirect("Default.aspx");
                            }
                            else
                            {
                                LoadControl();
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
                            masterPage.PageError = message;
                        }
                            
                    }

                }
            }
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

            ctl = (DropDownList)e.Item.FindControl("AvatarID");
            txt = (TextBox)e.Item.FindControl("AvatarIDTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("ProgID");
            txt = (TextBox)e.Item.FindControl("ProgIDTxt");
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

        protected void rptr_PreRender(object sender, EventArgs e)
        {
            var pwd = (TextBox)((Repeater)sender).Items[0].FindControl("Password");
            pwd.Attributes.Add("Value", pwd.Text);
        }

    }
}