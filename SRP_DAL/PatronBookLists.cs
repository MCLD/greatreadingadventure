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

[Serializable]    public class PatronBookLists : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPBLBID;
        private int myPID;
        private int myBLBID;
        private int myBLID;
        private bool myHasReadFlag;
        private DateTime myLastModDate;

        #endregion

        #region Accessors

        public int PBLBID
        {
            get { return myPBLBID; }
            set { myPBLBID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int BLBID
        {
            get { return myBLBID; }
            set { myBLBID = value; }
        }
        public int BLID
        {
            get { return myBLID; }
            set { myBLID = value; }
        }
        public bool HasReadFlag
        {
            get { return myHasReadFlag; }
            set { myHasReadFlag = value; }
        }
        public DateTime LastModDate
        {
            get { return myLastModDate; }
            set { myLastModDate = value; }
        }

        #endregion

        #region Constructors

        public PatronBookLists()
        {
        }

        #endregion

        #region stored procedure wrappers



        public static PatronBookLists FetchObject(int PBLBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PBLBID", PBLBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronBookLists_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronBookLists result = new PatronBookLists();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PBLBID"].ToString(), out _int)) result.PBLBID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["BLBID"].ToString(), out _int)) result.BLBID = _int;
                if (int.TryParse(dr["BLID"].ToString(), out _int)) result.BLID = _int;
                result.HasReadFlag = bool.Parse(dr["HasReadFlag"].ToString());
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PBLBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PBLBID", PBLBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronBookLists_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronBookLists result = new PatronBookLists();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PBLBID"].ToString(), out _int)) this.PBLBID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["BLBID"].ToString(), out _int)) this.BLBID = _int;
                if (int.TryParse(dr["BLID"].ToString(), out _int)) this.BLID = _int;
                this.HasReadFlag = bool.Parse(dr["HasReadFlag"].ToString());
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;

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

        public static int Insert(PatronBookLists o)
        {

            SqlParameter[] arrParams = new SqlParameter[6];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@BLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLBID, o.BLBID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@HasReadFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasReadFlag, o.HasReadFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@PBLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBLBID, o.PBLBID.GetTypeCode()));
            arrParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronBookLists_Insert", arrParams);

            o.PBLBID = int.Parse(arrParams[5].Value.ToString());

            return o.PBLBID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronBookLists o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[6];

            arrParams[0] = new SqlParameter("@PBLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBLBID, o.PBLBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@BLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLBID, o.BLBID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@HasReadFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasReadFlag, o.HasReadFlag.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronBookLists_Update", arrParams);

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

        public static int Delete(PatronBookLists o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PBLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBLBID, o.PBLBID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronBookLists_Delete", arrParams);

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

