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

[Serializable]    public class SurveyQuestion : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myQID;
        private int mySID = 0;
        private int myQNumber = 0;
        private int myQType = 0;
        private string myQName = "";
        private string myQText = "";
        private int myDisplayControl =  0;
        private int myDisplayDirection = 0;
        private bool myIsRequired = false;

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

        public int QID
        {
            get { return myQID; }
            set { myQID = value; }
        }
        public int SID
        {
            get { return mySID; }
            set { mySID = value; }
        }
        public int QNumber
        {
            get { return myQNumber; }
            set { myQNumber = value; }
        }
        public int QType
        {
            get { return myQType; }
            set { myQType = value; }
        }
        public string QName
        {
            get { return myQName; }
            set { myQName = value; }
        }
        public string QText
        {
            get { return myQText; }
            set { myQText = value; }
        }
        public int DisplayControl
        {
            get { return myDisplayControl; }
            set { myDisplayControl = value; }
        }
        public int DisplayDirection
        {
            get { return myDisplayDirection; }
            set { myDisplayDirection = value; }
        }
        public bool IsRequired
        {
            get { return myIsRequired; }
            set { myIsRequired = value; }
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

        public SurveyQuestion()
        {

        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int SID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SID", SID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyQuestion_GetAll", arrParams);
        }

        public static DataSet GetSurveyPage(int SID, int StartQNum)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@SID", SID);
            arrParams[1] = new SqlParameter("@QNum", StartQNum);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyQuestion_GetPageFromQNum", arrParams);
        }

        public static SurveyQuestion FetchObject(int QID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@QID", QID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyQuestion_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                var result = new SurveyQuestion();

                int _int;

                if (int.TryParse(dr["QID"].ToString(), out _int)) result.QID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                if (int.TryParse(dr["QNumber"].ToString(), out _int)) result.QNumber = _int;
                if (int.TryParse(dr["QType"].ToString(), out _int)) result.QType = _int;
                result.QName = dr["QName"].ToString();
                result.QText = dr["QText"].ToString();
                if (int.TryParse(dr["DisplayControl"].ToString(), out _int)) result.DisplayControl = _int;
                if (int.TryParse(dr["DisplayDirection"].ToString(), out _int)) result.DisplayDirection = _int;
                result.IsRequired = bool.Parse(dr["IsRequired"].ToString());
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

        public bool Fetch(int QID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@QID", QID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyQuestion_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                var result = new SurveyQuestion();

                int _int;


                if (int.TryParse(dr["QID"].ToString(), out _int)) this.QID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) this.SID = _int;
                if (int.TryParse(dr["QNumber"].ToString(), out _int)) this.QNumber = _int;
                if (int.TryParse(dr["QType"].ToString(), out _int)) this.QType = _int;
                this.QName = dr["QName"].ToString();
                this.QText = dr["QText"].ToString();
                if (int.TryParse(dr["DisplayControl"].ToString(), out _int)) this.DisplayControl = _int;
                if (int.TryParse(dr["DisplayDirection"].ToString(), out _int)) this.DisplayDirection = _int;
                this.IsRequired = bool.Parse(dr["IsRequired"].ToString());
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

        public static int Insert(SurveyQuestion o)
        {

            SqlParameter[] arrParams = new SqlParameter[18];

            arrParams[0] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@QNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QNumber, o.QNumber.GetTypeCode()));
            arrParams[2] = new SqlParameter("@QType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QType, o.QType.GetTypeCode()));
            arrParams[3] = new SqlParameter("@QName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QName, o.QName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@QText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QText, o.QText.GetTypeCode()));
            arrParams[5] = new SqlParameter("@DisplayControl", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DisplayControl, o.DisplayControl.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DisplayDirection", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DisplayDirection, o.DisplayDirection.GetTypeCode()));
            arrParams[7] = new SqlParameter("@IsRequired", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsRequired, o.IsRequired.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[17] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[17].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyQuestion_Insert", arrParams);

            o.QID = int.Parse(arrParams[17].Value.ToString());

            var s = Survey.FetchObject(o.SID);
            if (s.CanBeScored != Survey.IsScorable(s.SID))
            {
                s.CanBeScored = !s.CanBeScored;
                s.Update();
            }

            return o.QID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SurveyQuestion o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[18];

            arrParams[0] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@QNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QNumber, o.QNumber.GetTypeCode()));
            arrParams[3] = new SqlParameter("@QType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QType, o.QType.GetTypeCode()));
            arrParams[4] = new SqlParameter("@QName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QName, o.QName.GetTypeCode()));
            arrParams[5] = new SqlParameter("@QText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QText, o.QText.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DisplayControl", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DisplayControl, o.DisplayControl.GetTypeCode()));
            arrParams[7] = new SqlParameter("@DisplayDirection", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DisplayDirection, o.DisplayDirection.GetTypeCode()));
            arrParams[8] = new SqlParameter("@IsRequired", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IsRequired, o.IsRequired.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyQuestion_Update", arrParams);

                var s = Survey.FetchObject(o.SID);
                if (s.CanBeScored != Survey.IsScorable(s.SID))
                {
                    s.CanBeScored = !s.CanBeScored;
                    s.Update();
                }
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

        public static int Delete(SurveyQuestion o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyQuestion_Delete", arrParams);
                
                var s = Survey.FetchObject(o.SID);
                if (s.CanBeScored != Survey.IsScorable(s.SID))
                {
                    s.CanBeScored = !s.CanBeScored;
                    s.Update();
                }

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }


        public static void MoveUp(int QID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@QID", QID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyQuestion_MoveUp", arrParams);
        }

        public static void MoveDn(int QID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@QID", QID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyQuestion_MoveDn", arrParams);
        }

        #endregion


        public static string TypeDescription(int d)
        {
            switch (d)
            {
                case 1:
                    return "Instructions/Text/Description";
                case 2:
                    return "Multiple Choice";
                case 3:
                    return "Free Form Text";
                case 4:
                    return "Matrix of Questions";                
                case 5:
                    return "Page Break";
                case 6:
                    return "Survey/Test END";
                default:
                    return "N/A";
            }
        }

    }//end class

}//end namespace

