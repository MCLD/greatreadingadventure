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

[Serializable]    public class PatronReview : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPRID;
        private int myPID;
        private int myPRLID;
        private string myAuthor = "";
        private string myTitle = "";
        private string myReview="";
        private bool myisApproved=false;
        private DateTime myReviewDate;
        private DateTime myApprovalDate;
        private string myApprovedBy="";

        #endregion

        #region Accessors

        public int PRID
        {
            get { return myPRID; }
            set { myPRID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int PRLID
        {
            get { return myPRLID; }
            set { myPRLID = value; }
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
        public string Review
        {
            get { return myReview; }
            set { myReview = value; }
        }
        public bool isApproved
        {
            get { return myisApproved; }
            set { myisApproved = value; }
        }
        public DateTime ReviewDate
        {
            get { return myReviewDate; }
            set { myReviewDate = value; }
        }
        public DateTime ApprovalDate
        {
            get { return myApprovalDate; }
            set { myApprovalDate = value; }
        }
        public string ApprovedBy
        {
            get { return myApprovedBy; }
            set { myApprovedBy = value; }
        }

        #endregion

        #region Constructors

        public PatronReview()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PID)
        {

            // declare reader

            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronReview_GetAll", arrParams);
        }

        public static PatronReview FetchObject(int PRID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRID", PRID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronReview_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronReview result = new PatronReview();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRID"].ToString(), out _int)) result.PRID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["PRLID"].ToString(), out _int)) result.PRLID = _int;
                result.Author = dr["Author"].ToString();
                result.Title = dr["Title"].ToString();
                result.Review = dr["Review"].ToString();
                result.isApproved = bool.Parse(dr["isApproved"].ToString());
                if (DateTime.TryParse(dr["ReviewDate"].ToString(), out _datetime)) result.ReviewDate = _datetime;
                if (DateTime.TryParse(dr["ApprovalDate"].ToString(), out _datetime)) result.ApprovalDate = _datetime;
                result.ApprovedBy = dr["ApprovedBy"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public static PatronReview FetchObjectByLogId(int PRLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRLID", PRLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronReview_GetByLogID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronReview result = new PatronReview();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRID"].ToString(), out _int)) result.PRID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["PRLID"].ToString(), out _int)) result.PRLID = _int;
                result.Author = dr["Author"].ToString();
                result.Title = dr["Title"].ToString();
                result.Review = dr["Review"].ToString();
                result.isApproved = bool.Parse(dr["isApproved"].ToString());
                if (DateTime.TryParse(dr["ReviewDate"].ToString(), out _datetime)) result.ReviewDate = _datetime;
                if (DateTime.TryParse(dr["ApprovalDate"].ToString(), out _datetime)) result.ApprovalDate = _datetime;
                result.ApprovedBy = dr["ApprovedBy"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }



        public bool Fetch(int PRID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRID", PRID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronReview_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronReview result = new PatronReview();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRID"].ToString(), out _int)) this.PRID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["PRLID"].ToString(), out _int)) this.PRLID = _int;
                this.Author = dr["Author"].ToString();
                this.Title = dr["Title"].ToString();
                this.Review = dr["Review"].ToString();
                this.isApproved = bool.Parse(dr["isApproved"].ToString());
                if (DateTime.TryParse(dr["ReviewDate"].ToString(), out _datetime)) this.ReviewDate = _datetime;
                if (DateTime.TryParse(dr["ApprovalDate"].ToString(), out _datetime)) this.ApprovalDate = _datetime;
                this.ApprovedBy = dr["ApprovedBy"].ToString();

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

        public static int Insert(PatronReview o)
        {

            SqlParameter[] arrParams = new SqlParameter[10];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PRLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRLID, o.PRLID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Author", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Author, o.Author.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Review", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Review, o.Review.GetTypeCode()));
            arrParams[5] = new SqlParameter("@isApproved", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isApproved, o.isApproved.GetTypeCode()));
            arrParams[6] = new SqlParameter("@ReviewDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDate, o.ReviewDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ApprovalDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ApprovalDate, o.ApprovalDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@ApprovedBy", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ApprovedBy, o.ApprovedBy.GetTypeCode()));
            arrParams[9] = new SqlParameter("@PRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRID, o.PRID.GetTypeCode()));
            arrParams[9].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronReview_Insert", arrParams);

            o.PRID = int.Parse(arrParams[9].Value.ToString());

            return o.PRID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronReview o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[10];

            arrParams[0] = new SqlParameter("@PRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRID, o.PRID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PRLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRLID, o.PRLID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Author", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Author, o.Author.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Review", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Review, o.Review.GetTypeCode()));
            arrParams[6] = new SqlParameter("@isApproved", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isApproved, o.isApproved.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ReviewDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDate, o.ReviewDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@ApprovalDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ApprovalDate, o.ApprovalDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@ApprovedBy", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ApprovedBy, o.ApprovedBy.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronReview_Update", arrParams);

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

        public static int Delete(PatronReview o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRID, o.PRID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronReview_Delete", arrParams);

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

