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

[Serializable]    public class GamePlayStats : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myGPSID;
        private int myPID;
        private int myMGID;
        private int myMGType;
        private DateTime myStarted;
        private string myDifficulty;
        private bool myCompletedPlay;
        private DateTime myCompleted;

        #endregion

        #region Accessors

        public int GPSID
        {
            get { return myGPSID; }
            set { myGPSID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public int MGType
        {
            get { return myMGType; }
            set { myMGType = value; }
        }
        public DateTime Started
        {
            get { return myStarted; }
            set { myStarted = value; }
        }
        public string Difficulty
        {
            get { return myDifficulty; }
            set { myDifficulty = value; }
        }
        public bool CompletedPlay
        {
            get { return myCompletedPlay; }
            set { myCompletedPlay = value; }
        }
        public DateTime Completed
        {
            get { return myCompleted; }
            set { myCompleted = value; }
        }

        #endregion

        #region Constructors

        public GamePlayStats()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_GamePlayStats_GetAll");
        }

        public static GamePlayStats FetchObject(int GPSID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@GPSID", GPSID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_GamePlayStats_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                GamePlayStats result = new GamePlayStats();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["GPSID"].ToString(), out _int)) result.GPSID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["MGType"].ToString(), out _int)) result.MGType = _int;
                if (DateTime.TryParse(dr["Started"].ToString(), out _datetime)) result.Started = _datetime;
                result.Difficulty = dr["Difficulty"].ToString();
                result.CompletedPlay = bool.Parse(dr["CompletedPlay"].ToString());
                if (DateTime.TryParse(dr["Completed"].ToString(), out _datetime)) result.Completed = _datetime;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int GPSID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@GPSID", GPSID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_GamePlayStats_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                GamePlayStats result = new GamePlayStats();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["GPSID"].ToString(), out _int)) this.GPSID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["MGType"].ToString(), out _int)) this.MGType = _int;
                if (DateTime.TryParse(dr["Started"].ToString(), out _datetime)) this.Started = _datetime;
                this.Difficulty = dr["Difficulty"].ToString();
                this.CompletedPlay = bool.Parse(dr["CompletedPlay"].ToString());
                if (DateTime.TryParse(dr["Completed"].ToString(), out _datetime)) this.Completed = _datetime;

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

        public static int Insert(GamePlayStats o)
        {

            SqlParameter[] arrParams = new SqlParameter[8];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MGType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGType, o.MGType.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Started", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Started, o.Started.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Difficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Difficulty, o.Difficulty.GetTypeCode()));
            arrParams[5] = new SqlParameter("@CompletedPlay", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CompletedPlay, o.CompletedPlay.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Completed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Completed, o.Completed.GetTypeCode()));
            arrParams[7] = new SqlParameter("@GPSID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GPSID, o.GPSID.GetTypeCode()));
            arrParams[7].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_GamePlayStats_Insert", arrParams);

            o.GPSID = int.Parse(arrParams[7].Value.ToString());

            return o.GPSID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(GamePlayStats o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[8];

            arrParams[0] = new SqlParameter("@GPSID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GPSID, o.GPSID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@MGType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGType, o.MGType.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Started", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Started, o.Started.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Difficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Difficulty, o.Difficulty.GetTypeCode()));
            arrParams[6] = new SqlParameter("@CompletedPlay", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CompletedPlay, o.CompletedPlay.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Completed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Completed, o.Completed.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_GamePlayStats_Update", arrParams);

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

        public static int Delete(GamePlayStats o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@GPSID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GPSID, o.GPSID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_GamePlayStats_Delete", arrParams);

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

