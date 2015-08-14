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

[Serializable]    public class PrizeDrawingWinners : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPDWID;
        private int myPDID;
        private int myPatronID;
        private int myNotificationID;
        private bool myPrizePickedUpFlag;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int PDWID
        {
            get { return myPDWID; }
            set { myPDWID = value; }
        }
        public int PDID
        {
            get { return myPDID; }
            set { myPDID = value; }
        }
        public int PatronID
        {
            get { return myPatronID; }
            set { myPatronID = value; }
        }
        public int NotificationID
        {
            get { return myNotificationID; }
            set { myNotificationID = value; }
        }
        public bool PrizePickedUpFlag
        {
            get { return myPrizePickedUpFlag; }
            set { myPrizePickedUpFlag = value; }
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

        public PrizeDrawingWinners()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PrizeDrawingWinners_GetAll");
        }

        public static PrizeDrawingWinners FetchObject(int PDWID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PDWID", PDWID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PrizeDrawingWinners_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PrizeDrawingWinners result = new PrizeDrawingWinners();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PDWID"].ToString(), out _int)) result.PDWID = _int;
                if (int.TryParse(dr["PDID"].ToString(), out _int)) result.PDID = _int;
                if (int.TryParse(dr["PatronID"].ToString(), out _int)) result.PatronID = _int;
                if (int.TryParse(dr["NotificationID"].ToString(), out _int)) result.NotificationID = _int;
                result.PrizePickedUpFlag = bool.Parse(dr["PrizePickedUpFlag"].ToString());
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

        public bool Fetch(int PDWID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PDWID", PDWID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PrizeDrawingWinners_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PrizeDrawingWinners result = new PrizeDrawingWinners();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PDWID"].ToString(), out _int)) this.PDWID = _int;
                if (int.TryParse(dr["PDID"].ToString(), out _int)) this.PDID = _int;
                if (int.TryParse(dr["PatronID"].ToString(), out _int)) this.PatronID = _int;
                if (int.TryParse(dr["NotificationID"].ToString(), out _int)) this.NotificationID = _int;
                this.PrizePickedUpFlag = bool.Parse(dr["PrizePickedUpFlag"].ToString());
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

        public static int Insert(PrizeDrawingWinners o)
        {

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PDID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDID, o.PDID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PatronID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronID, o.PatronID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@NotificationID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationID, o.NotificationID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@PrizePickedUpFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizePickedUpFlag, o.PrizePickedUpFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PDWID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDWID, o.PDWID.GetTypeCode()));
            arrParams[8].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeDrawingWinners_Insert", arrParams);

            o.PDWID = int.Parse(arrParams[8].Value.ToString());

            return o.PDWID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PrizeDrawingWinners o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PDWID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDWID, o.PDWID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PDID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDID, o.PDID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PatronID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronID, o.PatronID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@NotificationID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationID, o.NotificationID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@PrizePickedUpFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizePickedUpFlag, o.PrizePickedUpFlag.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeDrawingWinners_Update", arrParams);

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

        public static int Delete(PrizeDrawingWinners o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PDWID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDWID, o.PDWID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeDrawingWinners_Delete", arrParams);

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

