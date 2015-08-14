using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

[Serializable]    
    public class ProgramGameLevel : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPGLID;
        private int myPGID;
        private int myLevelNumber;
        private int myLocationX;
        private int myLocationY;
        private int myPointNumber;
        private int myMinigame1ID;
        private int myMinigame2ID;
        private int myAwardBadgeID;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;
        private int myLocationXBonus;
        private int myLocationYBonus;
        private int myMinigame1IDBonus;
        private int myMinigame2IDBonus;
        private int myAwardBadgeIDBonus;

        #endregion

        #region Accessors

        public int PGLID
        {
            get { return myPGLID; }
            set { myPGLID = value; }
        }
        public int PGID
        {
            get { return myPGID; }
            set { myPGID = value; }
        }
        public int LevelNumber
        {
            get { return myLevelNumber; }
            set { myLevelNumber = value; }
        }
        public int LocationX
        {
            get { return myLocationX; }
            set { myLocationX = value; }
        }
        public int LocationY
        {
            get { return myLocationY; }
            set { myLocationY = value; }
        }

        public int LocationXBonus
        {
            get { return myLocationXBonus; }
            set { myLocationXBonus = value; }
        }
        public int LocationYBonus
        {
            get { return myLocationYBonus; }
            set { myLocationYBonus = value; }
        }

        public int PointNumber
        {
            get { return myPointNumber; }
            set { myPointNumber = value; }
        }
        public int Minigame1ID
        {
            get { return myMinigame1ID; }
            set { myMinigame1ID = value; }
        }
        public int Minigame2ID
        {
            get { return myMinigame2ID; }
            set { myMinigame2ID = value; }
        }
        public int AwardBadgeID
        {
            get { return myAwardBadgeID; }
            set { myAwardBadgeID = value; }
        }

        public int Minigame1IDBonus
        {
            get { return myMinigame1IDBonus; }
            set { myMinigame1IDBonus = value; }
        }
        public int Minigame2IDBonus
        {
            get { return myMinigame2IDBonus; }
            set { myMinigame2IDBonus = value; }
        }
        public int AwardBadgeIDBonus
        {
            get { return myAwardBadgeIDBonus; }
            set { myAwardBadgeIDBonus = value; }
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

        public ProgramGameLevel()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PGID)
        {
            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGID", PGID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_GetAll", arrParams);
        }

        public static ProgramGameLevel FetchObject(int PGLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGLID", PGLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                ProgramGameLevel result = new ProgramGameLevel();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PGLID"].ToString(), out _int)) result.PGLID = _int;
                if (int.TryParse(dr["PGID"].ToString(), out _int)) result.PGID = _int;
                if (int.TryParse(dr["LevelNumber"].ToString(), out _int)) result.LevelNumber = _int;
                if (int.TryParse(dr["LocationX"].ToString(), out _int)) result.LocationX = _int;
                if (int.TryParse(dr["LocationY"].ToString(), out _int)) result.LocationY = _int;
                if (int.TryParse(dr["PointNumber"].ToString(), out _int)) result.PointNumber = _int;
                if (int.TryParse(dr["Minigame1ID"].ToString(), out _int)) result.Minigame1ID = _int;
                if (int.TryParse(dr["Minigame2ID"].ToString(), out _int)) result.Minigame2ID = _int;
                if (int.TryParse(dr["AwardBadgeID"].ToString(), out _int)) result.AwardBadgeID = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                if (int.TryParse(dr["LocationXBonus"].ToString(), out _int)) result.LocationXBonus = _int;
                if (int.TryParse(dr["LocationYBonus"].ToString(), out _int)) result.LocationYBonus = _int;
                if (int.TryParse(dr["Minigame1IDBonus"].ToString(), out _int)) result.Minigame1IDBonus = _int;
                if (int.TryParse(dr["Minigame2IDBonus"].ToString(), out _int)) result.Minigame2IDBonus = _int;
                if (int.TryParse(dr["AwardBadgeIDBonus"].ToString(), out _int)) result.AwardBadgeIDBonus = _int;
                

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PGLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGLID", PGLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                ProgramGameLevel result = new ProgramGameLevel();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PGLID"].ToString(), out _int)) this.PGLID = _int;
                if (int.TryParse(dr["PGID"].ToString(), out _int)) this.PGID = _int;
                if (int.TryParse(dr["LevelNumber"].ToString(), out _int)) this.LevelNumber = _int;
                if (int.TryParse(dr["LocationX"].ToString(), out _int)) this.LocationX = _int;
                if (int.TryParse(dr["LocationY"].ToString(), out _int)) this.LocationY = _int;
                if (int.TryParse(dr["PointNumber"].ToString(), out _int)) this.PointNumber = _int;
                if (int.TryParse(dr["Minigame1ID"].ToString(), out _int)) this.Minigame1ID = _int;
                if (int.TryParse(dr["Minigame2ID"].ToString(), out _int)) this.Minigame2ID = _int;
                if (int.TryParse(dr["AwardBadgeID"].ToString(), out _int)) this.AwardBadgeID = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

                if (int.TryParse(dr["LocationXBonus"].ToString(), out _int)) this.LocationXBonus = _int;
                if (int.TryParse(dr["LocationYBonus"].ToString(), out _int)) this.LocationYBonus = _int;
                if (int.TryParse(dr["Minigame1IDBonus"].ToString(), out _int)) this.Minigame1IDBonus = _int;
                if (int.TryParse(dr["Minigame2IDBonus"].ToString(), out _int)) this.Minigame2IDBonus = _int;
                if (int.TryParse(dr["AwardBadgeIDBonus"].ToString(), out _int)) this.AwardBadgeIDBonus = _int;
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

        public static int Insert(ProgramGameLevel o)
        {

            SqlParameter[] arrParams = new SqlParameter[18];

            arrParams[0] = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@LevelNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LevelNumber, o.LevelNumber.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LocationX", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationX, o.LocationX.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LocationY", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationY, o.LocationY.GetTypeCode()));
            arrParams[4] = new SqlParameter("@PointNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointNumber, o.PointNumber.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Minigame1ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame1ID, o.Minigame1ID.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Minigame2ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame2ID, o.Minigame2ID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AwardBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeID, o.AwardBadgeID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[12] = new SqlParameter("@LocationXBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationXBonus, o.LocationXBonus.GetTypeCode()));
            arrParams[13] = new SqlParameter("@LocationYBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationYBonus, o.LocationYBonus.GetTypeCode()));
            arrParams[14] = new SqlParameter("@Minigame1IDBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame1IDBonus, o.Minigame1IDBonus.GetTypeCode()));
            arrParams[15] = new SqlParameter("@Minigame2IDBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame2IDBonus, o.Minigame2IDBonus.GetTypeCode()));
            arrParams[16] = new SqlParameter("@AwardBadgeIDBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeIDBonus, o.AwardBadgeIDBonus.GetTypeCode()));

            arrParams[17] = new SqlParameter("@PGLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGLID, o.PGLID.GetTypeCode()));
            arrParams[17].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_Insert", arrParams);

            o.PGLID = int.Parse(arrParams[17].Value.ToString());

            return o.PGLID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(ProgramGameLevel o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[18];

            arrParams[0] = new SqlParameter("@PGLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGLID, o.PGLID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LevelNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LevelNumber, o.LevelNumber.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LocationX", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationX, o.LocationX.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LocationY", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationY, o.LocationY.GetTypeCode()));
            arrParams[5] = new SqlParameter("@PointNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointNumber, o.PointNumber.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Minigame1ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame1ID, o.Minigame1ID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Minigame2ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame2ID, o.Minigame2ID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AwardBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeID, o.AwardBadgeID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[12] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[13] = new SqlParameter("@LocationXBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationXBonus, o.LocationXBonus.GetTypeCode()));
            arrParams[14] = new SqlParameter("@LocationYBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LocationYBonus, o.LocationYBonus.GetTypeCode()));
            arrParams[15] = new SqlParameter("@Minigame1IDBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame1IDBonus, o.Minigame1IDBonus.GetTypeCode()));
            arrParams[16] = new SqlParameter("@Minigame2IDBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame2IDBonus, o.Minigame2IDBonus.GetTypeCode()));
            arrParams[17] = new SqlParameter("@AwardBadgeIDBonus", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeIDBonus, o.AwardBadgeIDBonus.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_Update", arrParams);

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

        public static int Delete(ProgramGameLevel o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGLID, o.PGLID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }


        public static void MoveUp(int PGLID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PGLID", PGLID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_MoveUp", arrParams);
        }

        public static void MoveDn(int PGLID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PGLID", PGLID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGameLevel_MoveDn", arrParams);
        }

        #endregion

    }//end class

}//end namespace

