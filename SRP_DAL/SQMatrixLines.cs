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

[Serializable]    public class SQMatrixLines : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int mySQMLID;
        private int myQID = 0;
        private int myLineOrder = 999;
        private string myLineText = "";

        private int myFldInt1 = 0;
        private int myFldInt2 = 0;
        private int myFldInt3 = 0;
        private bool myFldBit1 = false;
        private bool myFldBit2 = false;
        private bool myFldBit3 = false;
        private string myFldText1 = "";
        private string myFldText2 = "";
        private string myFldText3 = "";

        #endregion

        #region Accessors

        public int SQMLID
        {
            get { return mySQMLID; }
            set { mySQMLID = value; }
        }
        public int QID
        {
            get { return myQID; }
            set { myQID = value; }
        }
        public int LineOrder
        {
            get { return myLineOrder; }
            set { myLineOrder = value; }
        }
        public string LineText
        {
            get { return myLineText; }
            set { myLineText = value; }
        }
        public int FldInt1
        {
            get { return myFldInt1; }
            set { myFldInt1 = value; }
        }
        public int FldInt2
        {
            get { return myFldInt2; }
            set { myFldInt2 = value; }
        }
        public int FldInt3
        {
            get { return myFldInt3; }
            set { myFldInt3 = value; }
        }
        public bool FldBit1
        {
            get { return myFldBit1; }
            set { myFldBit1 = value; }
        }
        public bool FldBit2
        {
            get { return myFldBit2; }
            set { myFldBit2 = value; }
        }
        public bool FldBit3
        {
            get { return myFldBit3; }
            set { myFldBit3 = value; }
        }
        public string FldText1
        {
            get { return myFldText1; }
            set { myFldText1 = value; }
        }
        public string FldText2
        {
            get { return myFldText2; }
            set { myFldText2 = value; }
        }
        public string FldText3
        {
            get { return myFldText3; }
            set { myFldText3 = value; }
        }

        #endregion

        #region Constructors

        public SQMatrixLines()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int QID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@QID", QID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SQMatrixLines_GetAll", arrParams);
        }

        public static SQMatrixLines FetchObject(int SQMLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SQMLID", SQMLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SQMatrixLines_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SQMatrixLines result = new SQMatrixLines();
                
                int _int;

                if (int.TryParse(dr["SQMLID"].ToString(), out _int)) result.SQMLID = _int;
                if (int.TryParse(dr["QID"].ToString(), out _int)) result.QID = _int;
                if (int.TryParse(dr["LineOrder"].ToString(), out _int)) result.LineOrder = _int;
                result.LineText = dr["LineText"].ToString();
                if (int.TryParse(dr["FldInt1"].ToString(), out _int)) result.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int)) result.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int)) result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int SQMLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SQMLID", SQMLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SQMatrixLines_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SQMatrixLines result = new SQMatrixLines();

                int _int;

                if (int.TryParse(dr["SQMLID"].ToString(), out _int)) this.SQMLID = _int;
                if (int.TryParse(dr["QID"].ToString(), out _int)) this.QID = _int;
                if (int.TryParse(dr["LineOrder"].ToString(), out _int)) this.LineOrder = _int;
                this.LineText = dr["LineText"].ToString();
                if (int.TryParse(dr["FldInt1"].ToString(), out _int)) this.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int)) this.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int)) this.FldInt3 = _int;
                this.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                this.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                this.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                this.FldText1 = dr["FldText1"].ToString();
                this.FldText2 = dr["FldText2"].ToString();
                this.FldText3 = dr["FldText3"].ToString();

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

        public static int Insert(SQMatrixLines o)
        {

            SqlParameter[] arrParams = new SqlParameter[13];

            arrParams[0] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@LineOrder", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LineOrder, o.LineOrder.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LineText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LineText, o.LineText.GetTypeCode()));
            arrParams[3] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[4] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[5] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[6] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[7] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[12] = new SqlParameter("@SQMLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQMLID, o.SQMLID.GetTypeCode()));
            arrParams[12].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQMatrixLines_Insert", arrParams);

            o.SQMLID = int.Parse(arrParams[12].Value.ToString());

            return o.SQMLID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SQMatrixLines o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[13];

            arrParams[0] = new SqlParameter("@SQMLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQMLID, o.SQMLID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LineOrder", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LineOrder, o.LineOrder.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LineText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LineText, o.LineText.GetTypeCode()));
            arrParams[4] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[5] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[6] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[7] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQMatrixLines_Update", arrParams);

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

        public static int Delete(SQMatrixLines o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SQMLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQMLID, o.SQMLID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQMatrixLines_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public static void MoveUp(int SQMLID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SQMLID", SQMLID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQMatrixLines_MoveUp", arrParams);
        }

        public static void MoveDn(int SQMLID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SQMLID", SQMLID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQMatrixLines_MoveDn", arrParams);
        }

        #endregion

    }//end class

}//end namespace

