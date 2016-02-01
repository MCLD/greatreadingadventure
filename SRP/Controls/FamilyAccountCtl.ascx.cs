using System;
using System.Data;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.Controls
{
    public partial class FamilyAccountCtl : System.Web.UI.UserControl
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

                //var patron = (Patron)Session["Patron"];
                //if (!patron.IsMasterAccount)
                if (Session[SessionKey.IsMasterAccount] == null || !(bool)Session[SessionKey.IsMasterAccount])
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }

                if (!Patron.CanManageSubAccount((int)Session["MasterAcctPID"], int.Parse(SA.Text)))
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }
                var sa = Patron.FetchObject(int.Parse(SA.Text));
                rptr.DataSource = Patron.GetPatronForEdit(sa.PID);
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);

            }
        }

        protected void dv_DataBound(object sender, EventArgs e)
        {
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "cancel")
            {
                Response.Redirect("~/FamilyAccountList.aspx");
            }

            lblError.Text= string.Empty;
            if (Page.IsValid)
            {
                if(e.CommandName == "save")
                {

                    var p = Patron.FetchObject(int.Parse(SA.Text));
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
                    

                    p.AvatarID = FormatHelper.SafeToInt(((System.Web.UI.HtmlControls.HtmlInputText)e.Item.FindControl("AvatarID")).Value);
                    // do the save
                    p.Update();
                    

                    rptr.DataSource = Patron.GetPatronForEdit(p.PID);
                    rptr.DataBind();

                    ((BaseSRPPage)Page).TranslateStrings(rptr);
                    

                    lblError.Text = "Family account information has been saved.<br><br>";
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
    }
}