using System;
using System.Collections.Generic;
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
using SRP_DAL;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL {

    [Serializable]
    public class MGCodeBreaker : EntityBase, IMinigame {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myCBID;
        private int myMGID;
        private string myEasyString = "";
        private bool myEnableMediumDifficulty = false;
        private bool myEnableHardDifficulty = false;
        private string myMediumString = "";
        private string myHardString = "";
        private DateTime myLastModDate;
        private string myLastModUser = "N/A";
        private DateTime myAddedDate;
        private string myAddedUser = "N/A";

        #endregion

        #region Accessors

        public int CBID
        {
            get { return myCBID; }
            set { myCBID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public string EasyString
        {
            get { return myEasyString; }
            set { myEasyString = value; }
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
        public string MediumString
        {
            get { return myMediumString; }
            set { myMediumString = value; }
        }
        public string HardString
        {
            get { return myHardString; }
            set { myHardString = value; }
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

        public MGCodeBreaker() {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll() {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_GetAll");
        }

        public static DataSet FetchWithParent(int MGID) {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_GetByIDWithParent", arrParams);
        }

        public static MGCodeBreaker FetchObjectByParent(int MGID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_GetByMGID", arrParams);

            if(dr.Read()) {

                // declare return value

                MGCodeBreaker result = new MGCodeBreaker();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["CBID"].ToString(), out _int))
                    result.CBID = _int;
                if(int.TryParse(dr["MGID"].ToString(), out _int))
                    result.MGID = _int;
                result.EasyString = dr["EasyString"].ToString();
                result.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                result.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
                result.MediumString = dr["MediumString"].ToString();
                result.HardString = dr["HardString"].ToString();
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



        public static MGCodeBreaker FetchObject(int CBID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CBID", CBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_GetByID", arrParams);

            if(dr.Read()) {

                // declare return value

                MGCodeBreaker result = new MGCodeBreaker();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["CBID"].ToString(), out _int))
                    result.CBID = _int;
                if(int.TryParse(dr["MGID"].ToString(), out _int))
                    result.MGID = _int;
                result.EasyString = dr["EasyString"].ToString();
                result.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                result.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
                result.MediumString = dr["MediumString"].ToString();
                result.HardString = dr["HardString"].ToString();
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

        public bool Fetch(int CBID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CBID", CBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_GetByID", arrParams);

            if(dr.Read()) {

                // declare return value

                MGCodeBreaker result = new MGCodeBreaker();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["CBID"].ToString(), out _int))
                    this.CBID = _int;
                if(int.TryParse(dr["MGID"].ToString(), out _int))
                    this.MGID = _int;
                this.EasyString = dr["EasyString"].ToString();
                this.EnableMediumDifficulty = bool.Parse(dr["EnableMediumDifficulty"].ToString());
                this.EnableHardDifficulty = bool.Parse(dr["EnableHardDifficulty"].ToString());
                this.MediumString = dr["MediumString"].ToString();
                this.HardString = dr["HardString"].ToString();
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

        public static int Insert(MGCodeBreaker o) {

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@EasyString", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EasyString, o.EasyString.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@MediumString", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MediumString, o.MediumString.GetTypeCode()));
            arrParams[5] = new SqlParameter("@HardString", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HardString, o.HardString.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@CBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CBID, o.CBID.GetTypeCode()));
            arrParams[10].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_Insert", arrParams);

            o.CBID = int.Parse(arrParams[10].Value.ToString());

            return o.CBID;

        }

        public int Update() {

            return Update(this);

        }

        public static int Update(MGCodeBreaker o) {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[11];

            arrParams[0] = new SqlParameter("@CBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CBID, o.CBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EasyString", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EasyString, o.EasyString.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EnableMediumDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableMediumDifficulty, o.EnableMediumDifficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EnableHardDifficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EnableHardDifficulty, o.EnableHardDifficulty.GetTypeCode()));
            arrParams[5] = new SqlParameter("@MediumString", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MediumString, o.MediumString.GetTypeCode()));
            arrParams[6] = new SqlParameter("@HardString", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HardString, o.HardString.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_Update", arrParams);

            } catch(SqlException exx) {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public int Delete() {

            return Delete(this);

        }

        public static int Delete(MGCodeBreaker o) {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CBID, o.CBID.GetTypeCode()));

            try {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGCodeBreaker_Delete", arrParams);

            } catch(SqlException exx) {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

        public IEnumerable<string> GetEncoded(string s, int difficulty) {
            var encodedMessage = new List<string>();
            string prefix = string.Empty;
            if(difficulty == 2) {
                prefix = "m_";
            }
            if(difficulty == 3) {
                prefix = "h_";
            }

            foreach(char c in s.ToUpper().ToCharArray()) {
                if(c != ' ') {
                    encodedMessage.Add(string.Format("{0}{1}_{2}.png",
                                                     prefix,
                                                     CBID,
                                                     (int)c));
                } else {
                    encodedMessage.Add(null);
                }
            }

            return encodedMessage;
        }

        public SortedDictionary<char, string> GetKey(string s, int difficulty) {
            var key = new SortedDictionary<char, string>();
            string prefix = string.Empty;
            if(difficulty == 2) {
                prefix = "m_";
            }
            if(difficulty == 3) {
                prefix = "h_";
            }

            char[] chars = s.ToUpper().ToCharArray();

            foreach(char c in chars) {
                if(c != ' ' && !key.ContainsKey(c)) {
                    key.Add(c, string.Format("{0}{1}_{2}.png", prefix, CBID, (int)c));
                }
            }

            return key;
        }

        public static IEnumerable<KeyItem> GetKeyCharacters(int CBID) {
            //33 46 44 63 (, . ! ? )
            var punctuationAscii = new int[] { 33, 46, 44, 63 };
            var lst = new List<KeyItem>();

            // 65 = A, 90 = Z
            // lowercase (79 - a to 122 - z) removed to reduce complexity
            for(var asciiValue = 65; asciiValue <= 90; asciiValue++) {
                lst.Add(new KeyItem {
                    Character_Num = asciiValue,
                    Character = Convert.ToChar(asciiValue).ToString(),
                    CBID = CBID
                });
            }

            foreach(var asciiValue in punctuationAscii) {
                lst.Add(new KeyItem {
                    Character_Num = asciiValue,
                    Character = Convert.ToChar(asciiValue).ToString(),
                    CBID = CBID
                });
            }

            return lst;
        }

    }//end class

    [Serializable]
    public class KeyItem {
        public int CBID { get; set; }
        public int Character_Num { get; set; }
        public string Character { get; set; }
    }

}//end namespace

