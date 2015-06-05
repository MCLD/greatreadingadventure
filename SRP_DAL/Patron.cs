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
using STG.SRP.Core.Utilities;
using STG.SRP.Utilities.CoreClasses;

namespace STG.SRP.DAL
{

[Serializable]    public class Patron : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = STG.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPID;
        private bool myIsMasterAccount =  false;
        private int myMasterAcctPID = 0;
        private string myUsername = "";
        private string myPassword = "";
        private DateTime myDOB;
        private int myAge = 0;
        private string mySchoolGrade = "";
        private int myProgID = 0;
        private string myFirstName = "";
        private string myMiddleName = "";
        private string myLastName = "";
        private string myGender = "";
        private string myEmailAddress = "";
        private string myPhoneNumber = "";
        private string myStreetAddress1 = "";
        private string myStreetAddress2 = "";
        private string myCity = "";
        private string myState = "";
        private string myZipCode = "";
        private string myCountry = "";
        private string myCounty = "";
        private string myParentGuardianFirstName = "";
        private string myParentGuardianLastName = "";
        private string myParentGuardianMiddleName = "";
        private int myPrimaryLibrary = 0;
        private string myLibraryCard = "";
        private string mySchoolName = "";
        private string myDistrict = "";
        private string myTeacher = "";
        private string myGroupTeamName = "";
        private int mySchoolType = 0;
        private int myLiteracyLevel1 = 0;
        private int myLiteracyLevel2 = 0;
        private bool myParentPermFlag = true;
        private bool myOver18Flag = false;
        private bool myShareFlag = false;
        private bool myTermsOfUseflag;
        private string myCustom1 = "";
        private string myCustom2 = "";
        private string myCustom3 = "";
        private string myCustom4 = "";
        private string myCustom5 = "";
        private int myAvatarID = 0;
        private int mySDistrict = 0;

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


        private int myScore1 = 0;
        private int myScore2 = 0;
        private decimal myScore1Pct = 0;
        private decimal myScore2Pct = 0;
        private DateTime myScore1Date ;
        private DateTime myScore2Date ;

        #endregion

        #region Accessors

        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public bool IsMasterAccount
        {
            get { return myIsMasterAccount; }
            set { myIsMasterAccount = value; }
        }
        public int MasterAcctPID
        {
            get { return myMasterAcctPID; }
            set { myMasterAcctPID = value; }
        }
        public string Username
        {
            get { return myUsername; }
            set { myUsername = value; }
        }
        public string Password
        {
            get { return myPassword; }
            set { myPassword = value; }
        }
        public DateTime DOB
        {
            get { return myDOB; }
            set { myDOB = value; }
        }
        public int Age
        {
            get { return myAge; }
            set { myAge = value; }
        }
        public string SchoolGrade
        {
            get { return mySchoolGrade; }
            set { mySchoolGrade = value; }
        }
        public int ProgID
        {
            get { return myProgID; }
            set { myProgID = value; }
        }
        public string FirstName
        {
            get { return myFirstName; }
            set { myFirstName = value; }
        }
        public string MiddleName
        {
            get { return myMiddleName; }
            set { myMiddleName = value; }
        }
        public string LastName
        {
            get { return myLastName; }
            set { myLastName = value; }
        }
        public string Gender
        {
            get { return myGender; }
            set { myGender = value; }
        }
        public string EmailAddress
        {
            get { return myEmailAddress; }
            set { myEmailAddress = value; }
        }
        public string PhoneNumber
        {
            get { return myPhoneNumber; }
            set { myPhoneNumber = value; }
        }
        public string StreetAddress1
        {
            get { return myStreetAddress1; }
            set { myStreetAddress1 = value; }
        }
        public string StreetAddress2
        {
            get { return myStreetAddress2; }
            set { myStreetAddress2 = value; }
        }
        public string City
        {
            get { return myCity; }
            set { myCity = value; }
        }
        public string State
        {
            get { return myState; }
            set { myState = value; }
        }
        public string ZipCode
        {
            get { return myZipCode; }
            set { myZipCode = value; }
        }
        public string Country
        {
            get { return myCountry; }
            set { myCountry = value; }
        }
        public string County
        {
            get { return myCounty; }
            set { myCounty = value; }
        }
        public string ParentGuardianFirstName
        {
            get { return myParentGuardianFirstName; }
            set { myParentGuardianFirstName = value; }
        }
        public string ParentGuardianLastName
        {
            get { return myParentGuardianLastName; }
            set { myParentGuardianLastName = value; }
        }
        public string ParentGuardianMiddleName
        {
            get { return myParentGuardianMiddleName; }
            set { myParentGuardianMiddleName = value; }
        }
        public int PrimaryLibrary
        {
            get { return myPrimaryLibrary; }
            set { myPrimaryLibrary = value; }
        }
        public string LibraryCard
        {
            get { return myLibraryCard; }
            set { myLibraryCard = value; }
        }
        public string SchoolName
        {
            get { return mySchoolName; }
            set { mySchoolName = value; }
        }
        public string District
        {
            get { return myDistrict; }
            set { myDistrict = value; }
        }
        public string Teacher
        {
            get { return myTeacher; }
            set { myTeacher = value; }
        }
        public string GroupTeamName
        {
            get { return myGroupTeamName; }
            set { myGroupTeamName = value; }
        }
        public int SchoolType
        {
            get { return mySchoolType; }
            set { mySchoolType = value; }
        }
        public int LiteracyLevel1
        {
            get { return myLiteracyLevel1; }
            set { myLiteracyLevel1 = value; }
        }
        public int LiteracyLevel2
        {
            get { return myLiteracyLevel2; }
            set { myLiteracyLevel2 = value; }
        }
        public bool ParentPermFlag
        {
            get { return myParentPermFlag; }
            set { myParentPermFlag = value; }
        }
        public bool Over18Flag
        {
            get { return myOver18Flag; }
            set { myOver18Flag = value; }
        }
        public bool ShareFlag
        {
            get { return myShareFlag; }
            set { myShareFlag = value; }
        }
        public bool TermsOfUseflag
        {
            get { return myTermsOfUseflag; }
            set { myTermsOfUseflag = value; }
        }
        public string Custom1
        {
            get { return myCustom1; }
            set { myCustom1 = value; }
        }
        public string Custom2
        {
            get { return myCustom2; }
            set { myCustom2 = value; }
        }
        public string Custom3
        {
            get { return myCustom3; }
            set { myCustom3 = value; }
        }
        public string Custom4
        {
            get { return myCustom4; }
            set { myCustom4 = value; }
        }
        public string Custom5
        {
            get { return myCustom5; }
            set { myCustom5 = value; }
        }
        public int AvatarID
        {
            get { return myAvatarID; }
            set { myAvatarID = value; }
        }
        public int SDistrict
        {
            get { return mySDistrict; }
            set { mySDistrict = value; }
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


        public int Score1
        {
            get { return myScore1; }
            set { myScore1 = value; }
        }
        public int Score2
        {
            get { return myScore2; }
            set { myScore2 = value; }
        }
        public decimal Score1Pct
        {
            get { return myScore1Pct; }
            set { myScore1Pct = value; }
        }
        public decimal Score2Pct
        {
            get { return myScore2Pct; }
            set { myScore2Pct = value; }
        }

        public DateTime Score1Date
        {
            get { return myScore1Date; }
            set { myScore1Date = value; }
        }

        public DateTime Score2Date
        {
            get { return myScore2Date; }
            set { myScore2Date = value; }
        }
        #endregion

        #region Constructors

        public Patron()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static string GetTestRank(int PID, int WhichScore)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@WhichScore", WhichScore);

            var ds =  SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetScoreRank", arrParams);
            var ret = "N/A";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ret = string.Format("{0:0.##} Percentile", ds.Tables[0].Rows[0]["Percentile"]);
            }
            return ret;
        }

        public static DataSet CRSearch()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetAll", arrParams);
        }


        /*
         *                 
                <asp:SessionParameter DefaultValue="" Name="searchFirstName" SessionField="PS_First" Type="String" />
                <asp:SessionParameter DefaultValue="" Name="searchLastName" SessionField="PS_Last" Type="String" />
                <asp:SessionParameter DefaultValue="" Name="searchUsername" SessionField="PS_User" Type="String" />
                <asp:SessionParameter DefaultValue="" Name="searchEmail" SessionField="PS_Email" Type="String" />
                <asp:SessionParameter DefaultValue="" Name="searchDOB" SessionField="PS_DOB" Type="String" />
                <asp:SessionParameter DefaultValue="" Name="searchProgram" SessionField="PS_Prog" Type="Int32" />
                <asp:SessionParameter DefaultValue="" Name="searchGender" SessionField="PS_Gender" Type="String" />
         * 
         */
        
        public static DataSet GetPaged(string sort, int startRowIndex, int maximumRows
            , string searchFirstName, string searchLastName, string searchUsername, string searchEmail, string searchDOB, int searchProgram, string searchGender)
        {
            SqlParameter[] arrParams = new SqlParameter[11];
            arrParams[0] = new SqlParameter("@startRowIndex", startRowIndex);
            arrParams[1] = new SqlParameter("@maximumRows", maximumRows);
            arrParams[2] = new SqlParameter("@sortString", sort);
            arrParams[3] = new SqlParameter("@searchFirstName", searchFirstName);
            arrParams[4] = new SqlParameter("@searchLastName", searchLastName);
            arrParams[5] = new SqlParameter("@searchUsername", searchUsername);
            arrParams[6] = new SqlParameter("@searchEmail", searchEmail);
            arrParams[7] = new SqlParameter("@searchDOB", GlobalUtilities.DBSafeValue(FormatHelper.SafeToDateTime(searchDOB), FormatHelper.SafeToDateTime(searchDOB).GetTypeCode()));
            arrParams[8] = new SqlParameter("@searchProgram", searchProgram);
            arrParams[9] = new SqlParameter("@searchGender", searchGender);
            arrParams[10] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetPatronsPaged", arrParams);
        }

        public static int GetTotalPagedCount(string sort, int startRowIndex, int maximumRows
           , string searchFirstName, string searchLastName, string searchUsername, string searchEmail, string searchDOB, int searchProgram, string searchGender)
        {
            SqlParameter[] arrParams = new SqlParameter[11];
            arrParams[0] = new SqlParameter("@startRowIndex", startRowIndex);
            arrParams[1] = new SqlParameter("@maximumRows", maximumRows);
            arrParams[2] = new SqlParameter("@sortString", sort);
            arrParams[3] = new SqlParameter("@searchFirstName", searchFirstName);
            arrParams[4] = new SqlParameter("@searchLastName", searchLastName);
            arrParams[5] = new SqlParameter("@searchUsername", searchUsername);
            arrParams[6] = new SqlParameter("@searchEmail", searchEmail);
            arrParams[7] = new SqlParameter("@searchDOB", GlobalUtilities.DBSafeValue(FormatHelper.SafeToDateTime(searchDOB), FormatHelper.SafeToDateTime(searchDOB).GetTypeCode()));
            arrParams[8] = new SqlParameter("@searchProgram", searchProgram);
            arrParams[9] = new SqlParameter("@searchGender", searchGender);
            arrParams[10] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return (int) SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetTotalPatrons", arrParams).Tables[0].Rows[0][0];
        }
        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetAll", arrParams);
        }

        public static Patron FetchObject(int PID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Patron_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Patron result = new Patron();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                result.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int)) result.MasterAcctPID = _int;
                result.Username = dr["Username"].ToString();
                result.Password = dr["Password"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime)) result.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int)) result.Age = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) result.ProgID = _int;
                result.FirstName = dr["FirstName"].ToString();
                result.MiddleName = dr["MiddleName"].ToString();
                result.LastName = dr["LastName"].ToString();
                result.Gender = dr["Gender"].ToString();
                result.EmailAddress = dr["EmailAddress"].ToString();
                result.PhoneNumber = dr["PhoneNumber"].ToString();
                result.StreetAddress1 = dr["StreetAddress1"].ToString();
                result.StreetAddress2 = dr["StreetAddress2"].ToString();
                result.City = dr["City"].ToString();
                result.State = dr["State"].ToString();
                result.ZipCode = dr["ZipCode"].ToString();
                result.Country = dr["Country"].ToString();
                result.County = dr["County"].ToString();
                result.ParentGuardianFirstName = dr["ParentGuardianFirstName"].ToString();
                result.ParentGuardianLastName = dr["ParentGuardianLastName"].ToString();
                result.ParentGuardianMiddleName = dr["ParentGuardianMiddleName"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) result.PrimaryLibrary = _int;
                result.LibraryCard = dr["LibraryCard"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int)) result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) result.LiteracyLevel2 = _int;
                result.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                result.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                result.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                result.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["AvatarID"].ToString(), out _int)) result.AvatarID = _int;
                if (int.TryParse(dr["SDistrict"].ToString(), out _int)) result.SDistrict = _int;

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

                if (int.TryParse(dr["Score1"].ToString(), out _int)) result.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int)) result.Score2 = _int;
                var _decimal = (decimal) 0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal)) result.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal)) result.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime)) result.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime)) result.Score2Date = _datetime;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Patron_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Patron result = new Patron();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                this.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int)) this.MasterAcctPID = _int;
                this.Username = dr["Username"].ToString();
                this.Password = dr["Password"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime)) this.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int)) this.Age = _int;
                this.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) this.ProgID = _int;
                this.FirstName = dr["FirstName"].ToString();
                this.MiddleName = dr["MiddleName"].ToString();
                this.LastName = dr["LastName"].ToString();
                this.Gender = dr["Gender"].ToString();
                this.EmailAddress = dr["EmailAddress"].ToString();
                this.PhoneNumber = dr["PhoneNumber"].ToString();
                this.StreetAddress1 = dr["StreetAddress1"].ToString();
                this.StreetAddress2 = dr["StreetAddress2"].ToString();
                this.City = dr["City"].ToString();
                this.State = dr["State"].ToString();
                this.ZipCode = dr["ZipCode"].ToString();
                this.Country = dr["Country"].ToString();
                this.County = dr["County"].ToString();
                this.ParentGuardianFirstName = dr["ParentGuardianFirstName"].ToString();
                this.ParentGuardianLastName = dr["ParentGuardianLastName"].ToString();
                this.ParentGuardianMiddleName = dr["ParentGuardianMiddleName"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) this.PrimaryLibrary = _int;
                this.LibraryCard = dr["LibraryCard"].ToString();
                this.SchoolName = dr["SchoolName"].ToString();
                this.District = dr["District"].ToString();
                this.Teacher = dr["Teacher"].ToString();
                this.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int)) this.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) this.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) this.LiteracyLevel2 = _int;
                this.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                this.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                this.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                this.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                this.Custom1 = dr["Custom1"].ToString();
                this.Custom2 = dr["Custom2"].ToString();
                this.Custom3 = dr["Custom3"].ToString();
                this.Custom4 = dr["Custom4"].ToString();
                this.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["AvatarID"].ToString(), out _int)) this.AvatarID = _int;
                if (int.TryParse(dr["SDistrict"].ToString(), out _int)) this.SDistrict = _int;

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

                if (int.TryParse(dr["Score1"].ToString(), out _int)) this.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int)) this.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal)) this.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal)) this.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime)) this.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime)) this.Score2Date = _datetime;

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

        public static int Insert(Patron o)
        {

            SqlParameter[] arrParams = new SqlParameter[61];

            arrParams[0] = new SqlParameter("@IsMasterAccount", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsMasterAccount, o.IsMasterAccount.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MasterAcctPID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MasterAcctPID, o.MasterAcctPID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Username", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Username, o.Username.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Password", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Password, o.Password.GetTypeCode()));
            arrParams[4] = new SqlParameter("@DOB", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB, o.DOB.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Age", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age, o.Age.GetTypeCode()));
            arrParams[6] = new SqlParameter("@SchoolGrade", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade, o.SchoolGrade.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ProgID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FirstName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName, o.FirstName.GetTypeCode()));
            arrParams[9] = new SqlParameter("@MiddleName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName, o.MiddleName.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName, o.LastName.GetTypeCode()));
            arrParams[11] = new SqlParameter("@Gender", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[12] = new SqlParameter("@EmailAddress", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress, o.EmailAddress.GetTypeCode()));
            arrParams[13] = new SqlParameter("@PhoneNumber", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber, o.PhoneNumber.GetTypeCode()));
            arrParams[14] = new SqlParameter("@StreetAddress1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1, o.StreetAddress1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@StreetAddress2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2, o.StreetAddress2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@City", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode()));
            arrParams[17] = new SqlParameter("@State", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State, o.State.GetTypeCode()));
            arrParams[18] = new SqlParameter("@ZipCode", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode, o.ZipCode.GetTypeCode()));
            arrParams[19] = new SqlParameter("@Country", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country, o.Country.GetTypeCode()));
            arrParams[20] = new SqlParameter("@County", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County, o.County.GetTypeCode()));
            arrParams[21] = new SqlParameter("@ParentGuardianFirstName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName, o.ParentGuardianFirstName.GetTypeCode()));
            arrParams[22] = new SqlParameter("@ParentGuardianLastName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName, o.ParentGuardianLastName.GetTypeCode()));
            arrParams[23] = new SqlParameter("@ParentGuardianMiddleName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName, o.ParentGuardianMiddleName.GetTypeCode()));
            arrParams[24] = new SqlParameter("@PrimaryLibrary", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary, o.PrimaryLibrary.GetTypeCode()));
            arrParams[25] = new SqlParameter("@LibraryCard", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard, o.LibraryCard.GetTypeCode()));
            arrParams[26] = new SqlParameter("@SchoolName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[27] = new SqlParameter("@District", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District, o.District.GetTypeCode()));
            arrParams[28] = new SqlParameter("@Teacher", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher, o.Teacher.GetTypeCode()));
            arrParams[29] = new SqlParameter("@GroupTeamName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName, o.GroupTeamName.GetTypeCode()));
            arrParams[30] = new SqlParameter("@SchoolType", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType, o.SchoolType.GetTypeCode()));
            arrParams[31] = new SqlParameter("@LiteracyLevel1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode()));
            arrParams[32] = new SqlParameter("@LiteracyLevel2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode()));
            arrParams[33] = new SqlParameter("@ParentPermFlag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag, o.ParentPermFlag.GetTypeCode()));
            arrParams[34] = new SqlParameter("@Over18Flag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag, o.Over18Flag.GetTypeCode()));
            arrParams[35] = new SqlParameter("@ShareFlag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag, o.ShareFlag.GetTypeCode()));
            arrParams[36] = new SqlParameter("@TermsOfUseflag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag, o.TermsOfUseflag.GetTypeCode()));
            arrParams[37] = new SqlParameter("@Custom1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode()));
            arrParams[38] = new SqlParameter("@Custom2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode()));
            arrParams[39] = new SqlParameter("@Custom3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode()));
            arrParams[40] = new SqlParameter("@Custom4", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4, o.Custom4.GetTypeCode()));
            arrParams[41] = new SqlParameter("@Custom5", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5, o.Custom5.GetTypeCode()));
            arrParams[42] = new SqlParameter("@AvatarID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AvatarID, o.AvatarID.GetTypeCode()));
            arrParams[43] = new SqlParameter("@SDistrict", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict, o.SDistrict.GetTypeCode()));

            arrParams[44] = new SqlParameter("@TenID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[45] = new SqlParameter("@FldInt1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[46] = new SqlParameter("@FldInt2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[47] = new SqlParameter("@FldInt3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[48] = new SqlParameter("@FldBit1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[49] = new SqlParameter("@FldBit2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[50] = new SqlParameter("@FldBit3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[51] = new SqlParameter("@FldText1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[52] = new SqlParameter("@FldText2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[53] = new SqlParameter("@FldText3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[54] = new SqlParameter("@Score1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1, o.Score1.GetTypeCode()));
            arrParams[55] = new SqlParameter("@Score2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2, o.Score2.GetTypeCode()));
            arrParams[56] = new SqlParameter("@Score1Pct", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1Pct, o.Score1Pct.GetTypeCode()));
            arrParams[57] = new SqlParameter("@Score2Pct", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2Pct, o.Score2Pct.GetTypeCode()));
            arrParams[58] = new SqlParameter("@Score1Date", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1Date, o.Score1Date.GetTypeCode()));
            arrParams[59] = new SqlParameter("@Score2Date", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2Date, o.Score2Date.GetTypeCode()));

            arrParams[60] = new SqlParameter("@PID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[60].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Patron_Insert", arrParams);

            o.PID = int.Parse(arrParams[60].Value.ToString());

            return o.PID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Patron o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[61];

            arrParams[0] = new SqlParameter("@PID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@IsMasterAccount", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsMasterAccount, o.IsMasterAccount.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MasterAcctPID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MasterAcctPID, o.MasterAcctPID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Username", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Username, o.Username.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Password", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Password, o.Password.GetTypeCode()));
            arrParams[5] = new SqlParameter("@DOB", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOB, o.DOB.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Age", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Age, o.Age.GetTypeCode()));
            arrParams[7] = new SqlParameter("@SchoolGrade", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade, o.SchoolGrade.GetTypeCode()));
            arrParams[8] = new SqlParameter("@ProgID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FirstName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName, o.FirstName.GetTypeCode()));
            arrParams[10] = new SqlParameter("@MiddleName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiddleName, o.MiddleName.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LastName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName, o.LastName.GetTypeCode()));
            arrParams[12] = new SqlParameter("@Gender", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[13] = new SqlParameter("@EmailAddress", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress, o.EmailAddress.GetTypeCode()));
            arrParams[14] = new SqlParameter("@PhoneNumber", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber, o.PhoneNumber.GetTypeCode()));
            arrParams[15] = new SqlParameter("@StreetAddress1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress1, o.StreetAddress1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@StreetAddress2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StreetAddress2, o.StreetAddress2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@City", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode()));
            arrParams[18] = new SqlParameter("@State", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State, o.State.GetTypeCode()));
            arrParams[19] = new SqlParameter("@ZipCode", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode, o.ZipCode.GetTypeCode()));
            arrParams[20] = new SqlParameter("@Country", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Country, o.Country.GetTypeCode()));
            arrParams[21] = new SqlParameter("@County", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County, o.County.GetTypeCode()));
            arrParams[22] = new SqlParameter("@ParentGuardianFirstName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianFirstName, o.ParentGuardianFirstName.GetTypeCode()));
            arrParams[23] = new SqlParameter("@ParentGuardianLastName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianLastName, o.ParentGuardianLastName.GetTypeCode()));
            arrParams[24] = new SqlParameter("@ParentGuardianMiddleName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentGuardianMiddleName, o.ParentGuardianMiddleName.GetTypeCode()));
            arrParams[25] = new SqlParameter("@PrimaryLibrary", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary, o.PrimaryLibrary.GetTypeCode()));
            arrParams[26] = new SqlParameter("@LibraryCard", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryCard, o.LibraryCard.GetTypeCode()));
            arrParams[27] = new SqlParameter("@SchoolName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[28] = new SqlParameter("@District", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District, o.District.GetTypeCode()));
            arrParams[29] = new SqlParameter("@Teacher", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher, o.Teacher.GetTypeCode()));
            arrParams[30] = new SqlParameter("@GroupTeamName", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName, o.GroupTeamName.GetTypeCode()));
            arrParams[31] = new SqlParameter("@SchoolType", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType, o.SchoolType.GetTypeCode()));
            arrParams[32] = new SqlParameter("@LiteracyLevel1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode()));
            arrParams[33] = new SqlParameter("@LiteracyLevel2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode()));
            arrParams[34] = new SqlParameter("@ParentPermFlag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentPermFlag, o.ParentPermFlag.GetTypeCode()));
            arrParams[35] = new SqlParameter("@Over18Flag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Over18Flag, o.Over18Flag.GetTypeCode()));
            arrParams[36] = new SqlParameter("@ShareFlag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShareFlag, o.ShareFlag.GetTypeCode()));
            arrParams[37] = new SqlParameter("@TermsOfUseflag", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TermsOfUseflag, o.TermsOfUseflag.GetTypeCode()));
            arrParams[38] = new SqlParameter("@Custom1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode()));
            arrParams[39] = new SqlParameter("@Custom2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode()));
            arrParams[40] = new SqlParameter("@Custom3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode()));
            arrParams[41] = new SqlParameter("@Custom4", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4, o.Custom4.GetTypeCode()));
            arrParams[42] = new SqlParameter("@Custom5", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5, o.Custom5.GetTypeCode()));
            arrParams[43] = new SqlParameter("@AvatarID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AvatarID, o.AvatarID.GetTypeCode()));
            arrParams[44] = new SqlParameter("@SDistrict", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict, o.SDistrict.GetTypeCode()));

            arrParams[45] = new SqlParameter("@TenID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[46] = new SqlParameter("@FldInt1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[47] = new SqlParameter("@FldInt2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[48] = new SqlParameter("@FldInt3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[49] = new SqlParameter("@FldBit1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[50] = new SqlParameter("@FldBit2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[51] = new SqlParameter("@FldBit3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[52] = new SqlParameter("@FldText1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[53] = new SqlParameter("@FldText2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[54] = new SqlParameter("@FldText3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));


            arrParams[55] = new SqlParameter("@Score1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1, o.Score1.GetTypeCode()));
            arrParams[56] = new SqlParameter("@Score2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2, o.Score2.GetTypeCode()));
            arrParams[57] = new SqlParameter("@Score1Pct", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1Pct, o.Score1Pct.GetTypeCode()));
            arrParams[58] = new SqlParameter("@Score2Pct", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2Pct, o.Score2Pct.GetTypeCode()));
            arrParams[59] = new SqlParameter("@Score1Date", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1Date, o.Score1Date.GetTypeCode()));
            arrParams[60] = new SqlParameter("@Score2Date", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2Date, o.Score2Date.GetTypeCode()));
            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Patron_Update", arrParams);

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

        public static int Delete(Patron o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Patron_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                Patron obj = GetObjectByUsername(Username);
                if (obj != null)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Username", "Username", "The Username you have chosen is already in use.  Please select a different Username.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }
            return (ErrorCodes.Count == 0);
            //return true;
        }


        public static bool Login(string logon, string password)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@UserName", logon);
            arrParams[1] = new SqlParameter("@Password", password);
            arrParams[2] = new SqlParameter("@SessionID", HttpContext.Current.Session.SessionID);


            var result = (int)SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "app_Patron_Login", arrParams);
            return (result == 1);
        }

        public static Patron GetObjectByUsername(string logon)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Username", logon);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Patron_GetByUsername", arrParams);

            if (dr.Read())
            {

                // declare return value

                Patron result = new Patron();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                result.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int)) result.MasterAcctPID = _int;
                result.Username = dr["Username"].ToString();
                result.Password = dr["Password"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime)) result.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int)) result.Age = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) result.ProgID = _int;
                result.FirstName = dr["FirstName"].ToString();
                result.MiddleName = dr["MiddleName"].ToString();
                result.LastName = dr["LastName"].ToString();
                result.Gender = dr["Gender"].ToString();
                result.EmailAddress = dr["EmailAddress"].ToString();
                result.PhoneNumber = dr["PhoneNumber"].ToString();
                result.StreetAddress1 = dr["StreetAddress1"].ToString();
                result.StreetAddress2 = dr["StreetAddress2"].ToString();
                result.City = dr["City"].ToString();
                result.State = dr["State"].ToString();
                result.ZipCode = dr["ZipCode"].ToString();
                result.Country = dr["Country"].ToString();
                result.County = dr["County"].ToString();
                result.ParentGuardianFirstName = dr["ParentGuardianFirstName"].ToString();
                result.ParentGuardianLastName = dr["ParentGuardianLastName"].ToString();
                result.ParentGuardianMiddleName = dr["ParentGuardianMiddleName"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) result.PrimaryLibrary = _int;
                result.LibraryCard = dr["LibraryCard"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int)) result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) result.LiteracyLevel2 = _int;
                result.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                result.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                result.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                result.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["AvatarID"].ToString(), out _int)) result.AvatarID = _int;
                if (int.TryParse(dr["SDistrict"].ToString(), out _int)) result.SDistrict = _int;

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

                if (int.TryParse(dr["Score1"].ToString(), out _int)) result.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int)) result.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal)) result.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal)) result.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime)) result.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime)) result.Score2Date = _datetime;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public static Patron GetObjectByEmail(string email)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Email", email);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Patron_GetByEmail", arrParams);

            if (dr.Read())
            {

                // declare return value

                Patron result = new Patron();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                result.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int)) result.MasterAcctPID = _int;
                result.Username = dr["Username"].ToString();
                result.Password = dr["Password"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime)) result.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int)) result.Age = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) result.ProgID = _int;
                result.FirstName = dr["FirstName"].ToString();
                result.MiddleName = dr["MiddleName"].ToString();
                result.LastName = dr["LastName"].ToString();
                result.Gender = dr["Gender"].ToString();
                result.EmailAddress = dr["EmailAddress"].ToString();
                result.PhoneNumber = dr["PhoneNumber"].ToString();
                result.StreetAddress1 = dr["StreetAddress1"].ToString();
                result.StreetAddress2 = dr["StreetAddress2"].ToString();
                result.City = dr["City"].ToString();
                result.State = dr["State"].ToString();
                result.ZipCode = dr["ZipCode"].ToString();
                result.Country = dr["Country"].ToString();
                result.County = dr["County"].ToString();
                result.ParentGuardianFirstName = dr["ParentGuardianFirstName"].ToString();
                result.ParentGuardianLastName = dr["ParentGuardianLastName"].ToString();
                result.ParentGuardianMiddleName = dr["ParentGuardianMiddleName"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) result.PrimaryLibrary = _int;
                result.LibraryCard = dr["LibraryCard"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int)) result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) result.LiteracyLevel2 = _int;
                result.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                result.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                result.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                result.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["AvatarID"].ToString(), out _int)) result.AvatarID = _int;
                if (int.TryParse(dr["SDistrict"].ToString(), out _int)) result.SDistrict = _int;

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

                if (int.TryParse(dr["Score1"].ToString(), out _int)) result.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int)) result.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal)) result.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal)) result.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime)) result.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime)) result.Score2Date = _datetime;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }


        public static bool CanManageSubAccount(int ma, int sa)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@MainAccount", ma); 
            arrParams[1] = new SqlParameter("@SubAccount", sa);

            var result = (int)SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "app_Patron_CanManageSubAccount", arrParams);
            return (result == 1);
        }


        public static DataSet GetSubAccountList(int maid)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", maid);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetSubAccountList", arrParams);
        }


        public static DataSet GetPatronForEdit(int PID)
        {
           var TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                            -1 :
                            (int)HttpContext.Current.Session["TenantID"]
                );

            return GetPatronForEdit(PID, TenID);
        }

        public static DataSet GetPatronForEdit(int PID, int TenID)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@TenID",TenID );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetPatronForEdit", arrParams);
        }


        //public bool Logoff()
        //{
        //    return Logoff(PID);

        //}

        //public static bool Logoff(int? uid)
        //{
        //    var arrParams = new SqlParameter[1];
        //    arrParams[0] = new SqlParameter("@UID", uid);
        //    SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Patron_Logout", arrParams);
        //    HttpContext.Current.Session.RemoveAll();
        //    return true;
        //}

    }//end class

}//end namespace

