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

[Serializable]    public class PatronReadingLog : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPRLID;
        private int myPID;
        private int myReadingType;
        private string myReadingTypeLabel;
        private int myReadingAmount;
        private int myReadingPoints;
        private string myLoggingDate;
        private string myAuthor;
        private string myTitle;
        private bool myHasReview;
        private int myReviewID;

        #endregion

        #region Accessors

        public int PRLID
        {
            get { return myPRLID; }
            set { myPRLID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int ReadingType
        {
            get { return myReadingType; }
            set { myReadingType = value; }
        }
        public string ReadingTypeLabel
        {
            get { return myReadingTypeLabel; }
            set { myReadingTypeLabel = value; }
        }
        public int ReadingAmount
        {
            get { return myReadingAmount; }
            set { myReadingAmount = value; }
        }
        public int ReadingPoints
        {
            get { return myReadingPoints; }
            set { myReadingPoints = value; }
        }
        public string LoggingDate
        {
            get { return myLoggingDate; }
            set { myLoggingDate = value; }
        }
        public string Author
        {
            get { return myAuthor; }
            set { myAuthor = value; }
        }
        public string Title
        {
            get { return myTitle; }
            set { myTitle = value; }
        }
        public bool HasReview
        {
            get { return myHasReview; }
            set { myHasReview = value; }
        }
        public int ReviewID
        {
            get { return myReviewID; }
            set { myReviewID = value; }
        }

        #endregion

        #region Constructors

        public PatronReadingLog()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PID)
        {
            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronReadingLog_GetAll", arrParams);
        }

        public static PatronReadingLog FetchObject(int PRLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRLID", PRLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronReadingLog_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronReadingLog result = new PatronReadingLog();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRLID"].ToString(), out _int)) result.PRLID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["ReadingType"].ToString(), out _int)) result.ReadingType = _int;
                result.ReadingTypeLabel = dr["ReadingTypeLabel"].ToString();
                if (int.TryParse(dr["ReadingAmount"].ToString(), out _int)) result.ReadingAmount = _int;
                if (int.TryParse(dr["ReadingPoints"].ToString(), out _int)) result.ReadingPoints = _int;
                result.LoggingDate = dr["LoggingDate"].ToString();
                result.Author = dr["Author"].ToString();
                result.Title = dr["Title"].ToString();
                result.HasReview = bool.Parse(dr["HasReview"].ToString());
                if (int.TryParse(dr["ReviewID"].ToString(), out _int)) result.ReviewID = _int;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PRLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRLID", PRLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronReadingLog_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronReadingLog result = new PatronReadingLog();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRLID"].ToString(), out _int)) this.PRLID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["ReadingType"].ToString(), out _int)) this.ReadingType = _int;
                this.ReadingTypeLabel = dr["ReadingTypeLabel"].ToString();
                if (int.TryParse(dr["ReadingAmount"].ToString(), out _int)) this.ReadingAmount = _int;
                if (int.TryParse(dr["ReadingPoints"].ToString(), out _int)) this.ReadingPoints = _int;
                this.LoggingDate = dr["LoggingDate"].ToString();
                this.Author = dr["Author"].ToString();
                this.Title = dr["Title"].ToString();
                this.HasReview = bool.Parse(dr["HasReview"].ToString());
                if (int.TryParse(dr["ReviewID"].ToString(), out _int)) this.ReviewID = _int;

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

        public static int Insert(PatronReadingLog o)
        {

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@ReadingType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingType, o.ReadingType.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ReadingTypeLabel", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingTypeLabel, o.ReadingTypeLabel.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ReadingAmount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingAmount, o.ReadingAmount.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ReadingPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingPoints, o.ReadingPoints.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LoggingDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LoggingDate, o.LoggingDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Author", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Author, o.Author.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[8] = new SqlParameter("@HasReview", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasReview, o.HasReview.GetTypeCode()));
            arrParams[9] = new SqlParameter("@ReviewID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewID, o.ReviewID.GetTypeCode()));
            arrParams[10] = new SqlParameter("@PRLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRLID, o.PRLID.GetTypeCode()));
            arrParams[10].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronReadingLog_Insert", arrParams);

            o.PRLID = int.Parse(arrParams[10].Value.ToString());

            return o.PRLID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronReadingLog o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@PRLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRLID, o.PRLID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ReadingType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingType, o.ReadingType.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ReadingTypeLabel", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingTypeLabel, o.ReadingTypeLabel.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ReadingAmount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingAmount, o.ReadingAmount.GetTypeCode()));
            arrParams[5] = new SqlParameter("@ReadingPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReadingPoints, o.ReadingPoints.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LoggingDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LoggingDate, o.LoggingDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Author", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Author, o.Author.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[9] = new SqlParameter("@HasReview", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasReview, o.HasReview.GetTypeCode()));
            arrParams[10] = new SqlParameter("@ReviewID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewID, o.ReviewID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronReadingLog_Update", arrParams);

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

        public static int Delete(PatronReadingLog o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRLID, o.PRLID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronReadingLog_Delete", arrParams);

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

