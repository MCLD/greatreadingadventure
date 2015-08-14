using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

[Serializable]    public class MGOnlineBookPages : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myOBPGID;
        private int myOBID;
        private int myMGID;
        private int myPageNumber;
        private string myTextEasy ="";
        private string myTextMedium = "";
        private string myTextHard = "";
        private string myAudioEasy = "";
        private string myAudioMedium = "";
        private string myAudioHard = "";
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int OBPGID
        {
            get { return myOBPGID; }
            set { myOBPGID = value; }
        }
        public int OBID
        {
            get { return myOBID; }
            set { myOBID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public int PageNumber
        {
            get { return myPageNumber; }
            set { myPageNumber = value; }
        }
        public string TextEasy
        {
            get { return myTextEasy; }
            set { myTextEasy = value; }
        }
        public string TextMedium
        {
            get { return myTextMedium; }
            set { myTextMedium = value; }
        }
        public string TextHard
        {
            get { return myTextHard; }
            set { myTextHard = value; }
        }
        public string AudioEasy
        {
            get { return myAudioEasy; }
            set { myAudioEasy = value; }
        }
        public string AudioMedium
        {
            get { return myAudioMedium; }
            set { myAudioMedium = value; }
        }
        public string AudioHard
        {
            get { return myAudioHard; }
            set { myAudioHard = value; }
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

        public MGOnlineBookPages()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int MGID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_GetAll", arrParams);
        }

        public static MGOnlineBookPages FetchObject(int OBPGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@OBPGID", OBPGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGOnlineBookPages result = new MGOnlineBookPages();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["OBPGID"].ToString(), out _int)) result.OBPGID = _int;
                if (int.TryParse(dr["OBID"].ToString(), out _int)) result.OBID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["PageNumber"].ToString(), out _int)) result.PageNumber = _int;
                result.TextEasy = dr["TextEasy"].ToString();
                result.TextMedium = dr["TextMedium"].ToString();
                result.TextHard = dr["TextHard"].ToString();
                result.AudioEasy = dr["AudioEasy"].ToString();
                result.AudioMedium = dr["AudioMedium"].ToString();
                result.AudioHard = dr["AudioHard"].ToString();
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

        public static MGOnlineBookPages FetchObjectByPage(int page, int obid)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@Page", page);
            arrParams[1] = new SqlParameter("@OBID", obid);
            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_GetByPage", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGOnlineBookPages result = new MGOnlineBookPages();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["OBPGID"].ToString(), out _int)) result.OBPGID = _int;
                if (int.TryParse(dr["OBID"].ToString(), out _int)) result.OBID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["PageNumber"].ToString(), out _int)) result.PageNumber = _int;
                result.TextEasy = dr["TextEasy"].ToString();
                result.TextMedium = dr["TextMedium"].ToString();
                result.TextHard = dr["TextHard"].ToString();
                result.AudioEasy = dr["AudioEasy"].ToString();
                result.AudioMedium = dr["AudioMedium"].ToString();
                result.AudioHard = dr["AudioHard"].ToString();
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

        public bool Fetch(int OBPGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@OBPGID", OBPGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGOnlineBookPages result = new MGOnlineBookPages();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["OBPGID"].ToString(), out _int)) this.OBPGID = _int;
                if (int.TryParse(dr["OBID"].ToString(), out _int)) this.OBID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["PageNumber"].ToString(), out _int)) this.PageNumber = _int;
                this.TextEasy = dr["TextEasy"].ToString();
                this.TextMedium = dr["TextMedium"].ToString();
                this.TextHard = dr["TextHard"].ToString();
                this.AudioEasy = dr["AudioEasy"].ToString();
                this.AudioMedium = dr["AudioMedium"].ToString();
                this.AudioHard = dr["AudioHard"].ToString();
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

        public static int Insert(MGOnlineBookPages o)
        {

            SqlParameter[] arrParams = new SqlParameter[14];

            arrParams[0] = new SqlParameter("@OBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OBID, o.OBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PageNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PageNumber, o.PageNumber.GetTypeCode()));
            arrParams[3] = new SqlParameter("@TextEasy", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TextEasy, o.TextEasy.GetTypeCode()));
            arrParams[4] = new SqlParameter("@TextMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TextMedium, o.TextMedium.GetTypeCode()));
            arrParams[5] = new SqlParameter("@TextHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TextHard, o.TextHard.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AudioEasy", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AudioEasy, o.AudioEasy.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AudioMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AudioMedium, o.AudioMedium.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AudioHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AudioHard, o.AudioHard.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[12] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[13] = new SqlParameter("@OBPGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OBPGID, o.OBPGID.GetTypeCode()));
            arrParams[13].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_Insert", arrParams);

            o.OBPGID = int.Parse(arrParams[13].Value.ToString());

            return o.OBPGID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGOnlineBookPages o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[14];

            arrParams[0] = new SqlParameter("@OBPGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OBPGID, o.OBPGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@OBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OBID, o.OBID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@PageNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PageNumber, o.PageNumber.GetTypeCode()));
            arrParams[4] = new SqlParameter("@TextEasy", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TextEasy, o.TextEasy.GetTypeCode()));
            arrParams[5] = new SqlParameter("@TextMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TextMedium, o.TextMedium.GetTypeCode()));
            arrParams[6] = new SqlParameter("@TextHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TextHard, o.TextHard.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AudioEasy", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AudioEasy, o.AudioEasy.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AudioMedium", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AudioMedium, o.AudioMedium.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AudioHard", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AudioHard, o.AudioHard.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[12] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[13] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_Update", arrParams);

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

        public static int Delete(MGOnlineBookPages o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@OBPGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OBPGID, o.OBPGID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

        public static void MoveUp(int OBPGID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@OBPGID", OBPGID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_MoveUp", arrParams);
        }

        public static void MoveDn(int OBPGID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@OBPGID", OBPGID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGOnlineBookPages_MoveDn", arrParams);
        }

    }//end class

}//end namespace

