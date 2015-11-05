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

[Serializable]    public class PatronPoints : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPPID;
        private int myPID;
        private int myNumPoints;
        private DateTime myAwardDate;
        private string myAwardReason;
        private int myAwardReasonCd;
        private bool myBadgeAwardedFlag;
        private int myBadgeID;
        private int myPBID;
        private bool myisReading;
        private int myLogID;
        private bool myisEvent;
        private int myEventID;
        private string myEventCode="";
        private bool myisBookList;
        private int myBookListID;
        private bool myisGame;
        private bool myisGameLevelActivity;
        private int myGameID;
        private int myGameLevel;
        private int myGameLevelID;
        private int myGameLevelActivityID;

        #endregion

        #region Accessors

        public int PPID
        {
            get { return myPPID; }
            set { myPPID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int NumPoints
        {
            get { return myNumPoints; }
            set { myNumPoints = value; }
        }
        public DateTime AwardDate
        {
            get { return myAwardDate; }
            set { myAwardDate = value; }
        }
        public string AwardReason
        {
            get { return myAwardReason; }
            set { myAwardReason = value; }
        }
        public int AwardReasonCd
        {
            get { return myAwardReasonCd; }
            set { myAwardReasonCd = value; }
        }
        public bool BadgeAwardedFlag
        {
            get { return myBadgeAwardedFlag; }
            set { myBadgeAwardedFlag = value; }
        }
        public int BadgeID
        {
            get { return myBadgeID; }
            set { myBadgeID = value; }
        }
        public int PBID
        {
            get { return myPBID; }
            set { myPBID = value; }
        }
        public bool isReading
        {
            get { return myisReading; }
            set { myisReading = value; }
        }
        public int LogID
        {
            get { return myLogID; }
            set { myLogID = value; }
        }
        public bool isEvent
        {
            get { return myisEvent; }
            set { myisEvent = value; }
        }
        public int EventID
        {
            get { return myEventID; }
            set { myEventID = value; }
        }
        public string EventCode
        {
            get { return myEventCode; }
            set { myEventCode = value; }
        }
        public bool isBookList
        {
            get { return myisBookList; }
            set { myisBookList = value; }
        }
        public int BookListID
        {
            get { return myBookListID; }
            set { myBookListID = value; }
        }
        public bool isGame
        {
            get { return myisGame; }
            set { myisGame = value; }
        }
        public bool isGameLevelActivity
        {
            get { return myisGameLevelActivity; }
            set { myisGameLevelActivity = value; }
        }
        public int GameID
        {
            get { return myGameID; }
            set { myGameID = value; }
        }
        public int GameLevel
        {
            get { return myGameLevel; }
            set { myGameLevel = value; }
        }
        public int GameLevelID
        {
            get { return myGameLevelID; }
            set { myGameLevelID = value; }
        }
        public int GameLevelActivityID
        {
            get { return myGameLevelActivityID; }
            set { myGameLevelActivityID = value; }
        }

        #endregion

        #region Constructors

        public PatronPoints()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetAll", arrParams);
        }

        public static int GetLastPatronEntryID(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetLastPatronEntryID", arrParams);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }
        

        public static int GetTotalPatronPoints(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return Convert.ToInt32(SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetTotalPatronPoints", arrParams).Tables[0].Rows[0][0]);
        }

        public static int GetTotalPatronPoints(int PID, DateTime now)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@Date", now);
            return Convert.ToInt32(SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetTotalPatronPointsOnDate", arrParams).Tables[0].Rows[0][0]);
        }

        public static bool HasRedeemedKeywordPoints(int PID, string key)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@Key", key);

            return (SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetPatronPointsByKeyword", arrParams).Tables[0].Rows.Count > 0);
        }

        public static bool HasEarnedMinigamePoints(int PID, int MGID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@MGID", MGID);

            return (SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetPatronPointsByMGID", arrParams).Tables[0].Rows.Count > 0);
        }

        public static bool HasEarnedBookList(int PID, int BLID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@BLID", BLID);

            return (SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPoints_GetPatronPointsBookList", arrParams).Tables[0].Rows.Count > 0);
        }

        public static PatronPoints FetchObject(int PPID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PPID", PPID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronPoints_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronPoints result = new PatronPoints();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PPID"].ToString(), out _int)) result.PPID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["NumPoints"].ToString(), out _int)) result.NumPoints = _int;
                if (DateTime.TryParse(dr["AwardDate"].ToString(), out _datetime)) result.AwardDate = _datetime;
                result.AwardReason = dr["AwardReason"].ToString();
                if (int.TryParse(dr["AwardReasonCd"].ToString(), out _int)) result.AwardReasonCd = _int;
                result.BadgeAwardedFlag = bool.Parse(dr["BadgeAwardedFlag"].ToString());
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["PBID"].ToString(), out _int)) result.PBID = _int;
                result.isReading = bool.Parse(dr["isReading"].ToString());
                if (int.TryParse(dr["LogID"].ToString(), out _int)) result.LogID = _int;
                result.isEvent = bool.Parse(dr["isEvent"].ToString());
                if (int.TryParse(dr["EventID"].ToString(), out _int)) result.EventID = _int;
                result.EventCode = dr["EventCode"].ToString();
                result.isBookList = bool.Parse(dr["isBookList"].ToString());
                if (int.TryParse(dr["BookListID"].ToString(), out _int)) result.BookListID = _int;
                result.isGame = bool.Parse(dr["isGame"].ToString());
                result.isGameLevelActivity = bool.Parse(dr["isGameLevelActivity"].ToString());
                if (int.TryParse(dr["GameID"].ToString(), out _int)) result.GameID = _int;
                if (int.TryParse(dr["GameLevel"].ToString(), out _int)) result.GameLevel = _int;
                if (int.TryParse(dr["GameLevelID"].ToString(), out _int)) result.GameLevelID = _int;
                if (int.TryParse(dr["GameLevelActivityID"].ToString(), out _int)) result.GameLevelActivityID = _int;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PPID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PPID", PPID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronPoints_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronPoints result = new PatronPoints();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PPID"].ToString(), out _int)) this.PPID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["NumPoints"].ToString(), out _int)) this.NumPoints = _int;
                if (DateTime.TryParse(dr["AwardDate"].ToString(), out _datetime)) this.AwardDate = _datetime;
                this.AwardReason = dr["AwardReason"].ToString();
                if (int.TryParse(dr["AwardReasonCd"].ToString(), out _int)) this.AwardReasonCd = _int;
                this.BadgeAwardedFlag = bool.Parse(dr["BadgeAwardedFlag"].ToString());
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) this.BadgeID = _int;
                if (int.TryParse(dr["PBID"].ToString(), out _int)) this.PBID = _int;
                this.isReading = bool.Parse(dr["isReading"].ToString());
                if (int.TryParse(dr["LogID"].ToString(), out _int)) this.LogID = _int;
                this.isEvent = bool.Parse(dr["isEvent"].ToString());
                if (int.TryParse(dr["EventID"].ToString(), out _int)) this.EventID = _int;
                this.EventCode = dr["EventCode"].ToString();
                this.isBookList = bool.Parse(dr["isBookList"].ToString());
                if (int.TryParse(dr["BookListID"].ToString(), out _int)) this.BookListID = _int;
                this.isGame = bool.Parse(dr["isGame"].ToString());
                this.isGameLevelActivity = bool.Parse(dr["isGameLevelActivity"].ToString());
                if (int.TryParse(dr["GameID"].ToString(), out _int)) this.GameID = _int;
                if (int.TryParse(dr["GameLevel"].ToString(), out _int)) this.GameLevel = _int;
                if (int.TryParse(dr["GameLevelID"].ToString(), out _int)) this.GameLevelID = _int;
                if (int.TryParse(dr["GameLevelActivityID"].ToString(), out _int)) this.GameLevelActivityID = _int;

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

        public static int Insert(PatronPoints o)
        {

            SqlParameter[] arrParams = new SqlParameter[22];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@NumPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumPoints, o.NumPoints.GetTypeCode()));
            arrParams[2] = new SqlParameter("@AwardDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardDate, o.AwardDate.GetTypeCode()));
            arrParams[3] = new SqlParameter("@AwardReason", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardReason, o.AwardReason.GetTypeCode()));
            arrParams[4] = new SqlParameter("@AwardReasonCd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardReasonCd, o.AwardReasonCd.GetTypeCode()));
            arrParams[5] = new SqlParameter("@BadgeAwardedFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeAwardedFlag, o.BadgeAwardedFlag.GetTypeCode()));
            arrParams[6] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@PBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBID, o.PBID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@isReading", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isReading, o.isReading.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LogID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogID, o.LogID.GetTypeCode()));
            arrParams[10] = new SqlParameter("@isEvent", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isEvent, o.isEvent.GetTypeCode()));
            arrParams[11] = new SqlParameter("@EventID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventID, o.EventID.GetTypeCode()));
            arrParams[12] = new SqlParameter("@EventCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventCode, o.EventCode.GetTypeCode()));
            arrParams[13] = new SqlParameter("@isBookList", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isBookList, o.isBookList.GetTypeCode()));
            arrParams[14] = new SqlParameter("@BookListID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BookListID, o.BookListID.GetTypeCode()));
            arrParams[15] = new SqlParameter("@isGame", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isGame, o.isGame.GetTypeCode()));
            arrParams[16] = new SqlParameter("@isGameLevelActivity", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isGameLevelActivity, o.isGameLevelActivity.GetTypeCode()));
            arrParams[17] = new SqlParameter("@GameID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameID, o.GameID.GetTypeCode()));
            arrParams[18] = new SqlParameter("@GameLevel", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameLevel, o.GameLevel.GetTypeCode()));
            arrParams[19] = new SqlParameter("@GameLevelID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameLevelID, o.GameLevelID.GetTypeCode()));
            arrParams[20] = new SqlParameter("@GameLevelActivityID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameLevelActivityID, o.GameLevelActivityID.GetTypeCode()));
            arrParams[21] = new SqlParameter("@PPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PPID, o.PPID.GetTypeCode()));
            arrParams[21].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronPoints_Insert", arrParams);

            o.PPID = int.Parse(arrParams[21].Value.ToString());

            return o.PPID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronPoints o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[22];

            arrParams[0] = new SqlParameter("@PPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PPID, o.PPID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@NumPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumPoints, o.NumPoints.GetTypeCode()));
            arrParams[3] = new SqlParameter("@AwardDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardDate, o.AwardDate.GetTypeCode()));
            arrParams[4] = new SqlParameter("@AwardReason", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardReason, o.AwardReason.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AwardReasonCd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardReasonCd, o.AwardReasonCd.GetTypeCode()));
            arrParams[6] = new SqlParameter("@BadgeAwardedFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeAwardedFlag, o.BadgeAwardedFlag.GetTypeCode()));
            arrParams[7] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBID, o.PBID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@isReading", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isReading, o.isReading.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LogID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogID, o.LogID.GetTypeCode()));
            arrParams[11] = new SqlParameter("@isEvent", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isEvent, o.isEvent.GetTypeCode()));
            arrParams[12] = new SqlParameter("@EventID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventID, o.EventID.GetTypeCode()));
            arrParams[13] = new SqlParameter("@EventCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventCode, o.EventCode.GetTypeCode()));
            arrParams[14] = new SqlParameter("@isBookList", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isBookList, o.isBookList.GetTypeCode()));
            arrParams[15] = new SqlParameter("@BookListID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BookListID, o.BookListID.GetTypeCode()));
            arrParams[16] = new SqlParameter("@isGame", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isGame, o.isGame.GetTypeCode()));
            arrParams[17] = new SqlParameter("@isGameLevelActivity", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isGameLevelActivity, o.isGameLevelActivity.GetTypeCode()));
            arrParams[18] = new SqlParameter("@GameID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameID, o.GameID.GetTypeCode()));
            arrParams[19] = new SqlParameter("@GameLevel", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameLevel, o.GameLevel.GetTypeCode()));
            arrParams[20] = new SqlParameter("@GameLevelID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameLevelID, o.GameLevelID.GetTypeCode()));
            arrParams[21] = new SqlParameter("@GameLevelActivityID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameLevelActivityID, o.GameLevelActivityID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronPoints_Update", arrParams);

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

        public static int Delete(PatronPoints o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PPID, o.PPID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronPoints_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

        public static string PointAwardReasonCdToDescription(PointAwardReason ar)
        {
            switch (ar)
            {
                case PointAwardReason.Reading:
                    return "Reading";
                case PointAwardReason.EventAttendance:
                    return "Attended Event ";
                case PointAwardReason.BookListCompletion:
                    return "Completed a Challenge";
                case PointAwardReason.GameCompletion:
                    return "Completed an Adventure";
                case PointAwardReason.MiniGameCompletion:
                    return "Completed an Adventure";
                case PointAwardReason.Other:
                    return "Other";
                default:
                    return "Unknown";

            }
        }

    }//end class


    public enum PointAwardReason
    {
         Reading = 0
        ,EventAttendance = 1
        ,BookListCompletion = 2
        ,GameCompletion = 3
        ,MiniGameCompletion = 4
        ,Other = 99
    }

}//end namespace

