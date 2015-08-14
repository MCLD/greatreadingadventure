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

    public class SurveyResults : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int mySRID = 0;
        private int myTenID = 0;
        private int myPID = 0;
        private int mySID = 0;
        private DateTime myStartDate;
        private DateTime myEndDate;
        private bool myIsComplete = false;
        private bool myIsScorable = false;
        private int myLastAnswered = 0;
        private int myScore = 0;
        private decimal myScorePct = 0;
        private string mySource = "";
        private int mySourceID = 0;
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

        public int SRID
        {
            get { return mySRID; }
            set { mySRID = value; }
        }
        public int TenID
        {
            get { return myTenID; }
            set { myTenID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int SID
        {
            get { return mySID; }
            set { mySID = value; }
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
        public bool IsComplete
        {
            get { return myIsComplete; }
            set { myIsComplete = value; }
        }
        public bool IsScorable
        {
            get { return myIsScorable; }
            set { myIsScorable = value; }
        }
        public int LastAnswered
        {
            get { return myLastAnswered; }
            set { myLastAnswered = value; }
        }
        public int Score
        {
            get { return myScore; }
            set { myScore = value; }
        }
        public decimal ScorePct
        {
            get { return myScorePct; }
            set { myScorePct = value; }
        }
        public string Source
        {
            get { return mySource; }
            set { mySource = value; }
        }
        public int SourceID
        {
            get { return mySourceID; }
            set { mySourceID = value; }
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

        public SurveyResults()
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

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetAll", arrParams);
        }

        public static DataSet GetAllByPatron(int PID)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@PID", PID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetAllComplete", arrParams);
        }

        public static DataSet GetAllBySurvey(int SID)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@SID", SID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetAll", arrParams);
        }

        public static DataSet GetAll(int PID, int SID)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@PID", PID);
            arrParams[2] = new SqlParameter("@SID", SID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetAll", arrParams);
        }

        public static SurveyResults FetchObject(int SRID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SRID", SRID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyResults_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SurveyResults result = new SurveyResults();

                DateTime _datetime;

                int _int;

                decimal _decimal;

                if (int.TryParse(dr["SRID"].ToString(), out _int)) result.SRID = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                if (DateTime.TryParse(dr["StartDate"].ToString(), out _datetime)) result.StartDate = _datetime;
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) result.EndDate = _datetime;
                result.IsComplete = bool.Parse(dr["IsComplete"].ToString());
                result.IsScorable = bool.Parse(dr["IsScorable"].ToString());
                if (int.TryParse(dr["LastAnswered"].ToString(), out _int)) result.LastAnswered = _int;
                if (int.TryParse(dr["Score"].ToString(), out _int)) result.Score = _int;
                if (decimal.TryParse(dr["ScorePct"].ToString(), out _decimal)) result.ScorePct = _decimal;
                result.Source = dr["Source"].ToString();
                if (int.TryParse(dr["SourceID"].ToString(), out _int)) result.SourceID = _int;
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

        public static SurveyResults FetchObject(int PID, int SID, string SrcType, int SrcID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[4];

            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@SID", SID);
            arrParams[2] = new SqlParameter("@SrcType", SrcType);
            arrParams[3] = new SqlParameter("@SrcID", SrcID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyResults_GetBySurveyAndSource", arrParams);

            if (dr.Read())
            {

                // declare return value

                SurveyResults result = new SurveyResults();

                DateTime _datetime;

                int _int;

                decimal _decimal;

                if (int.TryParse(dr["SRID"].ToString(), out _int)) result.SRID = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                if (DateTime.TryParse(dr["StartDate"].ToString(), out _datetime)) result.StartDate = _datetime;
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) result.EndDate = _datetime;
                result.IsComplete = bool.Parse(dr["IsComplete"].ToString());
                result.IsScorable = bool.Parse(dr["IsScorable"].ToString());
                if (int.TryParse(dr["LastAnswered"].ToString(), out _int)) result.LastAnswered = _int;
                if (int.TryParse(dr["Score"].ToString(), out _int)) result.Score = _int;
                if (decimal.TryParse(dr["ScorePct"].ToString(), out _decimal)) result.ScorePct = _decimal;
                result.Source = dr["Source"].ToString();
                if (int.TryParse(dr["SourceID"].ToString(), out _int)) result.SourceID = _int;
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


        public bool Fetch(int SRID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SRID", SRID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyResults_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SurveyResults result = new SurveyResults();

                DateTime _datetime;

                int _int;

                decimal _decimal;

                if (int.TryParse(dr["SRID"].ToString(), out _int)) this.SRID = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) this.SID = _int;
                if (DateTime.TryParse(dr["StartDate"].ToString(), out _datetime)) this.StartDate = _datetime;
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) this.EndDate = _datetime;
                this.IsComplete = bool.Parse(dr["IsComplete"].ToString());
                this.IsScorable = bool.Parse(dr["IsScorable"].ToString());
                if (int.TryParse(dr["LastAnswered"].ToString(), out _int)) this.LastAnswered = _int;
                if (int.TryParse(dr["Score"].ToString(), out _int)) this.Score = _int;
                if (decimal.TryParse(dr["ScorePct"].ToString(), out _decimal)) this.ScorePct = _decimal;
                this.Source = dr["Source"].ToString();
                if (int.TryParse(dr["SourceID"].ToString(), out _int)) this.SourceID = _int;
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

        public static int Insert(SurveyResults o)
        {

            SqlParameter[] arrParams = new SqlParameter[22];

            arrParams[0] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@StartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StartDate, o.StartDate.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@IsComplete", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsComplete, o.IsComplete.GetTypeCode()));
            arrParams[6] = new SqlParameter("@IsScorable", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsScorable, o.IsScorable.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastAnswered", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastAnswered, o.LastAnswered.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Score", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score, o.Score.GetTypeCode()));
            arrParams[9] = new SqlParameter("@ScorePct", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ScorePct, o.ScorePct.GetTypeCode()));
            arrParams[10] = new SqlParameter("@Source", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Source, o.Source.GetTypeCode()));
            arrParams[11] = new SqlParameter("@SourceID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SourceID, o.SourceID.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[21] = new SqlParameter("@SRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SRID, o.SRID.GetTypeCode()));
            arrParams[21].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyResults_Insert", arrParams);

            o.SRID = int.Parse(arrParams[21].Value.ToString());

            return o.SRID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SurveyResults o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[22];

            arrParams[0] = new SqlParameter("@SRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SRID, o.SRID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@StartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StartDate, o.StartDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@EndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@IsComplete", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsComplete, o.IsComplete.GetTypeCode()));
            arrParams[7] = new SqlParameter("@IsScorable", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsScorable, o.IsScorable.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastAnswered", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastAnswered, o.LastAnswered.GetTypeCode()));
            arrParams[9] = new SqlParameter("@Score", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score, o.Score.GetTypeCode()));
            arrParams[10] = new SqlParameter("@ScorePct", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ScorePct, o.ScorePct.GetTypeCode()));
            arrParams[11] = new SqlParameter("@Source", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Source, o.Source.GetTypeCode()));
            arrParams[12] = new SqlParameter("@SourceID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SourceID, o.SourceID.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyResults_Update", arrParams);

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

        public static int Delete(SurveyResults o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SRID, o.SRID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyResults_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public static int PerformScoring(int SRID)
        {
            var score = 0;

            var a = SurveyAnswers.GetAll(SRID);

            foreach (DataRow ansRow in a.Tables[0].Rows)
            {
                var aIDs = Convert.ToString(ansRow["ChoiceAnswerIDs"]);
                if (aIDs != "")
                {
                    var c = SQChoices.GetList(aIDs);
                    foreach (DataRow cRow in c.Tables[0].Rows)
                    {
                        score += Convert.ToInt32(cRow["Score"]);
                    }
                }

            }

            return score;
        }

        public static DataSet GetStats(int SID, string SourceType, int SourceID, int SchoolID)
        {
            var arrParams = new SqlParameter[5];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@SID", SID);
            if (SourceType == "")
            {
                arrParams[2] = new SqlParameter("@SourceType", DBNull.Value);
                arrParams[3] = new SqlParameter("@SourceID", DBNull.Value);
            }

            else
            {
                arrParams[2] = new SqlParameter("@SourceType", SourceType);
                arrParams[3] = new SqlParameter("@SourceID", SourceID);
            }
            if (SchoolID <= 0)
            {
                arrParams[4] = new SqlParameter("@SchoolID", DBNull.Value);
            }
            else
            {
                arrParams[4] = new SqlParameter("@SchoolID", SchoolID);
            }
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetAllStats", arrParams);
        }


        public static DataSet GetExport(int SID, string SourceType, int SourceID, int SchoolID)
        {
            var arrParams = new SqlParameter[4];
            arrParams[0] = new SqlParameter("@SID", SID);
            if (SourceType == "")
            {
                arrParams[1] = new SqlParameter("@SourceType", DBNull.Value);
                arrParams[2] = new SqlParameter("@SourceID", DBNull.Value);
            }

            else
            {
                arrParams[1] = new SqlParameter("@SourceType", SourceType);
                arrParams[2] = new SqlParameter("@SourceID", SourceID);
            }
            if (SchoolID <= 0)
            {
                arrParams[3] = new SqlParameter("@SchoolID", DBNull.Value);
            }
            else
            {
                arrParams[3] = new SqlParameter("@SchoolID", SchoolID);
            }
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetExport", arrParams);
        }

        public static DataSet GetQStatsSimple(int SID, int QID, int SQMLID, string SourceType, int SourceID)
        {
            var arrParams = new SqlParameter[5];
            arrParams[0] = new SqlParameter("@SID", SID);
            arrParams[1] = new SqlParameter("@QID", QID);
            arrParams[2] = new SqlParameter("@SQMLID", SQMLID);
            if (SourceType == "")
            {
                arrParams[3] = new SqlParameter("@SourceType", DBNull.Value);
                arrParams[4] = new SqlParameter("@SourceID", DBNull.Value);
            }

            else
            {
                arrParams[3] = new SqlParameter("@SourceType", SourceType);
                arrParams[4] = new SqlParameter("@SourceID", SourceID);
            }

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetQStatsSimple", arrParams);
        }

        public static DataSet GetQStatsMedium(int SID, int QID, int SQMLID, string SourceType, int SourceID)
        {
            var arrParams = new SqlParameter[5];
            arrParams[0] = new SqlParameter("@SID", SID);
            arrParams[1] = new SqlParameter("@QID", QID);
            arrParams[2] = new SqlParameter("@SQMLID", SQMLID);
            if (SourceType == "")
            {
                arrParams[3] = new SqlParameter("@SourceType", DBNull.Value);
                arrParams[4] = new SqlParameter("@SourceID", DBNull.Value);
            }

            else
            {
                arrParams[3] = new SqlParameter("@SourceType", SourceType);
                arrParams[4] = new SqlParameter("@SourceID", SourceID);
            }
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetQStatsMedium", arrParams);
        }

        public static DataSet GetQClarifications(int SID, int QID, int SQMLID, string SourceType, int SourceID, string Answer)
        {
            var arrParams = new SqlParameter[6];
            arrParams[0] = new SqlParameter("@SID", SID);
            arrParams[1] = new SqlParameter("@QID", QID);
            arrParams[2] = new SqlParameter("@SQMLID", SQMLID);
            arrParams[3] = new SqlParameter("@Answer", Answer);
            if (SourceType == "")
            {
                arrParams[4] = new SqlParameter("@SourceType", DBNull.Value);
                arrParams[5] = new SqlParameter("@SourceID", DBNull.Value);
            }

            else
            {
                arrParams[4] = new SqlParameter("@SourceType", SourceType);
                arrParams[5] = new SqlParameter("@SourceID", SourceID);
            }
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetQClarifications", arrParams);
        }

        public static DataSet GetQFreeForm(int SID, int QID, int SQMLID, string SourceType, int SourceID)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@SID", SID);
            arrParams[1] = new SqlParameter("@QID", QID);
            arrParams[2] = new SqlParameter("@SQMLID", SQMLID);
            if (SourceType == "")
            {
                arrParams[3] = new SqlParameter("@SourceType", DBNull.Value);
                arrParams[4] = new SqlParameter("@SourceID", DBNull.Value);
            }

            else
            {
                arrParams[3] = new SqlParameter("@SourceType", SourceType);
                arrParams[4] = new SqlParameter("@SourceID", SourceID);
            }
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetQFreeForm", arrParams);
        }

        public static DataSet GetSources(int SID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SID", SID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyResults_GetSources", arrParams);
        }
        #endregion

    }//end class

}//end namespace

