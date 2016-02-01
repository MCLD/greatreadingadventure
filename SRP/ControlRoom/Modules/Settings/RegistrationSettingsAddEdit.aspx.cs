using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


// --> MODULENAME 
// --> XXXXXRibbon 
// --> PERMISSIONID 
namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class RegistrationSettingsAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4100;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Registration Settings");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());
            
                lblPK.Text = "0"; //"Request["PK"];
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                //var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                //if (control!=null) control.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "Default.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                try
                {
                    odsData.DataBind();
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);

                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null) masterPage.PageMessage = SRPResources.RefreshOK;
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            {
                try
                {
                    var obj = new RegistrationSettings();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

                    obj.Literacy1Label = ((TextBox)((DetailsView)sender).FindControl("Literacy1Label")).Text;
                    obj.Literacy2Label = ((TextBox)((DetailsView)sender).FindControl("Literacy2Label")).Text;
                    obj.DOB_Prompt = ((CheckBox)((DetailsView)sender).FindControl("DOB_Prompt")).Checked;
                    obj.Age_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Age_Prompt")).Checked;
                    obj.SchoolGrade_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Prompt")).Checked;
                    obj.FirstName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Prompt")).Checked;
                    obj.MiddleName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Prompt")).Checked;
                    obj.LastName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LastName_Prompt")).Checked;
                    obj.Gender_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Gender_Prompt")).Checked;
                    obj.EmailAddress_Prompt = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Prompt")).Checked;
                    obj.PhoneNumber_Prompt = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Prompt")).Checked;
                    obj.StreetAddress1_Prompt = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Prompt")).Checked;
                    obj.StreetAddress2_Prompt = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Prompt")).Checked;
                    obj.City_Prompt = ((CheckBox)((DetailsView)sender).FindControl("City_Prompt")).Checked;
                    obj.State_Prompt = ((CheckBox)((DetailsView)sender).FindControl("State_Prompt")).Checked;
                    obj.ZipCode_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Prompt")).Checked;
                    obj.Country_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Country_Prompt")).Checked;
                    obj.County_Prompt = ((CheckBox)((DetailsView)sender).FindControl("County_Prompt")).Checked;
                    obj.ParentGuardianFirstName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Prompt")).Checked;
                    obj.ParentGuardianLastName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Prompt")).Checked;
                    obj.ParentGuardianMiddleName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Prompt")).Checked;
                    obj.PrimaryLibrary_Prompt = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Prompt")).Checked;
                    obj.LibraryCard_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Prompt")).Checked;
                    obj.SchoolName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Prompt")).Checked;
                    obj.District_Prompt = ((CheckBox)((DetailsView)sender).FindControl("District_Prompt")).Checked;
                    obj.SDistrict_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Prompt")).Checked;
                    obj.Teacher_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Prompt")).Checked;
                    obj.GroupTeamName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Prompt")).Checked;
                    obj.SchoolType_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Prompt")).Checked;
                    obj.LiteracyLevel1_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Prompt")).Checked;
                    obj.LiteracyLevel2_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Prompt")).Checked;
                    obj.ParentPermFlag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Prompt")).Checked;
                    obj.Over18Flag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Prompt")).Checked;
                    obj.ShareFlag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Prompt")).Checked;
                    obj.TermsOfUseflag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Prompt")).Checked;
                    obj.Custom1_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Prompt")).Checked;
                    obj.Custom2_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Prompt")).Checked;
                    obj.Custom3_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Prompt")).Checked;
                    obj.Custom4_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Prompt")).Checked;
                    obj.Custom5_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Prompt")).Checked;
                    obj.DOB_Req = ((CheckBox)((DetailsView)sender).FindControl("DOB_Req")).Checked;
                    obj.Age_Req = ((CheckBox)((DetailsView)sender).FindControl("Age_Req")).Checked;
                    obj.SchoolGrade_Req = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Req")).Checked;
                    obj.FirstName_Req = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Req")).Checked;
                    obj.MiddleName_Req = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Req")).Checked;
                    obj.LastName_Req = ((CheckBox)((DetailsView)sender).FindControl("LastName_Req")).Checked;
                    obj.Gender_Req = ((CheckBox)((DetailsView)sender).FindControl("Gender_Req")).Checked;
                    obj.EmailAddress_Req = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Req")).Checked;
                    obj.PhoneNumber_Req = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Req")).Checked;
                    obj.StreetAddress1_Req = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Req")).Checked;
                    obj.StreetAddress2_Req = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Req")).Checked;
                    obj.City_Req = ((CheckBox)((DetailsView)sender).FindControl("City_Req")).Checked;
                    obj.State_Req = ((CheckBox)((DetailsView)sender).FindControl("State_Req")).Checked;
                    obj.ZipCode_Req = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Req")).Checked;
                    obj.Country_Req = ((CheckBox)((DetailsView)sender).FindControl("Country_Req")).Checked;
                    obj.County_Req = ((CheckBox)((DetailsView)sender).FindControl("County_Req")).Checked;
                    obj.ParentGuardianFirstName_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Req")).Checked;
                    obj.ParentGuardianLastName_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Req")).Checked;
                    obj.ParentGuardianMiddleName_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Req")).Checked;
                    obj.PrimaryLibrary_Req = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Req")).Checked;
                    obj.LibraryCard_Req = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Req")).Checked;
                    obj.SchoolName_Req = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Req")).Checked;
                    obj.District_Req = ((CheckBox)((DetailsView)sender).FindControl("District_Req")).Checked;
                    obj.SDistrict_Req = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Req")).Checked;
                    obj.Teacher_Req = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Req")).Checked;
                    obj.GroupTeamName_Req = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Req")).Checked;
                    obj.SchoolType_Req = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Req")).Checked;
                    obj.LiteracyLevel1_Req = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Req")).Checked;
                    obj.LiteracyLevel2_Req = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Req")).Checked;
                    obj.ParentPermFlag_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Req")).Checked;
                    obj.Over18Flag_Req = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Req")).Checked;
                    obj.ShareFlag_Req = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Req")).Checked;
                    obj.TermsOfUseflag_Req = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Req")).Checked;
                    obj.Custom1_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Req")).Checked;
                    obj.Custom2_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Req")).Checked;
                    obj.Custom3_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Req")).Checked;
                    obj.Custom4_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Req")).Checked;
                    obj.Custom5_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Req")).Checked;
                    obj.DOB_Show = ((CheckBox)((DetailsView)sender).FindControl("DOB_Show")).Checked;
                    obj.Age_Show = ((CheckBox)((DetailsView)sender).FindControl("Age_Show")).Checked;
                    obj.SchoolGrade_Show = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Show")).Checked;
                    obj.FirstName_Show = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Show")).Checked;
                    obj.MiddleName_Show = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Show")).Checked;
                    obj.LastName_Show = ((CheckBox)((DetailsView)sender).FindControl("LastName_Show")).Checked;
                    obj.Gender_Show = ((CheckBox)((DetailsView)sender).FindControl("Gender_Show")).Checked;
                    obj.EmailAddress_Show = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Show")).Checked;
                    obj.PhoneNumber_Show = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Show")).Checked;
                    obj.StreetAddress1_Show = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Show")).Checked;
                    obj.StreetAddress2_Show = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Show")).Checked;
                    obj.City_Show = ((CheckBox)((DetailsView)sender).FindControl("City_Show")).Checked;
                    obj.State_Show = ((CheckBox)((DetailsView)sender).FindControl("State_Show")).Checked;
                    obj.ZipCode_Show = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Show")).Checked;
                    obj.Country_Show = ((CheckBox)((DetailsView)sender).FindControl("Country_Show")).Checked;
                    obj.County_Show = ((CheckBox)((DetailsView)sender).FindControl("County_Show")).Checked;
                    obj.ParentGuardianFirstName_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Show")).Checked;
                    obj.ParentGuardianLastName_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Show")).Checked;
                    obj.ParentGuardianMiddleName_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Show")).Checked;
                    obj.PrimaryLibrary_Show = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Show")).Checked;
                    obj.LibraryCard_Show = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Show")).Checked;
                    obj.SchoolName_Show = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Show")).Checked;
                    obj.District_Show = ((CheckBox)((DetailsView)sender).FindControl("District_Show")).Checked;
                    obj.SDistrict_Show = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Show")).Checked;
                    obj.Teacher_Show = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Show")).Checked;
                    obj.GroupTeamName_Show = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Show")).Checked;
                    obj.SchoolType_Show = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Show")).Checked;
                    obj.LiteracyLevel1_Show = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Show")).Checked;
                    obj.LiteracyLevel2_Show = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Show")).Checked;
                    obj.ParentPermFlag_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Show")).Checked;
                    obj.Over18Flag_Show = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Show")).Checked;
                    obj.ShareFlag_Show = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Show")).Checked;
                    obj.TermsOfUseflag_Show = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Show")).Checked;
                    obj.Custom1_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Show")).Checked;
                    obj.Custom2_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Show")).Checked;
                    obj.Custom3_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Show")).Checked;
                    obj.Custom4_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Show")).Checked;
                    obj.Custom5_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Show")).Checked;
                    obj.DOB_Edit = ((CheckBox)((DetailsView)sender).FindControl("DOB_Edit")).Checked;
                    obj.Age_Edit = ((CheckBox)((DetailsView)sender).FindControl("Age_Edit")).Checked;
                    obj.SchoolGrade_Edit = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Edit")).Checked;
                    obj.FirstName_Edit = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Edit")).Checked;
                    obj.MiddleName_Edit = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Edit")).Checked;
                    obj.LastName_Edit = ((CheckBox)((DetailsView)sender).FindControl("LastName_Edit")).Checked;
                    obj.Gender_Edit = ((CheckBox)((DetailsView)sender).FindControl("Gender_Edit")).Checked;
                    obj.EmailAddress_Edit = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Edit")).Checked;
                    obj.PhoneNumber_Edit = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Edit")).Checked;
                    obj.StreetAddress1_Edit = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Edit")).Checked;
                    obj.StreetAddress2_Edit = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Edit")).Checked;
                    obj.City_Edit = ((CheckBox)((DetailsView)sender).FindControl("City_Edit")).Checked;
                    obj.State_Edit = ((CheckBox)((DetailsView)sender).FindControl("State_Edit")).Checked;
                    obj.ZipCode_Edit = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Edit")).Checked;
                    obj.Country_Edit = ((CheckBox)((DetailsView)sender).FindControl("Country_Edit")).Checked;
                    obj.County_Edit = ((CheckBox)((DetailsView)sender).FindControl("County_Edit")).Checked;
                    obj.ParentGuardianFirstName_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Edit")).Checked;
                    obj.ParentGuardianLastName_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Edit")).Checked;
                    obj.ParentGuardianMiddleName_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Edit")).Checked;
                    obj.PrimaryLibrary_Edit = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Edit")).Checked;
                    obj.LibraryCard_Edit = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Edit")).Checked;
                    obj.SchoolName_Edit = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Edit")).Checked;
                    obj.District_Edit = ((CheckBox)((DetailsView)sender).FindControl("District_Edit")).Checked;
                    obj.SDistrict_Edit = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Edit")).Checked;
                    obj.Teacher_Edit = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Edit")).Checked;
                    obj.GroupTeamName_Edit = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Edit")).Checked;
                    obj.SchoolType_Edit = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Edit")).Checked;
                    obj.LiteracyLevel1_Edit = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Edit")).Checked;
                    obj.LiteracyLevel2_Edit = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Edit")).Checked;
                    obj.ParentPermFlag_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Edit")).Checked;
                    obj.Over18Flag_Edit = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Edit")).Checked;
                    obj.ShareFlag_Edit = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Edit")).Checked;
                    obj.TermsOfUseflag_Edit = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Edit")).Checked;
                    obj.Custom1_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Edit")).Checked;
                    obj.Custom2_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Edit")).Checked;
                    obj.Custom3_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Edit")).Checked;
                    obj.Custom4_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Edit")).Checked;
                    obj.Custom5_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Edit")).Checked;

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.RID.ToString();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new RegistrationSettings();
                    //int pk = 0;// int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    obj.Fetch();

                    obj.Literacy1Label = ((TextBox)((DetailsView)sender).FindControl("Literacy1Label")).Text;
                    obj.Literacy2Label = ((TextBox)((DetailsView)sender).FindControl("Literacy2Label")).Text;
                    obj.DOB_Prompt = ((CheckBox)((DetailsView)sender).FindControl("DOB_Prompt")).Checked;
                    obj.Age_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Age_Prompt")).Checked;
                    obj.SchoolGrade_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Prompt")).Checked;
                    obj.FirstName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Prompt")).Checked;
                    obj.MiddleName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Prompt")).Checked;
                    obj.LastName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LastName_Prompt")).Checked;
                    obj.Gender_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Gender_Prompt")).Checked;
                    obj.EmailAddress_Prompt = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Prompt")).Checked;
                    obj.PhoneNumber_Prompt = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Prompt")).Checked;
                    obj.StreetAddress1_Prompt = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Prompt")).Checked;
                    obj.StreetAddress2_Prompt = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Prompt")).Checked;
                    obj.City_Prompt = ((CheckBox)((DetailsView)sender).FindControl("City_Prompt")).Checked;
                    obj.State_Prompt = ((CheckBox)((DetailsView)sender).FindControl("State_Prompt")).Checked;
                    obj.ZipCode_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Prompt")).Checked;
                    obj.Country_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Country_Prompt")).Checked;
                    obj.County_Prompt = ((CheckBox)((DetailsView)sender).FindControl("County_Prompt")).Checked;
                    obj.ParentGuardianFirstName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Prompt")).Checked;
                    obj.ParentGuardianLastName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Prompt")).Checked;
                    obj.ParentGuardianMiddleName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Prompt")).Checked;
                    obj.PrimaryLibrary_Prompt = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Prompt")).Checked;
                    obj.LibraryCard_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Prompt")).Checked;
                    obj.SchoolName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Prompt")).Checked;
                    obj.District_Prompt = ((CheckBox)((DetailsView)sender).FindControl("District_Prompt")).Checked;
                    obj.SDistrict_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Prompt")).Checked;
                    obj.Teacher_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Prompt")).Checked;
                    obj.GroupTeamName_Prompt = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Prompt")).Checked;
                    obj.SchoolType_Prompt = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Prompt")).Checked;
                    obj.LiteracyLevel1_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Prompt")).Checked;
                    obj.LiteracyLevel2_Prompt = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Prompt")).Checked;
                    obj.ParentPermFlag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Prompt")).Checked;
                    obj.Over18Flag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Prompt")).Checked;
                    obj.ShareFlag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Prompt")).Checked;
                    obj.TermsOfUseflag_Prompt = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Prompt")).Checked;
                    obj.Custom1_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Prompt")).Checked;
                    obj.Custom2_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Prompt")).Checked;
                    obj.Custom3_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Prompt")).Checked;
                    obj.Custom4_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Prompt")).Checked;
                    obj.Custom5_Prompt = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Prompt")).Checked;
                    obj.DOB_Req = ((CheckBox)((DetailsView)sender).FindControl("DOB_Req")).Checked;
                    obj.Age_Req = ((CheckBox)((DetailsView)sender).FindControl("Age_Req")).Checked;
                    obj.SchoolGrade_Req = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Req")).Checked;
                    obj.FirstName_Req = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Req")).Checked;
                    obj.MiddleName_Req = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Req")).Checked;
                    obj.LastName_Req = ((CheckBox)((DetailsView)sender).FindControl("LastName_Req")).Checked;
                    obj.Gender_Req = ((CheckBox)((DetailsView)sender).FindControl("Gender_Req")).Checked;
                    obj.EmailAddress_Req = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Req")).Checked;
                    obj.PhoneNumber_Req = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Req")).Checked;
                    obj.StreetAddress1_Req = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Req")).Checked;
                    obj.StreetAddress2_Req = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Req")).Checked;
                    obj.City_Req = ((CheckBox)((DetailsView)sender).FindControl("City_Req")).Checked;
                    obj.State_Req = ((CheckBox)((DetailsView)sender).FindControl("State_Req")).Checked;
                    obj.ZipCode_Req = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Req")).Checked;
                    obj.Country_Req = ((CheckBox)((DetailsView)sender).FindControl("Country_Req")).Checked;
                    obj.County_Req = ((CheckBox)((DetailsView)sender).FindControl("County_Req")).Checked;
                    obj.ParentGuardianFirstName_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Req")).Checked;
                    obj.ParentGuardianLastName_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Req")).Checked;
                    obj.ParentGuardianMiddleName_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Req")).Checked;
                    obj.PrimaryLibrary_Req = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Req")).Checked;
                    obj.LibraryCard_Req = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Req")).Checked;
                    obj.SchoolName_Req = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Req")).Checked;
                    obj.District_Req = ((CheckBox)((DetailsView)sender).FindControl("District_Req")).Checked;
                    obj.SDistrict_Req = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Req")).Checked;
                    obj.Teacher_Req = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Req")).Checked;
                    obj.GroupTeamName_Req = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Req")).Checked;
                    obj.SchoolType_Req = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Req")).Checked;
                    obj.LiteracyLevel1_Req = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Req")).Checked;
                    obj.LiteracyLevel2_Req = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Req")).Checked;
                    obj.ParentPermFlag_Req = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Req")).Checked;
                    obj.Over18Flag_Req = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Req")).Checked;
                    obj.ShareFlag_Req = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Req")).Checked;
                    obj.TermsOfUseflag_Req = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Req")).Checked;
                    obj.Custom1_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Req")).Checked;
                    obj.Custom2_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Req")).Checked;
                    obj.Custom3_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Req")).Checked;
                    obj.Custom4_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Req")).Checked;
                    obj.Custom5_Req = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Req")).Checked;
                    obj.DOB_Show = ((CheckBox)((DetailsView)sender).FindControl("DOB_Show")).Checked;
                    obj.Age_Show = ((CheckBox)((DetailsView)sender).FindControl("Age_Show")).Checked;
                    obj.SchoolGrade_Show = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Show")).Checked;
                    obj.FirstName_Show = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Show")).Checked;
                    obj.MiddleName_Show = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Show")).Checked;
                    obj.LastName_Show = ((CheckBox)((DetailsView)sender).FindControl("LastName_Show")).Checked;
                    obj.Gender_Show = ((CheckBox)((DetailsView)sender).FindControl("Gender_Show")).Checked;
                    obj.EmailAddress_Show = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Show")).Checked;
                    obj.PhoneNumber_Show = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Show")).Checked;
                    obj.StreetAddress1_Show = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Show")).Checked;
                    obj.StreetAddress2_Show = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Show")).Checked;
                    obj.City_Show = ((CheckBox)((DetailsView)sender).FindControl("City_Show")).Checked;
                    obj.State_Show = ((CheckBox)((DetailsView)sender).FindControl("State_Show")).Checked;
                    obj.ZipCode_Show = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Show")).Checked;
                    obj.Country_Show = ((CheckBox)((DetailsView)sender).FindControl("Country_Show")).Checked;
                    obj.County_Show = ((CheckBox)((DetailsView)sender).FindControl("County_Show")).Checked;
                    obj.ParentGuardianFirstName_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Show")).Checked;
                    obj.ParentGuardianLastName_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Show")).Checked;
                    obj.ParentGuardianMiddleName_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Show")).Checked;
                    obj.PrimaryLibrary_Show = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Show")).Checked;
                    obj.LibraryCard_Show = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Show")).Checked;
                    obj.SchoolName_Show = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Show")).Checked;
                    obj.District_Show = ((CheckBox)((DetailsView)sender).FindControl("District_Show")).Checked;
                    obj.SDistrict_Show = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Show")).Checked;
                    obj.Teacher_Show = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Show")).Checked;
                    obj.GroupTeamName_Show = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Show")).Checked;
                    obj.SchoolType_Show = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Show")).Checked;
                    obj.LiteracyLevel1_Show = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Show")).Checked;
                    obj.LiteracyLevel2_Show = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Show")).Checked;
                    obj.ParentPermFlag_Show = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Show")).Checked;
                    obj.Over18Flag_Show = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Show")).Checked;
                    obj.ShareFlag_Show = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Show")).Checked;
                    obj.TermsOfUseflag_Show = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Show")).Checked;
                    obj.Custom1_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Show")).Checked;
                    obj.Custom2_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Show")).Checked;
                    obj.Custom3_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Show")).Checked;
                    obj.Custom4_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Show")).Checked;
                    obj.Custom5_Show = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Show")).Checked;
                    obj.DOB_Edit = ((CheckBox)((DetailsView)sender).FindControl("DOB_Edit")).Checked;
                    obj.Age_Edit = ((CheckBox)((DetailsView)sender).FindControl("Age_Edit")).Checked;
                    obj.SchoolGrade_Edit = ((CheckBox)((DetailsView)sender).FindControl("SchoolGrade_Edit")).Checked;
                    obj.FirstName_Edit = ((CheckBox)((DetailsView)sender).FindControl("FirstName_Edit")).Checked;
                    obj.MiddleName_Edit = ((CheckBox)((DetailsView)sender).FindControl("MiddleName_Edit")).Checked;
                    obj.LastName_Edit = ((CheckBox)((DetailsView)sender).FindControl("LastName_Edit")).Checked;
                    obj.Gender_Edit = ((CheckBox)((DetailsView)sender).FindControl("Gender_Edit")).Checked;
                    obj.EmailAddress_Edit = ((CheckBox)((DetailsView)sender).FindControl("EmailAddress_Edit")).Checked;
                    obj.PhoneNumber_Edit = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumber_Edit")).Checked;
                    obj.StreetAddress1_Edit = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress1_Edit")).Checked;
                    obj.StreetAddress2_Edit = ((CheckBox)((DetailsView)sender).FindControl("StreetAddress2_Edit")).Checked;
                    obj.City_Edit = ((CheckBox)((DetailsView)sender).FindControl("City_Edit")).Checked;
                    obj.State_Edit = ((CheckBox)((DetailsView)sender).FindControl("State_Edit")).Checked;
                    obj.ZipCode_Edit = ((CheckBox)((DetailsView)sender).FindControl("ZipCode_Edit")).Checked;
                    obj.Country_Edit = ((CheckBox)((DetailsView)sender).FindControl("Country_Edit")).Checked;
                    obj.County_Edit = ((CheckBox)((DetailsView)sender).FindControl("County_Edit")).Checked;
                    obj.ParentGuardianFirstName_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianFirstName_Edit")).Checked;
                    obj.ParentGuardianLastName_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianLastName_Edit")).Checked;
                    obj.ParentGuardianMiddleName_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentGuardianMiddleName_Edit")).Checked;
                    obj.PrimaryLibrary_Edit = ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibrary_Edit")).Checked;
                    obj.LibraryCard_Edit = ((CheckBox)((DetailsView)sender).FindControl("LibraryCard_Edit")).Checked;
                    obj.SchoolName_Edit = ((CheckBox)((DetailsView)sender).FindControl("SchoolName_Edit")).Checked;
                    obj.District_Edit = ((CheckBox)((DetailsView)sender).FindControl("District_Edit")).Checked;
                    obj.SDistrict_Edit = ((CheckBox)((DetailsView)sender).FindControl("SDistrict_Edit")).Checked;
                    obj.Teacher_Edit = ((CheckBox)((DetailsView)sender).FindControl("Teacher_Edit")).Checked;
                    obj.GroupTeamName_Edit = ((CheckBox)((DetailsView)sender).FindControl("GroupTeamName_Edit")).Checked;
                    obj.SchoolType_Edit = ((CheckBox)((DetailsView)sender).FindControl("SchoolType_Edit")).Checked;
                    obj.LiteracyLevel1_Edit = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1_Edit")).Checked;
                    obj.LiteracyLevel2_Edit = ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2_Edit")).Checked;
                    obj.ParentPermFlag_Edit = ((CheckBox)((DetailsView)sender).FindControl("ParentPermFlag_Edit")).Checked;
                    obj.Over18Flag_Edit = ((CheckBox)((DetailsView)sender).FindControl("Over18Flag_Edit")).Checked;
                    obj.ShareFlag_Edit = ((CheckBox)((DetailsView)sender).FindControl("ShareFlag_Edit")).Checked;
                    obj.TermsOfUseflag_Edit = ((CheckBox)((DetailsView)sender).FindControl("TermsOfUseflag_Edit")).Checked;
                    obj.Custom1_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom1_Edit")).Checked;
                    obj.Custom2_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom2_Edit")).Checked;
                    obj.Custom3_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom3_Edit")).Checked;
                    obj.Custom4_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom4_Edit")).Checked;
                    obj.Custom5_Edit = ((CheckBox)((DetailsView)sender).FindControl("Custom5_Edit")).Checked;

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();
                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
        }
    }
}

