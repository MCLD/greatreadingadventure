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

    public class SurveyAnswers : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int mySAID = 0;
        private int mySRID = 0;
        private int myTenID = 0;
        private int myPID = 0;
        private int mySID = 0;
        private int myQID = 0;
        private int mySQMLID = 0;
        private DateTime myDateAnswered;
        private int myQType = 0;
        private string myFreeFormAnswer = "";
        private string myClarificationText = "";
        private string myChoiceAnswerIDs = "";
        private string myChoiceAnswerText = "";
        private int myFldInt1 = 0;
        private int myFldInt2 = 0;
        private int myFldInt3 = 0;
        private bool myFldBit1 = false;
        private bool myFldBit2=false;
        private bool myFldBit3 = false;
        private string myFldText1 = "";
        private string myFldText2 = "";
        private string myFldText3 = "";

        #endregion

        #region Accessors

        public int SAID
        {
            get { return mySAID; }
            set { mySAID = value; }
        }
        public int SRID
        {
            get { return mySRID; }
            set { mySRID = value; }
        }
        public int TenID
        {
            get { return myTenID; }
            set { myTenID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int SID
        {
            get { return mySID; }
            set { mySID = value; }
        }
        public int QID
        {
            get { return myQID; }
            set { myQID = value; }
        }
        public int SQMLID
        {
            get { return mySQMLID; }
            set { mySQMLID = value; }
        }
        public DateTime DateAnswered
        {
            get { return myDateAnswered; }
            set { myDateAnswered = value; }
        }
        public int QType
        {
            get { return myQType; }
            set { myQType = value; }
        }
        public string FreeFormAnswer
        {
            get { return myFreeFormAnswer; }
            set { myFreeFormAnswer = value; }
        }
        public string ClarificationText
        {
            get { return myClarificationText; }
            set { myClarificationText = value; }
        }
        public string ChoiceAnswerIDs
        {
            get { return myChoiceAnswerIDs; }
            set { myChoiceAnswerIDs = value; }
        }
        public string ChoiceAnswerText
        {
            get { return myChoiceAnswerText; }
            set { myChoiceAnswerText = value; }
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

        public SurveyAnswers()
        {

            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int SRID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SRID", SRID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyAnswers_GetAll", arrParams);
        }

        public static DataSet GetAllExpanded(int SRID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SRID", SRID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SurveyAnswers_GetAllExpanded", arrParams);
        }


        public static SurveyAnswers FetchObject(int SAID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SAID", SAID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyAnswers_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SurveyAnswers result = new SurveyAnswers();

                DateTime _datetime;

                int _int;

                if (int.TryParse(dr["SAID"].ToString(), out _int)) result.SAID = _int;
                if (int.TryParse(dr["SRID"].ToString(), out _int)) result.SRID = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                if (int.TryParse(dr["QID"].ToString(), out _int)) result.QID = _int;
                if (int.TryParse(dr["SQMLID"].ToString(), out _int)) result.SQMLID = _int;
                if (DateTime.TryParse(dr["DateAnswered"].ToString(), out _datetime)) result.DateAnswered = _datetime;
                if (int.TryParse(dr["QType"].ToString(), out _int)) result.QType = _int;
                result.FreeFormAnswer = dr["FreeFormAnswer"].ToString();
                result.ClarificationText = dr["ClarificationText"].ToString();
                result.ChoiceAnswerIDs = dr["ChoiceAnswerIDs"].ToString();
                result.ChoiceAnswerText = dr["ChoiceAnswerText"].ToString();
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

        public bool Fetch(int SAID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SAID", SAID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SurveyAnswers_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SurveyAnswers result = new SurveyAnswers();

                DateTime _datetime;

                int _int;

                if (int.TryParse(dr["SAID"].ToString(), out _int)) this.SAID = _int;
                if (int.TryParse(dr["SRID"].ToString(), out _int)) this.SRID = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["SID"].ToString(), out _int)) this.SID = _int;
                if (int.TryParse(dr["QID"].ToString(), out _int)) this.QID = _int;
                if (int.TryParse(dr["SQMLID"].ToString(), out _int)) this.SQMLID = _int;
                if (DateTime.TryParse(dr["DateAnswered"].ToString(), out _datetime)) this.DateAnswered = _datetime;
                if (int.TryParse(dr["QType"].ToString(), out _int)) this.QType = _int;
                this.FreeFormAnswer = dr["FreeFormAnswer"].ToString();
                this.ClarificationText = dr["ClarificationText"].ToString();
                this.ChoiceAnswerIDs = dr["ChoiceAnswerIDs"].ToString();
                this.ChoiceAnswerText = dr["ChoiceAnswerText"].ToString();
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

        public static int Insert(SurveyAnswers o)
        {

            SqlParameter[] arrParams = new SqlParameter[22];

            arrParams[0] = new SqlParameter("@SRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SRID, o.SRID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[5] = new SqlParameter("@SQMLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQMLID, o.SQMLID.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DateAnswered", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateAnswered, o.DateAnswered.GetTypeCode()));
            arrParams[7] = new SqlParameter("@QType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QType, o.QType.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FreeFormAnswer", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FreeFormAnswer, o.FreeFormAnswer.GetTypeCode()));
            arrParams[9] = new SqlParameter("@ClarificationText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ClarificationText, o.ClarificationText.GetTypeCode()));
            arrParams[10] = new SqlParameter("@ChoiceAnswerIDs", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceAnswerIDs, o.ChoiceAnswerIDs.GetTypeCode()));
            arrParams[11] = new SqlParameter("@ChoiceAnswerText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceAnswerText, o.ChoiceAnswerText.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[21] = new SqlParameter("@SAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SAID, o.SAID.GetTypeCode()));
            arrParams[21].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyAnswers_Insert", arrParams);

            o.SAID = int.Parse(arrParams[21].Value.ToString());

            return o.SAID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SurveyAnswers o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[22];

            arrParams[0] = new SqlParameter("@SAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SAID, o.SAID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@SRID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SRID, o.SRID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[5] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[6] = new SqlParameter("@SQMLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQMLID, o.SQMLID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@DateAnswered", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateAnswered, o.DateAnswered.GetTypeCode()));
            arrParams[8] = new SqlParameter("@QType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QType, o.QType.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FreeFormAnswer", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FreeFormAnswer, o.FreeFormAnswer.GetTypeCode()));
            arrParams[10] = new SqlParameter("@ClarificationText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ClarificationText, o.ClarificationText.GetTypeCode()));
            arrParams[11] = new SqlParameter("@ChoiceAnswerIDs", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceAnswerIDs, o.ChoiceAnswerIDs.GetTypeCode()));
            arrParams[12] = new SqlParameter("@ChoiceAnswerText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceAnswerText, o.ChoiceAnswerText.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyAnswers_Update", arrParams);

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

        public static int Delete(SurveyAnswers o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SAID, o.SAID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SurveyAnswers_Delete", arrParams);

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

