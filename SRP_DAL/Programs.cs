using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.ApplicationBlocks.Data;
using System.Collections;
using GRA.SRP.Core.Utilities;
using GRA.Tools;
using System.Web.Caching;

namespace GRA.SRP.DAL
{

[Serializable]    public class Programs : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPID;
        private string myAdminName;
        private string myTitle;
        private string myTabName;
        private int myPOrder;
        private bool myIsActive;
        private bool myIsHidden;
        private DateTime myStartDate;
        private DateTime myEndDate;
        private int myMaxAge;
        private int myMaxGrade;
        private DateTime myLoggingStart;
        private DateTime myLoggingEnd;
        private bool myParentalConsentFlag;
        private string myParentalConsentText ="";
        private bool myPatronReviewFlag;
        private string myLogoutURL = "";
        private int myProgramGameID;
        private string myHTML1 = "";
        private string myHTML2 = "";
        private string myHTML3 = "";
        private string myHTML4 = "";
        private string myHTML5 = "";
        private string myHTML6 = "";
        private string myBannerImage = "";
        private int myRegistrationBadgeID = 0;
        private int myCompletionPoints = 0;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;
        private DateTime myLastModDate;

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

        private int myPreTestID = 0;
        private int myPostTestID = 0;
        private bool myPreTestMandatory = false;
        private DateTime myPreTestEndDate;
        private DateTime myPostTestStartDate;


        #endregion

        #region Accessors

        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public string AdminName
        {
            get { return myAdminName; }
            set { myAdminName = value; }
        }
        public string Title
        {
            get { return myTitle; }
            set { myTitle = value; }
        }
        public string TabName
        {
            get { return myTabName; }
            set { myTabName = value; }
        }
        public int POrder
        {
            get { return myPOrder; }
            set { myPOrder = value; }
        }
        public bool IsActive
        {
            get { return myIsActive; }
            set { myIsActive = value; }
        }
        public bool IsHidden
        {
            get { return myIsHidden; }
            set { myIsHidden = value; }
        }
        public DateTime StartDate
        {
            get { return myStartDate; }
            set { myStartDate = value; }
        }
        public DateTime EndDate
        {
            get { return myEndDate; }
            set { myEndDate = value; }
        }
        public int MaxAge
        {
            get { return myMaxAge; }
            set { myMaxAge = value; }
        }
        public int MaxGrade
        {
            get { return myMaxGrade; }
            set { myMaxGrade = value; }
        }
        public DateTime LoggingStart
        {
            get { return myLoggingStart; }
            set { myLoggingStart = value; }
        }
        public DateTime LoggingEnd
        {
            get { return myLoggingEnd; }
            set { myLoggingEnd = value; }
        }
        public bool ParentalConsentFlag
        {
            get { return myParentalConsentFlag; }
            set { myParentalConsentFlag = value; }
        }
        public string ParentalConsentText
        {
            get { return myParentalConsentText; }
            set { myParentalConsentText = value; }
        }
        public bool PatronReviewFlag
        {
            get { return myPatronReviewFlag; }
            set { myPatronReviewFlag = value; }
        }
        public string LogoutURL
        {
            get { return myLogoutURL; }
            set { myLogoutURL = value; }
        }
        public int ProgramGameID
        {
            get { return myProgramGameID; }
            set { myProgramGameID = value; }
        }
        public string HTML1
        {
            get { return myHTML1; }
            set { myHTML1 = value; }
        }
        public string HTML2
        {
            get { return myHTML2; }
            set { myHTML2 = value; }
        }
        public string HTML3
        {
            get { return myHTML3; }
            set { myHTML3 = value; }
        }
        public string HTML4
        {
            get { return myHTML4; }
            set { myHTML4 = value; }
        }
        public string HTML5
        {
            get { return myHTML5; }
            set { myHTML5 = value; }
        }
        public string HTML6
        {
            get { return myHTML6; }
            set { myHTML6 = value; }
        }
        public string BannerImage
        {
            get { return myBannerImage; }
            set { myBannerImage = value; }
        }
        public int RegistrationBadgeID
        {
            get { return myRegistrationBadgeID; }
            set { myRegistrationBadgeID = value; }
        }
        public int CompletionPoints
        {
            get { return myCompletionPoints; }
            set { myCompletionPoints = value; }
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
        public DateTime LastModDate
        {
            get { return myLastModDate; }
            set { myLastModDate = value; }
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

        public int PreTestID
        {
            get { return myPreTestID; }
            set { myPreTestID = value; }
        }

        public int PostTestID
        {
            get { return myPostTestID; }
            set { myPostTestID = value; }
        }

        public bool PreTestMandatory
        {
            get { return myPreTestMandatory; }
            set { myPreTestMandatory = value; }
        }

        public DateTime PreTestEndDate
        {
            get { return myPreTestEndDate; }
            set { myPreTestEndDate = value; }
        }

        public DateTime PostTestStartDate
        {
            get { return myPostTestStartDate; }
            set { myPostTestStartDate = value; }
        }

        #endregion

        #region Constructors

        public Programs()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
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

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Programs_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Programs_GetAll", arrParams);
        }

        public static DataSet GetAllOrdered()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Programs_GetAllOrdered", arrParams);
        }

        public static DataSet GetAllTabs()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Programs_GetAllTabs", arrParams);
        }
        
        public static DataSet GetAllActive()        
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Programs_GetAllActive", arrParams);
        }

        public static int GetDefaultProgramID()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return (int)SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "app_Programs_GetDefaultProgramID", arrParams);
        }

        public static int GetDefaultProgramID(int tenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",tenID);
            return (int)SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "app_Programs_GetDefaultProgramID", arrParams);
        }

        public static int GetDefaultProgramForAgeAndGrade(int age, int grade)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@Age", age);
            arrParams[1] = new SqlParameter("@Grade", grade);
            arrParams[2] = new SqlParameter("@TenID",
                    (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                            -1 :
                            (int)HttpContext.Current.Session["TenantID"])
                );
            return (int)SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "app_Programs_GetDefaultProgramForAgeAndGrade", arrParams);
        }

        public static DataSet GetLeaderboard(int PID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@ProgId", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetProgramLeaderboard", arrParams);
        }


        public static DataSet GetProgramMinigames(string LevelIDs, int whichMinigames, int defMGID = 0)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@LevelIDs", LevelIDs);
            arrParams[1] = new SqlParameter("@WhichMG", whichMinigames);
            arrParams[2] = new SqlParameter("@DefaultMG", defMGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Programs_GetProgramMinigames", arrParams);
        }

        public static Programs FetchObject(int PID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Programs_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Programs result = new Programs();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                result.AdminName = dr["AdminName"].ToString();
                result.Title = dr["Title"].ToString();
                result.TabName = dr["TabName"].ToString();
                if (int.TryParse(dr["POrder"].ToString(), out _int)) result.POrder = _int;
                result.IsActive = bool.Parse(dr["IsActive"].ToString());
                result.IsHidden = bool.Parse(dr["IsHidden"].ToString());
                if (DateTime.TryParse(dr["StartDate"].ToString(), out _datetime)) result.StartDate = _datetime;
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) result.EndDate = _datetime;
                if (int.TryParse(dr["MaxAge"].ToString(), out _int)) result.MaxAge = _int;
                if (int.TryParse(dr["MaxGrade"].ToString(), out _int)) result.MaxGrade = _int;
                if (DateTime.TryParse(dr["LoggingStart"].ToString(), out _datetime)) result.LoggingStart = _datetime;
                if (DateTime.TryParse(dr["LoggingEnd"].ToString(), out _datetime)) result.LoggingEnd = _datetime;
                result.ParentalConsentFlag = bool.Parse(dr["ParentalConsentFlag"].ToString());
                result.ParentalConsentText = dr["ParentalConsentText"].ToString();
                result.PatronReviewFlag = bool.Parse(dr["PatronReviewFlag"].ToString());
                result.LogoutURL = dr["LogoutURL"].ToString();
                if (int.TryParse(dr["ProgramGameID"].ToString(), out _int)) result.ProgramGameID = _int;
                result.HTML1 = dr["HTML1"].ToString();
                result.HTML2 = dr["HTML2"].ToString();
                result.HTML3 = dr["HTML3"].ToString();
                result.HTML4 = dr["HTML4"].ToString();
                result.HTML5 = dr["HTML5"].ToString();
                result.HTML6 = dr["HTML6"].ToString();
                result.BannerImage = dr["BannerImage"].ToString();
                if (int.TryParse(dr["RegistrationBadgeID"].ToString(), out _int)) result.RegistrationBadgeID = _int;
                if (int.TryParse(dr["CompletionPoints"].ToString(), out _int)) result.CompletionPoints = _int;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;

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

                if (int.TryParse(dr["PreTestID"].ToString(), out _int)) result.PreTestID = _int;
                if (int.TryParse(dr["PostTestID"].ToString(), out _int)) result.PostTestID = _int;
                result.PreTestMandatory = bool.Parse(dr["PreTestMandatory"].ToString());
                if (DateTime.TryParse(dr["PreTestEndDate"].ToString(), out _datetime)) result.PreTestEndDate = _datetime;
                if (DateTime.TryParse(dr["PostTestStartDate"].ToString(), out _datetime)) result.PostTestStartDate = _datetime;
                

                dr.Close();

                CheckAndPopulatePointConversions(result);

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

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Programs_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Programs result = new Programs();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                this.AdminName = dr["AdminName"].ToString();
                this.Title = dr["Title"].ToString();
                this.TabName = dr["TabName"].ToString();
                if (int.TryParse(dr["POrder"].ToString(), out _int)) this.POrder = _int;
                this.IsActive = bool.Parse(dr["IsActive"].ToString());
                this.IsHidden = bool.Parse(dr["IsHidden"].ToString());
                if (DateTime.TryParse(dr["StartDate"].ToString(), out _datetime)) this.StartDate = _datetime;
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) this.EndDate = _datetime;
                if (int.TryParse(dr["MaxAge"].ToString(), out _int)) this.MaxAge = _int;
                if (int.TryParse(dr["MaxGrade"].ToString(), out _int)) this.MaxGrade = _int;
                if (DateTime.TryParse(dr["LoggingStart"].ToString(), out _datetime)) this.LoggingStart = _datetime;
                if (DateTime.TryParse(dr["LoggingEnd"].ToString(), out _datetime)) this.LoggingEnd = _datetime;
                this.ParentalConsentFlag = bool.Parse(dr["ParentalConsentFlag"].ToString());
                this.ParentalConsentText = dr["ParentalConsentText"].ToString();
                this.PatronReviewFlag = bool.Parse(dr["PatronReviewFlag"].ToString());
                this.LogoutURL = dr["LogoutURL"].ToString();
                if (int.TryParse(dr["ProgramGameID"].ToString(), out _int)) this.ProgramGameID = _int;
                this.HTML1 = dr["HTML1"].ToString();
                this.HTML2 = dr["HTML2"].ToString();
                this.HTML3 = dr["HTML3"].ToString();
                this.HTML4 = dr["HTML4"].ToString();
                this.HTML5 = dr["HTML5"].ToString();
                this.HTML6 = dr["HTML6"].ToString();
                this.BannerImage = dr["BannerImage"].ToString();
                if (int.TryParse(dr["RegistrationBadgeID"].ToString(), out _int)) this.RegistrationBadgeID = _int;
                if (int.TryParse(dr["CompletionPoints"].ToString(), out _int)) this.CompletionPoints = _int;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;

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

                if (int.TryParse(dr["PreTestID"].ToString(), out _int)) this.PreTestID = _int;
                if (int.TryParse(dr["PostTestID"].ToString(), out _int)) this.PostTestID = _int;
                this.PreTestMandatory = bool.Parse(dr["PreTestMandatory"].ToString());
                if (DateTime.TryParse(dr["PreTestEndDate"].ToString(), out _datetime)) this.PreTestEndDate = _datetime;
                if (DateTime.TryParse(dr["PostTestStartDate"].ToString(), out _datetime)) this.PostTestStartDate = _datetime;

                dr.Close();

                CheckAndPopulatePointConversions(this);

                return true;

            }

            dr.Close();

            return false;

        }

        public static void CheckAndPopulatePointConversions(Programs obj)
        {

            var ds = ProgramGamePointConversion.GetAll(obj.PID);
            if (ds.Tables[0].Rows.Count ==0)
            {
                foreach (ActivityType val in Enum.GetValues(typeof(ActivityType)))
                {
                    var o = new ProgramGamePointConversion();
                    o.PGID = obj.PID;
                    o.ActivityTypeId = (int)val;
                    o.ActivityCount = 1;
                    o.PointCount = 0;
                    o.AddedDate = obj.AddedDate;
                    o.AddedUser = obj.AddedUser;
                    o.LastModDate = o.AddedDate;
                    o.LastModUser = o.AddedUser;

                    o.Insert();
                }                
            }
            else
            {
                foreach (ActivityType val in Enum.GetValues(typeof(ActivityType)))
                {
                    var o = ProgramGamePointConversion.FetchObjectByActivityId(obj.PID, (int)val);
                    if (o==null)
                    {
                        o = new ProgramGamePointConversion
                                {
                                    PGID = obj.PID,
                                    ActivityTypeId = (int) val,
                                    ActivityCount = 1,
                                    PointCount = 0,
                                    AddedDate = obj.AddedDate,
                                    AddedUser = obj.AddedUser
                                };
                        o.LastModDate = o.AddedDate;
                        o.LastModUser = o.AddedUser;

                        o.Insert();
                    }
                }   
            }
        }


        public int Insert()
        {

            return Insert(this);

        }

        public static int Insert(Programs o)
        {

            SqlParameter[] arrParams = new SqlParameter[46];

            arrParams[0] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[2] = new SqlParameter("@TabName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TabName, o.TabName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@POrder", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.POrder, o.POrder.GetTypeCode()));
            arrParams[4] = new SqlParameter("@IsActive", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsActive, o.IsActive.GetTypeCode()));
            arrParams[5] = new SqlParameter("@IsHidden", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsHidden, o.IsHidden.GetTypeCode()));
            arrParams[6] = new SqlParameter("@StartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StartDate, o.StartDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@EndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@MaxAge", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxAge, o.MaxAge.GetTypeCode()));
            arrParams[9] = new SqlParameter("@MaxGrade", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxGrade, o.MaxGrade.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LoggingStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LoggingStart, o.LoggingStart.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LoggingEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LoggingEnd, o.LoggingEnd.GetTypeCode()));
            arrParams[12] = new SqlParameter("@ParentalConsentFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentalConsentFlag, o.ParentalConsentFlag.GetTypeCode()));
            arrParams[13] = new SqlParameter("@ParentalConsentText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentalConsentText, o.ParentalConsentText.GetTypeCode()));
            arrParams[14] = new SqlParameter("@PatronReviewFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronReviewFlag, o.PatronReviewFlag.GetTypeCode()));
            arrParams[15] = new SqlParameter("@LogoutURL", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogoutURL, o.LogoutURL.GetTypeCode()));
            arrParams[16] = new SqlParameter("@ProgramGameID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgramGameID, o.ProgramGameID.GetTypeCode()));
            arrParams[17] = new SqlParameter("@HTML1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML1, o.HTML1.GetTypeCode()));
            arrParams[18] = new SqlParameter("@HTML2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML2, o.HTML2.GetTypeCode()));
            arrParams[19] = new SqlParameter("@HTML3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML3, o.HTML3.GetTypeCode()));
            arrParams[20] = new SqlParameter("@HTML4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML4, o.HTML4.GetTypeCode()));
            arrParams[21] = new SqlParameter("@HTML5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML5, o.HTML5.GetTypeCode()));
            arrParams[22] = new SqlParameter("@HTML6", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML6, o.HTML6.GetTypeCode()));
            arrParams[23] = new SqlParameter("@BannerImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BannerImage, o.BannerImage.GetTypeCode()));
            arrParams[24] = new SqlParameter("@RegistrationBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationBadgeID, o.RegistrationBadgeID.GetTypeCode()));
            arrParams[25] = new SqlParameter("@CompletionPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CompletionPoints, o.CompletionPoints.GetTypeCode()));
            arrParams[26] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[27] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[28] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[29] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));

            arrParams[30] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[31] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[32] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[33] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[34] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[35] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[36] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[37] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[38] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[39] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[40] = new SqlParameter("@PreTestID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PreTestID, o.PreTestID.GetTypeCode()));
            arrParams[41] = new SqlParameter("@PostTestID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PostTestID, o.PostTestID.GetTypeCode()));
            arrParams[42] = new SqlParameter("@PreTestMandatory", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PreTestMandatory, o.PreTestMandatory.GetTypeCode()));
            arrParams[43] = new SqlParameter("@PreTestEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PreTestEndDate, o.PreTestEndDate.GetTypeCode()));
            arrParams[44] = new SqlParameter("@PostTestStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PostTestStartDate, o.PostTestStartDate.GetTypeCode()));
            
            arrParams[45] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[45].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Programs_Insert", arrParams);

            o.PID = int.Parse(arrParams[45].Value.ToString());

            return o.PID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Programs o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[46];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[3] = new SqlParameter("@TabName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TabName, o.TabName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@POrder", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.POrder, o.POrder.GetTypeCode()));
            arrParams[5] = new SqlParameter("@IsActive", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsActive, o.IsActive.GetTypeCode()));
            arrParams[6] = new SqlParameter("@IsHidden", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsHidden, o.IsHidden.GetTypeCode()));
            arrParams[7] = new SqlParameter("@StartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StartDate, o.StartDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@EndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@MaxAge", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxAge, o.MaxAge.GetTypeCode()));
            arrParams[10] = new SqlParameter("@MaxGrade", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxGrade, o.MaxGrade.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LoggingStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LoggingStart, o.LoggingStart.GetTypeCode()));
            arrParams[12] = new SqlParameter("@LoggingEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LoggingEnd, o.LoggingEnd.GetTypeCode()));
            arrParams[13] = new SqlParameter("@ParentalConsentFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentalConsentFlag, o.ParentalConsentFlag.GetTypeCode()));
            arrParams[14] = new SqlParameter("@ParentalConsentText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ParentalConsentText, o.ParentalConsentText.GetTypeCode()));
            arrParams[15] = new SqlParameter("@PatronReviewFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronReviewFlag, o.PatronReviewFlag.GetTypeCode()));
            arrParams[16] = new SqlParameter("@LogoutURL", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogoutURL, o.LogoutURL.GetTypeCode()));
            arrParams[17] = new SqlParameter("@ProgramGameID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgramGameID, o.ProgramGameID.GetTypeCode()));
            arrParams[18] = new SqlParameter("@HTML1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML1, o.HTML1.GetTypeCode()));
            arrParams[19] = new SqlParameter("@HTML2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML2, o.HTML2.GetTypeCode()));
            arrParams[20] = new SqlParameter("@HTML3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML3, o.HTML3.GetTypeCode()));
            arrParams[21] = new SqlParameter("@HTML4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML4, o.HTML4.GetTypeCode()));
            arrParams[22] = new SqlParameter("@HTML5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML5, o.HTML5.GetTypeCode()));
            arrParams[23] = new SqlParameter("@HTML6", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML6, o.HTML6.GetTypeCode()));
            arrParams[24] = new SqlParameter("@BannerImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BannerImage, o.BannerImage.GetTypeCode()));
            arrParams[25] = new SqlParameter("@RegistrationBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationBadgeID, o.RegistrationBadgeID.GetTypeCode()));
            arrParams[26] = new SqlParameter("@CompletionPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CompletionPoints, o.CompletionPoints.GetTypeCode()));
            arrParams[27] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[28] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[29] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[30] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));

            arrParams[31] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[32] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[33] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[34] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[35] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[36] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[37] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[38] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[39] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[40] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[41] = new SqlParameter("@PreTestID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PreTestID, o.PreTestID.GetTypeCode()));
            arrParams[42] = new SqlParameter("@PostTestID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PostTestID, o.PostTestID.GetTypeCode()));
            arrParams[43] = new SqlParameter("@PreTestMandatory", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PreTestMandatory, o.PreTestMandatory.GetTypeCode()));
            arrParams[44] = new SqlParameter("@PreTestEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PreTestEndDate, o.PreTestEndDate.GetTypeCode()));
            arrParams[45] = new SqlParameter("@PostTestStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PostTestStartDate, o.PostTestStartDate.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Programs_Update", arrParams);
            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        //public int Delete()
        //{

        //    return Delete(this);

        //}

        public static int Delete(int PID, int PatronProgram, int PrizeProgram, int OfferProgram, int BookListProgram)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[5];

            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@PatronProgram", PatronProgram);
            arrParams[2] = new SqlParameter("@PrizeProgram", PrizeProgram);
            arrParams[3] = new SqlParameter("@OfferProgram", OfferProgram);
            arrParams[4] = new SqlParameter("@BookListProgram", BookListProgram);

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Programs_Delete", arrParams);
                var fileName = (HttpContext.Current.Server.MapPath("~/Images/Banners/") + "\\" + PID.ToString() + ".png");
                if(File.Exists(fileName)) {
                    File.Delete(fileName);
                }
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Banners/") + "\\" + PID.ToString() + "@2x.png");
                if(File.Exists(fileName)) {
                    File.Delete(fileName);
                }
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Banners/") + "\\" + PID.ToString() + ".jpg");
                if(File.Exists(fileName)) {
                    File.Delete(fileName);
                }
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Banners/") + "\\" + PID.ToString() + "@2x.jpg");
                if(File.Exists(fileName)) {
                    File.Delete(fileName);
                }

                fileName = (HttpContext.Current.Server.MapPath("~/css/program/") + "\\" + PID.ToString() + ".css");
                File.Delete(fileName);
                 fileName = (HttpContext.Current.Server.MapPath("~/resources/") + "\\program." + PID.ToString() + "en-US.txt");
                File.Delete(fileName);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public static void MoveUp(int PID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Programs_MoveUp", arrParams);
        }

        public static void MoveDn(int PID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Programs_MoveDn", arrParams);
        }

        public bool IsOpen
        {
            get { 
                bool open = false;

                DateTime now = DateTime.Now;
                if (IsActive && StartDate <= now && EndDate >= now && LoggingStart <= now && LoggingEnd >= now)
                {
                    open = true;
                }
                return open;
            }
        }

        #endregion

    }//end class

}//end namespace

