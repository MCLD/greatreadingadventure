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

[Serializable]    public class MGHiddenPic : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myHPID;
        private int myMGID;
        private bool myEnableMediumDifficulty;
        private bool myEnableHardDifficulty;
        private string myEasyDictionary="";
        private string myMediumDictionary = "";
        private string myHardDictionary = "";
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

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
        public string EasyDictionary
        {
            get { return myEasyDictionary; }
            set { myEasyDictionary = value; }
        }
        public string MediumDictionary
        {
            get { return myMediumDictionary; }
            set { myMediumDictionary = value; }
        }
        public string HardDictionary
        {
            get { return myHardDictionary; }
            set { myHardDictionary = value; }
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

        public MGHiddenPic()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGHiddenPic_GetAll");
        }

        public static DataSet FetchWithParent(int MGID)
        {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGHiddenPic_GetByIDWithParent", arrParams);
        }

        public static MGHiddenPic FetchObjectByParent(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGHiddenPic_GetByMGID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGHiddenPic result = new MGHiddenPic();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["HPID"].ToString(), out _int)) result.HPID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                result.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                result.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
                result.EasyDictionary = dr["EasyDictionary"].ToString();
                result.MediumDictionary = dr["MediumDictionary"].ToString();
                result.HardDictionary = dr["HardDictionary"].ToString();
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


        public static MGHiddenPic FetchObject(int HPID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@HPID", HPID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGHiddenPic_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGHiddenPic result = new MGHiddenPic();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["HPID"].ToString(), out _int)) result.HPID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                result.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                result.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
                result.EasyDictionary = dr["EasyDictionary"].ToString();
                result.MediumDictionary = dr["MediumDictionary"].ToString();
                result.HardDictionary = dr["HardDictionary"].ToString();
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

        public bool Fetch(int HPID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@HPID", HPID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGHiddenPic_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGHiddenPic result = new MGHiddenPic();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["HPID"].ToString(), out _int)) this.HPID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                this.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                this.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
                this.EasyDictionary = dr["EasyDictionary"].ToString();
                this.MediumDictionary = dr["MediumDictionary"].ToString();
                this.HardDictionary = dr["HardDictionary"].ToString();
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

        public static int Insert(MGHiddenPic o)
        {

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EasyDictionary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EasyDictionary, o.EasyDictionary.GetTypeCode()));
            arrParams[4] = new SqlParameter("@MediumDictionary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MediumDictionary, o.MediumDictionary.GetTypeCode()));
            arrParams[5] = new SqlParameter("@HardDictionary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HardDictionary, o.HardDictionary.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@HPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPID, o.HPID.GetTypeCode()));
            arrParams[10].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPic_Insert", arrParams);

            o.HPID = int.Parse(arrParams[10].Value.ToString());

            return o.HPID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGHiddenPic o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@HPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPID, o.HPID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EasyDictionary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EasyDictionary, o.EasyDictionary.GetTypeCode()));
            arrParams[5] = new SqlParameter("@MediumDictionary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MediumDictionary, o.MediumDictionary.GetTypeCode()));
            arrParams[6] = new SqlParameter("@HardDictionary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HardDictionary, o.HardDictionary.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPic_Update", arrParams);

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

        public static int Delete(MGHiddenPic o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@HPID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HPID, o.HPID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPic_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public static int GetRandomBK(int pHPID)
        {

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@HPID", pHPID);
            arrParams[1] = new SqlParameter("@HPBID", -1);
            arrParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGHiddenPic_GetRandomBK", arrParams);

            return int.Parse(arrParams[1].Value.ToString());
        }


        #endregion

    }//end class

}//end namespace

