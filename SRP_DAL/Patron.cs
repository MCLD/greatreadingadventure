using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using GRA.Tools.PasswordHash;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace GRA.SRP.DAL
{

    [Serializable]
    public class Patron : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        private const int pbkdf2Iterations = 150000;

        private static string conn = GlobalUtilities.SRPDB;

        #region Accessors

        public int PID { get; set; }
        public bool IsMasterAccount { get; set; }
        public int MasterAcctPID { get; set; }
        public string Username { get; set; }
        public string NewPassword { private get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public string SchoolGrade { get; set; }
        public int ProgID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string ParentGuardianFirstName { get; set; }
        public string ParentGuardianLastName { get; set; }
        public string ParentGuardianMiddleName { get; set; }
        public int PrimaryLibrary { get; set; }
        public string LibraryCard { get; set; }
        public string SchoolName { get; set; }
        public string District { get; set; }
        public string Teacher { get; set; }
        public string GroupTeamName { get; set; }
        public int SchoolType { get; set; }
        public int LiteracyLevel1 { get; set; }
        public int LiteracyLevel2 { get; set; }
        public bool ParentPermFlag { get; set; }
        public bool Over18Flag { get; set; }
        public bool ShareFlag { get; set; }
        public bool TermsOfUseflag { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public int SDistrict { get; set; }
        public int GoalCache { get; set; } /* GoalCache is the total number of points from doing an activity over the duratino of a program */
        public int Goal { get; set; } /* Daily Goal refers to the number of an activity (minutes, pages,etc) */
        public string AvatarState { get; set; }

        public int TenID { get; set; }

        public int FldInt1 { get; set; }

        public int FldInt2 { get; set; }

        public int FldInt3 { get; set; }

        public bool FldBit1 { get; set; }

        public bool FldBit2 { get; set; }

        public bool FldBit3 { get; set; }

        public string FldText1 { get; set; }

        public string FldText2 { get; set; }

        public string FldText3 { get; set; }


        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public decimal Score1Pct { get; set; }
        public decimal Score2Pct { get; set; }

        public DateTime Score1Date { get; set; }

        public DateTime Score2Date { get; set; }

        public DateTime RegistrationDate { get; set; }
        public bool SuppressFromFeed { get; set; }
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

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetScoreRank", arrParams);
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

        public static DataSet GetPaged(string sort,
            int startRowIndex,
            int maximumRows,
            string searchFirstName,
            string searchLastName,
            string searchUsername,
            string searchEmail,
            string searchDOB,
            int searchProgram,
            string searchGender,
            int searchLibraryId,
            int searchLibraryDistrictId)
        {

            var arrParams = new List<SqlParameter>();
            arrParams.Add(new SqlParameter("@startRowIndex", startRowIndex));
            arrParams.Add(new SqlParameter("@maximumRows", maximumRows));
            arrParams.Add(new SqlParameter("@sortString", sort));
            arrParams.Add(new SqlParameter("@searchFirstName", searchFirstName));
            arrParams.Add(new SqlParameter("@searchLastName", searchLastName));
            arrParams.Add(new SqlParameter("@searchUsername", searchUsername));
            arrParams.Add(new SqlParameter("@searchEmail", searchEmail));
            arrParams.Add(new SqlParameter("@searchDOB", GlobalUtilities.DBSafeValue(FormatHelper.SafeToDateTime(searchDOB), FormatHelper.SafeToDateTime(searchDOB).GetTypeCode())));
            arrParams.Add(new SqlParameter("@searchProgram", searchProgram));
            arrParams.Add(new SqlParameter("@searchGender", searchGender));
            arrParams.Add(new SqlParameter("@searchLibraryId", searchLibraryId));
            arrParams.Add(new SqlParameter("@searchLibraryDistrictId", searchLibraryDistrictId));
            arrParams.Add(new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            ));

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetPatronsPaged", arrParams.ToArray());
        }

        public static int GetTotalPagedCount(string sort,
            int startRowIndex,
            int maximumRows,
            string searchFirstName,
            string searchLastName,
            string searchUsername,
            string searchEmail,
            string searchDOB,
            int searchProgram,
            string searchGender,
            int searchLibraryId,
            int searchLibraryDistrictId)
        {
            var arrParams = new List<SqlParameter>();
            arrParams.Add(new SqlParameter("@startRowIndex", startRowIndex));
            arrParams.Add(new SqlParameter("@maximumRows", maximumRows));
            arrParams.Add(new SqlParameter("@sortString", sort));
            arrParams.Add(new SqlParameter("@searchFirstName", searchFirstName));
            arrParams.Add(new SqlParameter("@searchLastName", searchLastName));
            arrParams.Add(new SqlParameter("@searchUsername", searchUsername));
            arrParams.Add(new SqlParameter("@searchEmail", searchEmail));
            arrParams.Add(new SqlParameter("@searchDOB", GlobalUtilities.DBSafeValue(FormatHelper.SafeToDateTime(searchDOB), FormatHelper.SafeToDateTime(searchDOB).GetTypeCode())));
            arrParams.Add(new SqlParameter("@searchProgram", searchProgram));
            arrParams.Add(new SqlParameter("@searchGender", searchGender));
            arrParams.Add(new SqlParameter("@searchLibraryId", searchLibraryId));
            arrParams.Add(new SqlParameter("@searchLibraryDistrictId", searchLibraryDistrictId));
            arrParams.Add(new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            ));

            return (int)SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetTotalPatrons", arrParams.ToArray()).Tables[0].Rows[0][0];
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

                if (int.TryParse(dr["PID"].ToString(), out _int))
                    result.PID = _int;
                result.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int))
                    result.MasterAcctPID = _int;
                result.Username = dr["Username"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime))
                    result.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int))
                    result.Age = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int))
                    result.ProgID = _int;
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
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int))
                    result.PrimaryLibrary = _int;
                result.LibraryCard = dr["LibraryCard"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int))
                    result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int))
                    result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int))
                    result.LiteracyLevel2 = _int;
                result.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                result.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                result.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                result.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["SDistrict"].ToString(), out _int))
                    result.SDistrict = _int;
                if (int.TryParse(dr["Goal"].ToString(), out _int))
                    result.Goal = _int;
                result.AvatarState = dr["AvatarState"].ToString();
                if (int.TryParse(dr["GoalCache"].ToString(), out _int))
                    result.GoalCache = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int))
                    result.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int))
                    result.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int))
                    result.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int))
                    result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();

                if (int.TryParse(dr["Score1"].ToString(), out _int))
                    result.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int))
                    result.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal))
                    result.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal))
                    result.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime))
                    result.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime))
                    result.Score2Date = _datetime;

                if (DateTime.TryParse(dr["RegistrationDate"].ToString(), out _datetime))
                {
                    result.RegistrationDate = _datetime;
                }
                result.SuppressFromFeed = dr["SuppressFromFeed"] as bool? ?? false;

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

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int))
                    this.PID = _int;
                this.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int))
                    this.MasterAcctPID = _int;
                this.Username = dr["Username"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime))
                    this.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int))
                    this.Age = _int;
                this.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int))
                    this.ProgID = _int;
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
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int))
                    this.PrimaryLibrary = _int;
                this.LibraryCard = dr["LibraryCard"].ToString();
                this.SchoolName = dr["SchoolName"].ToString();
                this.District = dr["District"].ToString();
                this.Teacher = dr["Teacher"].ToString();
                this.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int))
                    this.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int))
                    this.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int))
                    this.LiteracyLevel2 = _int;
                this.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                this.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                this.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                this.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                this.Custom1 = dr["Custom1"].ToString();
                this.Custom2 = dr["Custom2"].ToString();
                this.Custom3 = dr["Custom3"].ToString();
                this.Custom4 = dr["Custom4"].ToString();
                this.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["SDistrict"].ToString(), out _int))
                    this.SDistrict = _int;
                if (int.TryParse(dr["Goal"].ToString(), out _int))
                    this.Goal = _int;
                this.AvatarState = dr["AvatarState"].ToString();
                if (int.TryParse(dr["GoalCache"].ToString(), out _int))
                    this.GoalCache = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int))
                    this.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int))
                    this.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int))
                    this.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int))
                    this.FldInt3 = _int;
                this.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                this.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                this.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                this.FldText1 = dr["FldText1"].ToString();
                this.FldText2 = dr["FldText2"].ToString();
                this.FldText3 = dr["FldText3"].ToString();

                if (int.TryParse(dr["Score1"].ToString(), out _int))
                    this.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int))
                    this.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal))
                    this.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal))
                    this.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime))
                    this.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime))
                    this.Score2Date = _datetime;

                if (DateTime.TryParse(dr["RegistrationDate"].ToString(), out _datetime))
                {
                    this.RegistrationDate = _datetime;
                }
                this.SuppressFromFeed = dr["SuppressFromFeed"] as bool? ?? false;

                dr.Close();

                return true;

            }

            dr.Close();

            return false;

        }

        public int Insert()
        {

            List<SqlParameter> parameters = new List<SqlParameter>();

            string passwordHash = PasswordHash.CreateHash(this.NewPassword,
                                                          pbkdf2Iterations);

            parameters.Add(new SqlParameter("@IsMasterAccount", this.IsMasterAccount));
            parameters.Add(new SqlParameter("@MasterAcctPID", this.MasterAcctPID));
            parameters.Add(new SqlParameter("@Username", this.Username));
            parameters.Add(new SqlParameter("@Password", passwordHash));
            parameters.Add(new SqlParameter("@DOB", GlobalUtilities.DBSafeValue(this.DOB, this.DOB.GetTypeCode())));
            parameters.Add(new SqlParameter("@Age", this.Age));
            parameters.Add(new SqlParameter("@SchoolGrade", this.SchoolGrade ?? string.Empty));
            parameters.Add(new SqlParameter("@ProgID", this.ProgID));
            parameters.Add(new SqlParameter("@FirstName", this.FirstName ?? string.Empty));
            parameters.Add(new SqlParameter("@MiddleName", this.MiddleName ?? string.Empty));
            parameters.Add(new SqlParameter("@LastName", this.LastName ?? string.Empty));
            parameters.Add(new SqlParameter("@Gender", this.Gender ?? string.Empty));
            parameters.Add(new SqlParameter("@EmailAddress", this.EmailAddress ?? string.Empty));
            parameters.Add(new SqlParameter("@PhoneNumber", this.PhoneNumber ?? string.Empty));
            parameters.Add(new SqlParameter("@StreetAddress1", this.StreetAddress1 ?? string.Empty));
            parameters.Add(new SqlParameter("@StreetAddress2", this.StreetAddress2 ?? string.Empty));
            parameters.Add(new SqlParameter("@City", this.City ?? string.Empty));
            parameters.Add(new SqlParameter("@State", this.State ?? string.Empty));
            parameters.Add(new SqlParameter("@ZipCode", this.ZipCode ?? string.Empty));
            parameters.Add(new SqlParameter("@Country", this.Country ?? string.Empty));
            parameters.Add(new SqlParameter("@County", this.County ?? string.Empty));
            parameters.Add(new SqlParameter("@ParentGuardianFirstName", this.ParentGuardianFirstName ?? string.Empty));
            parameters.Add(new SqlParameter("@ParentGuardianLastName", this.ParentGuardianLastName ?? string.Empty));
            parameters.Add(new SqlParameter("@ParentGuardianMiddleName", this.ParentGuardianMiddleName ?? string.Empty));
            parameters.Add(new SqlParameter("@PrimaryLibrary", this.PrimaryLibrary));
            parameters.Add(new SqlParameter("@LibraryCard", this.LibraryCard ?? string.Empty));
            parameters.Add(new SqlParameter("@SchoolName", this.SchoolName ?? string.Empty));
            parameters.Add(new SqlParameter("@District", this.District ?? string.Empty));
            parameters.Add(new SqlParameter("@Teacher", this.Teacher ?? string.Empty));
            parameters.Add(new SqlParameter("@GroupTeamName", this.GroupTeamName ?? string.Empty));
            parameters.Add(new SqlParameter("@SchoolType", this.SchoolType));
            parameters.Add(new SqlParameter("@LiteracyLevel1", this.LiteracyLevel1));
            parameters.Add(new SqlParameter("@LiteracyLevel2", this.LiteracyLevel2));
            parameters.Add(new SqlParameter("@ParentPermFlag", this.ParentPermFlag));
            parameters.Add(new SqlParameter("@Over18Flag", this.Over18Flag));
            parameters.Add(new SqlParameter("@ShareFlag", this.ShareFlag));
            parameters.Add(new SqlParameter("@TermsOfUseflag", this.TermsOfUseflag));
            parameters.Add(new SqlParameter("@Custom1", this.Custom1 ?? string.Empty));
            parameters.Add(new SqlParameter("@Custom2", this.Custom2 ?? string.Empty));
            parameters.Add(new SqlParameter("@Custom3", this.Custom3 ?? string.Empty));
            parameters.Add(new SqlParameter("@Custom4", this.Custom4 ?? string.Empty));
            parameters.Add(new SqlParameter("@Custom5", this.Custom5 ?? string.Empty));
            parameters.Add(new SqlParameter("@SDistrict", this.SDistrict));
            parameters.Add(new SqlParameter("@Goal", this.Goal));
            parameters.Add(new SqlParameter("@GoalCache", this.GoalCache));
            parameters.Add(new SqlParameter("@AvatarState", this.AvatarState));


            parameters.Add(new SqlParameter("@TenID", this.TenID));
            parameters.Add(new SqlParameter("@FldInt1", this.FldInt1));
            parameters.Add(new SqlParameter("@FldInt2", this.FldInt2));
            parameters.Add(new SqlParameter("@FldInt3", this.FldInt3));
            parameters.Add(new SqlParameter("@FldBit1", this.FldBit1));
            parameters.Add(new SqlParameter("@FldBit2", this.FldBit2));
            parameters.Add(new SqlParameter("@FldBit3", this.FldBit3));
            parameters.Add(new SqlParameter("@FldText1", this.FldText1 ?? string.Empty));
            parameters.Add(new SqlParameter("@FldText2", this.FldText2 ?? string.Empty));
            parameters.Add(new SqlParameter("@FldText3", this.FldText3 ?? string.Empty));

            parameters.Add(new SqlParameter("@Score1", this.Score1));
            parameters.Add(new SqlParameter("@Score2", this.Score2));
            parameters.Add(new SqlParameter("@Score1Pct", this.Score1Pct));
            parameters.Add(new SqlParameter("@Score2Pct", this.Score2Pct));
            parameters.Add(new SqlParameter("@Score1Date", GlobalUtilities.DBSafeValue(this.Score1Date, this.Score1Date.GetTypeCode())));
            parameters.Add(new SqlParameter("@Score2Date", GlobalUtilities.DBSafeValue(this.Score2Date, this.Score2Date.GetTypeCode())));
            parameters.Add(new SqlParameter("@SuppressFromFeed", this.SuppressFromFeed));
            var pidOutput = new SqlParameter("@PID", this.PID);
            pidOutput.Direction = ParameterDirection.Output;
            parameters.Add(pidOutput);

            SqlHelper.ExecuteNonQuery(conn,
                                      CommandType.StoredProcedure,
                                      "app_Patron_Insert",
                                      parameters.ToArray());

            this.PID = int.Parse(pidOutput.Value.ToString());

            return this.PID;
        }

        public static bool VerifyPassword(string logon, string password)
        {
            string passwordHashQuery = "SELECT [Password] FROM [Patron] WHERE [UserName] = @UserName";
            SqlParameter parameter = new SqlParameter("UserName", logon);
            string passwordHash = (string)SqlHelper.ExecuteScalar(conn,
                                                                  CommandType.Text,
                                                                  passwordHashQuery,
                                                                  parameter);
            if (string.IsNullOrEmpty(passwordHash))
            {
                // no such user
                return false;
            }
            return PasswordHash.ValidatePassword(password, passwordHash);
        }


        public int Update()
        {
            return this.Update(false);
        }

        private int Update(bool clearTokens = false)
        {
            var arrParams = new List<SqlParameter>();

            string passwordHash = null;
            if (!string.IsNullOrEmpty(this.NewPassword))
            {
                passwordHash = PasswordHash.CreateHash(this.NewPassword,
                                                       pbkdf2Iterations);
            }
            else
            {
                string passwordHashQuery = "SELECT [Password] FROM [Patron] WHERE [Pid] = @pid";
                SqlParameter parameter = new SqlParameter("pid", this.PID);
                passwordHash = (string)SqlHelper.ExecuteScalar(conn,
                                                               CommandType.Text,
                                                               passwordHashQuery,
                                                               parameter);
            }

            arrParams.Add(new SqlParameter("@PID", GlobalUtilities.DBSafeValue(this.PID, this.PID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@IsMasterAccount", GlobalUtilities.DBSafeValue(this.IsMasterAccount, this.IsMasterAccount.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MasterAcctPID", GlobalUtilities.DBSafeValue(this.MasterAcctPID, this.MasterAcctPID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Username", GlobalUtilities.DBSafeValue(this.Username, this.Username.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Password", passwordHash));
            arrParams.Add(new SqlParameter("@DOB", GlobalUtilities.DBSafeValue(this.DOB, this.DOB.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Age", GlobalUtilities.DBSafeValue(this.Age, this.Age.GetTypeCode())));
            try
            {
                arrParams.Add(new SqlParameter("@SchoolGrade", GlobalUtilities.DBSafeValue(this.SchoolGrade, this.SchoolGrade.GetTypeCode())));
            }
            catch (Exception ex)
            {
                this.Log().Error("Unable to set SchoolGrade to: {0}: {1} - {2}",
                                 this.SchoolGrade,
                                 ex.Message,
                                 ex.StackTrace);
                arrParams.Add(new SqlParameter("@SchoolGrade", string.Empty));
            }
            arrParams.Add(new SqlParameter("@ProgID", GlobalUtilities.DBSafeValue(this.ProgID, this.ProgID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FirstName", GlobalUtilities.DBSafeValue(this.FirstName, this.FirstName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MiddleName", GlobalUtilities.DBSafeValue(this.MiddleName, this.MiddleName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastName", GlobalUtilities.DBSafeValue(this.LastName, this.LastName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Gender", GlobalUtilities.DBSafeValue(this.Gender, this.Gender.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EmailAddress", GlobalUtilities.DBSafeValue(this.EmailAddress, this.EmailAddress.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhoneNumber", GlobalUtilities.DBSafeValue(this.PhoneNumber, this.PhoneNumber.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress1", GlobalUtilities.DBSafeValue(this.StreetAddress1, this.StreetAddress1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@StreetAddress2", GlobalUtilities.DBSafeValue(this.StreetAddress2, this.StreetAddress2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City", GlobalUtilities.DBSafeValue(this.City, this.City.GetTypeCode())));
            arrParams.Add(new SqlParameter("@State", GlobalUtilities.DBSafeValue(this.State, this.State.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ZipCode", GlobalUtilities.DBSafeValue(this.ZipCode, this.ZipCode.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Country", GlobalUtilities.DBSafeValue(this.Country, this.Country.GetTypeCode())));
            arrParams.Add(new SqlParameter("@County", GlobalUtilities.DBSafeValue(this.County, this.County.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianFirstName", GlobalUtilities.DBSafeValue(this.ParentGuardianFirstName, this.ParentGuardianFirstName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianLastName", GlobalUtilities.DBSafeValue(this.ParentGuardianLastName, this.ParentGuardianLastName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentGuardianMiddleName", GlobalUtilities.DBSafeValue(this.ParentGuardianMiddleName, this.ParentGuardianMiddleName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PrimaryLibrary", GlobalUtilities.DBSafeValue(this.PrimaryLibrary, this.PrimaryLibrary.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryCard", GlobalUtilities.DBSafeValue(this.LibraryCard, this.LibraryCard.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolName", GlobalUtilities.DBSafeValue(this.SchoolName, this.SchoolName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@District", GlobalUtilities.DBSafeValue(this.District, this.District.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Teacher", GlobalUtilities.DBSafeValue(this.Teacher, this.Teacher.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GroupTeamName", GlobalUtilities.DBSafeValue(this.GroupTeamName, this.GroupTeamName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolType", GlobalUtilities.DBSafeValue(this.SchoolType, this.SchoolType.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1", GlobalUtilities.DBSafeValue(this.LiteracyLevel1, this.LiteracyLevel1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2", GlobalUtilities.DBSafeValue(this.LiteracyLevel2, this.LiteracyLevel2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ParentPermFlag", GlobalUtilities.DBSafeValue(this.ParentPermFlag, this.ParentPermFlag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Over18Flag", GlobalUtilities.DBSafeValue(this.Over18Flag, this.Over18Flag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ShareFlag", GlobalUtilities.DBSafeValue(this.ShareFlag, this.ShareFlag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@TermsOfUseflag", GlobalUtilities.DBSafeValue(this.TermsOfUseflag, this.TermsOfUseflag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1", GlobalUtilities.DBSafeValue(this.Custom1, this.Custom1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2", GlobalUtilities.DBSafeValue(this.Custom2, this.Custom2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3", GlobalUtilities.DBSafeValue(this.Custom3, this.Custom3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom4", GlobalUtilities.DBSafeValue(this.Custom4, this.Custom4.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom5", GlobalUtilities.DBSafeValue(this.Custom5, this.Custom5.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SDistrict", GlobalUtilities.DBSafeValue(this.SDistrict, this.SDistrict.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Goal", GlobalUtilities.DBSafeValue(this.Goal, this.Goal.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AvatarState", GlobalUtilities.DBSafeValue(this.AvatarState, this.AvatarState.GetTypeCode())));

            arrParams.Add(new SqlParameter("@GoalCache", GlobalUtilities.DBSafeValue(this.GoalCache, this.GoalCache.GetTypeCode())));


            arrParams.Add(new SqlParameter("@TenID", GlobalUtilities.DBSafeValue(this.TenID, this.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GlobalUtilities.DBSafeValue(this.FldInt1, this.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GlobalUtilities.DBSafeValue(this.FldInt2, this.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GlobalUtilities.DBSafeValue(this.FldInt3, this.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GlobalUtilities.DBSafeValue(this.FldBit1, this.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GlobalUtilities.DBSafeValue(this.FldBit2, this.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GlobalUtilities.DBSafeValue(this.FldBit3, this.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GlobalUtilities.DBSafeValue(this.FldText1, string.Empty.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GlobalUtilities.DBSafeValue(this.FldText2, string.Empty.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GlobalUtilities.DBSafeValue(this.FldText3, string.Empty.GetTypeCode())));


            arrParams.Add(new SqlParameter("@Score1", GlobalUtilities.DBSafeValue(this.Score1, this.Score1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Score2", GlobalUtilities.DBSafeValue(this.Score2, this.Score2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Score1Pct", GlobalUtilities.DBSafeValue(this.Score1Pct, this.Score1Pct.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Score2Pct", GlobalUtilities.DBSafeValue(this.Score2Pct, this.Score2Pct.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Score1Date", GlobalUtilities.DBSafeValue(this.Score1Date, this.Score1Date.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Score2Date", GlobalUtilities.DBSafeValue(this.Score2Date, this.Score2Date.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SuppressFromFeed", this.SuppressFromFeed));
            try
            {
                using (var connection = new SqlConnection(conn))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction("UpdatePatron"))
                    {
                        SqlHelper.ExecuteNonQuery(transaction,
                                                  CommandType.StoredProcedure,
                                                  "dbo.app_Patron_Update",
                                                  arrParams.ToArray());
                        if (clearTokens)
                        {
                            string removeTokensQuery = "DELETE FROM [PatronRecovery] WHERE [PID] = @pid";
                            SqlHelper.ExecuteNonQuery(transaction,
                                                      CommandType.Text,
                                                      removeTokensQuery,
                                                      new SqlParameter("pid", this.PID));
                        }
                        transaction.Commit();
                    }
                    connection.Close();
                }
                return 1;

            }
            catch (Exception ex)
            {
                this.Log().Error(() => string.Format("Unable to update Patron {0}/{1}: {2}",
                                                     this.PID,
                                                     this.Username,
                                                     ex.Message));
                return 0;
            }
        }

        public static Patron GetUserByToken(string token, int hoursWindow = 24)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT [PID] FROM [PatronRecovery] WHERE [Token] = @token ");
            sql.Append("AND DateDiff(hh, [Generated], GETDATE()) <= @hourswindow");

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("token", token));
            parameters.Add(new SqlParameter("hourswindow", hoursWindow));

            object pidObject = null;
            try
            {
                pidObject = SqlHelper.ExecuteScalar(conn,
                                                    CommandType.Text,
                                                    sql.ToString(),
                                                    parameters.ToArray());
            }
            catch (Exception ex)
            {
                "Patron".Log().Error(() => string.Format("Unable to retrieve patron from token {0}",
                                                         token),
                                     ex);
                return null;
            }

            int pid = 0;
            if (pidObject == null
               || !int.TryParse(pidObject.ToString(), out pid))
            {
                "SRPUser".Log().Info("Password recovery lookup failed: patron looked up = {0}, token was {1}",
                                     pidObject ?? "NULL",
                                     token);
                return null;
            }

            Patron p = new Patron();
            if (p.Fetch(pid))
            {
                return p;
            }
            else
            {
                return null;
            }
        }

        public static Patron UpdatePasswordByToken(string token,
                                                   string newPassword,
                                                   int hoursWindow = 24)
        {
            Patron user = GetUserByToken(token, hoursWindow);
            user.NewPassword = newPassword;
            user.Update(true);

            user.NewPassword = null;
            return user;
        }

        public string GeneratePasswordResetToken(int desiredLength = 12)
        {
            if (this.PID == 0)
            {
                throw new Exception("Unable to perform password reset, no user provided.");
            }
            string resetToken = Password.GeneratePasswordResetToken(desiredLength);

            StringBuilder sql = new StringBuilder();

            sql.AppendLine("DELETE FROM [PatronRecovery] WHERE [PID] = @pid;");
            sql.Append("INSERT INTO [PatronRecovery] ([Token], [PID], [Generated]) values ");
            sql.AppendLine("(@token, @pid, GETDATE());");

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("pid", this.PID));
            parameters.Add(new SqlParameter("token", resetToken));

            try
            {
                SqlHelper.ExecuteNonQuery(conn,
                                          CommandType.Text,
                                          sql.ToString(),
                                          parameters.ToArray());
                return resetToken;
            }
            catch (Exception ex)
            {
                this.Log().Error(() => "Unable to save password reset token to database", ex);
                return null;
            }
        }

        public int Delete()
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", GlobalUtilities.DBSafeValue(this.PID, this.PID.GetTypeCode()));

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
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                Patron obj = GetObjectByUsername(Username);
                if (obj != null)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Username",
                        "Username",
                        "The Username you have chosen is already in use.  Please select a different Username.",
                        BusinessRulesValidationCode.UNSPECIFIED));
                }
                if (string.IsNullOrEmpty(this.NewPassword))
                {
                    AddErrorCode(new BusinessRulesValidationMessage("NewPassword",
                        "Password",
                        "You must supply a password for a new user",
                        BusinessRulesValidationCode.REQUIRED_FIELD));
                }
            }
            return (ErrorCodes.Count == 0);
            //return true;
        }

        public int RecalculateGoalCache(Programs program, ProgramGamePointConversion conversion)
        {

            int goal = this.Goal;
            GoalInterval interval = program.GetGoalInterval;

            if (program != null)
            {
                int programLength = 1;

                switch (interval)
                {
                    case GoalInterval.Program:
                        programLength = 1;
                        break;
                    case GoalInterval.Daily:
                        programLength = (int)((program.LoggingEnd - program.LoggingStart).TotalDays);
                        break;
                    case GoalInterval.Weekly:
                        programLength = (int)((program.LoggingEnd - program.LoggingStart).TotalDays) / 7;
                        break;
                }

                goal *= programLength;
            }


            if (conversion != null)
            {
                goal *= conversion.PointCount;

                if (conversion.ActivityCount != 0)
                    goal /= conversion.ActivityCount;
            }

            /* protect against divide by zero error */
            if (goal < 1)
            {
                goal = 0;
            }

            this.GoalCache = goal;
            return this.GoalCache;
        }



        public static bool Login(string logon, string password)
        {
            return VerifyPassword(logon, password);
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

                if (int.TryParse(dr["PID"].ToString(), out _int))
                    result.PID = _int;
                result.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int))
                    result.MasterAcctPID = _int;
                result.Username = dr["Username"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime))
                    result.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int))
                    result.Age = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int))
                    result.ProgID = _int;
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
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int))
                    result.PrimaryLibrary = _int;
                result.LibraryCard = dr["LibraryCard"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int))
                    result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int))
                    result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int))
                    result.LiteracyLevel2 = _int;
                result.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                result.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                result.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                result.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["SDistrict"].ToString(), out _int))
                    result.SDistrict = _int;
                if (int.TryParse(dr["Goal"].ToString(), out _int))
                    result.Goal = _int;
                result.AvatarState = dr["AvatarState"].ToString();
                if (int.TryParse(dr["GoalCache"].ToString(), out _int))
                    result.GoalCache = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int))
                    result.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int))
                    result.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int))
                    result.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int))
                    result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();

                if (int.TryParse(dr["Score1"].ToString(), out _int))
                    result.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int))
                    result.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal))
                    result.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal))
                    result.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime))
                    result.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime))
                    result.Score2Date = _datetime;
                result.SuppressFromFeed = dr["SuppressFromFeed"] as bool? ?? false;

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

                if (int.TryParse(dr["PID"].ToString(), out _int))
                    result.PID = _int;
                result.IsMasterAccount = bool.Parse(dr["IsMasterAccount"].ToString());
                if (int.TryParse(dr["MasterAcctPID"].ToString(), out _int))
                    result.MasterAcctPID = _int;
                result.Username = dr["Username"].ToString();
                if (DateTime.TryParse(dr["DOB"].ToString(), out _datetime))
                    result.DOB = _datetime;
                if (int.TryParse(dr["Age"].ToString(), out _int))
                    result.Age = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int))
                    result.ProgID = _int;
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
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int))
                    result.PrimaryLibrary = _int;
                result.LibraryCard = dr["LibraryCard"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int))
                    result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int))
                    result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int))
                    result.LiteracyLevel2 = _int;
                result.ParentPermFlag = bool.Parse(dr["ParentPermFlag"].ToString());
                result.Over18Flag = bool.Parse(dr["Over18Flag"].ToString());
                result.ShareFlag = bool.Parse(dr["ShareFlag"].ToString());
                result.TermsOfUseflag = bool.Parse(dr["TermsOfUseflag"].ToString());
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (int.TryParse(dr["SDistrict"].ToString(), out _int))
                    result.SDistrict = _int;
                if (int.TryParse(dr["Goal"].ToString(), out _int))
                    result.Goal = _int;
                result.AvatarState = dr["AvatarState"].ToString();
                if (int.TryParse(dr["GoalCache"].ToString(), out _int))
                    result.GoalCache = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int))
                    result.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int))
                    result.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int))
                    result.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int))
                    result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();

                if (int.TryParse(dr["Score1"].ToString(), out _int))
                    result.Score1 = _int;
                if (int.TryParse(dr["Score2"].ToString(), out _int))
                    result.Score2 = _int;
                var _decimal = (decimal)0.0;
                if (decimal.TryParse(dr["Score1Pct"].ToString(), out _decimal))
                    result.Score1Pct = _decimal;
                if (decimal.TryParse(dr["Score2Pct"].ToString(), out _decimal))
                    result.Score2Pct = _decimal;
                if (DateTime.TryParse(dr["Score1Date"].ToString(), out _datetime))
                    result.Score1Date = _datetime;
                if (DateTime.TryParse(dr["Score2Date"].ToString(), out _datetime))
                    result.Score2Date = _datetime;
                result.SuppressFromFeed = dr["SuppressFromFeed"] as bool? ?? false;

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
            arrParams[1] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Patron_GetPatronForEdit", arrParams);
        }

        public static DataSet GetReadingList(int pid)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT DISTINCT [Author], [Title] FROM [PatronReadingLog] ");
            query.Append("WHERE [PID] = @PID AND (AUTHOR != '' AND TITLE != '') ");
            query.Append("ORDER BY [Title]");

            var patronParameter = new SqlParameter("@PID", pid);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, query.ToString(), patronParameter);
        }


        public static List<int> ReadAvatarStateString(string stateString)
        {
            var stateValues = new List<int>();


            if (stateString != null && stateString.Length != 0 && stateString.Length % 2 == 0)
            {
                for (int pos = 0; pos < stateString.Length; pos += 2)
                {
                    string hexValue = stateString.Substring(pos, 2);

                    int value = Convert.ToInt32(hexValue, 16);
                    stateValues.Add(value);
                }
            }

            return stateValues;
        }

        public static string WriteAvatarStateString(List<int> state)
        {
            String stateString = "";

            foreach (int value in state)
            {
                stateString += value.ToString("X2");
            }

            return stateString;
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

