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
using System.Diagnostics;

namespace GRA.SRP.DAL
{

    public class ProgramCodes : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPCID;
        private int myPID;
        private int myCodeNumber;
        private string myCodeValue = "";
        private bool myisUsed;
        private DateTime myDateCreated;
        private DateTime myDateUsed;
        private int myPatronId;
        private string myShortCode = "";

        #endregion

        #region Accessors

        public int PCID {
            get { return myPCID; }
            set { myPCID = value; }
        }
        public int PID {
            get { return myPID; }
            set { myPID = value; }
        }
        public int CodeNumber {
            get { return myCodeNumber; }
            set { myCodeNumber = value; }
        }
        public string CodeValue {
            get { return myCodeValue; }
            set { myCodeValue = value; }
        }
        public string ShortCode {
            get { return myShortCode; }
            set { myShortCode = value; }
        }
        public bool isUsed {
            get { return myisUsed; }
            set { myisUsed = value; }
        }
        public DateTime DateCreated {
            get { return myDateCreated; }
            set { myDateCreated = value; }
        }
        public DateTime DateUsed {
            get { return myDateUsed; }
            set { myDateUsed = value; }
        }
        public int PatronId {
            get { return myPatronId; }
            set { myPatronId = value; }
        }

        #endregion

        #region Constructors

        public ProgramCodes()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetExportList(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramCodes_GetExportList", arrParams);
        }

        public static DataSet GetAllByProgram(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramCodes_GetAllByProgram", arrParams);
        }

        public static DataSet GetProgramStats(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramCodes_Stats", arrParams);
        }

        public static DataSet GetAllForPatron(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramCodes_GetAllForPatron", arrParams);
        }

        public static string AssignCodeForPatron(int pid, int patronId)
        {
            string ret = "";


            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@PID", pid);
            arrParams[1] = new SqlParameter("@PatronId", patronId);

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramCodes_AssignCodeForPatron", arrParams);


            if (ds.Tables[0].Rows.Count > 0)
            {
                //ret = ds.Tables[0].Rows[1]["CodeValue"].ToString();
                ret = ds.Tables[0].Rows[0]["ShortCode"].ToString();
            }

            return ret;
        }

        public static ProgramCodes FetchObject(int PCID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PCID", PCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramCodes_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                ProgramCodes result = new ProgramCodes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PCID"].ToString(), out _int)) result.PCID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) result.PID = _int;
                if (int.TryParse(dr["CodeNumber"].ToString(), out _int)) result.CodeNumber = _int;
                result.CodeValue = dr["CodeValue"].ToString();
                result.ShortCode = dr["ShortCode"].ToString();
                result.isUsed = bool.Parse(dr["isUsed"].ToString());
                if (DateTime.TryParse(dr["DateCreated"].ToString(), out _datetime)) result.DateCreated = _datetime;
                if (DateTime.TryParse(dr["DateUsed"].ToString(), out _datetime)) result.DateUsed = _datetime;
                if (int.TryParse(dr["PatronId"].ToString(), out _int)) result.PatronId = _int;

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PCID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PCID", PCID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramCodes_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                ProgramCodes result = new ProgramCodes();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PCID"].ToString(), out _int)) this.PCID = _int;
                if (int.TryParse(dr["PID"].ToString(), out _int)) this.PID = _int;
                if (int.TryParse(dr["CodeNumber"].ToString(), out _int)) this.CodeNumber = _int;
                this.CodeValue = dr["CodeValue"].ToString();
                this.ShortCode = dr["ShortCode"].ToString();
                this.isUsed = bool.Parse(dr["isUsed"].ToString());
                if (DateTime.TryParse(dr["DateCreated"].ToString(), out _datetime)) this.DateCreated = _datetime;
                if (DateTime.TryParse(dr["DateUsed"].ToString(), out _datetime)) this.DateUsed = _datetime;
                if (int.TryParse(dr["PatronId"].ToString(), out _int)) this.PatronId = _int;

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

        public static int Insert(ProgramCodes o)
        {

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@CodeNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CodeNumber, o.CodeNumber.GetTypeCode()));
            arrParams[2] = new SqlParameter("@CodeValue", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CodeValue, o.CodeValue.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ShortCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShortCode, o.ShortCode.GetTypeCode()));
            arrParams[4] = new SqlParameter("@isUsed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isUsed, o.isUsed.GetTypeCode()));
            arrParams[5] = new SqlParameter("@DateCreated", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateCreated, o.DateCreated.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DateUsed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateUsed, o.DateUsed.GetTypeCode()));
            arrParams[7] = new SqlParameter("@PatronId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronId, o.PatronId.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCID, o.PCID.GetTypeCode()));
            arrParams[8].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramCodes_Insert", arrParams);

            o.PCID = int.Parse(arrParams[8].Value.ToString());

            return o.PCID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(ProgramCodes o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[9];

            arrParams[0] = new SqlParameter("@PCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCID, o.PCID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PID, o.PID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@CodeNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CodeNumber, o.CodeNumber.GetTypeCode()));
            arrParams[3] = new SqlParameter("@CodeValue", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CodeValue, o.CodeValue.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ShortCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShortCode, o.ShortCode.GetTypeCode()));
            arrParams[5] = new SqlParameter("@isUsed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isUsed, o.isUsed.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DateCreated", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateCreated, o.DateCreated.GetTypeCode()));
            arrParams[7] = new SqlParameter("@DateUsed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DateUsed, o.DateUsed.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PatronId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PatronId, o.PatronId.GetTypeCode()));

            try

            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramCodes_Update", arrParams);

            }

            catch (SqlException ex)

            {
                "GRA.SRP.DAL.ProgramCodes".Log().Error("Error in Update: {0} - {1}",
                    ex.Message,
                    ex.StackTrace);
            }

            return iReturn;

        }

        public int Delete()
        {

            return Delete(this);

        }

        public static int Delete(ProgramCodes o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PCID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCID, o.PCID.GetTypeCode()));

            try

            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramCodes_Delete", arrParams);

            }

            catch (SqlException ex)
            {
                "GRA.SRP.DAL.ProgramCodes".Log().Error("Error in Delete: {0} - {1}",
                    ex.Message,
                    ex.StackTrace);
            }

            return iReturn;

        }

        private const string SelectExistingCodesCommand = "SELECT [ShortCode] FROM [ProgramCodes]";
        public static string Generate(int start, int end, int PID)
        {
            int inserted = 0;
            var existingCodes = new HashSet<string>();
            using (var sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();

                Stopwatch s = new Stopwatch();
                s.Start();
                using (var sqlCmd = new SqlCommand(SelectExistingCodesCommand, sqlConnection))
                {
                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingCodes.Add(reader.GetString(0));
                        }
                        reader.Close();
                    }
                }
                s.Stop();
                "ProgramCodes".Log().Info("Loaded {0} existing codes in {1:c}",
                    existingCodes.Count,
                    s.Elapsed);

                s.Reset();
                s.Start();
                using (var codeTool = new Tools.ProgramCode(15))
                {
                    using (var sqlCmd = new SqlCommand("app_ProgramCodes_Insert", sqlConnection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "PCID",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Output
                        });
                        sqlCmd.Parameters.AddWithValue("PID", PID);
                        sqlCmd.Parameters.Add("CodeNumber", SqlDbType.Int);
                        sqlCmd.Parameters.AddWithValue("CodeValue", DBNull.Value);
                        sqlCmd.Parameters.AddWithValue("isUsed", false);
                        sqlCmd.Parameters.AddWithValue("DateCreated", DateTime.Now);
                        sqlCmd.Parameters.AddWithValue("DateUsed", DBNull.Value);
                        sqlCmd.Parameters.AddWithValue("PatronId", 0);
                        sqlCmd.Parameters.Add("ShortCode", SqlDbType.VarChar, 20);
                        for (int count = start; count < end + 1; count++)
                        {
                            string code = null;
                            do
                            {
                                code = codeTool.Generate();
                            } while (existingCodes.Contains(code));
                            existingCodes.Add(code);
                            sqlCmd.Parameters["CodeNumber"].Value = count;
                            sqlCmd.Parameters["ShortCode"].Value = code;
                            inserted += sqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                s.Stop();

                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        "ProgramCodes".Log().Error("Problem closing SQL connection in Generate: {0} - {1}",
                            ex.Message,
                            ex.StackTrace);
                    }
                }

                string result = string.Format("Inserted {0} new codes in {1:mm\\:ss} ({2:f0} codes per second)",
                    inserted,
                    s.Elapsed,
                    inserted / s.Elapsed.TotalSeconds);
                "ProgramCodes".Log().Info(result);
                return result;
            }
        }

        public static int GetCountByTenantId(int tenantId)
        {
            var tenantParameter = new SqlParameter[] { new SqlParameter("@TenID", tenantId) };
            try
            {
                return SqlHelper.ExecuteScalar(conn,
                    CommandType.StoredProcedure,
                    "app_ProgramCodes_GetAllByTenantId",
                    tenantParameter) as int? ?? 0;
            }
            catch (SqlException ex)
            {
                "GRA.SRP.DAL.ProgramCodes".Log().Error("Error in GetCountyByTenantId: {0} - {1}",
                    ex.Message,
                    ex.StackTrace);
            }
            return 0;
        }

        #endregion

    }//end class

}//end namespace

