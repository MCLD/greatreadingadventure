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
using GRA.SRP.Core.Utilities;
using System.Collections.Generic;


namespace GRA.SRP.DAL
{

[Serializable]    public class RegistrationSettings : EntityBase
    {
        public static new string Version { get { return "2.0"; } }


        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

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

        #region Accessors

        public int RID { get; set; }
        public string Literacy1Label { get; set; }
        public string Literacy2Label { get; set; }
        public bool DOB_Prompt { get; set; }
        public bool Age_Prompt { get; set; }
        public bool SchoolGrade_Prompt { get; set; }
        public bool FirstName_Prompt { get; set; }
        public bool MiddleName_Prompt { get; set; }
        public bool LastName_Prompt { get; set; }
        public bool Gender_Prompt { get; set; }
        public bool EmailAddress_Prompt { get; set; }
        public bool PhoneNumber_Prompt { get; set; }
        public bool StreetAddress1_Prompt { get; set; }
        public bool StreetAddress2_Prompt { get; set; }
        public bool City_Prompt { get; set; }
        public bool State_Prompt { get; set; }
        public bool ZipCode_Prompt { get; set; }
        public bool Country_Prompt { get; set; }
        public bool County_Prompt { get; set; }
        public bool ParentGuardianFirstName_Prompt { get; set; }
        public bool ParentGuardianLastName_Prompt { get; set; }
        public bool ParentGuardianMiddleName_Prompt { get; set; }
        public bool PrimaryLibrary_Prompt { get; set; }
        public bool LibraryCard_Prompt { get; set; }
        public bool Goal_Prompt { get; set; }
        public bool SchoolName_Prompt { get; set; }
        public bool District_Prompt { get; set; }
        public bool SDistrict_Prompt { get; set; }
        public bool Teacher_Prompt { get; set; }
        public bool GroupTeamName_Prompt { get; set; }
        public bool SchoolType_Prompt { get; set; }
        public bool LiteracyLevel1_Prompt { get; set; }
        public bool LiteracyLevel2_Prompt { get; set; }
        public bool ParentPermFlag_Prompt { get; set; }
        public bool Over18Flag_Prompt { get; set; }
        public bool ShareFlag_Prompt { get; set; }
        public bool TermsOfUseflag_Prompt { get; set; }
        public bool Custom1_Prompt { get; set; }
        public bool Custom2_Prompt { get; set; }
        public bool Custom3_Prompt { get; set; }
        public bool Custom4_Prompt { get; set; }
        public bool Custom5_Prompt { get; set; }
        public bool DOB_Req { get; set; }
        public bool Age_Req { get; set; }
        public bool SchoolGrade_Req { get; set; }
        public bool FirstName_Req { get; set; }
        public bool MiddleName_Req { get; set; }
        public bool LastName_Req { get; set; }
        public bool Gender_Req { get; set; }
        public bool EmailAddress_Req { get; set; }
        public bool PhoneNumber_Req { get; set; }
        public bool StreetAddress1_Req { get; set; }
        public bool StreetAddress2_Req { get; set; }
        public bool City_Req { get; set; }
        public bool State_Req { get; set; }
        public bool ZipCode_Req { get; set; }
        public bool Country_Req { get; set; }
        public bool County_Req { get; set; }
        public bool ParentGuardianFirstName_Req { get; set; }
        public bool ParentGuardianLastName_Req { get; set; }
        public bool ParentGuardianMiddleName_Req { get; set; }
        public bool PrimaryLibrary_Req { get; set; }
        public bool LibraryCard_Req { get; set; }
        public bool Goal_Req { get; set; }
        public bool SchoolName_Req { get; set; }
        public bool District_Req { get; set; } 
        public bool SDistrict_Req { get; set; }
        public bool Teacher_Req { get; set; }
        public bool GroupTeamName_Req { get; set; }
        public bool SchoolType_Req { get; set; }
        public bool LiteracyLevel1_Req { get; set; }
        public bool LiteracyLevel2_Req { get; set; }
        public bool ParentPermFlag_Req { get; set; }
        public bool Over18Flag_Req { get; set; }
        public bool ShareFlag_Req { get; set; }
        public bool TermsOfUseflag_Req { get; set; }
        public bool Custom1_Req { get; set; }
        public bool Custom2_Req { get; set; }
        public bool Custom3_Req { get; set; }
        public bool Custom4_Req { get; set; }
        public bool Custom5_Req { get; set; }
        public bool DOB_Show { get; set; }
        public bool Age_Show { get; set; }
        public bool SchoolGrade_Show { get; set; }
        public bool FirstName_Show { get; set; }
        public bool MiddleName_Show { get; set; }
        public bool LastName_Show { get; set; }
        public bool Gender_Show { get; set; }
        public bool EmailAddress_Show { get; set; }
        public bool PhoneNumber_Show { get; set; }
        public bool StreetAddress1_Show { get; set; }
        public bool StreetAddress2_Show { get; set; }
        public bool City_Show { get; set; }
        public bool State_Show { get; set; }
        public bool ZipCode_Show { get; set; }
        public bool Country_Show { get; set; }
        public bool County_Show { get; set; }
        public bool ParentGuardianFirstName_Show { get; set; }
        public bool ParentGuardianLastName_Show { get; set; }
        public bool ParentGuardianMiddleName_Show { get; set; }
        public bool PrimaryLibrary_Show { get; set; }
        public bool LibraryCard_Show { get; set; }
        public bool Goal_Show { get; set; }
        public bool SchoolName_Show { get; set; }
        public bool District_Show { get; set; }
        public bool SDistrict_Show { get; set; }
        public bool Teacher_Show { get; set; }
        public bool GroupTeamName_Show { get; set; }
        public bool SchoolType_Show { get; set; }
        public bool LiteracyLevel1_Show { get; set; }
        public bool LiteracyLevel2_Show { get; set; }
        public bool ParentPermFlag_Show { get; set; }
        public bool Over18Flag_Show { get; set; }
        public bool ShareFlag_Show { get; set; }
        public bool TermsOfUseflag_Show { get; set; }
        public bool Custom1_Show { get; set; }
        public bool Custom2_Show { get; set; }
        public bool Custom3_Show { get; set; }
        public bool Custom4_Show { get; set; }
        public bool Custom5_Show { get; set; }
        public bool DOB_Edit { get; set; }
        public bool Age_Edit { get; set; }
        public bool SchoolGrade_Edit { get; set; }
        public bool FirstName_Edit { get; set; }
        public bool MiddleName_Edit { get; set; }
        public bool LastName_Edit { get; set; }
        public bool Gender_Edit { get; set; }
        public bool EmailAddress_Edit { get; set; }
        public bool PhoneNumber_Edit { get; set; }
        public bool StreetAddress1_Edit { get; set; }
        public bool StreetAddress2_Edit { get; set; }
        public bool City_Edit { get; set; }
        public bool State_Edit { get; set; }
        public bool ZipCode_Edit { get; set; }
        public bool Country_Edit { get; set; }
        public bool County_Edit { get; set; }
        public bool ParentGuardianFirstName_Edit { get; set; }
        public bool ParentGuardianLastName_Edit { get; set; }
        public bool ParentGuardianMiddleName_Edit { get; set; }
        public bool PrimaryLibrary_Edit { get; set; }
        public bool LibraryCard_Edit { get; set; }
        public bool Goal_Edit { get; set; }
        public bool SchoolName_Edit { get; set; }
        public bool District_Edit { get; set; }
        public bool SDistrict_Edit { get; set; }
        public bool Teacher_Edit { get; set; }
        public bool GroupTeamName_Edit { get; set; }
        public bool SchoolType_Edit { get; set; }
        public bool LiteracyLevel1_Edit { get; set; }
        public bool LiteracyLevel2_Edit { get; set; }
        public bool ParentPermFlag_Edit { get; set; }
        public bool Over18Flag_Edit { get; set; }
        public bool ShareFlag_Edit { get; set; }
        public bool TermsOfUseflag_Edit { get; set; }
        public bool Custom1_Edit { get; set; }
        public bool Custom2_Edit { get; set; }
        public bool Custom3_Edit { get; set; }
        public bool Custom4_Edit { get; set; }
        public bool Custom5_Edit { get; set; }
        public DateTime LastModDate { get; set; }
        public string LastModUser { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedUser { get; set; }

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

                result.Goal_Prompt = bool.Parse(dr["Goal_Prompt"].ToString());
                result.Goal_Show = bool.Parse(dr["Goal_Show"].ToString());
                result.Goal_Edit = bool.Parse(dr["Goal_Edit"].ToString());
                result.Goal_Req = bool.Parse(dr["Goal_Req"].ToString());


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

                this.Goal_Prompt = bool.Parse(dr["Goal_Prompt"].ToString());
                this.Goal_Show = bool.Parse(dr["Goal_Show"].ToString());
                this.Goal_Edit = bool.Parse(dr["Goal_Edit"].ToString());
                this.Goal_Req = bool.Parse(dr["Goal_Req"].ToString());

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

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@Literacy1Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy1Label, o.Literacy1Label.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Literacy2Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy2Label, o.Literacy2Label.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Prompt, o.DOB_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Prompt, o.Age_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Prompt, o.SchoolGrade_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Prompt, o.FirstName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Prompt, o.MiddleName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Prompt, o.LastName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Prompt, o.Gender_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Prompt, o.EmailAddress_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Prompt, o.PhoneNumber_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Prompt, o.StreetAddress1_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Prompt, o.StreetAddress2_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Prompt, o.City_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Prompt, o.State_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Prompt, o.ZipCode_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Prompt, o.Country_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Prompt, o.County_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Prompt, o.ParentGuardianFirstName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Prompt, o.ParentGuardianLastName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Prompt, o.ParentGuardianMiddleName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Prompt, o.PrimaryLibrary_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Prompt, o.LibraryCard_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Prompt, o.SchoolName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Prompt, o.District_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Prompt, o.Teacher_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Prompt, o.GroupTeamName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Prompt, o.SchoolType_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Prompt, o.LiteracyLevel1_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Prompt, o.LiteracyLevel2_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Prompt, o.ParentPermFlag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Prompt, o.Over18Flag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Prompt, o.ShareFlag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Prompt, o.TermsOfUseflag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Prompt, o.Custom1_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Prompt, o.Custom2_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Prompt, o.Custom3_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Prompt, o.Custom4_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Prompt, o.Custom5_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Req, o.DOB_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Req, o.Age_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Req, o.SchoolGrade_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Req, o.FirstName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Req, o.MiddleName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Req, o.LastName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Req, o.Gender_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Req, o.EmailAddress_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Req, o.PhoneNumber_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Req, o.StreetAddress1_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Req, o.StreetAddress2_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Req, o.City_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Req, o.State_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Req, o.ZipCode_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Req, o.Country_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Req, o.County_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Req, o.ParentGuardianFirstName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Req, o.ParentGuardianLastName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Req, o.ParentGuardianMiddleName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Req, o.PrimaryLibrary_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Req, o.LibraryCard_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Req, o.SchoolName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Req, o.District_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Req, o.Teacher_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Req, o.GroupTeamName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Req, o.SchoolType_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Req, o.LiteracyLevel1_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Req, o.LiteracyLevel2_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Req, o.ParentPermFlag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Req, o.Over18Flag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Req, o.ShareFlag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Req, o.TermsOfUseflag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Req, o.Custom1_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Req, o.Custom2_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Req, o.Custom3_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Req, o.Custom4_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Req, o.Custom5_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Show, o.DOB_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Show, o.Age_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Show, o.SchoolGrade_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Show, o.FirstName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Show, o.MiddleName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Show, o.LastName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Show, o.Gender_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Show, o.EmailAddress_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Show, o.PhoneNumber_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Show, o.StreetAddress1_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Show, o.StreetAddress2_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Show, o.City_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Show, o.State_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Show, o.ZipCode_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Show, o.Country_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Show, o.County_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Show, o.ParentGuardianFirstName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Show, o.ParentGuardianLastName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Show, o.ParentGuardianMiddleName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Show, o.PrimaryLibrary_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Show, o.LibraryCard_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Show, o.SchoolName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Show, o.District_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Show, o.Teacher_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Show, o.GroupTeamName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Show, o.SchoolType_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Show, o.LiteracyLevel1_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Show, o.LiteracyLevel2_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Show, o.ParentPermFlag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Show, o.Over18Flag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Show, o.ShareFlag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Show, o.TermsOfUseflag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Show, o.Custom1_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Show, o.Custom2_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Show, o.Custom3_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Show, o.Custom4_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Show, o.Custom5_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Edit, o.DOB_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Edit, o.Age_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Edit, o.SchoolGrade_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Edit, o.FirstName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Edit, o.MiddleName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Edit, o.LastName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Edit, o.Gender_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Edit, o.EmailAddress_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Edit, o.PhoneNumber_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Edit, o.StreetAddress1_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Edit, o.StreetAddress2_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Edit, o.City_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Edit, o.State_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Edit, o.ZipCode_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Edit, o.Country_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Edit, o.County_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Edit, o.ParentGuardianFirstName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Edit, o.ParentGuardianLastName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Edit, o.ParentGuardianMiddleName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Edit, o.PrimaryLibrary_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Edit, o.LibraryCard_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Edit, o.SchoolName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Edit, o.District_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Edit, o.Teacher_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Edit, o.GroupTeamName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Edit, o.SchoolType_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Edit, o.LiteracyLevel1_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Edit, o.LiteracyLevel2_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Edit, o.ParentPermFlag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Edit, o.Over18Flag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Edit, o.ShareFlag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Edit, o.TermsOfUseflag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Edit, o.Custom1_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Edit, o.Custom2_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Edit, o.Custom3_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Edit, o.Custom4_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Edit, o.Custom5_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

            arrParams.Add(new SqlParameter("@SDistrict_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Prompt, o.SDistrict_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Req, o.SDistrict_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Show, o.SDistrict_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Edit, o.SDistrict_Edit.GetTypeCode())));

            arrParams.Add(new SqlParameter("@Goal_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Prompt, o.Goal_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Req, o.Goal_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Show, o.Goal_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Edit, o.Goal_Edit.GetTypeCode())));

            arrParams.Add(new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode())));

            var newIdParam = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));
            newIdParam.Direction = ParameterDirection.Output;
            arrParams.Add(newIdParam);

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_RegistrationSettings_Insert", arrParams.ToArray());

            o.RID = int.Parse(newIdParam.Value.ToString());

            return o.RID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(RegistrationSettings o)
        {

            int iReturn = -1; //assume the worst

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Literacy1Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy1Label, o.Literacy1Label.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Literacy2Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Literacy2Label, o.Literacy2Label.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Prompt, o.DOB_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Prompt, o.Age_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Prompt, o.SchoolGrade_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Prompt, o.FirstName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Prompt, o.MiddleName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Prompt, o.LastName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Prompt, o.Gender_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Prompt, o.EmailAddress_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Prompt, o.PhoneNumber_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Prompt, o.StreetAddress1_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Prompt, o.StreetAddress2_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Prompt, o.City_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Prompt, o.State_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Prompt, o.ZipCode_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Prompt, o.Country_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Prompt, o.County_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Prompt, o.ParentGuardianFirstName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Prompt, o.ParentGuardianLastName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Prompt, o.ParentGuardianMiddleName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Prompt, o.PrimaryLibrary_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Prompt, o.LibraryCard_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Prompt, o.SchoolName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Prompt, o.District_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Prompt, o.Teacher_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Prompt, o.GroupTeamName_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Prompt, o.SchoolType_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Prompt, o.LiteracyLevel1_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Prompt, o.LiteracyLevel2_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Prompt, o.ParentPermFlag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Prompt, o.Over18Flag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Prompt, o.ShareFlag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Prompt, o.TermsOfUseflag_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Prompt, o.Custom1_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Prompt, o.Custom2_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Prompt, o.Custom3_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Prompt, o.Custom4_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Prompt, o.Custom5_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Req, o.DOB_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Req, o.Age_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Req, o.SchoolGrade_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Req, o.FirstName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Req, o.MiddleName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Req, o.LastName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Req, o.Gender_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Req, o.EmailAddress_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Req, o.PhoneNumber_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Req, o.StreetAddress1_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Req, o.StreetAddress2_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Req, o.City_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Req, o.State_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Req, o.ZipCode_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Req, o.Country_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Req, o.County_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Req, o.ParentGuardianFirstName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Req, o.ParentGuardianLastName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Req, o.ParentGuardianMiddleName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Req, o.PrimaryLibrary_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Req, o.LibraryCard_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Req, o.SchoolName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Req, o.District_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Req, o.Teacher_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Req, o.GroupTeamName_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Req, o.SchoolType_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Req, o.LiteracyLevel1_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Req, o.LiteracyLevel2_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Req, o.ParentPermFlag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Req, o.Over18Flag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Req, o.ShareFlag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Req, o.TermsOfUseflag_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Req, o.Custom1_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Req, o.Custom2_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Req, o.Custom3_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Req, o.Custom4_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Req, o.Custom5_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Show, o.DOB_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Show, o.Age_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Show, o.SchoolGrade_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Show, o.FirstName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Show, o.MiddleName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Show, o.LastName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Show, o.Gender_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Show, o.EmailAddress_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Show, o.PhoneNumber_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Show, o.StreetAddress1_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Show, o.StreetAddress2_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Show, o.City_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Show, o.State_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Show, o.ZipCode_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Show, o.Country_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Show, o.County_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Show, o.ParentGuardianFirstName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Show, o.ParentGuardianLastName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Show, o.ParentGuardianMiddleName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Show, o.PrimaryLibrary_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Show, o.LibraryCard_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Show, o.SchoolName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Show, o.District_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Show, o.Teacher_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Show, o.GroupTeamName_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Show, o.SchoolType_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Show, o.LiteracyLevel1_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Show, o.LiteracyLevel2_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Show, o.ParentPermFlag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Show, o.Over18Flag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Show, o.ShareFlag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Show, o.TermsOfUseflag_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Show, o.Custom1_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Show, o.Custom2_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Show, o.Custom3_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Show, o.Custom4_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Show, o.Custom5_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DOB_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB_Edit, o.DOB_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age_Edit, o.Age_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolGrade_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade_Edit, o.SchoolGrade_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName_Edit, o.FirstName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName_Edit, o.MiddleName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName_Edit, o.LastName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender_Edit, o.Gender_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress_Edit, o.EmailAddress_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber_Edit, o.PhoneNumber_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1_Edit, o.StreetAddress1_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2_Edit, o.StreetAddress2_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City_Edit, o.City_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State_Edit, o.State_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode_Edit, o.ZipCode_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country_Edit, o.Country_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County_Edit, o.County_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName_Edit, o.ParentGuardianFirstName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName_Edit, o.ParentGuardianLastName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName_Edit, o.ParentGuardianMiddleName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary_Edit, o.PrimaryLibrary_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard_Edit, o.LibraryCard_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName_Edit, o.SchoolName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District_Edit, o.District_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher_Edit, o.Teacher_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName_Edit, o.GroupTeamName_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType_Edit, o.SchoolType_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1_Edit, o.LiteracyLevel1_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2_Edit, o.LiteracyLevel2_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag_Edit, o.ParentPermFlag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag_Edit, o.Over18Flag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag_Edit, o.ShareFlag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag_Edit, o.TermsOfUseflag_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1_Edit, o.Custom1_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2_Edit, o.Custom2_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3_Edit, o.Custom3_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4_Edit, o.Custom4_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5_Edit, o.Custom5_Edit.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Prompt, o.SDistrict_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Req, o.SDistrict_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Show, o.SDistrict_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict_Edit, o.SDistrict_Edit.GetTypeCode())));


            arrParams.Add(new SqlParameter("@Goal_Prompt", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Prompt, o.Goal_Prompt.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal_Req", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Req, o.Goal_Req.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal_Show", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Show, o.Goal_Show.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal_Edit", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Goal_Edit, o.Goal_Edit.GetTypeCode())));



            arrParams.Add(new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode())));

	
            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_RegistrationSettings_Update", arrParams.ToArray());

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

