using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL {

    [Serializable]
    public class ProgramGamePointConversion : EntityBase {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPGCID;
        private int myPGID;
        private int myActivityTypeId;
        private int myActivityCount;
        private int myPointCount;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int PGCID {
            get { return myPGCID; }
            set { myPGCID = value; }
        }
        public int PGID {
            get { return myPGID; }
            set { myPGID = value; }
        }
        public int ActivityTypeId {
            get { return myActivityTypeId; }
            set { myActivityTypeId = value; }
        }
        public int ActivityCount {
            get { return myActivityCount; }
            set { myActivityCount = value; }
        }
        public int PointCount {
            get { return myPointCount; }
            set { myPointCount = value; }
        }
        public DateTime LastModDate {
            get { return myLastModDate; }
            set { myLastModDate = value; }
        }
        public string LastModUser {
            get { return myLastModUser; }
            set { myLastModUser = value; }
        }
        public DateTime AddedDate {
            get { return myAddedDate; }
            set { myAddedDate = value; }
        }
        public string AddedUser {
            get { return myAddedUser; }
            set { myAddedUser = value; }
        }

        #endregion

        #region Constructors

        public ProgramGamePointConversion() {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PGID) {
            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGID", PGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_GetAll", arrParams);
        }

        public static ProgramGamePointConversion FetchObject(int PGCID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGCID", PGCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_GetByID", arrParams);

            if(dr.Read()) {

                // declare return value

                ProgramGamePointConversion result = new ProgramGamePointConversion();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["PGCID"].ToString(), out _int))
                    result.PGCID = _int;
                if(int.TryParse(dr["PGID"].ToString(), out _int))
                    result.PGID = _int;
                if(int.TryParse(dr["ActivityTypeId"].ToString(), out _int))
                    result.ActivityTypeId = _int;
                if(int.TryParse(dr["ActivityCount"].ToString(), out _int))
                    result.ActivityCount = _int;
                if(int.TryParse(dr["PointCount"].ToString(), out _int))
                    result.PointCount = _int;
                if(DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if(DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PGCID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGCID", PGCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_GetByID", arrParams);

            if(dr.Read()) {

                // declare return value

                ProgramGamePointConversion result = new ProgramGamePointConversion();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["PGCID"].ToString(), out _int))
                    this.PGCID = _int;
                if(int.TryParse(dr["PGID"].ToString(), out _int))
                    this.PGID = _int;
                if(int.TryParse(dr["ActivityTypeId"].ToString(), out _int))
                    this.ActivityTypeId = _int;
                if(int.TryParse(dr["ActivityCount"].ToString(), out _int))
                    this.ActivityCount = _int;
                if(int.TryParse(dr["PointCount"].ToString(), out _int))
                    this.PointCount = _int;
                if(DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if(DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return true;

            }

            dr.Close();

            return false;

        }

        public static ProgramGamePointConversion FetchObjectByActivityId(int PGID, int ActivityId) {

            // declare reader

            SqlDataReader dr;
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PGID", PGID);
            arrParams[1] = new SqlParameter("@ActivityTypeID", ActivityId);
            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_GetByActivityType", arrParams);

            if(dr.Read()) {

                // declare return value

                ProgramGamePointConversion result = new ProgramGamePointConversion();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["PGCID"].ToString(), out _int))
                    result.PGCID = _int;
                if(int.TryParse(dr["PGID"].ToString(), out _int))
                    result.PGID = _int;
                if(int.TryParse(dr["ActivityTypeId"].ToString(), out _int))
                    result.ActivityTypeId = _int;
                if(int.TryParse(dr["ActivityCount"].ToString(), out _int))
                    result.ActivityCount = _int;
                if(int.TryParse(dr["PointCount"].ToString(), out _int))
                    result.PointCount = _int;
                if(DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if(DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool FetchByActivityId(int PGID, int ActivityId) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@PGID", PGID);
            arrParams[1] = new SqlParameter("@ActivityTypeID", ActivityId);
            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_GetByActivityType", arrParams);

            if(dr.Read()) {

                // declare return value

                ProgramGamePointConversion result = new ProgramGamePointConversion();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["PGCID"].ToString(), out _int))
                    this.PGCID = _int;
                if(int.TryParse(dr["PGID"].ToString(), out _int))
                    this.PGID = _int;
                if(int.TryParse(dr["ActivityTypeId"].ToString(), out _int))
                    this.ActivityTypeId = _int;
                if(int.TryParse(dr["ActivityCount"].ToString(), out _int))
                    this.ActivityCount = _int;
                if(int.TryParse(dr["PointCount"].ToString(), out _int))
                    this.PointCount = _int;
                if(DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if(DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return true;

            }

            dr.Close();

            return false;

        }

        public int Insert() {

            return Insert(this);

        }

        public static int Insert(ProgramGamePointConversion o) {

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@ActivityTypeId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ActivityTypeId, o.ActivityTypeId.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ActivityCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ActivityCount, o.ActivityCount.GetTypeCode()));
            arrParams[3] = new SqlParameter("@PointCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointCount, o.PointCount.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PGCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGCID, o.PGCID.GetTypeCode()));
            arrParams[8].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_Insert", arrParams);

            o.PGCID = int.Parse(arrParams[8].Value.ToString());

            return o.PGCID;

        }

        public int Update() {

            return Update(this);

        }

        public static int Update(ProgramGamePointConversion o) {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PGCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGCID, o.PGCID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ActivityTypeId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ActivityTypeId, o.ActivityTypeId.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ActivityCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ActivityCount, o.ActivityCount.GetTypeCode()));
            arrParams[4] = new SqlParameter("@PointCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointCount, o.PointCount.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_Update", arrParams);

            } catch(SqlException exx) {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public int Delete() {

            return Delete(this);

        }

        public static int Delete(ProgramGamePointConversion o) {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PGCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGCID, o.PGCID.GetTypeCode()));

            try {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGamePointConversion_Delete", arrParams);

            } catch(SqlException exx) {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

    }//end class

    public enum ActivityType {
        Books = 0,
        Pages = 1,
        //Paragraphs = 2,
        Minutes = 3
    }


    //foreach (EMyEnum val in Enum.GetValues(typeof(EMyEnum)))
    //{
    //   Console.WriteLine(val);
    //}

}//end namespace

