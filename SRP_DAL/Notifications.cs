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
using System.Collections.Generic;

namespace GRA.SRP.DAL {

    [Serializable]
    public class Notifications : EntityBase {
        // Note: PID = 0 is a system notification (to the admins or from the admins)
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myNID;
        private int myPID_To;
        private int myPID_From;
        private bool myisQuestion = false;
        private string mySubject = "";
        private string myBody = "";
        private DateTime myAddedDate;
        private string myAddedUser = "N/A";
        private DateTime myLastModDate;
        private string myLastModUser = "N/A";

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

        private bool myisUnread = true;
        #endregion

        #region Accessors

        public int NID
        {
            get { return myNID; }
            set { myNID = value; }
        }
        public int PID_To
        {
            get { return myPID_To; }
            set { myPID_To = value; }
        }
        public int PID_From
        {
            get { return myPID_From; }
            set { myPID_From = value; }
        }
        public bool isQuestion
        {
            get { return myisQuestion; }
            set { myisQuestion = value; }
        }
        public string Subject
        {
            get { return mySubject; }
            set { mySubject = value; }
        }
        public string Body
        {
            get { return myBody; }
            set { myBody = value; }
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

        public bool isUnread
        {
            get { return myisUnread; }
            set { myisUnread = value; }
        }
        #endregion

        #region Constructors

        public Notifications() {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll() {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Notifications_GetAll", arrParams);
        }

        public static DataSet GetAllToPatron(int PID) {
            //SqlDataReader dr;
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Notifications_GetAllToPatron", arrParams);
        }

        public static DataSet GetAllUnreadToPatron(int PID) {
            //SqlDataReader dr;
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Notifications_GetAllUnreadToPatron", arrParams);
        }

        public static DataSet GetAllQuestions() {
            //SqlDataReader dr;
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", 0);
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Notifications_GetAllToPatron", arrParams);
        }


        public static DataSet GetAllFromPatron(int PID) {
            //SqlDataReader dr;
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Notifications_GetAllFromPatron", arrParams);
        }

        public static DataSet GetAllToOrFromPatron(int PID) {
            var arrParams = new List<SqlParameter>();
            arrParams.Add(new SqlParameter("@PID", PID));
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Notifications_GetAllToOrFromPatron", arrParams.ToArray());
        }


        public static Notifications FetchObject(int NID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@NID", NID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Notifications_GetByID", arrParams);

            if(dr.Read()) {

                // declare return value

                Notifications result = new Notifications();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["NID"].ToString(), out _int))
                    result.NID = _int;
                if(int.TryParse(dr["PID_To"].ToString(), out _int))
                    result.PID_To = _int;
                if(int.TryParse(dr["PID_From"].ToString(), out _int))
                    result.PID_From = _int;
                result.isQuestion = bool.Parse(dr["isQuestion"].ToString());
                result.Subject = dr["Subject"].ToString();
                result.Body = dr["Body"].ToString();
                if(DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();
                if(DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();

                if(int.TryParse(dr["TenID"].ToString(), out _int))
                    result.TenID = _int;
                if(int.TryParse(dr["FldInt1"].ToString(), out _int))
                    result.FldInt1 = _int;
                if(int.TryParse(dr["FldInt2"].ToString(), out _int))
                    result.FldInt2 = _int;
                if(int.TryParse(dr["FldInt3"].ToString(), out _int))
                    result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();

                result.isUnread = bool.Parse(dr["isUnread"].ToString());

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int NID) {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@NID", NID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Notifications_GetByID", arrParams);

            if(dr.Read()) {

                // declare return value

                Notifications result = new Notifications();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if(int.TryParse(dr["NID"].ToString(), out _int))
                    this.NID = _int;
                if(int.TryParse(dr["PID_To"].ToString(), out _int))
                    this.PID_To = _int;
                if(int.TryParse(dr["PID_From"].ToString(), out _int))
                    this.PID_From = _int;
                this.isQuestion = bool.Parse(dr["isQuestion"].ToString());
                this.Subject = dr["Subject"].ToString();
                this.Body = dr["Body"].ToString();
                if(DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();
                if(DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();

                if(int.TryParse(dr["TenID"].ToString(), out _int))
                    this.TenID = _int;
                if(int.TryParse(dr["FldInt1"].ToString(), out _int))
                    this.FldInt1 = _int;
                if(int.TryParse(dr["FldInt2"].ToString(), out _int))
                    this.FldInt2 = _int;
                if(int.TryParse(dr["FldInt3"].ToString(), out _int))
                    this.FldInt3 = _int;
                this.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                this.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                this.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                this.FldText1 = dr["FldText1"].ToString();
                this.FldText2 = dr["FldText2"].ToString();
                this.FldText3 = dr["FldText3"].ToString();

                this.isUnread = bool.Parse(dr["isUnread"].ToString());

                dr.Close();

                return true;

            }

            dr.Close();

            return false;

        }

        public int Insert() {

            return Insert(this);

        }

        public static int Insert(Notifications o) {

            SqlParameter[] arrParams = new SqlParameter[21];

            arrParams[0] = new SqlParameter("@PID_To", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID_To, o.PID_To.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID_From", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID_From, o.PID_From.GetTypeCode()));
            arrParams[2] = new SqlParameter("@isQuestion", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isQuestion, o.isQuestion.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Subject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Subject, o.Subject.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Body", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Body, o.Body.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));

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

            arrParams[19] = new SqlParameter("@isUnread", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isUnread, o.isUnread.GetTypeCode()));

            arrParams[20] = new SqlParameter("@NID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NID, o.NID.GetTypeCode()));
            arrParams[20].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Notifications_Insert", arrParams);

            o.NID = int.Parse(arrParams[20].Value.ToString());

            return o.NID;

        }

        public int Update() {

            return Update(this);

        }

        public static int Update(Notifications o) {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[21];

            arrParams[0] = new SqlParameter("@NID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NID, o.NID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID_To", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID_To, o.PID_To.GetTypeCode()));
            arrParams[2] = new SqlParameter("@PID_From", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID_From, o.PID_From.GetTypeCode()));
            arrParams[3] = new SqlParameter("@isQuestion", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isQuestion, o.isQuestion.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Subject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Subject, o.Subject.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Body", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Body, o.Body.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));

            arrParams[10] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[20] = new SqlParameter("@isUnread", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isUnread, o.isUnread.GetTypeCode()));

            try {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Notifications_Update", arrParams);

            } catch(SqlException exx) {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public int Delete() {

            return Delete(this);

        }

        public static int Delete(Notifications o) {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@NID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NID, o.NID.GetTypeCode()));

            try {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Notifications_Delete", arrParams);

            } catch(SqlException exx) {
                "GRA.SRP.DAL.Notifications".Log().Error("SQL error deleting notification {0}: {1} - {2}",
                                                        o.NID,
                                                        exx.Message,
                                                        exx.StackTrace);
                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

    }//end class

}//end namespace

