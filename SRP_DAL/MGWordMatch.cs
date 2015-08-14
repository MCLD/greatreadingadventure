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

[Serializable]    public class MGWordMatch : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myWMID;
        private int myMGID;
        private int myCorrectRoundsToWinCount=1;
        private int myNumOptionsToChooseFrom=3;
        private bool myEnableMediumDifficulty;
        private bool myEnableHardDifficulty;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int WMID
        {
            get { return myWMID; }
            set { myWMID = value; }
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
        public int NumOptionsToChooseFrom
        {
            get { return myNumOptionsToChooseFrom; }
            set { myNumOptionsToChooseFrom = value; }
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

        public MGWordMatch()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGWordMatch_GetAll");
        }

        public static DataSet GetPlayItems(int WMID, int NumOptionsToChooseFrom)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@WMID", WMID);
            arrParams[1] = new SqlParameter("@Num", NumOptionsToChooseFrom);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGWordMatch_GetRandomX", arrParams);
        }

        public static DataSet FetchWithParent(int MGID)
        {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGWordMatch_GetByIDWithParent", arrParams);
        }

        public static MGWordMatch FetchObjectByParent(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGWordMatch_GetByMGID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGWordMatch result = new MGWordMatch();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["WMID"].ToString(), out _int)) result.WMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) result.CorrectRoundsToWinCount = _int;
                if (int.TryParse(dr["NumOptionsToChooseFrom"].ToString(), out _int)) result.NumOptionsToChooseFrom = _int;
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

        public static MGWordMatch FetchObject(int WMID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@WMID", WMID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGWordMatch_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGWordMatch result = new MGWordMatch();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["WMID"].ToString(), out _int)) result.WMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) result.CorrectRoundsToWinCount = _int;
                if (int.TryParse(dr["NumOptionsToChooseFrom"].ToString(), out _int)) result.NumOptionsToChooseFrom = _int;
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

        public bool Fetch(int WMID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@WMID", WMID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGWordMatch_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGWordMatch result = new MGWordMatch();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["WMID"].ToString(), out _int)) this.WMID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["CorrectRoundsToWinCount"].ToString(), out _int)) this.CorrectRoundsToWinCount = _int;
                if (int.TryParse(dr["NumOptionsToChooseFrom"].ToString(), out _int)) this.NumOptionsToChooseFrom = _int;
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

        public static int Insert(MGWordMatch o)
        {

            SqlParameter[] arrParams = new SqlParameter[10];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@CorrectRoundsToWinCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CorrectRoundsToWinCount, o.CorrectRoundsToWinCount.GetTypeCode()));
            arrParams[2] = new SqlParameter("@NumOptionsToChooseFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumOptionsToChooseFrom, o.NumOptionsToChooseFrom.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[9] = new SqlParameter("@WMID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.WMID, o.WMID.GetTypeCode()));
            arrParams[9].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGWordMatch_Insert", arrParams);

            o.WMID = int.Parse(arrParams[9].Value.ToString());

            return o.WMID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGWordMatch o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[10];

            arrParams[0] = new SqlParameter("@WMID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.WMID, o.WMID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@CorrectRoundsToWinCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CorrectRoundsToWinCount, o.CorrectRoundsToWinCount.GetTypeCode()));
            arrParams[3] = new SqlParameter("@NumOptionsToChooseFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumOptionsToChooseFrom, o.NumOptionsToChooseFrom.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[5] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGWordMatch_Update", arrParams);

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

        public static int Delete(MGWordMatch o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@WMID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.WMID, o.WMID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGWordMatch_Delete", arrParams);

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

