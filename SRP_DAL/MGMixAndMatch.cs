using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

[Serializable]    public class MGMixAndMatch : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myMMID;
        private int myMGID;
        private int myCorrectRoundsToWinCount = 1;
        private bool myEnableMediumDifficulty = false;
        private bool myEnableHardDifficulty = false;
        private DateTime myLastModDate;
        private string myLastModUser = "N/A";
        private DateTime myAddedDate;
        private string myAddedUser = "N/A";

        #endregion

        #region Accessors

        public int MMID
        {
            get { return myMMID; }
            set { myMMID = value; }
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

        public MGMixAndMatch()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_GetAll");
        }

        public static MGMixAndMatch FetchObject(int MMID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MMID", MMID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMixAndMatch result = new MGMixAndMatch();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MMID"].ToString(), out _int)) result.MMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) result.CorrectRoundsToWinCount = _int;
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


        public static void Get3RandomItems(int MMID, out MGMixAndMatchItems i1, out MGMixAndMatchItems i2, out MGMixAndMatchItems i3 )
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@MMID", MMID);

            var dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMixAndMatchItems_GetRandom3", arrParams);

            DateTime _datetime;
            int _int;
            //decimal _decimal;

            i1 = i2 = i3 = null;
            for (var i = 0; i < 3 ; i++ ) 
            {
                if (!dr.Read()) continue;

                var result = new MGMixAndMatchItems();

                if (int.TryParse(dr["MMIID"].ToString(), out _int)) result.MMIID = _int;
                if (int.TryParse(dr["MMID"].ToString(), out _int)) result.MMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                result.ItemImage = dr["ItemImage"].ToString();
                result.EasyLabel = dr["EasyLabel"].ToString();
                result.MediumLabel = dr["MediumLabel"].ToString();
                result.HardLabel = dr["HardLabel"].ToString();
                result.AudioEasy = dr["AudioEasy"].ToString();
                result.AudioMedium = dr["AudioMedium"].ToString();
                result.AudioHard = dr["AudioHard"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                if (i == 0) i1 = result;
                if (i == 1) i2 = result;
                if (i == 2) i3 = result;
            }
            dr.Close();

        }


        public static DataSet FetchWithParent(int MGID)
        {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_GetByIDWithParent", arrParams);
        }

        public static MGMixAndMatch FetchObjectByParent(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_GetByMGID", arrParams);

            if (dr.Read())
            {

                // declare return value

                var result = new MGMixAndMatch();

                DateTime _datetime;

                int _int;

                if (int.TryParse(dr["MMID"].ToString(), out _int)) result.MMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) result.CorrectRoundsToWinCount = _int;
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


        public bool Fetch(int MMID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MMID", MMID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMixAndMatch result = new MGMixAndMatch();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MMID"].ToString(), out _int)) this.MMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) this.CorrectRoundsToWinCount = _int;
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

        public static int Insert(MGMixAndMatch o)
        {

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@CorrectRoundsToWinCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CorrectRoundsToWinCount, o.CorrectRoundsToWinCount.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@MMID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MMID, o.MMID.GetTypeCode()));
            arrParams[8].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_Insert", arrParams);

            o.MMID = int.Parse(arrParams[8].Value.ToString());

            return o.MMID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGMixAndMatch o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@MMID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MMID, o.MMID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@CorrectRoundsToWinCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CorrectRoundsToWinCount, o.CorrectRoundsToWinCount.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_Update", arrParams);

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

        public static int Delete(MGMixAndMatch o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MMID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MMID, o.MMID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMixAndMatch_Delete", arrParams);

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

