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

[Serializable]    public class PatronRewardCodes : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPRCID;
        private int myPID;
        private int myBadgeID;
        private int myProgID;
        private string myRewardCode;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int PRCID
        {
            get { return myPRCID; }
            set { myPRCID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int BadgeID
        {
            get { return myBadgeID; }
            set { myBadgeID = value; }
        }
        public int ProgID
        {
            get { return myProgID; }
            set { myProgID = value; }
        }
        public string RewardCode
        {
            get { return myRewardCode; }
            set { myRewardCode = value; }
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

        #endregion

        #region Constructors

        public PatronRewardCodes()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronRewardCodes_GetAll", arrParams);
        }

        public static PatronRewardCodes FetchObject(int PRCID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRCID", PRCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronRewardCodes_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronRewardCodes result = new PatronRewardCodes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRCID"].ToString(), out _int)) result.PRCID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) result.ProgID = _int;
                result.RewardCode = dr["RewardCode"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PRCID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRCID", PRCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronRewardCodes_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronRewardCodes result = new PatronRewardCodes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PRCID"].ToString(), out _int)) this.PRCID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) this.BadgeID = _int;
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) this.ProgID = _int;
                this.RewardCode = dr["RewardCode"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

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

        public static int Insert(PatronRewardCodes o)
        {

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@RewardCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RewardCode, o.RewardCode.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PRCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRCID, o.PRCID.GetTypeCode()));
            arrParams[8].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronRewardCodes_Insert", arrParams);

            o.PRCID = int.Parse(arrParams[8].Value.ToString());

            return o.PRCID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronRewardCodes o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PRCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRCID, o.PRCID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@RewardCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RewardCode, o.RewardCode.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronRewardCodes_Update", arrParams);

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

        public static int Delete(PatronRewardCodes o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PRCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PRCID, o.PRCID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronRewardCodes_Delete", arrParams);

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

