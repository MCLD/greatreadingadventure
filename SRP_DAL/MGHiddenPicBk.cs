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

[Serializable]    public class MGHiddenPicBk : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myHPBID;
        private int myHPID;
        private int myMGID;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int HPBID
        {
            get { return myHPBID; }
            set { myHPBID = value; }
        }
        public int HPID
        {
            get { return myHPID; }
            set { myHPID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
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

        public MGHiddenPicBk()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int MGID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGHiddenPicBk_GetAll", arrParams);
        }

        public static MGHiddenPicBk FetchObject(int HPBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@HPBID", HPBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGHiddenPicBk_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGHiddenPicBk result = new MGHiddenPicBk();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["HPBID"].ToString(), out _int)) result.HPBID = _int;
                if (int.TryParse(dr["HPID"].ToString(), out _int)) result.HPID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
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

        public bool Fetch(int HPBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@HPBID", HPBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGHiddenPicBk_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGHiddenPicBk result = new MGHiddenPicBk();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["HPBID"].ToString(), out _int)) this.HPBID = _int;
                if (int.TryParse(dr["HPID"].ToString(), out _int)) this.HPID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
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

        public static int Insert(MGHiddenPicBk o)
        {

            SqlParameter[] arrParams = new SqlParameter[7];

            arrParams[0] = new SqlParameter("@HPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPID, o.HPID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[4] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@HPBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPBID, o.HPBID.GetTypeCode()));
            arrParams[6].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPicBk_Insert", arrParams);

            o.HPBID = int.Parse(arrParams[6].Value.ToString());

            return o.HPBID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGHiddenPicBk o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[7];

            arrParams[0] = new SqlParameter("@HPBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPBID, o.HPBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@HPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPID, o.HPID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPicBk_Update", arrParams);

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

        public static int Delete(MGHiddenPicBk o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@HPBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPBID, o.HPBID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPicBk_Delete", arrParams);

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

