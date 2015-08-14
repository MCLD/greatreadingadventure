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

[Serializable]    public class MGMatchingGame : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myMAGID;
        private int myMGID;
        private int myCorrectRoundsToWinCount;
        private int myEasyGameSize;
        private int myMediumGameSize;
        private int myHardGameSize;
        private bool myEnableMediumDifficulty;
        private bool myEnableHardDifficulty;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int MAGID
        {
            get { return myMAGID; }
            set { myMAGID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public int CorrectRoundsToWinCount
        {
            get { return myCorrectRoundsToWinCount; }
            set { myCorrectRoundsToWinCount = value; }
        }
        public int EasyGameSize
        {
            get { return myEasyGameSize; }
            set { myEasyGameSize = value; }
        }
        public int MediumGameSize
        {
            get { return myMediumGameSize; }
            set { myMediumGameSize = value; }
        }
        public int HardGameSize
        {
            get { return myHardGameSize; }
            set { myHardGameSize = value; }
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

        public MGMatchingGame()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGMatchingGame_GetAll");
        }

        public static DataSet GetRandomPlayItems(int MAGID, int NumItems, int Difficulty)
        {

            SqlParameter[] arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@MAGID", MAGID);
            arrParams[1] = new SqlParameter("@NumItems", NumItems);
            arrParams[2] = new SqlParameter("@Difficulty", Difficulty);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGMatchingGame_GetRandomPlayItems", arrParams);
        }



        public static DataSet FetchWithParent(int MGID)
        {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGMatchingGame_GetByIDWithParent", arrParams);
        }

        public static MGMatchingGame FetchObjectByParent(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMatchingGame_GetByMGID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMatchingGame result = new MGMatchingGame();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MAGID"].ToString(), out _int)) result.MAGID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) result.CorrectRoundsToWinCount = _int;
                if (int.TryParse(dr["EasyGameSize"].ToString(), out _int)) result.EasyGameSize = _int;
                if (int.TryParse(dr["MediumGameSize"].ToString(), out _int)) result.MediumGameSize = _int;
                if (int.TryParse(dr["HardGameSize"].ToString(), out _int)) result.HardGameSize = _int;
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


        public static MGMatchingGame FetchObject(int MAGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MAGID", MAGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMatchingGame_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMatchingGame result = new MGMatchingGame();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MAGID"].ToString(), out _int)) result.MAGID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) result.CorrectRoundsToWinCount = _int;
                if (int.TryParse(dr["EasyGameSize"].ToString(), out _int)) result.EasyGameSize = _int;
                if (int.TryParse(dr["MediumGameSize"].ToString(), out _int)) result.MediumGameSize = _int;
                if (int.TryParse(dr["HardGameSize"].ToString(), out _int)) result.HardGameSize = _int;
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

        public bool Fetch(int MAGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MAGID", MAGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMatchingGame_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMatchingGame result = new MGMatchingGame();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MAGID"].ToString(), out _int)) this.MAGID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) this.CorrectRoundsToWinCount = _int;
                if (int.TryParse(dr["EasyGameSize"].ToString(), out _int)) this.EasyGameSize = _int;
                if (int.TryParse(dr["MediumGameSize"].ToString(), out _int)) this.MediumGameSize = _int;
                if (int.TryParse(dr["HardGameSize"].ToString(), out _int)) this.HardGameSize = _int;
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

        public static int Insert(MGMatchingGame o)
        {

            SqlParameter[] arrParams = new SqlParameter[12];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@CorrectRoundsToWinCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CorrectRoundsToWinCount, o.CorrectRoundsToWinCount.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EasyGameSize", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EasyGameSize, o.EasyGameSize.GetTypeCode()));
            arrParams[3] = new SqlParameter("@MediumGameSize", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MediumGameSize, o.MediumGameSize.GetTypeCode()));
            arrParams[4] = new SqlParameter("@HardGameSize", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HardGameSize, o.HardGameSize.GetTypeCode()));
            arrParams[5] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[6] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[11] = new SqlParameter("@MAGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGID, o.MAGID.GetTypeCode()));
            arrParams[11].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMatchingGame_Insert", arrParams);

            o.MAGID = int.Parse(arrParams[11].Value.ToString());

            return o.MAGID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGMatchingGame o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[12];

            arrParams[0] = new SqlParameter("@MAGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGID, o.MAGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@CorrectRoundsToWinCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CorrectRoundsToWinCount, o.CorrectRoundsToWinCount.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EasyGameSize", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EasyGameSize, o.EasyGameSize.GetTypeCode()));
            arrParams[4] = new SqlParameter("@MediumGameSize", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MediumGameSize, o.MediumGameSize.GetTypeCode()));
            arrParams[5] = new SqlParameter("@HardGameSize", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HardGameSize, o.HardGameSize.GetTypeCode()));
            arrParams[6] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[7] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMatchingGame_Update", arrParams);

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

        public static int Delete(MGMatchingGame o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MAGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGID, o.MAGID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMatchingGame_Delete", arrParams);

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

