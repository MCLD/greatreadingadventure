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

[Serializable]    public class PatronPrizes : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPPID;
        private int myPID;
        private int myPrizeSource;
        private int myBadgeID;
        private int myDrawingID;
        private string myPrizeName;
        private bool myRedeemedFlag;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

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
        public int PrizeSource
        {
            get { return myPrizeSource; }
            set { myPrizeSource = value; }
        }
        public int BadgeID
        {
            get { return myBadgeID; }
            set { myBadgeID = value; }
        }
        public int DrawingID
        {
            get { return myDrawingID; }
            set { myDrawingID = value; }
        }
        public string PrizeName
        {
            get { return myPrizeName; }
            set { myPrizeName = value; }
        }
        public bool RedeemedFlag
        {
            get { return myRedeemedFlag; }
            set { myRedeemedFlag = value; }
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

        public PatronPrizes()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronPrizes_GetAll", arrParams);
        }

        public static PatronPrizes FetchObjectByDrawing(int DrawingID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@DrawingID", DrawingID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronPrizes_GetByDrawingID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronPrizes result = new PatronPrizes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PPID"].ToString(), out _int)) result.PPID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["PrizeSource"].ToString(), out _int)) result.PrizeSource = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["DrawingID"].ToString(), out _int)) result.DrawingID = _int;
                result.PrizeName = dr["PrizeName"].ToString();
                result.RedeemedFlag = bool.Parse(dr["RedeemedFlag"].ToString());
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

        public static PatronPrizes FetchObject(int PPID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PPID", PPID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronPrizes_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronPrizes result = new PatronPrizes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PPID"].ToString(), out _int)) result.PPID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["PrizeSource"].ToString(), out _int)) result.PrizeSource = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["DrawingID"].ToString(), out _int)) result.DrawingID = _int;
                result.PrizeName = dr["PrizeName"].ToString();
                result.RedeemedFlag = bool.Parse(dr["RedeemedFlag"].ToString());
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

        public bool Fetch(int PPID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PPID", PPID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronPrizes_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronPrizes result = new PatronPrizes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PPID"].ToString(), out _int)) this.PPID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["PrizeSource"].ToString(), out _int)) this.PrizeSource = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) this.BadgeID = _int;
                if (int.TryParse(dr["DrawingID"].ToString(), out _int)) this.DrawingID = _int;
                this.PrizeName = dr["PrizeName"].ToString();
                this.RedeemedFlag = bool.Parse(dr["RedeemedFlag"].ToString());
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

        public static int Insert(PatronPrizes o)
        {

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PrizeSource", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizeSource, o.PrizeSource.GetTypeCode()));
            arrParams[2] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@DrawingID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DrawingID, o.DrawingID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@PrizeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizeName, o.PrizeName.GetTypeCode()));
            arrParams[5] = new SqlParameter("@RedeemedFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RedeemedFlag, o.RedeemedFlag.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@PPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PPID, o.PPID.GetTypeCode()));
            arrParams[10].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronPrizes_Insert", arrParams);

            o.PPID = int.Parse(arrParams[10].Value.ToString());

            return o.PPID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronPrizes o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@PPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PPID, o.PPID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PrizeSource", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizeSource, o.PrizeSource.GetTypeCode()));
            arrParams[3] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@DrawingID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DrawingID, o.DrawingID.GetTypeCode()));
            arrParams[5] = new SqlParameter("@PrizeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizeName, o.PrizeName.GetTypeCode()));
            arrParams[6] = new SqlParameter("@RedeemedFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RedeemedFlag, o.RedeemedFlag.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronPrizes_Update", arrParams);

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

        public static int Delete(PatronPrizes o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PPID, o.PPID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronPrizes_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

    }//end class


    public enum PrizeSource
    {
        Drawing = 0,
        Badge   = 1,
        Admin   = 2
    }
}//end namespace

