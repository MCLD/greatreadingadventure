using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.ApplicationBlocks.Data;
using System.Collections;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

[Serializable]    public class RegistrationSettings : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myRID;
        private string myLiteracy1Label;
        private string myLiteracy2Label;
        private bool myDOB_Prompt;
        private bool myAge_Prompt;
        private bool mySchoolGrade_Prompt;
        private bool myFirstName_Prompt;
        private bool myMiddleName_Prompt;
        private bool myLastName_Prompt;
        private bool myGender_Prompt;
        private bool myEmailAddress_Prompt;
        private bool myPhoneNumber_Prompt;
        private bool myStreetAddress1_Prompt;
        private bool myStreetAddress2_Prompt;
        private bool myCity_Prompt;
        private bool myState_Prompt;
        private bool myZipCode_Prompt;
        private bool myCountry_Prompt;
        private bool myCounty_Prompt;
        private bool myParentGuardianFirstName_Prompt;
        private bool myParentGuardianLastName_Prompt;
        private bool myParentGuardianMiddleName_Prompt;
        private bool myPrimaryLibrary_Prompt;
        private bool myLibraryCard_Prompt;
        private bool mySchoolName_Prompt;
        private bool myDistrict_Prompt;
        private bool mySDistrict_Prompt;
        private bool myTeacher_Prompt;
        private bool myGroupTeamName_Prompt;
        private bool mySchoolType_Prompt;
        private bool myLiteracyLevel1_Prompt;
        private bool myLiteracyLevel2_Prompt;
        private bool myParentPermFlag_Prompt;
        private bool myOver18Flag_Prompt;
        private bool myShareFlag_Prompt;
        private bool myTermsOfUseflag_Prompt;
        private bool myCustom1_Prompt;
        private bool myCustom2_Prompt;
        private bool myCustom3_Prompt;
        private bool myCustom4_Prompt;
        private bool myCustom5_Prompt;
        private bool myDOB_Req;
        private bool myAge_Req;
        private bool mySchoolGrade_Req;
        private bool myFirstName_Req;
        private bool myMiddleName_Req;
        private bool myLastName_Req;
        private bool myGender_Req;
        private bool myEmailAddress_Req;
        private bool myPhoneNumber_Req;
        private bool myStreetAddress1_Req;
        private bool myStreetAddress2_Req;
        private bool myCity_Req;
        private bool myState_Req;
        private bool myZipCode_Req;
        private bool myCountry_Req;
        private bool myCounty_Req;
        private bool myParentGuardianFirstName_Req;
        private bool myParentGuardianLastName_Req;
        private bool myParentGuardianMiddleName_Req;
        private bool myPrimaryLibrary_Req;
        private bool myLibraryCard_Req;
        private bool mySchoolName_Req;
        private bool myDistrict_Req;
        private bool mySDistrict_Req;
        private bool myTeacher_Req;
        private bool myGroupTeamName_Req;
        private bool mySchoolType_Req;
        private bool myLiteracyLevel1_Req;
        private bool myLiteracyLevel2_Req;
        private bool myParentPermFlag_Req;
        private bool myOver18Flag_Req;
        private bool myShareFlag_Req;
        private bool myTermsOfUseflag_Req;
        private bool myCustom1_Req;
        private bool myCustom2_Req;
        private bool myCustom3_Req;
        private bool myCustom4_Req;
        private bool myCustom5_Req;
        private bool myDOB_Show;
        private bool myAge_Show;
        private bool mySchoolGrade_Show;
        private bool myFirstName_Show;
        private bool myMiddleName_Show;
        private bool myLastName_Show;
        private bool myGender_Show;
        private bool myEmailAddress_Show;
        private bool myPhoneNumber_Show;
        private bool myStreetAddress1_Show;
        private bool myStreetAddress2_Show;
        private bool myCity_Show;
        private bool myState_Show;
        private bool myZipCode_Show;
        private bool myCountry_Show;
        private bool myCounty_Show;
        private bool myParentGuardianFirstName_Show;
        private bool myParentGuardianLastName_Show;
        private bool myParentGuardianMiddleName_Show;
        private bool myPrimaryLibrary_Show;
        private bool myLibraryCard_Show;
        private bool mySchoolName_Show;
        private bool myDistrict_Show;
        private bool mySDistrict_Show;
        private bool myTeacher_Show;
        private bool myGroupTeamName_Show;
        private bool mySchoolType_Show;
        private bool myLiteracyLevel1_Show;
        private bool myLiteracyLevel2_Show;
        private bool myParentPermFlag_Show;
        private bool myOver18Flag_Show;
        private bool myShareFlag_Show;
        private bool myTermsOfUseflag_Show;
        private bool myCustom1_Show;
        private bool myCustom2_Show;
        private bool myCustom3_Show;
        private bool myCustom4_Show;
        private bool myCustom5_Show;
        private bool myDOB_Edit;
        private bool myAge_Edit;
        private bool mySchoolGrade_Edit;
        private bool myFirstName_Edit;
        private bool myMiddleName_Edit;
        private bool myLastName_Edit;
        private bool myGender_Edit;
        private bool myEmailAddress_Edit;
        private bool myPhoneNumber_Edit;
        private bool myStreetAddress1_Edit;
        private bool myStreetAddress2_Edit;
        private bool myCity_Edit;
        private bool myState_Edit;
        private bool myZipCode_Edit;
        private bool myCountry_Edit;
        private bool myCounty_Edit;
        private bool myParentGuardianFirstName_Edit;
        private bool myParentGuardianLastName_Edit;
        private bool myParentGuardianMiddleName_Edit;
        private bool myPrimaryLibrary_Edit;
        private bool myLibraryCard_Edit;
        private bool mySchoolName_Edit;
        private bool myDistrict_Edit;
        private bool mySDistrict_Edit;
        private bool myTeacher_Edit;
        private bool myGroupTeamName_Edit;
        private bool mySchoolType_Edit;
        private bool myLiteracyLevel1_Edit;
        private bool myLiteracyLevel2_Edit;
        private bool myParentPermFlag_Edit;
        private bool myOver18Flag_Edit;
        private bool myShareFlag_Edit;
        private bool myTermsOfUseflag_Edit;
        private bool myCustom1_Edit;
        private bool myCustom2_Edit;
        private bool myCustom3_Edit;
        private bool myCustom4_Edit;
        private bool myCustom5_Edit;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        private int myTenID = 0;
        private int myFldInt1 = 0;
        private int myFldInt2 = 0;
        private int myFldInt3 = 0;
        private bool myFldBit1 = false;
        private bool myFldBit2 = false;
        private bool myFldBit3 = false;
        private string myFldText1 = "";
        private string myFldText2 = "";
        private string myFldText3 = "";
        #endregion

        #region Accessors

        public int RID
        {
            get { return myRID; }
            set { myRID = value; }
        }
        public string Literacy1Label
        {
            get { return myLiteracy1Label; }
            set { myLiteracy1Label = value; }
        }
        public string Literacy2Label
        {
            get { return myLiteracy2Label; }
            set { myLiteracy2Label = value; }
        }
        public bool DOB_Prompt
        {
            get { return myDOB_Prompt; }
            set { myDOB_Prompt = value; }
        }
        public bool Age_Prompt
        {
            get { return myAge_Prompt; }
            set { myAge_Prompt = value; }
        }
        public bool SchoolGrade_Prompt
        {
            get { return mySchoolGrade_Prompt; }
            set { mySchoolGrade_Prompt = value; }
        }
        public bool FirstName_Prompt
        {
            get { return myFirstName_Prompt; }
            set { myFirstName_Prompt = value; }
        }
        public bool MiddleName_Prompt
        {
            get { return myMiddleName_Prompt; }
            set { myMiddleName_Prompt = value; }
        }
        public bool LastName_Prompt
        {
            get { return myLastName_Prompt; }
            set { myLastName_Prompt = value; }
        }
        public bool Gender_Prompt
        {
            get { return myGender_Prompt; }
            set { myGender_Prompt = value; }
        }
        public bool EmailAddress_Prompt
        {
            get { return myEmailAddress_Prompt; }
            set { myEmailAddress_Prompt = value; }
        }
        public bool PhoneNumber_Prompt
        {
            get { return myPhoneNumber_Prompt; }
            set { myPhoneNumber_Prompt = value; }
        }
        public bool StreetAddress1_Prompt
        {
            get { return myStreetAddress1_Prompt; }
            set { myStreetAddress1_Prompt = value; }
        }
        public bool StreetAddress2_Prompt
        {
            get { return myStreetAddress2_Prompt; }
            set { myStreetAddress2_Prompt = value; }
        }
        public bool City_Prompt
        {
            get { return myCity_Prompt; }
            set { myCity_Prompt = value; }
        }
        public bool State_Prompt
        {
            get { return myState_Prompt; }
            set { myState_Prompt = value; }
        }
        public bool ZipCode_Prompt
        {
            get { return myZipCode_Prompt; }
            set { myZipCode_Prompt = value; }
        }
        public bool Country_Prompt
        {
            get { return myCountry_Prompt; }
            set { myCountry_Prompt = value; }
        }
        public bool County_Prompt
        {
            get { return myCounty_Prompt; }
            set { myCounty_Prompt = value; }
        }
        public bool ParentGuardianFirstName_Prompt
        {
            get { return myParentGuardianFirstName_Prompt; }
            set { myParentGuardianFirstName_Prompt = value; }
        }
        public bool ParentGuardianLastName_Prompt
        {
            get { return myParentGuardianLastName_Prompt; }
            set { myParentGuardianLastName_Prompt = value; }
        }
        public bool ParentGuardianMiddleName_Prompt
        {
            get { return myParentGuardianMiddleName_Prompt; }
            set { myParentGuardianMiddleName_Prompt = value; }
        }
        public bool PrimaryLibrary_Prompt
        {
            get { return myPrimaryLibrary_Prompt; }
            set { myPrimaryLibrary_Prompt = value; }
        }
        public bool LibraryCard_Prompt
        {
            get { return myLibraryCard_Prompt; }
            set { myLibraryCard_Prompt = value; }
        }
        public bool SchoolName_Prompt
        {
            get { return mySchoolName_Prompt; }
            set { mySchoolName_Prompt = value; }
        }
        public bool District_Prompt
        {
            get { return myDistrict_Prompt; }
            set { myDistrict_Prompt = value; }
        }
        public bool SDistrict_Prompt
        {
            get { return mySDistrict_Prompt; }
            set { mySDistrict_Prompt = value; }
        }
        public bool Teacher_Prompt
        {
            get { return myTeacher_Prompt; }
            set { myTeacher_Prompt = value; }
        }
        public bool GroupTeamName_Prompt
        {
            get { return myGroupTeamName_Prompt; }
            set { myGroupTeamName_Prompt = value; }
        }
        public bool SchoolType_Prompt
        {
            get { return mySchoolType_Prompt; }
            set { mySchoolType_Prompt = value; }
        }
        public bool LiteracyLevel1_Prompt
        {
            get { return myLiteracyLevel1_Prompt; }
            set { myLiteracyLevel1_Prompt = value; }
        }
        public bool LiteracyLevel2_Prompt
        {
            get { return myLiteracyLevel2_Prompt; }
            set { myLiteracyLevel2_Prompt = value; }
        }
        public bool ParentPermFlag_Prompt
        {
            get { return myParentPermFlag_Prompt; }
            set { myParentPermFlag_Prompt = value; }
        }
        public bool Over18Flag_Prompt
        {
            get { return myOver18Flag_Prompt; }
            set { myOver18Flag_Prompt = value; }
        }
        public bool ShareFlag_Prompt
        {
            get { return myShareFlag_Prompt; }
            set { myShareFlag_Prompt = value; }
        }
        public bool TermsOfUseflag_Prompt
        {
            get { return myTermsOfUseflag_Prompt; }
            set { myTermsOfUseflag_Prompt = value; }
        }
        public bool Custom1_Prompt
        {
            get { return myCustom1_Prompt; }
            set { myCustom1_Prompt = value; }
        }
        public bool Custom2_Prompt
        {
            get { return myCustom2_Prompt; }
            set { myCustom2_Prompt = value; }
        }
        public bool Custom3_Prompt
        {
            get { return myCustom3_Prompt; }
            set { myCustom3_Prompt = value; }
        }
        public bool Custom4_Prompt
        {
            get { return myCustom4_Prompt; }
            set { myCustom4_Prompt = value; }
        }
        public bool Custom5_Prompt
        {
            get { return myCustom5_Prompt; }
            set { myCustom5_Prompt = value; }
        }
        public bool DOB_Req
        {
            get { return myDOB_Req; }
            set { myDOB_Req = value; }
        }
        public bool Age_Req
        {
            get { return myAge_Req; }
            set { myAge_Req = value; }
        }
        public bool SchoolGrade_Req
        {
            get { return mySchoolGrade_Req; }
            set { mySchoolGrade_Req = value; }
        }
        public bool FirstName_Req
        {
            get { return myFirstName_Req; }
            set { myFirstName_Req = value; }
        }
        public bool MiddleName_Req
        {
            get { return myMiddleName_Req; }
            set { myMiddleName_Req = value; }
        }
        public bool LastName_Req
        {
            get { return myLastName_Req; }
            set { myLastName_Req = value; }
        }
        public bool Gender_Req
        {
            get { return myGender_Req; }
            set { myGender_Req = value; }
        }
        public bool EmailAddress_Req
        {
            get { return myEmailAddress_Req; }
            set { myEmailAddress_Req = value; }
        }
        public bool PhoneNumber_Req
        {
            get { return myPhoneNumber_Req; }
            set { myPhoneNumber_Req = value; }
        }
        public bool StreetAddress1_Req
        {
            get { return myStreetAddress1_Req; }
            set { myStreetAddress1_Req = value; }
        }
        public bool StreetAddress2_Req
        {
            get { return myStreetAddress2_Req; }
            set { myStreetAddress2_Req = value; }
        }
        public bool City_Req
        {
            get { return myCity_Req; }
            set { myCity_Req = value; }
        }
        public bool State_Req
        {
            get { return myState_Req; }
            set { myState_Req = value; }
        }
        public bool ZipCode_Req
        {
            get { return myZipCode_Req; }
            set { myZipCode_Req = value; }
        }
        public bool Country_Req
        {
            get { return myCountry_Req; }
            set { myCountry_Req = value; }
        }
        public bool County_Req
        {
            get { return myCounty_Req; }
            set { myCounty_Req = value; }
        }
        public bool ParentGuardianFirstName_Req
        {
            get { return myParentGuardianFirstName_Req; }
            set { myParentGuardianFirstName_Req = value; }
        }
        public bool ParentGuardianLastName_Req
        {
            get { return myParentGuardianLastName_Req; }
            set { myParentGuardianLastName_Req = value; }
        }
        public bool ParentGuardianMiddleName_Req
        {
            get { return myParentGuardianMiddleName_Req; }
            set { myParentGuardianMiddleName_Req = value; }
        }
        public bool PrimaryLibrary_Req
        {
            get { return myPrimaryLibrary_Req; }
            set { myPrimaryLibrary_Req = value; }
        }
        public bool LibraryCard_Req
        {
            get { return myLibraryCard_Req; }
            set { myLibraryCard_Req = value; }
        }
        public bool SchoolName_Req
        {
            get { return mySchoolName_Req; }
            set { mySchoolName_Req = value; }
        }
        public bool District_Req
        {
            get { return myDistrict_Req; }
            set { myDistrict_Req = value; }
        }
        public bool SDistrict_Req
        {
            get { return mySDistrict_Req; }
            set { mySDistrict_Req = value; }
        }
        public bool Teacher_Req
        {
            get { return myTeacher_Req; }
            set { myTeacher_Req = value; }
        }
        public bool GroupTeamName_Req
        {
            get { return myGroupTeamName_Req; }
            set { myGroupTeamName_Req = value; }
        }
        public bool SchoolType_Req
        {
            get { return mySchoolType_Req; }
            set { mySchoolType_Req = value; }
        }
        public bool LiteracyLevel1_Req
        {
            get { return myLiteracyLevel1_Req; }
            set { myLiteracyLevel1_Req = value; }
        }
        public bool LiteracyLevel2_Req
        {
            get { return myLiteracyLevel2_Req; }
            set { myLiteracyLevel2_Req = value; }
        }
        public bool ParentPermFlag_Req
        {
            get { return myParentPermFlag_Req; }
            set { myParentPermFlag_Req = value; }
        }
        public bool Over18Flag_Req
        {
            get { return myOver18Flag_Req; }
            set { myOver18Flag_Req = value; }
        }
        public bool ShareFlag_Req
        {
            get { return myShareFlag_Req; }
            set { myShareFlag_Req = value; }
        }
        public bool TermsOfUseflag_Req
        {
            get { return myTermsOfUseflag_Req; }
            set { myTermsOfUseflag_Req = value; }
        }
        public bool Custom1_Req
        {
            get { return myCustom1_Req; }
            set { myCustom1_Req = value; }
        }
        public bool Custom2_Req
        {
            get { return myCustom2_Req; }
            set { myCustom2_Req = value; }
        }
        public bool Custom3_Req
        {
            get { return myCustom3_Req; }
            set { myCustom3_Req = value; }
        }
        public bool Custom4_Req
        {
            get { return myCustom4_Req; }
            set { myCustom4_Req = value; }
        }
        public bool Custom5_Req
        {
            get { return myCustom5_Req; }
            set { myCustom5_Req = value; }
        }
        public bool DOB_Show
        {
            get { return myDOB_Show; }
            set { myDOB_Show = value; }
        }
        public bool Age_Show
        {
            get { return myAge_Show; }
            set { myAge_Show = value; }
        }
        public bool SchoolGrade_Show
        {
            get { return mySchoolGrade_Show; }
            set { mySchoolGrade_Show = value; }
        }
        public bool FirstName_Show
        {
            get { return myFirstName_Show; }
            set { myFirstName_Show = value; }
        }
        public bool MiddleName_Show
        {
            get { return myMiddleName_Show; }
            set { myMiddleName_Show = value; }
        }
        public bool LastName_Show
        {
            get { return myLastName_Show; }
            set { myLastName_Show = value; }
        }
        public bool Gender_Show
        {
            get { return myGender_Show; }
            set { myGender_Show = value; }
        }
        public bool EmailAddress_Show
        {
            get { return myEmailAddress_Show; }
            set { myEmailAddress_Show = value; }
        }
        public bool PhoneNumber_Show
        {
            get { return myPhoneNumber_Show; }
            set { myPhoneNumber_Show = value; }
        }
        public bool StreetAddress1_Show
        {
            get { return myStreetAddress1_Show; }
            set { myStreetAddress1_Show = value; }
        }
        public bool StreetAddress2_Show
        {
            get { return myStreetAddress2_Show; }
            set { myStreetAddress2_Show = value; }
        }
        public bool City_Show
        {
            get { return myCity_Show; }
            set { myCity_Show = value; }
        }
        public bool State_Show
        {
            get { return myState_Show; }
            set { myState_Show = value; }
        }
        public bool ZipCode_Show
        {
            get { return myZipCode_Show; }
            set { myZipCode_Show = value; }
        }
        public bool Country_Show
        {
            get { return myCountry_Show; }
            set { myCountry_Show = value; }
        }
        public bool County_Show
        {
            get { return myCounty_Show; }
            set { myCounty_Show = value; }
        }
        public bool ParentGuardianFirstName_Show
        {
            get { return myParentGuardianFirstName_Show; }
            set { myParentGuardianFirstName_Show = value; }
        }
        public bool ParentGuardianLastName_Show
        {
            get { return myParentGuardianLastName_Show; }
            set { myParentGuardianLastName_Show = value; }
        }
        public bool ParentGuardianMiddleName_Show
        {
            get { return myParentGuardianMiddleName_Show; }
            set { myParentGuardianMiddleName_Show = value; }
        }
        public bool PrimaryLibrary_Show
        {
            get { return myPrimaryLibrary_Show; }
            set { myPrimaryLibrary_Show = value; }
        }
        public bool LibraryCard_Show
        {
            get { return myLibraryCard_Show; }
            set { myLibraryCard_Show = value; }
        }
        public bool SchoolName_Show
        {
            get { return mySchoolName_Show; }
            set { mySchoolName_Show = value; }
        }
        public bool District_Show
        {
            get { return myDistrict_Show; }
            set { myDistrict_Show = value; }
        }
        public bool SDistrict_Show
        {
            get { return mySDistrict_Show; }
            set { mySDistrict_Show = value; }
        }
        public bool Teacher_Show
        {
            get { return myTeacher_Show; }
            set { myTeacher_Show = value; }
        }
        public bool GroupTeamName_Show
        {
            get { return myGroupTeamName_Show; }
            set { myGroupTeamName_Show = value; }
        }
        public bool SchoolType_Show
        {
            get { return mySchoolType_Show; }
            set { mySchoolType_Show = value; }
        }
        public bool LiteracyLevel1_Show
        {
            get { return myLiteracyLevel1_Show; }
            set { myLiteracyLevel1_Show = value; }
        }
        public bool LiteracyLevel2_Show
        {
            get { return myLiteracyLevel2_Show; }
            set { myLiteracyLevel2_Show = value; }
        }
        public bool ParentPermFlag_Show
        {
            get { return myParentPermFlag_Show; }
            set { myParentPermFlag_Show = value; }
        }
        public bool Over18Flag_Show
        {
            get { return myOver18Flag_Show; }
            set { myOver18Flag_Show = value; }
        }
        public bool ShareFlag_Show
        {
            get { return myShareFlag_Show; }
            set { myShareFlag_Show = value; }
        }
        public bool TermsOfUseflag_Show
        {
            get { return myTermsOfUseflag_Show; }
            set { myTermsOfUseflag_Show = value; }
        }
        public bool Custom1_Show
        {
            get { return myCustom1_Show; }
            set { myCustom1_Show = value; }
        }
        public bool Custom2_Show
        {
            get { return myCustom2_Show; }
            set { myCustom2_Show = value; }
        }
        public bool Custom3_Show
        {
            get { return myCustom3_Show; }
            set { myCustom3_Show = value; }
        }
        public bool Custom4_Show
        {
            get { return myCustom4_Show; }
            set { myCustom4_Show = value; }
        }
        public bool Custom5_Show
        {
            get { return myCustom5_Show; }
            set { myCustom5_Show = value; }
        }
        public bool DOB_Edit
        {
            get { return myDOB_Edit; }
            set { myDOB_Edit = value; }
        }
        public bool Age_Edit
        {
            get { return myAge_Edit; }
            set { myAge_Edit = value; }
        }
        public bool SchoolGrade_Edit
        {
            get { return mySchoolGrade_Edit; }
            set { mySchoolGrade_Edit = value; }
        }
        public bool FirstName_Edit
        {
            get { return myFirstName_Edit; }
            set { myFirstName_Edit = value; }
        }
        public bool MiddleName_Edit
        {
            get { return myMiddleName_Edit; }
            set { myMiddleName_Edit = value; }
        }
        public bool LastName_Edit
        {
            get { return myLastName_Edit; }
            set { myLastName_Edit = value; }
        }
        public bool Gender_Edit
        {
            get { return myGender_Edit; }
            set { myGender_Edit = value; }
        }
        public bool EmailAddress_Edit
        {
            get { return myEmailAddress_Edit; }
            set { myEmailAddress_Edit = value; }
        }
        public bool PhoneNumber_Edit
        {
            get { return myPhoneNumber_Edit; }
            set { myPhoneNumber_Edit = value; }
        }
        public bool StreetAddress1_Edit
        {
            get { return myStreetAddress1_Edit; }
            set { myStreetAddress1_Edit = value; }
        }
        public bool StreetAddress2_Edit
        {
            get { return myStreetAddress2_Edit; }
            set { myStreetAddress2_Edit = value; }
        }
        public bool City_Edit
        {
            get { return myCity_Edit; }
            set { myCity_Edit = value; }
        }
        public bool State_Edit
        {
            get { return myState_Edit; }
            set { myState_Edit = value; }
        }
        public bool ZipCode_Edit
        {
            get { return myZipCode_Edit; }
            set { myZipCode_Edit = value; }
        }
        public bool Country_Edit
        {
            get { return myCountry_Edit; }
            set { myCountry_Edit = value; }
        }
        public bool County_Edit
        {
            get { return myCounty_Edit; }
            set { myCounty_Edit = value; }
        }
        public bool ParentGuardianFirstName_Edit
        {
            get { return myParentGuardianFirstName_Edit; }
            set { myParentGuardianFirstName_Edit = value; }
        }
        public bool ParentGuardianLastName_Edit
        {
            get { return myParentGuardianLastName_Edit; }
            set { myParentGuardianLastName_Edit = value; }
        }
        public bool ParentGuardianMiddleName_Edit
        {
            get { return myParentGuardianMiddleName_Edit; }
            set { myParentGuardianMiddleName_Edit = value; }
        }
        public bool PrimaryLibrary_Edit
        {
            get { return myPrimaryLibrary_Edit; }
            set { myPrimaryLibrary_Edit = value; }
        }
        public bool LibraryCard_Edit
        {
            get { return myLibraryCard_Edit; }
            set { myLibraryCard_Edit = value; }
        }
        public bool SchoolName_Edit
        {
            get { return mySchoolName_Edit; }
            set { mySchoolName_Edit = value; }
        }
        public bool District_Edit
        {
            get { return myDistrict_Edit; }
            set { myDistrict_Edit = value; }
        }
        public bool SDistrict_Edit
        {
            get { return mySDistrict_Edit; }
            set { mySDistrict_Edit = value; }
        }
        public bool Teacher_Edit
        {
            get { return myTeacher_Edit; }
            set { myTeacher_Edit = value; }
        }
        public bool GroupTeamName_Edit
        {
            get { return myGroupTeamName_Edit; }
            set { myGroupTeamName_Edit = value; }
        }
        public bool SchoolType_Edit
        {
            get { return mySchoolType_Edit; }
            set { mySchoolType_Edit = value; }
        }
        public bool LiteracyLevel1_Edit
        {
            get { return myLiteracyLevel1_Edit; }
            set { myLiteracyLevel1_Edit = value; }
        }
        public bool LiteracyLevel2_Edit
        {
            get { return myLiteracyLevel2_Edit; }
            set { myLiteracyLevel2_Edit = value; }
        }
        public bool ParentPermFlag_Edit
        {
            get { return myParentPermFlag_Edit; }
            set { myParentPermFlag_Edit = value; }
        }
        public bool Over18Flag_Edit
        {
            get { return myOver18Flag_Edit; }
            set { myOver18Flag_Edit = value; }
        }
        public bool ShareFlag_Edit
        {
            get { return myShareFlag_Edit; }
            set { myShareFlag_Edit = value; }
        }
        public bool TermsOfUseflag_Edit
        {
            get { return myTermsOfUseflag_Edit; }
            set { myTermsOfUseflag_Edit = value; }
        }
        public bool Custom1_Edit
        {
            get { return myCustom1_Edit; }
            set { myCustom1_Edit = value; }
        }
        public bool Custom2_Edit
        {
            get { return myCustom2_Edit; }
            set { myCustom2_Edit = value; }
        }
        public bool Custom3_Edit
        {
            get { return myCustom3_Edit; }
            set { myCustom3_Edit = value; }
        }
        public bool Custom4_Edit
        {
            get { return myCustom4_Edit; }
            set { myCustom4_Edit = value; }
        }
        public bool Custom5_Edit
        {
            get { return myCustom5_Edit; }
            set { myCustom5_Edit = value; }
        }
        public DateTime LastModDate
        {
            get { return myLastModDate; }
            set { myLastModDate = value; }
        }
        public string LastModUser
        {
            get { return myLastModUser; }
            set { myLastModUser = value; }
        }
        public DateTime AddedDate
        {
            get { return myAddedDate; }
            set { myAddedDate = value; }
        }
        public string AddedUser
        {
            get { return myAddedUser; }
            set { myAddedUser = value; }
        }

        public int TenID
        {
            get { return myTenID; }
            set { myTenID = value; }
        }

        public int FldInt1
        {
            get { return myFldInt1; }
            set { myFldInt1 = value; }
        }

        public int FldInt2
        {
            get { return myFldInt2; }
            set { myFldInt2 = value; }
        }

        public int FldInt3
        {
            get { return myFldInt3; }
            set { myFldInt3 = value; }
        }

        public bool FldBit1
        {
            get { return myFldBit1; }
            set { myFldBit1 = value; }
        }

        public bool FldBit2
        {
            get { return myFldBit2; }
            set { myFldBit2 = value; }
        }

        public bool FldBit3
        {
            get { return myFldBit3; }
            set { myFldBit3 = value; }
        }

        public string FldText1
        {
            get { return myFldText1; }
            set { myFldText1 = value; }
        }

        public string FldText2
        {
            get { return myFldText2; }
            set { myFldText2 = value; }
        }

        public string FldText3
        {
            get { return myFldText3; }
            set { myFldText3 = value; }
        }

        #endregion

        #region Constructors

        public RegistrationSettings()
        {
            this.TenID = (HttpContext.Current.Session["TenantID"] == null ||
                 HttpContext.Current.Session["TenantID"].ToString() == ""
                     ? -1
                     : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_RegistrationSettings_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_RegistrationSettings_GetAll", arrParams);
        }

        public static RegistrationSettings FetchObject(int TenID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@TenID", TenID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_RegistrationSettings_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                RegistrationSettings result = new RegistrationSettings();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["RID"].ToString(), out _int)) result.RID = _int;
                result.Literacy1Label = dr["Literacy1Label"].ToString();
                result.Literacy2Label = dr["Literacy2Label"].ToString();
                result.DOB_Prompt = bool.Parse(dr["DOB_Prompt"].ToString());
                result.Age_Prompt = bool.Parse(dr["Age_Prompt"].ToString());
                result.SchoolGrade_Prompt = bool.Parse(dr["SchoolGrade_Prompt"].ToString());
                result.FirstName_Prompt = bool.Parse(dr["FirstName_Prompt"].ToString());
                result.MiddleName_Prompt = bool.Parse(dr["MiddleName_Prompt"].ToString());
                result.LastName_Prompt = bool.Parse(dr["LastName_Prompt"].ToString());
                result.Gender_Prompt = bool.Parse(dr["Gender_Prompt"].ToString());
                result.EmailAddress_Prompt = bool.Parse(dr["EmailAddress_Prompt"].ToString());
                result.PhoneNumber_Prompt = bool.Parse(dr["PhoneNumber_Prompt"].ToString());
                result.StreetAddress1_Prompt = bool.Parse(dr["StreetAddress1_Prompt"].ToString());
                result.StreetAddress2_Prompt = bool.Parse(dr["StreetAddress2_Prompt"].ToString());
                result.City_Prompt = bool.Parse(dr["City_Prompt"].ToString());
                result.State_Prompt = bool.Parse(dr["State_Prompt"].ToString());
                result.ZipCode_Prompt = bool.Parse(dr["ZipCode_Prompt"].ToString());
                result.Country_Prompt = bool.Parse(dr["Country_Prompt"].ToString());
                result.County_Prompt = bool.Parse(dr["County_Prompt"].ToString());
                result.ParentGuardianFirstName_Prompt = bool.Parse(dr["ParentGuardianFirstName_Prompt"].ToString());
                result.ParentGuardianLastName_Prompt = bool.Parse(dr["ParentGuardianLastName_Prompt"].ToString());
                result.ParentGuardianMiddleName_Prompt = bool.Parse(dr["ParentGuardianMiddleName_Prompt"].ToString());
                result.PrimaryLibrary_Prompt = bool.Parse(dr["PrimaryLibrary_Prompt"].ToString());
                result.LibraryCard_Prompt = bool.Parse(dr["LibraryCard_Prompt"].ToString());
                result.SchoolName_Prompt = bool.Parse(dr["SchoolName_Prompt"].ToString());
                result.District_Prompt = bool.Parse(dr["District_Prompt"].ToString());
                result.SDistrict_Prompt = bool.Parse(dr["SDistrict_Prompt"].ToString());
                result.Teacher_Prompt = bool.Parse(dr["Teacher_Prompt"].ToString());
                result.GroupTeamName_Prompt = bool.Parse(dr["GroupTeamName_Prompt"].ToString());
                result.SchoolType_Prompt = bool.Parse(dr["SchoolType_Prompt"].ToString());
                result.LiteracyLevel1_Prompt = bool.Parse(dr["LiteracyLevel1_Prompt"].ToString());
                result.LiteracyLevel2_Prompt = bool.Parse(dr["LiteracyLevel2_Prompt"].ToString());
                result.ParentPermFlag_Prompt = bool.Parse(dr["ParentPermFlag_Prompt"].ToString());
                result.Over18Flag_Prompt = bool.Parse(dr["Over18Flag_Prompt"].ToString());
                result.ShareFlag_Prompt = bool.Parse(dr["ShareFlag_Prompt"].ToString());
                result.TermsOfUseflag_Prompt = bool.Parse(dr["TermsOfUseflag_Prompt"].ToString());
                result.Custom1_Prompt = bool.Parse(dr["Custom1_Prompt"].ToString());
                result.Custom2_Prompt = bool.Parse(dr["Custom2_Prompt"].ToString());
                result.Custom3_Prompt = bool.Parse(dr["Custom3_Prompt"].ToString());
                result.Custom4_Prompt = bool.Parse(dr["Custom4_Prompt"].ToString());
                result.Custom5_Prompt = bool.Parse(dr["Custom5_Prompt"].ToString());
                result.DOB_Req = bool.Parse(dr["DOB_Req"].ToString());
                result.Age_Req = bool.Parse(dr["Age_Req"].ToString());
                result.SchoolGrade_Req = bool.Parse(dr["SchoolGrade_Req"].ToString());
                result.FirstName_Req = bool.Parse(dr["FirstName_Req"].ToString());
                result.MiddleName_Req = bool.Parse(dr["MiddleName_Req"].ToString());
                result.LastName_Req = bool.Parse(dr["LastName_Req"].ToString());
                result.Gender_Req = bool.Parse(dr["Gender_Req"].ToString());
                result.EmailAddress_Req = bool.Parse(dr["EmailAddress_Req"].ToString());
                result.PhoneNumber_Req = bool.Parse(dr["PhoneNumber_Req"].ToString());
                result.StreetAddress1_Req = bool.Parse(dr["StreetAddress1_Req"].ToString());
                result.StreetAddress2_Req = bool.Parse(dr["StreetAddress2_Req"].ToString());
                result.City_Req = bool.Parse(dr["City_Req"].ToString());
                result.State_Req = bool.Parse(dr["State_Req"].ToString());
                result.ZipCode_Req = bool.Parse(dr["ZipCode_Req"].ToString());
                result.Country_Req = bool.Parse(dr["Country_Req"].ToString());
                result.County_Req = bool.Parse(dr["County_Req"].ToString());
                result.ParentGuardianFirstName_Req = bool.Parse(dr["ParentGuardianFirstName_Req"].ToString());
                result.ParentGuardianLastName_Req = bool.Parse(dr["ParentGuardianLastName_Req"].ToString());
                result.ParentGuardianMiddleName_Req = bool.Parse(dr["ParentGuardianMiddleName_Req"].ToString());
                result.PrimaryLibrary_Req = bool.Parse(dr["PrimaryLibrary_Req"].ToString());
                result.LibraryCard_Req = bool.Parse(dr["LibraryCard_Req"].ToString());
                result.SchoolName_Req = bool.Parse(dr["SchoolName_Req"].ToString());
                result.District_Req = bool.Parse(dr["District_Req"].ToString());
                result.SDistrict_Req = bool.Parse(dr["SDistrict_Req"].ToString());
                result.Teacher_Req = bool.Parse(dr["Teacher_Req"].ToString());
                result.GroupTeamName_Req = bool.Parse(dr["GroupTeamName_Req"].ToString());
                result.SchoolType_Req = bool.Parse(dr["SchoolType_Req"].ToString());
                result.LiteracyLevel1_Req = bool.Parse(dr["LiteracyLevel1_Req"].ToString());
                result.LiteracyLevel2_Req = bool.Parse(dr["LiteracyLevel2_Req"].ToString());
                result.ParentPermFlag_Req = bool.Parse(dr["ParentPermFlag_Req"].ToString());
                result.Over18Flag_Req = bool.Parse(dr["Over18Flag_Req"].ToString());
                result.ShareFlag_Req = bool.Parse(dr["ShareFlag_Req"].ToString());
                result.TermsOfUseflag_Req = bool.Parse(dr["TermsOfUseflag_Req"].ToString());
                result.Custom1_Req = bool.Parse(dr["Custom1_Req"].ToString());
                result.Custom2_Req = bool.Parse(dr["Custom2_Req"].ToString());
                result.Custom3_Req = bool.Parse(dr["Custom3_Req"].ToString());
                result.Custom4_Req = bool.Parse(dr["Custom4_Req"].ToString());
                result.Custom5_Req = bool.Parse(dr["Custom5_Req"].ToString());
                result.DOB_Show = bool.Parse(dr["DOB_Show"].ToString());
                result.Age_Show = bool.Parse(dr["Age_Show"].ToString());
                result.SchoolGrade_Show = bool.Parse(dr["SchoolGrade_Show"].ToString());
                result.FirstName_Show = bool.Parse(dr["FirstName_Show"].ToString());
                result.MiddleName_Show = bool.Parse(dr["MiddleName_Show"].ToString());
                result.LastName_Show = bool.Parse(dr["LastName_Show"].ToString());
                result.Gender_Show = bool.Parse(dr["Gender_Show"].ToString());
                result.EmailAddress_Show = bool.Parse(dr["EmailAddress_Show"].ToString());
                result.PhoneNumber_Show = bool.Parse(dr["PhoneNumber_Show"].ToString());
                result.StreetAddress1_Show = bool.Parse(dr["StreetAddress1_Show"].ToString());
                result.StreetAddress2_Show = bool.Parse(dr["StreetAddress2_Show"].ToString());
                result.City_Show = bool.Parse(dr["City_Show"].ToString());
                result.State_Show = bool.Parse(dr["State_Show"].ToString());
                result.ZipCode_Show = bool.Parse(dr["ZipCode_Show"].ToString());
                result.Country_Show = bool.Parse(dr["Country_Show"].ToString());
                result.County_Show = bool.Parse(dr["County_Show"].ToString());
                result.ParentGuardianFirstName_Show = bool.Parse(dr["ParentGuardianFirstName_Show"].ToString());
                result.ParentGuardianLastName_Show = bool.Parse(dr["ParentGuardianLastName_Show"].ToString());
                result.ParentGuardianMiddleName_Show = bool.Parse(dr["ParentGuardianMiddleName_Show"].ToString());
                result.PrimaryLibrary_Show = bool.Parse(dr["PrimaryLibrary_Show"].ToString());
                result.LibraryCard_Show = bool.Parse(dr["LibraryCard_Show"].ToString());
                result.SchoolName_Show = bool.Parse(dr["SchoolName_Show"].ToString());
                result.District_Show = bool.Parse(dr["District_Show"].ToString());
                result.SDistrict_Show = bool.Parse(dr["SDistrict_Show"].ToString());
                result.Teacher_Show = bool.Parse(dr["Teacher_Show"].ToString());
                result.GroupTeamName_Show = bool.Parse(dr["GroupTeamName_Show"].ToString());
                result.SchoolType_Show = bool.Parse(dr["SchoolType_Show"].ToString());
                result.LiteracyLevel1_Show = bool.Parse(dr["LiteracyLevel1_Show"].ToString());
                result.LiteracyLevel2_Show = bool.Parse(dr["LiteracyLevel2_Show"].ToString());
                result.ParentPermFlag_Show = bool.Parse(dr["ParentPermFlag_Show"].ToString());
                result.Over18Flag_Show = bool.Parse(dr["Over18Flag_Show"].ToString());
                result.ShareFlag_Show = bool.Parse(dr["ShareFlag_Show"].ToString());
                result.TermsOfUseflag_Show = bool.Parse(dr["TermsOfUseflag_Show"].ToString());
                result.Custom1_Show = bool.Parse(dr["Custom1_Show"].ToString());
                result.Custom2_Show = bool.Parse(dr["Custom2_Show"].ToString());
                result.Custom3_Show = bool.Parse(dr["Custom3_Show"].ToString());
                result.Custom4_Show = bool.Parse(dr["Custom4_Show"].ToString());
                result.Custom5_Show = bool.Parse(dr["Custom5_Show"].ToString());
                result.DOB_Edit = bool.Parse(dr["DOB_Edit"].ToString());
                result.Age_Edit = bool.Parse(dr["Age_Edit"].ToString());
                result.SchoolGrade_Edit = bool.Parse(dr["SchoolGrade_Edit"].ToString());
                result.FirstName_Edit = bool.Parse(dr["FirstName_Edit"].ToString());
                result.MiddleName_Edit = bool.Parse(dr["MiddleName_Edit"].ToString());
                result.LastName_Edit = bool.Parse(dr["LastName_Edit"].ToString());
                result.Gender_Edit = bool.Parse(dr["Gender_Edit"].ToString());
                result.EmailAddress_Edit = bool.Parse(dr["EmailAddress_Edit"].ToString());
                result.PhoneNumber_Edit = bool.Parse(dr["PhoneNumber_Edit"].ToString());
                result.StreetAddress1_Edit = bool.Parse(dr["StreetAddress1_Edit"].ToString());
                result.StreetAddress2_Edit = bool.Parse(dr["StreetAddress2_Edit"].ToString());
                result.City_Edit = bool.Parse(dr["City_Edit"].ToString());
                result.State_Edit = bool.Parse(dr["State_Edit"].ToString());
                result.ZipCode_Edit = bool.Parse(dr["ZipCode_Edit"].ToString());
                result.Country_Edit = bool.Parse(dr["Country_Edit"].ToString());
                result.County_Edit = bool.Parse(dr["County_Edit"].ToString());
                result.ParentGuardianFirstName_Edit = bool.Parse(dr["ParentGuardianFirstName_Edit"].ToString());
                result.ParentGuardianLastName_Edit = bool.Parse(dr["ParentGuardianLastName_Edit"].ToString());
                result.ParentGuardianMiddleName_Edit = bool.Parse(dr["ParentGuardianMiddleName_Edit"].ToString());
                result.PrimaryLibrary_Edit = bool.Parse(dr["PrimaryLibrary_Edit"].ToString());
                result.LibraryCard_Edit = bool.Parse(dr["LibraryCard_Edit"].ToString());
                result.SchoolName_Edit = bool.Parse(dr["SchoolName_Edit"].ToString());
                result.District_Edit = bool.Parse(dr["District_Edit"].ToString());
                result.SDistrict_Edit = bool.Parse(dr["SDistrict_Edit"].ToString());
                result.Teacher_Edit = bool.Parse(dr["Teacher_Edit"].ToString());
                result.GroupTeamName_Edit = bool.Parse(dr["GroupTeamName_Edit"].ToString());
                result.SchoolType_Edit = bool.Parse(dr["SchoolType_Edit"].ToString());
                result.LiteracyLevel1_Edit = bool.Parse(dr["LiteracyLevel1_Edit"].ToString());
                result.LiteracyLevel2_Edit = bool.Parse(dr["LiteracyLevel2_Edit"].ToString());
                result.ParentPermFlag_Edit = bool.Parse(dr["ParentPermFlag_Edit"].ToString());
                result.Over18Flag_Edit = bool.Parse(dr["Over18Flag_Edit"].ToString());
                result.ShareFlag_Edit = bool.Parse(dr["ShareFlag_Edit"].ToString());
                result.TermsOfUseflag_Edit = bool.Parse(dr["TermsOfUseflag_Edit"].ToString());
                result.Custom1_Edit = bool.Parse(dr["Custom1_Edit"].ToString());
                result.Custom2_Edit = bool.Parse(dr["Custom2_Edit"].ToString());
                result.Custom3_Edit = bool.Parse(dr["Custom3_Edit"].ToString());
                result.Custom4_Edit = bool.Parse(dr["Custom4_Edit"].ToString());
                result.Custom5_Edit = bool.Parse(dr["Custom5_Edit"].ToString());
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int)) result.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int)) result.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int)) result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();
                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch()
        {
            return Fetch(TenID);
        }

        public bool Fetch(int aTenID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            if (aTenID == 0)
            {
                aTenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
            }

            arrParams[0] = new SqlParameter("@TenID", aTenID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_RegistrationSettings_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                RegistrationSettings result = new RegistrationSettings();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["RID"].ToString(), out _int)) this.RID = _int;
                this.Literacy1Label = dr["Literacy1Label"].ToString();
                this.Literacy2Label = dr["Literacy2Label"].ToString();
                this.DOB_Prompt = bool.Parse(dr["DOB_Prompt"].ToString());
                this.Age_Prompt = bool.Parse(dr["Age_Prompt"].ToString());
                this.SchoolGrade_Prompt = bool.Parse(dr["SchoolGrade_Prompt"].ToString());
                this.FirstName_Prompt = bool.Parse(dr["FirstName_Prompt"].ToString());
                this.MiddleName_Prompt = bool.Parse(dr["MiddleName_Prompt"].ToString());
                this.LastName_Prompt = bool.Parse(dr["LastName_Prompt"].ToString());
                this.Gender_Prompt = bool.Parse(dr["Gender_Prompt"].ToString());
                this.EmailAddress_Prompt = bool.Parse(dr["EmailAddress_Prompt"].ToString());
                this.PhoneNumber_Prompt = bool.Parse(dr["PhoneNumber_Prompt"].ToString());
                this.StreetAddress1_Prompt = bool.Parse(dr["StreetAddress1_Prompt"].ToString());
                this.StreetAddress2_Prompt = bool.Parse(dr["StreetAddress2_Prompt"].ToString());
                this.City_Prompt = bool.Parse(dr["City_Prompt"].ToString());
                this.State_Prompt = bool.Parse(dr["State_Prompt"].ToString());
                this.ZipCode_Prompt = bool.Parse(dr["ZipCode_Prompt"].ToString());
                this.Country_Prompt = bool.Parse(dr["Country_Prompt"].ToString());
                this.County_Prompt = bool.Parse(dr["County_Prompt"].ToString());
                this.ParentGuardianFirstName_Prompt = bool.Parse(dr["ParentGuardianFirstName_Prompt"].ToString());
                this.ParentGuardianLastName_Prompt = bool.Parse(dr["ParentGuardianLastName_Prompt"].ToString());
                this.ParentGuardianMiddleName_Prompt = bool.Parse(dr["ParentGuardianMiddleName_Prompt"].ToString());
                this.PrimaryLibrary_Prompt = bool.Parse(dr["PrimaryLibrary_Prompt"].ToString());
                this.LibraryCard_Prompt = bool.Parse(dr["LibraryCard_Prompt"].ToString());
                this.SchoolName_Prompt = bool.Parse(dr["SchoolName_Prompt"].ToString());
                this.District_Prompt = bool.Parse(dr["District_Prompt"].ToString());
                this.SDistrict_Prompt = bool.Parse(dr["SDistrict_Prompt"].ToString());
                this.Teacher_Prompt = bool.Parse(dr["Teacher_Prompt"].ToString());
                this.GroupTeamName_Prompt = bool.Parse(dr["GroupTeamName_Prompt"].ToString());
                this.SchoolType_Prompt = bool.Parse(dr["SchoolType_Prompt"].ToString());
                this.LiteracyLevel1_Prompt = bool.Parse(dr["LiteracyLevel1_Prompt"].ToString());
                this.LiteracyLevel2_Prompt = bool.Parse(dr["LiteracyLevel2_Prompt"].ToString());
                this.ParentPermFlag_Prompt = bool.Parse(dr["ParentPermFlag_Prompt"].ToString());
                this.Over18Flag_Prompt = bool.Parse(dr["Over18Flag_Prompt"].ToString());
                this.ShareFlag_Prompt = bool.Parse(dr["ShareFlag_Prompt"].ToString());
                this.TermsOfUseflag_Prompt = bool.Parse(dr["TermsOfUseflag_Prompt"].ToString());
                this.Custom1_Prompt = bool.Parse(dr["Custom1_Prompt"].ToString());
                this.Custom2_Prompt = bool.Parse(dr["Custom2_Prompt"].ToString());
                this.Custom3_Prompt = bool.Parse(dr["Custom3_Prompt"].ToString());
                this.Custom4_Prompt = bool.Parse(dr["Custom4_Prompt"].ToString());
                this.Custom5_Prompt = bool.Parse(dr["Custom5_Prompt"].ToString());
                this.DOB_Req = bool.Parse(dr["DOB_Req"].ToString());
                this.Age_Req = bool.Parse(dr["Age_Req"].ToString());
                this.SchoolGrade_Req = bool.Parse(dr["SchoolGrade_Req"].ToString());
                this.FirstName_Req = bool.Parse(dr["FirstName_Req"].ToString());
                this.MiddleName_Req = bool.Parse(dr["MiddleName_Req"].ToString());
                this.LastName_Req = bool.Parse(dr["LastName_Req"].ToString());
                this.Gender_Req = bool.Parse(dr["Gender_Req"].ToString());
                this.EmailAddress_Req = bool.Parse(dr["EmailAddress_Req"].ToString());
                this.PhoneNumber_Req = bool.Parse(dr["PhoneNumber_Req"].ToString());
                this.StreetAddress1_Req = bool.Parse(dr["StreetAddress1_Req"].ToString());
                this.StreetAddress2_Req = bool.Parse(dr["StreetAddress2_Req"].ToString());
                this.City_Req = bool.Parse(dr["City_Req"].ToString());
                this.State_Req = bool.Parse(dr["State_Req"].ToString());
                this.ZipCode_Req = bool.Parse(dr["ZipCode_Req"].ToString());
                this.Country_Req = bool.Parse(dr["Country_Req"].ToString());
                this.County_Req = bool.Parse(dr["County_Req"].ToString());
                this.ParentGuardianFirstName_Req = bool.Parse(dr["ParentGuardianFirstName_Req"].ToString());
                this.ParentGuardianLastName_Req = bool.Parse(dr["ParentGuardianLastName_Req"].ToString());
                this.ParentGuardianMiddleName_Req = bool.Parse(dr["ParentGuardianMiddleName_Req"].ToString());
                this.PrimaryLibrary_Req = bool.Parse(dr["PrimaryLibrary_Req"].ToString());
                this.LibraryCard_Req = bool.Parse(dr["LibraryCard_Req"].ToString());
                this.SchoolName_Req = bool.Parse(dr["SchoolName_Req"].ToString());
                this.District_Req = bool.Parse(dr["District_Req"].ToString());
                this.SDistrict_Req = bool.Parse(dr["SDistrict_Req"].ToString());
                this.Teacher_Req = bool.Parse(dr["Teacher_Req"].ToString());
                this.GroupTeamName_Req = bool.Parse(dr["GroupTeamName_Req"].ToString());
                this.SchoolType_Req = bool.Parse(dr["SchoolType_Req"].ToString());
                this.LiteracyLevel1_Req = bool.Parse(dr["LiteracyLevel1_Req"].ToString());
                this.LiteracyLevel2_Req = bool.Parse(dr["LiteracyLevel2_Req"].ToString());
                this.ParentPermFlag_Req = bool.Parse(dr["ParentPermFlag_Req"].ToString());
                this.Over18Flag_Req = bool.Parse(dr["Over18Flag_Req"].ToString());
                this.ShareFlag_Req = bool.Parse(dr["ShareFlag_Req"].ToString());
                this.TermsOfUseflag_Req = bool.Parse(dr["TermsOfUseflag_Req"].ToString());
                this.Custom1_Req = bool.Parse(dr["Custom1_Req"].ToString());
                this.Custom2_Req = bool.Parse(dr["Custom2_Req"].ToString());
                this.Custom3_Req = bool.Parse(dr["Custom3_Req"].ToString());
                this.Custom4_Req = bool.Parse(dr["Custom4_Req"].ToString());
                this.Custom5_Req = bool.Parse(dr["Custom5_Req"].ToString());
                this.DOB_Show = bool.Parse(dr["DOB_Show"].ToString());
                this.Age_Show = bool.Parse(dr["Age_Show"].ToString());
                this.SchoolGrade_Show = bool.Parse(dr["SchoolGrade_Show"].ToString());
                this.FirstName_Show = bool.Parse(dr["FirstName_Show"].ToString());
                this.MiddleName_Show = bool.Parse(dr["MiddleName_Show"].ToString());
                this.LastName_Show = bool.Parse(dr["LastName_Show"].ToString());
                this.Gender_Show = bool.Parse(dr["Gender_Show"].ToString());
                this.EmailAddress_Show = bool.Parse(dr["EmailAddress_Show"].ToString());
                this.PhoneNumber_Show = bool.Parse(dr["PhoneNumber_Show"].ToString());
                this.StreetAddress1_Show = bool.Parse(dr["StreetAddress1_Show"].ToString());
                this.StreetAddress2_Show = bool.Parse(dr["StreetAddress2_Show"].ToString());
                this.City_Show = bool.Parse(dr["City_Show"].ToString());
                this.State_Show = bool.Parse(dr["State_Show"].ToString());
                this.ZipCode_Show = bool.Parse(dr["ZipCode_Show"].ToString());
                this.Country_Show = bool.Parse(dr["Country_Show"].ToString());
                this.County_Show = bool.Parse(dr["County_Show"].ToString());
                this.ParentGuardianFirstName_Show = bool.Parse(dr["ParentGuardianFirstName_Show"].ToString());
                this.ParentGuardianLastName_Show = bool.Parse(dr["ParentGuardianLastName_Show"].ToString());
                this.ParentGuardianMiddleName_Show = bool.Parse(dr["ParentGuardianMiddleName_Show"].ToString());
                this.PrimaryLibrary_Show = bool.Parse(dr["PrimaryLibrary_Show"].ToString());
                this.LibraryCard_Show = bool.Parse(dr["LibraryCard_Show"].ToString());
                this.SchoolName_Show = bool.Parse(dr["SchoolName_Show"].ToString());
                this.District_Show = bool.Parse(dr["District_Show"].ToString());
                this.SDistrict_Show = bool.Parse(dr["SDistrict_Show"].ToString());
                this.Teacher_Show = bool.Parse(dr["Teacher_Show"].ToString());
                this.GroupTeamName_Show = bool.Parse(dr["GroupTeamName_Show"].ToString());
                this.SchoolType_Show = bool.Parse(dr["SchoolType_Show"].ToString());
                this.LiteracyLevel1_Show = bool.Parse(dr["LiteracyLevel1_Show"].ToString());
                this.LiteracyLevel2_Show = bool.Parse(dr["LiteracyLevel2_Show"].ToString());
                this.ParentPermFlag_Show = bool.Parse(dr["ParentPermFlag_Show"].ToString());
                this.Over18Flag_Show = bool.Parse(dr["Over18Flag_Show"].ToString());
                this.ShareFlag_Show = bool.Parse(dr["ShareFlag_Show"].ToString());
                this.TermsOfUseflag_Show = bool.Parse(dr["TermsOfUseflag_Show"].ToString());
                this.Custom1_Show = bool.Parse(dr["Custom1_Show"].ToString());
                this.Custom2_Show = bool.Parse(dr["Custom2_Show"].ToString());
                this.Custom3_Show = bool.Parse(dr["Custom3_Show"].ToString());
                this.Custom4_Show = bool.Parse(dr["Custom4_Show"].ToString());
                this.Custom5_Show = bool.Parse(dr["Custom5_Show"].ToString());
                this.DOB_Edit = bool.Parse(dr["DOB_Edit"].ToString());
                this.Age_Edit = bool.Parse(dr["Age_Edit"].ToString());
                this.SchoolGrade_Edit = bool.Parse(dr["SchoolGrade_Edit"].ToString());
                this.FirstName_Edit = bool.Parse(dr["FirstName_Edit"].ToString());
                this.MiddleName_Edit = bool.Parse(dr["MiddleName_Edit"].ToString());
                this.LastName_Edit = bool.Parse(dr["LastName_Edit"].ToString());
                this.Gender_Edit = bool.Parse(dr["Gender_Edit"].ToString());
                this.EmailAddress_Edit = bool.Parse(dr["EmailAddress_Edit"].ToString());
                this.PhoneNumber_Edit = bool.Parse(dr["PhoneNumber_Edit"].ToString());
                this.StreetAddress1_Edit = bool.Parse(dr["StreetAddress1_Edit"].ToString());
                this.StreetAddress2_Edit = bool.Parse(dr["StreetAddress2_Edit"].ToString());
                this.City_Edit = bool.Parse(dr["City_Edit"].ToString());
                this.State_Edit = bool.Parse(dr["State_Edit"].ToString());
                this.ZipCode_Edit = bool.Parse(dr["ZipCode_Edit"].ToString());
                this.Country_Edit = bool.Parse(dr["Country_Edit"].ToString());
                this.County_Edit = bool.Parse(dr["County_Edit"].ToString());
                this.ParentGuardianFirstName_Edit = bool.Parse(dr["ParentGuardianFirstName_Edit"].ToString());
                this.ParentGuardianLastName_Edit = bool.Parse(dr["ParentGuardianLastName_Edit"].ToString());
                this.ParentGuardianMiddleName_Edit = bool.Parse(dr["ParentGuardianMiddleName_Edit"].ToString());
                this.PrimaryLibrary_Edit = bool.Parse(dr["PrimaryLibrary_Edit"].ToString());
                this.LibraryCard_Edit = bool.Parse(dr["LibraryCard_Edit"].ToString());
                this.SchoolName_Edit = bool.Parse(dr["SchoolName_Edit"].ToString());
                this.District_Edit = bool.Parse(dr["District_Edit"].ToString());
                this.SDistrict_Edit = bool.Parse(dr["SDistrict_Edit"].ToString());
                this.Teacher_Edit = bool.Parse(dr["Teacher_Edit"].ToString());
                this.GroupTeamName_Edit = bool.Parse(dr["GroupTeamName_Edit"].ToString());
                this.SchoolType_Edit = bool.Parse(dr["SchoolType_Edit"].ToString());
                this.LiteracyLevel1_Edit = bool.Parse(dr["LiteracyLevel1_Edit"].ToString());
                this.LiteracyLevel2_Edit = bool.Parse(dr["LiteracyLevel2_Edit"].ToString());
                this.ParentPermFlag_Edit = bool.Parse(dr["ParentPermFlag_Edit"].ToString());
                this.Over18Flag_Edit = bool.Parse(dr["Over18Flag_Edit"].ToString());
                this.ShareFlag_Edit = bool.Parse(dr["ShareFlag_Edit"].ToString());
                this.TermsOfUseflag_Edit = bool.Parse(dr["TermsOfUseflag_Edit"].ToString());
                this.Custom1_Edit = bool.Parse(dr["Custom1_Edit"].ToString());
                this.Custom2_Edit = bool.Parse(dr["Custom2_Edit"].ToString());
                this.Custom3_Edit = bool.Parse(dr["Custom3_Edit"].ToString());
                this.Custom4_Edit = bool.Parse(dr["Custom4_Edit"].ToString());
                this.Custom5_Edit = bool.Parse(dr["Custom5_Edit"].ToString());
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

                if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int)) this.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int)) this.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int)) this.FldInt3 = _int;
                this.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                this.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                this.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                this.FldText1 = dr["FldText1"].ToString();
                this.FldText2 = dr["FldText2"].ToString();
                this.FldText3 = dr["FldText3"].ToString();
                dr.Close();

                return true;

            }

            dr.Close();

            return false;

        }

        public int Insert()
        {

            return Insert(this);

        }

        public static int Insert(RegistrationSettings o)
        {

            SqlParameter[] arrParams = new SqlParameter[169];

            arrParams[0] = new SqlParameter("@Literacy1Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy1Label, o.Literacy1Label.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Literacy2Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy2Label, o.Literacy2Label.GetTypeCode()));
            arrParams[2] = new SqlParameter("@DOB_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Prompt, o.DOB_Prompt.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Age_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Prompt, o.Age_Prompt.GetTypeCode()));
            arrParams[4] = new SqlParameter("@SchoolGrade_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Prompt, o.SchoolGrade_Prompt.GetTypeCode()));
            arrParams[5] = new SqlParameter("@FirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Prompt, o.FirstName_Prompt.GetTypeCode()));
            arrParams[6] = new SqlParameter("@MiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Prompt, o.MiddleName_Prompt.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Prompt, o.LastName_Prompt.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Gender_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Prompt, o.Gender_Prompt.GetTypeCode()));
            arrParams[9] = new SqlParameter("@EmailAddress_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Prompt, o.EmailAddress_Prompt.GetTypeCode()));
            arrParams[10] = new SqlParameter("@PhoneNumber_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Prompt, o.PhoneNumber_Prompt.GetTypeCode()));
            arrParams[11] = new SqlParameter("@StreetAddress1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Prompt, o.StreetAddress1_Prompt.GetTypeCode()));
            arrParams[12] = new SqlParameter("@StreetAddress2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Prompt, o.StreetAddress2_Prompt.GetTypeCode()));
            arrParams[13] = new SqlParameter("@City_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Prompt, o.City_Prompt.GetTypeCode()));
            arrParams[14] = new SqlParameter("@State_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Prompt, o.State_Prompt.GetTypeCode()));
            arrParams[15] = new SqlParameter("@ZipCode_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Prompt, o.ZipCode_Prompt.GetTypeCode()));
            arrParams[16] = new SqlParameter("@Country_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Prompt, o.Country_Prompt.GetTypeCode()));
            arrParams[17] = new SqlParameter("@County_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Prompt, o.County_Prompt.GetTypeCode()));
            arrParams[18] = new SqlParameter("@ParentGuardianFirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Prompt, o.ParentGuardianFirstName_Prompt.GetTypeCode()));
            arrParams[19] = new SqlParameter("@ParentGuardianLastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Prompt, o.ParentGuardianLastName_Prompt.GetTypeCode()));
            arrParams[20] = new SqlParameter("@ParentGuardianMiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Prompt, o.ParentGuardianMiddleName_Prompt.GetTypeCode()));
            arrParams[21] = new SqlParameter("@PrimaryLibrary_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Prompt, o.PrimaryLibrary_Prompt.GetTypeCode()));
            arrParams[22] = new SqlParameter("@LibraryCard_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Prompt, o.LibraryCard_Prompt.GetTypeCode()));
            arrParams[23] = new SqlParameter("@SchoolName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Prompt, o.SchoolName_Prompt.GetTypeCode()));
            arrParams[24] = new SqlParameter("@District_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Prompt, o.District_Prompt.GetTypeCode()));
            arrParams[25] = new SqlParameter("@Teacher_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Prompt, o.Teacher_Prompt.GetTypeCode()));
            arrParams[26] = new SqlParameter("@GroupTeamName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Prompt, o.GroupTeamName_Prompt.GetTypeCode()));
            arrParams[27] = new SqlParameter("@SchoolType_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Prompt, o.SchoolType_Prompt.GetTypeCode()));
            arrParams[28] = new SqlParameter("@LiteracyLevel1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Prompt, o.LiteracyLevel1_Prompt.GetTypeCode()));
            arrParams[29] = new SqlParameter("@LiteracyLevel2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Prompt, o.LiteracyLevel2_Prompt.GetTypeCode()));
            arrParams[30] = new SqlParameter("@ParentPermFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Prompt, o.ParentPermFlag_Prompt.GetTypeCode()));
            arrParams[31] = new SqlParameter("@Over18Flag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Prompt, o.Over18Flag_Prompt.GetTypeCode()));
            arrParams[32] = new SqlParameter("@ShareFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Prompt, o.ShareFlag_Prompt.GetTypeCode()));
            arrParams[33] = new SqlParameter("@TermsOfUseflag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Prompt, o.TermsOfUseflag_Prompt.GetTypeCode()));
            arrParams[34] = new SqlParameter("@Custom1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Prompt, o.Custom1_Prompt.GetTypeCode()));
            arrParams[35] = new SqlParameter("@Custom2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Prompt, o.Custom2_Prompt.GetTypeCode()));
            arrParams[36] = new SqlParameter("@Custom3_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Prompt, o.Custom3_Prompt.GetTypeCode()));
            arrParams[37] = new SqlParameter("@Custom4_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Prompt, o.Custom4_Prompt.GetTypeCode()));
            arrParams[38] = new SqlParameter("@Custom5_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Prompt, o.Custom5_Prompt.GetTypeCode()));
            arrParams[39] = new SqlParameter("@DOB_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Req, o.DOB_Req.GetTypeCode()));
            arrParams[40] = new SqlParameter("@Age_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Req, o.Age_Req.GetTypeCode()));
            arrParams[41] = new SqlParameter("@SchoolGrade_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Req, o.SchoolGrade_Req.GetTypeCode()));
            arrParams[42] = new SqlParameter("@FirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Req, o.FirstName_Req.GetTypeCode()));
            arrParams[43] = new SqlParameter("@MiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Req, o.MiddleName_Req.GetTypeCode()));
            arrParams[44] = new SqlParameter("@LastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Req, o.LastName_Req.GetTypeCode()));
            arrParams[45] = new SqlParameter("@Gender_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Req, o.Gender_Req.GetTypeCode()));
            arrParams[46] = new SqlParameter("@EmailAddress_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Req, o.EmailAddress_Req.GetTypeCode()));
            arrParams[47] = new SqlParameter("@PhoneNumber_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Req, o.PhoneNumber_Req.GetTypeCode()));
            arrParams[48] = new SqlParameter("@StreetAddress1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Req, o.StreetAddress1_Req.GetTypeCode()));
            arrParams[49] = new SqlParameter("@StreetAddress2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Req, o.StreetAddress2_Req.GetTypeCode()));
            arrParams[50] = new SqlParameter("@City_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Req, o.City_Req.GetTypeCode()));
            arrParams[51] = new SqlParameter("@State_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Req, o.State_Req.GetTypeCode()));
            arrParams[52] = new SqlParameter("@ZipCode_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Req, o.ZipCode_Req.GetTypeCode()));
            arrParams[53] = new SqlParameter("@Country_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Req, o.Country_Req.GetTypeCode()));
            arrParams[54] = new SqlParameter("@County_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Req, o.County_Req.GetTypeCode()));
            arrParams[55] = new SqlParameter("@ParentGuardianFirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Req, o.ParentGuardianFirstName_Req.GetTypeCode()));
            arrParams[56] = new SqlParameter("@ParentGuardianLastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Req, o.ParentGuardianLastName_Req.GetTypeCode()));
            arrParams[57] = new SqlParameter("@ParentGuardianMiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Req, o.ParentGuardianMiddleName_Req.GetTypeCode()));
            arrParams[58] = new SqlParameter("@PrimaryLibrary_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Req, o.PrimaryLibrary_Req.GetTypeCode()));
            arrParams[59] = new SqlParameter("@LibraryCard_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Req, o.LibraryCard_Req.GetTypeCode()));
            arrParams[60] = new SqlParameter("@SchoolName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Req, o.SchoolName_Req.GetTypeCode()));
            arrParams[61] = new SqlParameter("@District_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Req, o.District_Req.GetTypeCode()));
            arrParams[62] = new SqlParameter("@Teacher_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Req, o.Teacher_Req.GetTypeCode()));
            arrParams[63] = new SqlParameter("@GroupTeamName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Req, o.GroupTeamName_Req.GetTypeCode()));
            arrParams[64] = new SqlParameter("@SchoolType_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Req, o.SchoolType_Req.GetTypeCode()));
            arrParams[65] = new SqlParameter("@LiteracyLevel1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Req, o.LiteracyLevel1_Req.GetTypeCode()));
            arrParams[66] = new SqlParameter("@LiteracyLevel2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Req, o.LiteracyLevel2_Req.GetTypeCode()));
            arrParams[67] = new SqlParameter("@ParentPermFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Req, o.ParentPermFlag_Req.GetTypeCode()));
            arrParams[68] = new SqlParameter("@Over18Flag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Req, o.Over18Flag_Req.GetTypeCode()));
            arrParams[69] = new SqlParameter("@ShareFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Req, o.ShareFlag_Req.GetTypeCode()));
            arrParams[70] = new SqlParameter("@TermsOfUseflag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Req, o.TermsOfUseflag_Req.GetTypeCode()));
            arrParams[71] = new SqlParameter("@Custom1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Req, o.Custom1_Req.GetTypeCode()));
            arrParams[72] = new SqlParameter("@Custom2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Req, o.Custom2_Req.GetTypeCode()));
            arrParams[73] = new SqlParameter("@Custom3_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Req, o.Custom3_Req.GetTypeCode()));
            arrParams[74] = new SqlParameter("@Custom4_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Req, o.Custom4_Req.GetTypeCode()));
            arrParams[75] = new SqlParameter("@Custom5_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Req, o.Custom5_Req.GetTypeCode()));
            arrParams[76] = new SqlParameter("@DOB_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Show, o.DOB_Show.GetTypeCode()));
            arrParams[77] = new SqlParameter("@Age_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Show, o.Age_Show.GetTypeCode()));
            arrParams[78] = new SqlParameter("@SchoolGrade_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Show, o.SchoolGrade_Show.GetTypeCode()));
            arrParams[79] = new SqlParameter("@FirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Show, o.FirstName_Show.GetTypeCode()));
            arrParams[80] = new SqlParameter("@MiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Show, o.MiddleName_Show.GetTypeCode()));
            arrParams[81] = new SqlParameter("@LastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Show, o.LastName_Show.GetTypeCode()));
            arrParams[82] = new SqlParameter("@Gender_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Show, o.Gender_Show.GetTypeCode()));
            arrParams[83] = new SqlParameter("@EmailAddress_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Show, o.EmailAddress_Show.GetTypeCode()));
            arrParams[84] = new SqlParameter("@PhoneNumber_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Show, o.PhoneNumber_Show.GetTypeCode()));
            arrParams[85] = new SqlParameter("@StreetAddress1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Show, o.StreetAddress1_Show.GetTypeCode()));
            arrParams[86] = new SqlParameter("@StreetAddress2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Show, o.StreetAddress2_Show.GetTypeCode()));
            arrParams[87] = new SqlParameter("@City_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Show, o.City_Show.GetTypeCode()));
            arrParams[88] = new SqlParameter("@State_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Show, o.State_Show.GetTypeCode()));
            arrParams[89] = new SqlParameter("@ZipCode_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Show, o.ZipCode_Show.GetTypeCode()));
            arrParams[90] = new SqlParameter("@Country_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Show, o.Country_Show.GetTypeCode()));
            arrParams[91] = new SqlParameter("@County_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Show, o.County_Show.GetTypeCode()));
            arrParams[92] = new SqlParameter("@ParentGuardianFirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Show, o.ParentGuardianFirstName_Show.GetTypeCode()));
            arrParams[93] = new SqlParameter("@ParentGuardianLastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Show, o.ParentGuardianLastName_Show.GetTypeCode()));
            arrParams[94] = new SqlParameter("@ParentGuardianMiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Show, o.ParentGuardianMiddleName_Show.GetTypeCode()));
            arrParams[95] = new SqlParameter("@PrimaryLibrary_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Show, o.PrimaryLibrary_Show.GetTypeCode()));
            arrParams[96] = new SqlParameter("@LibraryCard_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Show, o.LibraryCard_Show.GetTypeCode()));
            arrParams[97] = new SqlParameter("@SchoolName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Show, o.SchoolName_Show.GetTypeCode()));
            arrParams[98] = new SqlParameter("@District_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Show, o.District_Show.GetTypeCode()));
            arrParams[99] = new SqlParameter("@Teacher_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Show, o.Teacher_Show.GetTypeCode()));
            arrParams[100] = new SqlParameter("@GroupTeamName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Show, o.GroupTeamName_Show.GetTypeCode()));
            arrParams[101] = new SqlParameter("@SchoolType_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Show, o.SchoolType_Show.GetTypeCode()));
            arrParams[102] = new SqlParameter("@LiteracyLevel1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Show, o.LiteracyLevel1_Show.GetTypeCode()));
            arrParams[103] = new SqlParameter("@LiteracyLevel2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Show, o.LiteracyLevel2_Show.GetTypeCode()));
            arrParams[104] = new SqlParameter("@ParentPermFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Show, o.ParentPermFlag_Show.GetTypeCode()));
            arrParams[105] = new SqlParameter("@Over18Flag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Show, o.Over18Flag_Show.GetTypeCode()));
            arrParams[106] = new SqlParameter("@ShareFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Show, o.ShareFlag_Show.GetTypeCode()));
            arrParams[107] = new SqlParameter("@TermsOfUseflag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Show, o.TermsOfUseflag_Show.GetTypeCode()));
            arrParams[108] = new SqlParameter("@Custom1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Show, o.Custom1_Show.GetTypeCode()));
            arrParams[109] = new SqlParameter("@Custom2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Show, o.Custom2_Show.GetTypeCode()));
            arrParams[110] = new SqlParameter("@Custom3_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Show, o.Custom3_Show.GetTypeCode()));
            arrParams[111] = new SqlParameter("@Custom4_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Show, o.Custom4_Show.GetTypeCode()));
            arrParams[112] = new SqlParameter("@Custom5_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Show, o.Custom5_Show.GetTypeCode()));
            arrParams[113] = new SqlParameter("@DOB_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Edit, o.DOB_Edit.GetTypeCode()));
            arrParams[114] = new SqlParameter("@Age_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Edit, o.Age_Edit.GetTypeCode()));
            arrParams[115] = new SqlParameter("@SchoolGrade_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Edit, o.SchoolGrade_Edit.GetTypeCode()));
            arrParams[116] = new SqlParameter("@FirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Edit, o.FirstName_Edit.GetTypeCode()));
            arrParams[117] = new SqlParameter("@MiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Edit, o.MiddleName_Edit.GetTypeCode()));
            arrParams[118] = new SqlParameter("@LastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Edit, o.LastName_Edit.GetTypeCode()));
            arrParams[119] = new SqlParameter("@Gender_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Edit, o.Gender_Edit.GetTypeCode()));
            arrParams[120] = new SqlParameter("@EmailAddress_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Edit, o.EmailAddress_Edit.GetTypeCode()));
            arrParams[121] = new SqlParameter("@PhoneNumber_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Edit, o.PhoneNumber_Edit.GetTypeCode()));
            arrParams[122] = new SqlParameter("@StreetAddress1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Edit, o.StreetAddress1_Edit.GetTypeCode()));
            arrParams[123] = new SqlParameter("@StreetAddress2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Edit, o.StreetAddress2_Edit.GetTypeCode()));
            arrParams[124] = new SqlParameter("@City_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Edit, o.City_Edit.GetTypeCode()));
            arrParams[125] = new SqlParameter("@State_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Edit, o.State_Edit.GetTypeCode()));
            arrParams[126] = new SqlParameter("@ZipCode_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Edit, o.ZipCode_Edit.GetTypeCode()));
            arrParams[127] = new SqlParameter("@Country_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Edit, o.Country_Edit.GetTypeCode()));
            arrParams[128] = new SqlParameter("@County_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Edit, o.County_Edit.GetTypeCode()));
            arrParams[129] = new SqlParameter("@ParentGuardianFirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Edit, o.ParentGuardianFirstName_Edit.GetTypeCode()));
            arrParams[130] = new SqlParameter("@ParentGuardianLastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Edit, o.ParentGuardianLastName_Edit.GetTypeCode()));
            arrParams[131] = new SqlParameter("@ParentGuardianMiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Edit, o.ParentGuardianMiddleName_Edit.GetTypeCode()));
            arrParams[132] = new SqlParameter("@PrimaryLibrary_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Edit, o.PrimaryLibrary_Edit.GetTypeCode()));
            arrParams[133] = new SqlParameter("@LibraryCard_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Edit, o.LibraryCard_Edit.GetTypeCode()));
            arrParams[134] = new SqlParameter("@SchoolName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Edit, o.SchoolName_Edit.GetTypeCode()));
            arrParams[135] = new SqlParameter("@District_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Edit, o.District_Edit.GetTypeCode()));
            arrParams[136] = new SqlParameter("@Teacher_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Edit, o.Teacher_Edit.GetTypeCode()));
            arrParams[137] = new SqlParameter("@GroupTeamName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Edit, o.GroupTeamName_Edit.GetTypeCode()));
            arrParams[138] = new SqlParameter("@SchoolType_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Edit, o.SchoolType_Edit.GetTypeCode()));
            arrParams[139] = new SqlParameter("@LiteracyLevel1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Edit, o.LiteracyLevel1_Edit.GetTypeCode()));
            arrParams[140] = new SqlParameter("@LiteracyLevel2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Edit, o.LiteracyLevel2_Edit.GetTypeCode()));
            arrParams[141] = new SqlParameter("@ParentPermFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Edit, o.ParentPermFlag_Edit.GetTypeCode()));
            arrParams[142] = new SqlParameter("@Over18Flag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Edit, o.Over18Flag_Edit.GetTypeCode()));
            arrParams[143] = new SqlParameter("@ShareFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Edit, o.ShareFlag_Edit.GetTypeCode()));
            arrParams[144] = new SqlParameter("@TermsOfUseflag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Edit, o.TermsOfUseflag_Edit.GetTypeCode()));
            arrParams[145] = new SqlParameter("@Custom1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Edit, o.Custom1_Edit.GetTypeCode()));
            arrParams[146] = new SqlParameter("@Custom2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Edit, o.Custom2_Edit.GetTypeCode()));
            arrParams[147] = new SqlParameter("@Custom3_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Edit, o.Custom3_Edit.GetTypeCode()));
            arrParams[148] = new SqlParameter("@Custom4_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Edit, o.Custom4_Edit.GetTypeCode()));
            arrParams[149] = new SqlParameter("@Custom5_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Edit, o.Custom5_Edit.GetTypeCode()));
            arrParams[150] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[151] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[152] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[153] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[154] = new SqlParameter("@SDistrict_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Prompt, o.SDistrict_Prompt.GetTypeCode()));
            arrParams[155] = new SqlParameter("@SDistrict_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Req, o.SDistrict_Req.GetTypeCode()));
            arrParams[156] = new SqlParameter("@SDistrict_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Show, o.SDistrict_Show.GetTypeCode()));
            arrParams[157] = new SqlParameter("@SDistrict_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Edit, o.SDistrict_Edit.GetTypeCode()));

            arrParams[158] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[159] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[160] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[161] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[162] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[163] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[164] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[165] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[166] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[167] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[168] = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));
            arrParams[168].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_RegistrationSettings_Insert", arrParams);

            o.RID = int.Parse(arrParams[168].Value.ToString());

            return o.RID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(RegistrationSettings o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[169];

            arrParams[0] = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Literacy1Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy1Label, o.Literacy1Label.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Literacy2Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy2Label, o.Literacy2Label.GetTypeCode()));
            arrParams[3] = new SqlParameter("@DOB_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Prompt, o.DOB_Prompt.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Age_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Prompt, o.Age_Prompt.GetTypeCode()));
            arrParams[5] = new SqlParameter("@SchoolGrade_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Prompt, o.SchoolGrade_Prompt.GetTypeCode()));
            arrParams[6] = new SqlParameter("@FirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Prompt, o.FirstName_Prompt.GetTypeCode()));
            arrParams[7] = new SqlParameter("@MiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Prompt, o.MiddleName_Prompt.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Prompt, o.LastName_Prompt.GetTypeCode()));
            arrParams[9] = new SqlParameter("@Gender_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Prompt, o.Gender_Prompt.GetTypeCode()));
            arrParams[10] = new SqlParameter("@EmailAddress_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Prompt, o.EmailAddress_Prompt.GetTypeCode()));
            arrParams[11] = new SqlParameter("@PhoneNumber_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Prompt, o.PhoneNumber_Prompt.GetTypeCode()));
            arrParams[12] = new SqlParameter("@StreetAddress1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Prompt, o.StreetAddress1_Prompt.GetTypeCode()));
            arrParams[13] = new SqlParameter("@StreetAddress2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Prompt, o.StreetAddress2_Prompt.GetTypeCode()));
            arrParams[14] = new SqlParameter("@City_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Prompt, o.City_Prompt.GetTypeCode()));
            arrParams[15] = new SqlParameter("@State_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Prompt, o.State_Prompt.GetTypeCode()));
            arrParams[16] = new SqlParameter("@ZipCode_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Prompt, o.ZipCode_Prompt.GetTypeCode()));
            arrParams[17] = new SqlParameter("@Country_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Prompt, o.Country_Prompt.GetTypeCode()));
            arrParams[18] = new SqlParameter("@County_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Prompt, o.County_Prompt.GetTypeCode()));
            arrParams[19] = new SqlParameter("@ParentGuardianFirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Prompt, o.ParentGuardianFirstName_Prompt.GetTypeCode()));
            arrParams[20] = new SqlParameter("@ParentGuardianLastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Prompt, o.ParentGuardianLastName_Prompt.GetTypeCode()));
            arrParams[21] = new SqlParameter("@ParentGuardianMiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Prompt, o.ParentGuardianMiddleName_Prompt.GetTypeCode()));
            arrParams[22] = new SqlParameter("@PrimaryLibrary_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Prompt, o.PrimaryLibrary_Prompt.GetTypeCode()));
            arrParams[23] = new SqlParameter("@LibraryCard_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Prompt, o.LibraryCard_Prompt.GetTypeCode()));
            arrParams[24] = new SqlParameter("@SchoolName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Prompt, o.SchoolName_Prompt.GetTypeCode()));
            arrParams[25] = new SqlParameter("@District_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Prompt, o.District_Prompt.GetTypeCode()));
            arrParams[26] = new SqlParameter("@Teacher_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Prompt, o.Teacher_Prompt.GetTypeCode()));
            arrParams[27] = new SqlParameter("@GroupTeamName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Prompt, o.GroupTeamName_Prompt.GetTypeCode()));
            arrParams[28] = new SqlParameter("@SchoolType_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Prompt, o.SchoolType_Prompt.GetTypeCode()));
            arrParams[29] = new SqlParameter("@LiteracyLevel1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Prompt, o.LiteracyLevel1_Prompt.GetTypeCode()));
            arrParams[30] = new SqlParameter("@LiteracyLevel2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Prompt, o.LiteracyLevel2_Prompt.GetTypeCode()));
            arrParams[31] = new SqlParameter("@ParentPermFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Prompt, o.ParentPermFlag_Prompt.GetTypeCode()));
            arrParams[32] = new SqlParameter("@Over18Flag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Prompt, o.Over18Flag_Prompt.GetTypeCode()));
            arrParams[33] = new SqlParameter("@ShareFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Prompt, o.ShareFlag_Prompt.GetTypeCode()));
            arrParams[34] = new SqlParameter("@TermsOfUseflag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Prompt, o.TermsOfUseflag_Prompt.GetTypeCode()));
            arrParams[35] = new SqlParameter("@Custom1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Prompt, o.Custom1_Prompt.GetTypeCode()));
            arrParams[36] = new SqlParameter("@Custom2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Prompt, o.Custom2_Prompt.GetTypeCode()));
            arrParams[37] = new SqlParameter("@Custom3_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Prompt, o.Custom3_Prompt.GetTypeCode()));
            arrParams[38] = new SqlParameter("@Custom4_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Prompt, o.Custom4_Prompt.GetTypeCode()));
            arrParams[39] = new SqlParameter("@Custom5_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Prompt, o.Custom5_Prompt.GetTypeCode()));
            arrParams[40] = new SqlParameter("@DOB_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Req, o.DOB_Req.GetTypeCode()));
            arrParams[41] = new SqlParameter("@Age_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Req, o.Age_Req.GetTypeCode()));
            arrParams[42] = new SqlParameter("@SchoolGrade_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Req, o.SchoolGrade_Req.GetTypeCode()));
            arrParams[43] = new SqlParameter("@FirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Req, o.FirstName_Req.GetTypeCode()));
            arrParams[44] = new SqlParameter("@MiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Req, o.MiddleName_Req.GetTypeCode()));
            arrParams[45] = new SqlParameter("@LastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Req, o.LastName_Req.GetTypeCode()));
            arrParams[46] = new SqlParameter("@Gender_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Req, o.Gender_Req.GetTypeCode()));
            arrParams[47] = new SqlParameter("@EmailAddress_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Req, o.EmailAddress_Req.GetTypeCode()));
            arrParams[48] = new SqlParameter("@PhoneNumber_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Req, o.PhoneNumber_Req.GetTypeCode()));
            arrParams[49] = new SqlParameter("@StreetAddress1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Req, o.StreetAddress1_Req.GetTypeCode()));
            arrParams[50] = new SqlParameter("@StreetAddress2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Req, o.StreetAddress2_Req.GetTypeCode()));
            arrParams[51] = new SqlParameter("@City_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Req, o.City_Req.GetTypeCode()));
            arrParams[52] = new SqlParameter("@State_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Req, o.State_Req.GetTypeCode()));
            arrParams[53] = new SqlParameter("@ZipCode_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Req, o.ZipCode_Req.GetTypeCode()));
            arrParams[54] = new SqlParameter("@Country_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Req, o.Country_Req.GetTypeCode()));
            arrParams[55] = new SqlParameter("@County_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Req, o.County_Req.GetTypeCode()));
            arrParams[56] = new SqlParameter("@ParentGuardianFirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Req, o.ParentGuardianFirstName_Req.GetTypeCode()));
            arrParams[57] = new SqlParameter("@ParentGuardianLastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Req, o.ParentGuardianLastName_Req.GetTypeCode()));
            arrParams[58] = new SqlParameter("@ParentGuardianMiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Req, o.ParentGuardianMiddleName_Req.GetTypeCode()));
            arrParams[59] = new SqlParameter("@PrimaryLibrary_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Req, o.PrimaryLibrary_Req.GetTypeCode()));
            arrParams[60] = new SqlParameter("@LibraryCard_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Req, o.LibraryCard_Req.GetTypeCode()));
            arrParams[61] = new SqlParameter("@SchoolName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Req, o.SchoolName_Req.GetTypeCode()));
            arrParams[62] = new SqlParameter("@District_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Req, o.District_Req.GetTypeCode()));
            arrParams[63] = new SqlParameter("@Teacher_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Req, o.Teacher_Req.GetTypeCode()));
            arrParams[64] = new SqlParameter("@GroupTeamName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Req, o.GroupTeamName_Req.GetTypeCode()));
            arrParams[65] = new SqlParameter("@SchoolType_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Req, o.SchoolType_Req.GetTypeCode()));
            arrParams[66] = new SqlParameter("@LiteracyLevel1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Req, o.LiteracyLevel1_Req.GetTypeCode()));
            arrParams[67] = new SqlParameter("@LiteracyLevel2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Req, o.LiteracyLevel2_Req.GetTypeCode()));
            arrParams[68] = new SqlParameter("@ParentPermFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Req, o.ParentPermFlag_Req.GetTypeCode()));
            arrParams[69] = new SqlParameter("@Over18Flag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Req, o.Over18Flag_Req.GetTypeCode()));
            arrParams[70] = new SqlParameter("@ShareFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Req, o.ShareFlag_Req.GetTypeCode()));
            arrParams[71] = new SqlParameter("@TermsOfUseflag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Req, o.TermsOfUseflag_Req.GetTypeCode()));
            arrParams[72] = new SqlParameter("@Custom1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Req, o.Custom1_Req.GetTypeCode()));
            arrParams[73] = new SqlParameter("@Custom2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Req, o.Custom2_Req.GetTypeCode()));
            arrParams[74] = new SqlParameter("@Custom3_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Req, o.Custom3_Req.GetTypeCode()));
            arrParams[75] = new SqlParameter("@Custom4_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Req, o.Custom4_Req.GetTypeCode()));
            arrParams[76] = new SqlParameter("@Custom5_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Req, o.Custom5_Req.GetTypeCode()));
            arrParams[77] = new SqlParameter("@DOB_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Show, o.DOB_Show.GetTypeCode()));
            arrParams[78] = new SqlParameter("@Age_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Show, o.Age_Show.GetTypeCode()));
            arrParams[79] = new SqlParameter("@SchoolGrade_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Show, o.SchoolGrade_Show.GetTypeCode()));
            arrParams[80] = new SqlParameter("@FirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Show, o.FirstName_Show.GetTypeCode()));
            arrParams[81] = new SqlParameter("@MiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Show, o.MiddleName_Show.GetTypeCode()));
            arrParams[82] = new SqlParameter("@LastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Show, o.LastName_Show.GetTypeCode()));
            arrParams[83] = new SqlParameter("@Gender_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Show, o.Gender_Show.GetTypeCode()));
            arrParams[84] = new SqlParameter("@EmailAddress_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Show, o.EmailAddress_Show.GetTypeCode()));
            arrParams[85] = new SqlParameter("@PhoneNumber_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Show, o.PhoneNumber_Show.GetTypeCode()));
            arrParams[86] = new SqlParameter("@StreetAddress1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Show, o.StreetAddress1_Show.GetTypeCode()));
            arrParams[87] = new SqlParameter("@StreetAddress2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Show, o.StreetAddress2_Show.GetTypeCode()));
            arrParams[88] = new SqlParameter("@City_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Show, o.City_Show.GetTypeCode()));
            arrParams[89] = new SqlParameter("@State_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Show, o.State_Show.GetTypeCode()));
            arrParams[90] = new SqlParameter("@ZipCode_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Show, o.ZipCode_Show.GetTypeCode()));
            arrParams[91] = new SqlParameter("@Country_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Show, o.Country_Show.GetTypeCode()));
            arrParams[92] = new SqlParameter("@County_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Show, o.County_Show.GetTypeCode()));
            arrParams[93] = new SqlParameter("@ParentGuardianFirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Show, o.ParentGuardianFirstName_Show.GetTypeCode()));
            arrParams[94] = new SqlParameter("@ParentGuardianLastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Show, o.ParentGuardianLastName_Show.GetTypeCode()));
            arrParams[95] = new SqlParameter("@ParentGuardianMiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Show, o.ParentGuardianMiddleName_Show.GetTypeCode()));
            arrParams[96] = new SqlParameter("@PrimaryLibrary_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Show, o.PrimaryLibrary_Show.GetTypeCode()));
            arrParams[97] = new SqlParameter("@LibraryCard_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Show, o.LibraryCard_Show.GetTypeCode()));
            arrParams[98] = new SqlParameter("@SchoolName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Show, o.SchoolName_Show.GetTypeCode()));
            arrParams[99] = new SqlParameter("@District_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Show, o.District_Show.GetTypeCode()));
            arrParams[100] = new SqlParameter("@Teacher_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Show, o.Teacher_Show.GetTypeCode()));
            arrParams[101] = new SqlParameter("@GroupTeamName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Show, o.GroupTeamName_Show.GetTypeCode()));
            arrParams[102] = new SqlParameter("@SchoolType_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Show, o.SchoolType_Show.GetTypeCode()));
            arrParams[103] = new SqlParameter("@LiteracyLevel1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Show, o.LiteracyLevel1_Show.GetTypeCode()));
            arrParams[104] = new SqlParameter("@LiteracyLevel2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Show, o.LiteracyLevel2_Show.GetTypeCode()));
            arrParams[105] = new SqlParameter("@ParentPermFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Show, o.ParentPermFlag_Show.GetTypeCode()));
            arrParams[106] = new SqlParameter("@Over18Flag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Show, o.Over18Flag_Show.GetTypeCode()));
            arrParams[107] = new SqlParameter("@ShareFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Show, o.ShareFlag_Show.GetTypeCode()));
            arrParams[108] = new SqlParameter("@TermsOfUseflag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Show, o.TermsOfUseflag_Show.GetTypeCode()));
            arrParams[109] = new SqlParameter("@Custom1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Show, o.Custom1_Show.GetTypeCode()));
            arrParams[110] = new SqlParameter("@Custom2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Show, o.Custom2_Show.GetTypeCode()));
            arrParams[111] = new SqlParameter("@Custom3_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Show, o.Custom3_Show.GetTypeCode()));
            arrParams[112] = new SqlParameter("@Custom4_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Show, o.Custom4_Show.GetTypeCode()));
            arrParams[113] = new SqlParameter("@Custom5_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Show, o.Custom5_Show.GetTypeCode()));
            arrParams[114] = new SqlParameter("@DOB_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Edit, o.DOB_Edit.GetTypeCode()));
            arrParams[115] = new SqlParameter("@Age_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Edit, o.Age_Edit.GetTypeCode()));
            arrParams[116] = new SqlParameter("@SchoolGrade_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Edit, o.SchoolGrade_Edit.GetTypeCode()));
            arrParams[117] = new SqlParameter("@FirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Edit, o.FirstName_Edit.GetTypeCode()));
            arrParams[118] = new SqlParameter("@MiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Edit, o.MiddleName_Edit.GetTypeCode()));
            arrParams[119] = new SqlParameter("@LastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Edit, o.LastName_Edit.GetTypeCode()));
            arrParams[120] = new SqlParameter("@Gender_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Edit, o.Gender_Edit.GetTypeCode()));
            arrParams[121] = new SqlParameter("@EmailAddress_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Edit, o.EmailAddress_Edit.GetTypeCode()));
            arrParams[122] = new SqlParameter("@PhoneNumber_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Edit, o.PhoneNumber_Edit.GetTypeCode()));
            arrParams[123] = new SqlParameter("@StreetAddress1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Edit, o.StreetAddress1_Edit.GetTypeCode()));
            arrParams[124] = new SqlParameter("@StreetAddress2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Edit, o.StreetAddress2_Edit.GetTypeCode()));
            arrParams[125] = new SqlParameter("@City_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Edit, o.City_Edit.GetTypeCode()));
            arrParams[126] = new SqlParameter("@State_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Edit, o.State_Edit.GetTypeCode()));
            arrParams[127] = new SqlParameter("@ZipCode_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Edit, o.ZipCode_Edit.GetTypeCode()));
            arrParams[128] = new SqlParameter("@Country_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Edit, o.Country_Edit.GetTypeCode()));
            arrParams[129] = new SqlParameter("@County_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Edit, o.County_Edit.GetTypeCode()));
            arrParams[130] = new SqlParameter("@ParentGuardianFirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Edit, o.ParentGuardianFirstName_Edit.GetTypeCode()));
            arrParams[131] = new SqlParameter("@ParentGuardianLastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Edit, o.ParentGuardianLastName_Edit.GetTypeCode()));
            arrParams[132] = new SqlParameter("@ParentGuardianMiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Edit, o.ParentGuardianMiddleName_Edit.GetTypeCode()));
            arrParams[133] = new SqlParameter("@PrimaryLibrary_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Edit, o.PrimaryLibrary_Edit.GetTypeCode()));
            arrParams[134] = new SqlParameter("@LibraryCard_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Edit, o.LibraryCard_Edit.GetTypeCode()));
            arrParams[135] = new SqlParameter("@SchoolName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Edit, o.SchoolName_Edit.GetTypeCode()));
            arrParams[136] = new SqlParameter("@District_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Edit, o.District_Edit.GetTypeCode()));
            arrParams[137] = new SqlParameter("@Teacher_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Edit, o.Teacher_Edit.GetTypeCode()));
            arrParams[138] = new SqlParameter("@GroupTeamName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Edit, o.GroupTeamName_Edit.GetTypeCode()));
            arrParams[139] = new SqlParameter("@SchoolType_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Edit, o.SchoolType_Edit.GetTypeCode()));
            arrParams[140] = new SqlParameter("@LiteracyLevel1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Edit, o.LiteracyLevel1_Edit.GetTypeCode()));
            arrParams[141] = new SqlParameter("@LiteracyLevel2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Edit, o.LiteracyLevel2_Edit.GetTypeCode()));
            arrParams[142] = new SqlParameter("@ParentPermFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Edit, o.ParentPermFlag_Edit.GetTypeCode()));
            arrParams[143] = new SqlParameter("@Over18Flag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Edit, o.Over18Flag_Edit.GetTypeCode()));
            arrParams[144] = new SqlParameter("@ShareFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Edit, o.ShareFlag_Edit.GetTypeCode()));
            arrParams[145] = new SqlParameter("@TermsOfUseflag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Edit, o.TermsOfUseflag_Edit.GetTypeCode()));
            arrParams[146] = new SqlParameter("@Custom1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Edit, o.Custom1_Edit.GetTypeCode()));
            arrParams[147] = new SqlParameter("@Custom2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Edit, o.Custom2_Edit.GetTypeCode()));
            arrParams[148] = new SqlParameter("@Custom3_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Edit, o.Custom3_Edit.GetTypeCode()));
            arrParams[149] = new SqlParameter("@Custom4_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Edit, o.Custom4_Edit.GetTypeCode()));
            arrParams[150] = new SqlParameter("@Custom5_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Edit, o.Custom5_Edit.GetTypeCode()));
            arrParams[151] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[152] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[153] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[154] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[155] = new SqlParameter("@SDistrict_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Prompt, o.SDistrict_Prompt.GetTypeCode()));
            arrParams[156] = new SqlParameter("@SDistrict_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Req, o.SDistrict_Req.GetTypeCode()));
            arrParams[157] = new SqlParameter("@SDistrict_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Show, o.SDistrict_Show.GetTypeCode()));
            arrParams[158] = new SqlParameter("@SDistrict_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Edit, o.SDistrict_Edit.GetTypeCode()));


            arrParams[159] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[160] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[161] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[162] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[163] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[164] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[165] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[166] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[167] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[168] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

	
            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_RegistrationSettings_Update", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public int Delete()
        {

            return Delete(this);

        }

        public static int Delete(RegistrationSettings o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_RegistrationSettings_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

    }//end class

}//end namespace

