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

[Serializable]    public class MGChooseAdv : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myCAID;
        private int myMGID;
        private bool myEnableMediumDifficulty;
        private bool myEnableHardDifficulty;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int CAID
        {
            get { return myCAID; }
            set { myCAID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public bool EnableMediumDifficulty
        {
            get { return myEnableMediumDifficulty; }
            set { myEnableMediumDifficulty = value; }
        }
        public bool EnableHardDifficulty
        {
            get { return myEnableHardDifficulty; }
            set { myEnableHardDifficulty = value; }
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

        public MGChooseAdv()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGChooseAdv_GetAll");
        }

        public static DataSet FetchWithParent(int MGID)
        {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGChooseAdv_GetByIDWithParent", arrParams);
        }

        public static MGChooseAdv FetchObjectByParent(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGChooseAdv_GetByMGID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGChooseAdv result = new MGChooseAdv();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CAID"].ToString(), out _int)) result.CAID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                result.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                result.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
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


        public static MGChooseAdv FetchObject(int CAID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CAID", CAID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGChooseAdv_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGChooseAdv result = new MGChooseAdv();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CAID"].ToString(), out _int)) result.CAID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                result.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                result.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
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

        public bool Fetch(int CAID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CAID", CAID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGChooseAdv_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGChooseAdv result = new MGChooseAdv();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CAID"].ToString(), out _int)) this.CAID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                this.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                this.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
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

        public static int Insert(MGChooseAdv o)
        {

            SqlParameter[] arrParams = new SqlParameter[8];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@CAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CAID, o.CAID.GetTypeCode()));
            arrParams[7].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdv_Insert", arrParams);

            o.CAID = int.Parse(arrParams[7].Value.ToString());

            return o.CAID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGChooseAdv o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[8];

            arrParams[0] = new SqlParameter("@CAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CAID, o.CAID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdv_Update", arrParams);

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

        public static int Delete(MGChooseAdv o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CAID, o.CAID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdv_Delete", arrParams);

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

