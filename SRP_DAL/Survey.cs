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

[Serializable]    public class Survey : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int mySID;
        private string myName = "";
        private string myLongName = "";
        private string myDescription = "";
        private string myPreamble = "";
        private int myStatus = 0;
        private int myTakenCount=0;
        private int myPatronCount=0;
        private bool myCanBeScored=false;

        private int myTenID = 0;
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

        public int SID
        {
            get { return mySID; }
            set { mySID = value; }
        }
        public string Name
        {
            get { return myName; }
            set { myName = value; }
        }
        public string LongName
        {
            get { return myLongName; }
            set { myLongName = value; }
        }
        public string Description
        {
            get { return myDescription; }
            set { myDescription = value; }
        }
        public string Preamble
        {
            get { return myPreamble; }
            set { myPreamble = value; }
        }
        public int Status
        {
            get { return myStatus; }
            set { myStatus = value; }
        }
        public int TakenCount
        {
            get { return myTakenCount; }
            set { myTakenCount = value; }
        }
        public int PatronCount
        {
            get { return myPatronCount; }
            set { myPatronCount = value; }
        }
        public bool CanBeScored
        {
            get { return myCanBeScored; }
            set { myCanBeScored = value; }
        }
        public int TenID
        {
            get { return myTenID; }
            set { myTenID = value; }
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

        public Survey()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Survey_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Survey_GetAll", arrParams);
        }

        public static DataSet GetAllFinalized()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Survey_GetAllFinalized", arrParams);
        }

        public static Survey FetchObject(int SID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SID", SID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Survey_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Survey result = new Survey();

                int _int;

                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                result.Name = dr["Name"].ToString();
                result.LongName = dr["LongName"].ToString();
                result.Description = dr["Description"].ToString();
                result.Preamble = dr["Preamble"].ToString();
                if (int.TryParse(dr["Status"].ToString(), out _int)) result.Status = _int;
                if (int.TryParse(dr["TakenCount"].ToString(), out _int)) result.TakenCount = _int;
                if (int.TryParse(dr["PatronCount"].ToString(), out _int)) result.PatronCount = _int;
                result.CanBeScored = bool.Parse(dr["CanBeScored"].ToString());

                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
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

        public bool Fetch(int SID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SID", SID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Survey_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Survey result = new Survey();

                int _int;

                if (int.TryParse(dr["SID"].ToString(), out _int)) this.SID = _int;
                this.Name = dr["Name"].ToString();
                this.LongName = dr["LongName"].ToString();
                this.Description = dr["Description"].ToString();
                this.Preamble = dr["Preamble"].ToString();
                if (int.TryParse(dr["Status"].ToString(), out _int)) this.Status = _int;
                if (int.TryParse(dr["TakenCount"].ToString(), out _int)) this.TakenCount = _int;
                if (int.TryParse(dr["PatronCount"].ToString(), out _int)) this.PatronCount = _int;
                this.CanBeScored = bool.Parse(dr["CanBeScored"].ToString());
                if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
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

        public static int Insert(Survey o)
        {

            SqlParameter[] arrParams = new SqlParameter[19];

            arrParams[0] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[1] = new SqlParameter("@LongName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LongName, o.LongName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Preamble", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Preamble, o.Preamble.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Status", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Status, o.Status.GetTypeCode()));
            arrParams[5] = new SqlParameter("@TakenCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TakenCount, o.TakenCount.GetTypeCode()));
            arrParams[6] = new SqlParameter("@PatronCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronCount, o.PatronCount.GetTypeCode()));
            arrParams[7] = new SqlParameter("@CanBeScored", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CanBeScored, o.CanBeScored.GetTypeCode()));

            arrParams[8] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[18] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[18].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Survey_Insert", arrParams);

            o.SID = int.Parse(arrParams[18].Value.ToString());

            return o.SID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Survey o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[19];

            arrParams[0] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LongName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LongName, o.LongName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Preamble", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Preamble, o.Preamble.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Status", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Status, o.Status.GetTypeCode()));
            arrParams[6] = new SqlParameter("@TakenCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TakenCount, o.TakenCount.GetTypeCode()));
            arrParams[7] = new SqlParameter("@PatronCount", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronCount, o.PatronCount.GetTypeCode()));
            arrParams[8] = new SqlParameter("@CanBeScored", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CanBeScored, o.CanBeScored.GetTypeCode()));

            arrParams[9] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Survey_Update", arrParams);

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

        public static int Delete(Survey o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Survey_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion


        public static string StatusDescription(int Status)
        {
            switch (Status)
            {
                case 1:
                    return "Work In Progress";
                case 2:
                    return "Locked / Active";
                default:
                    return "N/A";
            }
        }

        public static int GetNumQuestions(int SID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@SID", SID);
            var dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Survey_GetNumQuestions", arrParams);

            if (dr.Read())
            {
                int _int;
                int.TryParse(dr["NumQuestions"].ToString(), out _int);
                dr.Close();
                return _int;
            }
            dr.Close();
            return 0;
        }

        public static bool IsScorable(int SID)
        {
            var dsQ = SurveyQuestion.GetAll(SID);
            foreach (DataRow qRow in dsQ.Tables[0].Rows)
            {
                var qType = Convert.ToInt32(qRow["QType"]);
                if (qType == 3) return false;  // cannot score a test that has multiple choice

                var isRequired = Convert.ToBoolean(qRow["IsRequired"]);
                if ((qType == 2 && !isRequired) || (qType == 4 && !isRequired)) return false;  // cannot score a test that does not have all questions/answers required
            }

            return true;
        }

        public static int MaxScore(int SID)
        {
            var maxScore = 0;
            var dsQ = SurveyQuestion.GetAll(SID);
            foreach (DataRow qRow in dsQ.Tables[0].Rows)
            {
                var qType = Convert.ToInt32(qRow["QType"]);
                if (qType == 3) return 0;  // cannot score a test that has multiple choice

                var isRequired = Convert.ToBoolean(qRow["IsRequired"]);
                if ((qType == 2 && !isRequired) || (qType == 4 && !isRequired)) return 0;  // cannot score a test that does not have all questions/answers required

                var isCheckbox = Convert.ToInt32(qRow["DisplayControl"]) == 1;
                var QID = Convert.ToInt32(qRow["QID"]);
                var qScore = 0;
                
                if (qType == 2 || qType == 4)
                {
                    var dsA = SQChoices.GetAll(QID);
                    var maxAScore = 0;
                    foreach (DataRow aRow in dsA.Tables[0].Rows)
                    {
                        var score = Convert.ToInt32(aRow["Score"]);
                        if (isCheckbox)
                        {
                            qScore += score;
                        }
                        else
                        {
                           if (score > maxAScore) maxAScore = score;
                        }
                    }
                    if (!isCheckbox) qScore += maxAScore;
                }
                if (qType == 4)
                {
                    //Matrix, how many lines?

                    var dsML = SQMatrixLines.GetAll(QID);
                    var numLines = dsML.Tables[0].Rows.Count;
                    qScore = qScore*numLines;
                }
                maxScore += qScore;
            }

            return maxScore;
        }


        public static string Source(int i)
        {
            switch (i)
            {
                case 1:
                    return "Program Pre-Test";
                case 2:
                    return "Program Post-Test";
                case 3:
                    return "Game";
                case 4:
                    return "Book List";
                case 5:
                    return "Event";
                case 6:
                    return "Reading Log";
                default:
                    return "N/A";
            }
        }

        public static int Source(string s)
        {
            switch (s.ToLower())
            {
                case "program pre-test":
                    return 1;
                case "program post-test":
                    return 2;
                case "game":
                    return 3;
                case "book list":
                    return 4;
                case "event":
                    return 5;
                case "reading log":
                    return 6;
                default:
                    return 99;
            }
        }

    }//end class

}//end namespace

