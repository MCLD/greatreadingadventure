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

[Serializable]    public class MGMatchingGameTiles : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myMAGTID;
        private int myMAGID;
        private int myMGID;
        private bool myTile1UseMedium;
        private bool myTile1UseHard;
        private bool myTile2UseMedium;
        private bool myTile2UseHard;
        private bool myTile3UseMedium;
        private bool myTile3UseHard;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int MAGTID
        {
            get { return myMAGTID; }
            set { myMAGTID = value; }
        }
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
        public bool Tile1UseMedium
        {
            get { return myTile1UseMedium; }
            set { myTile1UseMedium = value; }
        }
        public bool Tile1UseHard
        {
            get { return myTile1UseHard; }
            set { myTile1UseHard = value; }
        }
        public bool Tile2UseMedium
        {
            get { return myTile2UseMedium; }
            set { myTile2UseMedium = value; }
        }
        public bool Tile2UseHard
        {
            get { return myTile2UseHard; }
            set { myTile2UseHard = value; }
        }
        public bool Tile3UseMedium
        {
            get { return myTile3UseMedium; }
            set { myTile3UseMedium = value; }
        }
        public bool Tile3UseHard
        {
            get { return myTile3UseHard; }
            set { myTile3UseHard = value; }
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

        public MGMatchingGameTiles()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int MGID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGMatchingGameTiles_GetAll", arrParams);
        }

        public static MGMatchingGameTiles FetchObject(int MAGTID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MAGTID", MAGTID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMatchingGameTiles_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMatchingGameTiles result = new MGMatchingGameTiles();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MAGTID"].ToString(), out _int)) result.MAGTID = _int;
                if (int.TryParse(dr["MAGID"].ToString(), out _int)) result.MAGID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                result.Tile1UseMedium = bool.Parse(dr["Tile1UseMedium"].ToString());
                result.Tile1UseHard = bool.Parse(dr["Tile1UseHard"].ToString());
                result.Tile2UseMedium = bool.Parse(dr["Tile2UseMedium"].ToString());
                result.Tile2UseHard = bool.Parse(dr["Tile2UseHard"].ToString());
                result.Tile3UseMedium = bool.Parse(dr["Tile3UseMedium"].ToString());
                result.Tile3UseHard = bool.Parse(dr["Tile3UseHard"].ToString());
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

        public bool Fetch(int MAGTID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MAGTID", MAGTID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGMatchingGameTiles_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGMatchingGameTiles result = new MGMatchingGameTiles();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MAGTID"].ToString(), out _int)) this.MAGTID = _int;
                if (int.TryParse(dr["MAGID"].ToString(), out _int)) this.MAGID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                this.Tile1UseMedium = bool.Parse(dr["Tile1UseMedium"].ToString());
                this.Tile1UseHard = bool.Parse(dr["Tile1UseHard"].ToString());
                this.Tile2UseMedium = bool.Parse(dr["Tile2UseMedium"].ToString());
                this.Tile2UseHard = bool.Parse(dr["Tile2UseHard"].ToString());
                this.Tile3UseMedium = bool.Parse(dr["Tile3UseMedium"].ToString());
                this.Tile3UseHard = bool.Parse(dr["Tile3UseHard"].ToString());
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

        public static int Insert(MGMatchingGameTiles o)
        {

            SqlParameter[] arrParams = new SqlParameter[13];

            arrParams[0] = new SqlParameter("@MAGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGID, o.MAGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Tile1UseMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile1UseMedium, o.Tile1UseMedium.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Tile1UseHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile1UseHard, o.Tile1UseHard.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Tile2UseMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile2UseMedium, o.Tile2UseMedium.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Tile2UseHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile2UseHard, o.Tile2UseHard.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Tile3UseMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile3UseMedium, o.Tile3UseMedium.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Tile3UseHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile3UseHard, o.Tile3UseHard.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[12] = new SqlParameter("@MAGTID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGTID, o.MAGTID.GetTypeCode()));
            arrParams[12].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMatchingGameTiles_Insert", arrParams);

            o.MAGTID = int.Parse(arrParams[12].Value.ToString());

            return o.MAGTID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGMatchingGameTiles o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[13];

            arrParams[0] = new SqlParameter("@MAGTID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGTID, o.MAGTID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MAGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGID, o.MAGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Tile1UseMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile1UseMedium, o.Tile1UseMedium.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Tile1UseHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile1UseHard, o.Tile1UseHard.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Tile2UseMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile2UseMedium, o.Tile2UseMedium.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Tile2UseHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile2UseHard, o.Tile2UseHard.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Tile3UseMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile3UseMedium, o.Tile3UseMedium.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Tile3UseHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Tile3UseHard, o.Tile3UseHard.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[12] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMatchingGameTiles_Update", arrParams);

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

        public static int Delete(MGMatchingGameTiles o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MAGTID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MAGTID, o.MAGTID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGMatchingGameTiles_Delete", arrParams);

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

