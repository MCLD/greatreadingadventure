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
using System.Collections.Generic;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

    [Serializable]
    public class LibraryCrosswalk : EntityBase
    {
        #region Private Variables
        private static string conn = GlobalUtilities.SRPDB;
        #endregion

        #region Accessors
        public int ID { get; set; }
        public int BranchID { get; set; }
        public int DistrictID { get; set; }
        public string City { get; set; }
        public int TenID { get; set; }
        public int FldInt1 { get; set; }
        public int FldInt2 { get; set; }
        public int FldInt3 { get; set; }
        public bool FldBit1 { get; set; }
        public bool FldBit2 { get; set; }
        public bool FldBit3 { get; set; }
        public string FldText1 { get; set; }
        public string FldText2 { get; set; }
        public string FldText3 { get; set; }
        public string BranchLink { get; set; }
        public string BranchAddress { get; set; }
        public string BranchTelephone { get; set; }

        #endregion

        #region Constructors

        public LibraryCrosswalk()
        {
            this.TenID = (HttpContext.Current.Session["TenantID"] == null ||
                 HttpContext.Current.Session["TenantID"].ToString() == ""
                     ? -1
                     : (int)HttpContext.Current.Session["TenantID"]);

            this.City = string.Empty;
            this.FldText1 = string.Empty;
            this.FldText2 = string.Empty;
            this.FldText3 = string.Empty;
            this.BranchLink = string.Empty;
            this.BranchAddress = string.Empty;
            this.BranchTelephone = string.Empty;
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
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_GetAll", arrParams);
        }

        public static DataSet GetFilteredDistrictDDValues(string city)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@City", string.Empty);
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_GetFilteredDistrictDDValues", arrParams);
        }

        public static DataSet GetFilteredBranchDDValues(int districtID, string city)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@DistrictID", districtID);
            arrParams[1] = new SqlParameter("@City", string.Empty);
            arrParams[2] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_GetFilteredBranchDDValues", arrParams);
        }

        public static LibraryCrosswalk FetchObjectByLibraryID(int ID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", ID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_GetByLibraryID", arrParams);

            if (dr.Read())
            {

                // declare return value

                LibraryCrosswalk result = new LibraryCrosswalk();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["ID"].ToString(), out _int)) result.ID = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) result.BranchID = _int;
                if (int.TryParse(dr["DistrictID"].ToString(), out _int)) result.DistrictID = _int;
                result.City = dr["City"].ToString();
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

                result.BranchLink = dr["BranchLink"].ToString();
                result.BranchAddress = dr["BranchAddress"].ToString();
                result.BranchTelephone = dr["BranchTelephone"].ToString();

                dr.Close();

                return result;
            }

            dr.Close();

            return null;
        }


        public static LibraryCrosswalk FetchObject(int ID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", ID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                LibraryCrosswalk result = new LibraryCrosswalk();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["ID"].ToString(), out _int)) result.ID = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) result.BranchID = _int;
                if (int.TryParse(dr["DistrictID"].ToString(), out _int)) result.DistrictID = _int;
                result.City = dr["City"].ToString();

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

                result.BranchLink = dr["BranchLink"].ToString();
                result.BranchAddress = dr["BranchAddress"].ToString();
                result.BranchTelephone = dr["BranchTelephone"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int ID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", ID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                LibraryCrosswalk result = new LibraryCrosswalk();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["ID"].ToString(), out _int)) this.ID = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) this.BranchID = _int;
                if (int.TryParse(dr["DistrictID"].ToString(), out _int)) this.DistrictID = _int;
                this.City = dr["City"].ToString();

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

                this.BranchLink = dr["BranchLink"].ToString();
                this.BranchAddress = dr["BranchAddress"].ToString();
                this.BranchTelephone = dr["BranchTelephone"].ToString();

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

        public static int Insert(LibraryCrosswalk o)
        {

            var arrParams = new List<SqlParameter>();
            arrParams.Add(new SqlParameter("@BranchID", GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DistrictID", GlobalUtilities.DBSafeValue(o.DistrictID, o.DistrictID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City", GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode())));

            arrParams.Add(new SqlParameter("@TenID", GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode())));

            arrParams.Add(new SqlParameter("@BranchLink", GlobalUtilities.DBSafeValue(o.BranchLink, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchAddress", GlobalUtilities.DBSafeValue(o.BranchAddress, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchTelephone", GlobalUtilities.DBSafeValue(o.BranchTelephone, o.FldText3.GetTypeCode())));

            var param = new SqlParameter("@ID", GlobalUtilities.DBSafeValue(o.ID, o.ID.GetTypeCode()));
            param.Direction = ParameterDirection.Output;
            arrParams.Add(param);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_Insert", arrParams.ToArray());

            o.ID = int.Parse(param.Value.ToString());

            return o.ID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(LibraryCrosswalk o)
        {

            int iReturn = -1; //assume the worst

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@ID", GlobalUtilities.DBSafeValue(o.ID, o.ID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchID", GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DistrictID", GlobalUtilities.DBSafeValue(o.DistrictID, o.DistrictID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City", GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode())));

            arrParams.Add(new SqlParameter("@TenID", GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode())));

            arrParams.Add(new SqlParameter("@BranchLink", GlobalUtilities.DBSafeValue(o.BranchLink, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchAddress", GlobalUtilities.DBSafeValue(o.BranchAddress, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchTelephone", GlobalUtilities.DBSafeValue(o.BranchTelephone, o.FldText3.GetTypeCode())));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_Update", arrParams.ToArray());

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

        public static int Delete(LibraryCrosswalk o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", GlobalUtilities.DBSafeValue(o.ID, o.ID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public static DataSet GetExport()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_LibraryCrosswalk_Export", arrParams);
        }

        #endregion

    }//end class

}//end namespace

