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

[Serializable]    public class SQChoices : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int mySQCID;
        private int myQID = 0;
        private int myChoiceOrder =  999;
        private string myChoiceText = "";
        private int myScore = 0;
        private int myJumpToQuestion = 0;
        private bool myAskClarification = false;
        private bool myClarificationRequired = false;

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

        public int SQCID
        {
            get { return mySQCID; }
            set { mySQCID = value; }
        }
        public int QID
        {
            get { return myQID; }
            set { myQID = value; }
        }
        public int ChoiceOrder
        {
            get { return myChoiceOrder; }
            set { myChoiceOrder = value; }
        }
        public string ChoiceText
        {
            get { return myChoiceText; }
            set { myChoiceText = value; }
        }
        public int Score
        {
            get { return myScore; }
            set { myScore = value; }
        }
        public int JumpToQuestion
        {
            get { return myJumpToQuestion; }
            set { myJumpToQuestion = value; }
        }
        public bool AskClarification
        {
            get { return myAskClarification; }
            set { myAskClarification = value; }
        }
        public bool ClarificationRequired
        {
            get { return myClarificationRequired; }
            set { myClarificationRequired = value; }
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

        public SQChoices()
        {

        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetList(string list)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@List", list);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SQChoices_GetAllInList", arrParams);
        }

        public static DataSet GetAll(int QID)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@QID", QID);
            arrParams[1] = new SqlParameter("@Echo", 0);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SQChoices_GetAll", arrParams);
        }

        public static DataSet GetAllWEcho(int QID, int Echo = 0)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@QID", QID);
            arrParams[1] = new SqlParameter("@Echo", Echo);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SQChoices_GetAll", arrParams);
        }

        public static SQChoices FetchObject(int SQCID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SQCID", SQCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SQChoices_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SQChoices result = new SQChoices();

                int _int;

                if (int.TryParse(dr["SQCID"].ToString(), out _int)) result.SQCID = _int;
                if (int.TryParse(dr["QID"].ToString(), out _int)) result.QID = _int;
                if (int.TryParse(dr["ChoiceOrder"].ToString(), out _int)) result.ChoiceOrder = _int;
                result.ChoiceText = dr["ChoiceText"].ToString();
                if (int.TryParse(dr["Score"].ToString(), out _int)) result.Score = _int;
                if (int.TryParse(dr["JumpToQuestion"].ToString(), out _int)) result.JumpToQuestion = _int;
                result.AskClarification = bool.Parse(dr["AskClarification"].ToString());
                result.ClarificationRequired = bool.Parse(dr["ClarificationRequired"].ToString());
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

        public bool Fetch(int SQCID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SQCID", SQCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SQChoices_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SQChoices result = new SQChoices();

                int _int;

                if (int.TryParse(dr["SQCID"].ToString(), out _int)) this.SQCID = _int;
                if (int.TryParse(dr["QID"].ToString(), out _int)) this.QID = _int;
                if (int.TryParse(dr["ChoiceOrder"].ToString(), out _int)) this.ChoiceOrder = _int;
                this.ChoiceText = dr["ChoiceText"].ToString();
                if (int.TryParse(dr["Score"].ToString(), out _int)) this.Score = _int;
                if (int.TryParse(dr["JumpToQuestion"].ToString(), out _int)) this.JumpToQuestion = _int;
                this.AskClarification = bool.Parse(dr["AskClarification"].ToString());
                this.ClarificationRequired = bool.Parse(dr["ClarificationRequired"].ToString());
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

        public static int Insert(SQChoices o)
        {

            SqlParameter[] arrParams = new SqlParameter[17];

            arrParams[0] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@ChoiceOrder", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceOrder, o.ChoiceOrder.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ChoiceText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceText, o.ChoiceText.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Score", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score, o.Score.GetTypeCode()));
            arrParams[4] = new SqlParameter("@JumpToQuestion", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.JumpToQuestion, o.JumpToQuestion.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AskClarification", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AskClarification, o.AskClarification.GetTypeCode()));
            arrParams[6] = new SqlParameter("@ClarificationRequired", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ClarificationRequired, o.ClarificationRequired.GetTypeCode()));
            arrParams[7] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@SQCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQCID, o.SQCID.GetTypeCode()));
            arrParams[16].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQChoices_Insert", arrParams);

            o.SQCID = int.Parse(arrParams[16].Value.ToString());

            return o.SQCID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SQChoices o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[17];

            arrParams[0] = new SqlParameter("@SQCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQCID, o.SQCID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@QID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.QID, o.QID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ChoiceOrder", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceOrder, o.ChoiceOrder.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ChoiceText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ChoiceText, o.ChoiceText.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Score", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score, o.Score.GetTypeCode()));
            arrParams[5] = new SqlParameter("@JumpToQuestion", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.JumpToQuestion, o.JumpToQuestion.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AskClarification", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AskClarification, o.AskClarification.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ClarificationRequired", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ClarificationRequired, o.ClarificationRequired.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQChoices_Update", arrParams);

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

        public static int Delete(SQChoices o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SQCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SQCID, o.SQCID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQChoices_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public static void MoveUp(int SQCID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SQCID", SQCID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQChoices_MoveUp", arrParams);
        }

        public static void MoveDn(int SQCID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SQCID", SQCID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SQChoices_MoveDn", arrParams);
        }

        #endregion

    }//end class

}//end namespace

