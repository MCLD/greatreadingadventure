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
using GRA.Tools;

namespace GRA.SRP.DAL
{

    [Serializable] public class BookList : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        public int BLID { get; set; }
        public string AdminName { get; set; }
        public string ListName { get; set; }
        public string AdminDescription { get; set; }
        public string Description { get; set; }
        public int LiteracyLevel1 { get; set; }
        public int LiteracyLevel2 { get; set; }
        public int ProgID { get; set; }
        public int LibraryID { get; set; }
        public int AwardBadgeID { get; set; }
        public int AwardPoints { get; set; }
        public DateTime LastModDate { get; set; }
        public string LastModUser { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedUser { get; set; }
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

        public int NumBooksToComplete { get; set; }

        #endregion

        #region Constructors

        public BookList()
        {
            LastModUser = string.Empty;
            AddedUser = string.Empty;
            FldText1 = string.Empty;
            FldText2 = string.Empty;
            FldText3 = string.Empty;

            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetForDisplay(int PID, string searchText)
        {
            var TenID =
                (HttpContext.Current.Session["TenantID"] == null ||
                 HttpContext.Current.Session["TenantID"].ToString() == ""
                     ? -1
                     : (int)HttpContext.Current.Session["TenantID"]);
            return GetForDisplay(PID, TenID, searchText);
        }

        public static DataSet GetForDisplay(int PID, int TenID, string searchText)
        {
            var arrParams = new List<SqlParameter>();
            arrParams.Add(new SqlParameter("@PID", PID));
            arrParams.Add(new SqlParameter("@TenID", TenID));
            if(!string.IsNullOrWhiteSpace(searchText))
            {
                arrParams.Add(new SqlParameter("@SearchText",
                    new DatabaseTools().PrepareSearchString(searchText.Trim())));
            }
            return SqlHelper.ExecuteDataset(conn,
                CommandType.StoredProcedure,
                "app_BookList_GetForDisplay",
                arrParams.ToArray());
        }


        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_BookList_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_BookList_GetAll", arrParams);
        }

        public static BookList FetchObject(int BLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BLID", BLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_BookList_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                BookList result = new BookList();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BLID"].ToString(), out _int)) result.BLID = _int;
                result.AdminName = dr["AdminName"].ToString();
                result.ListName = dr["ListName"].ToString();
                result.AdminDescription = dr["AdminDescription"].ToString();
                result.Description = dr["Description"].ToString();
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) result.LiteracyLevel2 = _int;
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) result.ProgID = _int;
                if (int.TryParse(dr["LibraryID"].ToString(), out _int)) result.LibraryID = _int;
                if (int.TryParse(dr["AwardBadgeID"].ToString(), out _int)) result.AwardBadgeID = _int;
                if (int.TryParse(dr["AwardPoints"].ToString(), out _int)) result.AwardPoints = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

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

                if (int.TryParse(dr["NumBooksToComplete"].ToString(), out _int)) result.NumBooksToComplete = _int;
                

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int BLID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BLID", BLID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_BookList_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                BookList result = new BookList();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BLID"].ToString(), out _int)) this.BLID = _int;
                this.AdminName = dr["AdminName"].ToString();
                this.ListName = dr["ListName"].ToString();
                this.AdminDescription = dr["AdminDescription"].ToString();
                this.Description = dr["Description"].ToString();
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) this.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) this.LiteracyLevel2 = _int;
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) this.ProgID = _int;
                if (int.TryParse(dr["LibraryID"].ToString(), out _int)) this.LibraryID = _int;
                if (int.TryParse(dr["AwardBadgeID"].ToString(), out _int)) this.AwardBadgeID = _int;
                if (int.TryParse(dr["AwardPoints"].ToString(), out _int)) this.AwardPoints = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

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

                if (int.TryParse(dr["NumBooksToComplete"].ToString(), out _int)) this.NumBooksToComplete = _int;
                
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

        public static int Insert(BookList o)
        {
            List<SqlParameter> arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ListName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ListName, o.ListName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AdminDescription", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminDescription, o.AdminDescription.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryID, o.LibraryID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AwardBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeID, o.AwardBadgeID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AwardPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardPoints, o.AwardPoints.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

            arrParams.Add(new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode())));

            arrParams.Add(new SqlParameter("@NumBooksToComplete", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumBooksToComplete, o.NumBooksToComplete.GetTypeCode())));

            SqlParameter param = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            param.Direction = ParameterDirection.Output;
            arrParams.Add(param);

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookList_Insert", arrParams.ToArray());

            o.BLID = int.Parse(param.Value.ToString());

            return o.BLID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(BookList o)
        {

            int iReturn = -1; //assume the worst

            List<SqlParameter> arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ListName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ListName, o.ListName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AdminDescription", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminDescription, o.AdminDescription.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LiteracyLevel2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LibraryID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryID, o.LibraryID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AwardBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeID, o.AwardBadgeID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AwardPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardPoints, o.AwardPoints.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

            arrParams.Add(new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode())));

            arrParams.Add(new SqlParameter("@NumBooksToComplete", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumBooksToComplete, o.NumBooksToComplete.GetTypeCode())));

            try
            {
                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookList_Update", arrParams.ToArray());
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

        public static int Delete(BookList o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                        (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                -1 :
                                (int)HttpContext.Current.Session["TenantID"])
                    );
            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookList_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

        public static DataSet GetFiltered(string searchText, int branchId)
        {
            var arrParams = new List<SqlParameter>();
            var tenantId = HttpContext.Current.Session["TenantID"];
            arrParams.Add(new SqlParameter("@TenID",
                            tenantId == null || string.IsNullOrEmpty(tenantId.ToString())
                                ? -1
                                : (int)tenantId));
            if (!string.IsNullOrEmpty(searchText))
            {
                if (!searchText.StartsWith("%"))
                {
                    searchText = string.Format("%{0}", searchText);
                }
                if (!searchText.EndsWith("%"))
                {
                    searchText = string.Format("{0}%", searchText);
                }
                arrParams.Add(new SqlParameter("@SearchText", searchText));
            }
            if (branchId > 0)
            {
                arrParams.Add(new SqlParameter("@BranchId", branchId));
            }

            return SqlHelper.ExecuteDataset(conn,
                CommandType.StoredProcedure,
                "app_BookList_Filter",
                arrParams.ToArray());
        }

    }//end class

}//end namespace

