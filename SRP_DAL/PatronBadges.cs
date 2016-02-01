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

namespace GRA.SRP.DAL
{

[Serializable]    public class PatronBadges : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPBID;
        private int myPID;
        private int myBadgeID;
        private DateTime myDateEarned;

        #endregion

        #region Accessors

        public int PBID
        {
            get { return myPBID; }
            set { myPBID = value; }
        }
        public int PID
        {
            get { return myPID; }
            set { myPID = value; }
        }
        public int BadgeID
        {
            get { return myBadgeID; }
            set { myBadgeID = value; }
        }
        public DateTime DateEarned
        {
            get { return myDateEarned; }
            set { myDateEarned = value; }
        }

        #endregion

        #region Constructors

        public PatronBadges()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PatronBadges_GetAll", arrParams);
        }

        public static DataSet GetTop(int PID, int count) {
            string query = string.Format("SELECT TOP {0} pb.*, b.UserName as [Title] FROM [PatronBadges] pb INNER JOIN [Badge] b ON pb.[BadgeID] = b.[BID] WHERE [PID] = @PID ORDER BY [PBID] DESC",
                                         count.ToString());
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PID", PID.ToString()));
            return SqlHelper.ExecuteDataset(conn, CommandType.Text, query, parameters.ToArray());
        }

        public static PatronBadges FetchObject(int PBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PBID", PBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronBadges_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronBadges result = new PatronBadges();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PBID"].ToString(), out _int)) result.PBID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (DateTime.TryParse(dr["DateEarned"].ToString(), out _datetime)) result.DateEarned = _datetime;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PBID", PBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PatronBadges_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PatronBadges result = new PatronBadges();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PBID"].ToString(), out _int)) this.PBID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) this.BadgeID = _int;
                if (DateTime.TryParse(dr["DateEarned"].ToString(), out _datetime)) this.DateEarned = _datetime;

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

        public static int Insert(PatronBadges o)
        {

            SqlParameter[] arrParams = new SqlParameter[4];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@DateEarned", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateEarned, o.DateEarned.GetTypeCode()));
            arrParams[3] = new SqlParameter("@PBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBID, o.PBID.GetTypeCode()));
            arrParams[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronBadges_Insert", arrParams);

            o.PBID = int.Parse(arrParams[3].Value.ToString());

            return o.PBID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PatronBadges o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[4];

            arrParams[0] = new SqlParameter("@PBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBID, o.PBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@DateEarned", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateEarned, o.DateEarned.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronBadges_Update", arrParams);

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

        public static int Delete(PatronBadges o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PBID, o.PBID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PatronBadges_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                if (BadgeID == 0)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Badge", "Badge", "You must select a badge.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }
            return (ErrorCodes.Count == 0);
            //return true;
        }


        #endregion

    }//end class

}//end namespace

