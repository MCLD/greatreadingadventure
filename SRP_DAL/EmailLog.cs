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

namespace GRA.SRP.DAL
{

[Serializable]    public class EmailLog
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myEID;
        private DateTime mySentDateTime;
        private string mySentFrom;
        private string mySentTo;
        private string mySubject;
        private string myBody;

        #endregion

        #region Accessors

        public int EID
        {
            get { return myEID; }
            set { myEID = value; }
        }
        public DateTime SentDateTime
        {
            get { return mySentDateTime; }
            set { mySentDateTime = value; }
        }
        public string SentFrom
        {
            get { return mySentFrom; }
            set { mySentFrom = value; }
        }
        public string SentTo
        {
            get { return mySentTo; }
            set { mySentTo = value; }
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

        #endregion

        #region Constructors

        public EmailLog()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SentEmailLog_GetAll");
        }

        public EmailLog GetEmailLog(int EID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@EID", EID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SentEmailLog_GetByID", arrParams);

            try
            {

                dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SentEmailLog_GetByID", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            if (dr.Read())
            {

                // declare return value

                EmailLog result = new EmailLog();

                //DateTime _datetime;

                //int _int;

                //decimal _decimal;


                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public int Insert()
        {

            return Insert(this);

        }

        public static int Insert(EmailLog o)
        {

            SqlParameter[] arrParams = new SqlParameter[6];

            arrParams[0] = new SqlParameter("@SentDateTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SentDateTime, o.SentDateTime.GetTypeCode()));
            arrParams[1] = new SqlParameter("@SentFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SentFrom, o.SentFrom.GetTypeCode()));
            arrParams[2] = new SqlParameter("@SentTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SentTo, o.SentTo.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Subject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Subject, o.Subject.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Body", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Body, o.Body.GetTypeCode()));
            arrParams[5] = new SqlParameter("@EID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));
            arrParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SentEmailLog_Insert", arrParams);

            return int.Parse(arrParams[5].Value.ToString());

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(EmailLog o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[6];

            arrParams[0] = new SqlParameter("@EID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@SentDateTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SentDateTime, o.SentDateTime.GetTypeCode()));
            arrParams[2] = new SqlParameter("@SentFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SentFrom, o.SentFrom.GetTypeCode()));
            arrParams[3] = new SqlParameter("@SentTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SentTo, o.SentTo.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Subject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Subject, o.Subject.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Body", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Body, o.Body.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SentEmailLog_Update", arrParams);

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

        public static int Delete(EmailLog o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@EID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SentEmailLog_Delete", arrParams);

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

