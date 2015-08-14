using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
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

[Serializable]    public class Offer : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myOID;
        private bool myisEnabled;
        private string myAdminName="";
        private string myTitle = "";
        private bool myExternalRedirectFlag;
        private string myRedirectURL = "";
        private int myMaxImpressions=0;
        private int myTotalImpressions=0;
        private string mySerialPrefix = "";
        private string myZipCode = "";
        private int myAgeStart=0;
        private int myAgeEnd=0;
        private int myProgramId=0;
        private int myBranchId=0;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

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

        public int OID
        {
            get { return myOID; }
            set { myOID = value; }
        }
        public bool isEnabled
        {
            get { return myisEnabled; }
            set { myisEnabled = value; }
        }
        public string AdminName
        {
            get { return myAdminName; }
            set { myAdminName = value; }
        }
        public string Title
        {
            get { return myTitle; }
            set { myTitle = value; }
        }
        public bool ExternalRedirectFlag
        {
            get { return myExternalRedirectFlag; }
            set { myExternalRedirectFlag = value; }
        }
        public string RedirectURL
        {
            get { return myRedirectURL; }
            set { myRedirectURL = value; }
        }
        public int MaxImpressions
        {
            get { return myMaxImpressions; }
            set { myMaxImpressions = value; }
        }
        public int TotalImpressions
        {
            get { return myTotalImpressions; }
            set { myTotalImpressions = value; }
        }
        public string SerialPrefix
        {
            get { return mySerialPrefix; }
            set { mySerialPrefix = value; }
        }
        public string ZipCode
        {
            get { return myZipCode; }
            set { myZipCode = value; }
        }
        public int AgeStart
        {
            get { return myAgeStart; }
            set { myAgeStart = value; }
        }
        public int AgeEnd
        {
            get { return myAgeEnd; }
            set { myAgeEnd = value; }
        }
        public int ProgramId
        {
            get { return myProgramId; }
            set { myProgramId = value; }
        }
        public int BranchId
        {
            get { return myBranchId; }
            set { myBranchId = value; }
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

        public Offer()
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
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Offer_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",TenID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Offer_GetAll", arrParams);
        }


        public static DataSet GetForDisplay(int PID)
        {
            var TenID =
                (HttpContext.Current.Session["TenantID"] == null ||
                 HttpContext.Current.Session["TenantID"].ToString() == ""
                     ? -1
                     : (int) HttpContext.Current.Session["TenantID"]);
            return GetForDisplay(PID, TenID);
        }

        public static DataSet GetForDisplay(int PID, int TenID)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@TenID", TenID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Offers_GetForDisplay", arrParams);
        }

        public static Offer FetchObject(int OID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@OID", OID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Offer_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Offer result = new Offer();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["OID"].ToString(), out _int)) result.OID = _int;
                result.isEnabled = bool.Parse(dr["isEnabled"].ToString());
                result.AdminName = dr["AdminName"].ToString();
                result.Title = dr["Title"].ToString();
                result.ExternalRedirectFlag = bool.Parse(dr["ExternalRedirectFlag"].ToString());
                result.RedirectURL = dr["RedirectURL"].ToString();
                if (int.TryParse(dr["MaxImpressions"].ToString(), out _int)) result.MaxImpressions = _int;
                if (int.TryParse(dr["TotalImpressions"].ToString(), out _int)) result.TotalImpressions = _int;
                result.SerialPrefix = dr["SerialPrefix"].ToString();
                result.ZipCode = dr["ZipCode"].ToString();
                if (int.TryParse(dr["AgeStart"].ToString(), out _int)) result.AgeStart = _int;
                if (int.TryParse(dr["AgeEnd"].ToString(), out _int)) result.AgeEnd = _int;
                if (int.TryParse(dr["ProgramId"].ToString(), out _int)) result.ProgramId = _int;
                if (int.TryParse(dr["BranchId"].ToString(), out _int)) result.BranchId = _int;
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

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int OID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@OID", OID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Offer_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Offer result = new Offer();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["OID"].ToString(), out _int)) this.OID = _int;
                this.isEnabled = bool.Parse(dr["isEnabled"].ToString());
                this.AdminName = dr["AdminName"].ToString();
                this.Title = dr["Title"].ToString();
                this.ExternalRedirectFlag = bool.Parse(dr["ExternalRedirectFlag"].ToString());
                this.RedirectURL = dr["RedirectURL"].ToString();
                if (int.TryParse(dr["MaxImpressions"].ToString(), out _int)) this.MaxImpressions = _int;
                if (int.TryParse(dr["TotalImpressions"].ToString(), out _int)) this.TotalImpressions = _int;
                this.SerialPrefix = dr["SerialPrefix"].ToString();
                this.ZipCode = dr["ZipCode"].ToString();
                if (int.TryParse(dr["AgeStart"].ToString(), out _int)) this.AgeStart = _int;
                if (int.TryParse(dr["AgeEnd"].ToString(), out _int)) this.AgeEnd = _int;
                if (int.TryParse(dr["ProgramId"].ToString(), out _int)) this.ProgramId = _int;
                if (int.TryParse(dr["BranchId"].ToString(), out _int)) this.BranchId = _int;
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

        public static int Insert(Offer o)
        {

            SqlParameter[] arrParams = new SqlParameter[28];

            arrParams[0] = new SqlParameter("@isEnabled", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isEnabled, o.isEnabled.GetTypeCode()));
            arrParams[1] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ExternalRedirectFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ExternalRedirectFlag, o.ExternalRedirectFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@RedirectURL", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RedirectURL, o.RedirectURL.GetTypeCode()));
            arrParams[5] = new SqlParameter("@MaxImpressions", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxImpressions, o.MaxImpressions.GetTypeCode()));
            arrParams[6] = new SqlParameter("@TotalImpressions", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TotalImpressions, o.TotalImpressions.GetTypeCode()));
            arrParams[7] = new SqlParameter("@SerialPrefix", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SerialPrefix, o.SerialPrefix.GetTypeCode()));
            arrParams[8] = new SqlParameter("@ZipCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode, o.ZipCode.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AgeStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeStart, o.AgeStart.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AgeEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeEnd, o.AgeEnd.GetTypeCode()));
            arrParams[11] = new SqlParameter("@ProgramId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgramId, o.ProgramId.GetTypeCode()));
            arrParams[12] = new SqlParameter("@BranchId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BranchId, o.BranchId.GetTypeCode()));
            arrParams[13] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[14] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[15] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[16] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[17] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[27] = new SqlParameter("@OID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OID, o.OID.GetTypeCode()));
            arrParams[27].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Offer_Insert", arrParams);

            o.OID = int.Parse(arrParams[27].Value.ToString());

            return o.OID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Offer o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[28];

            arrParams[0] = new SqlParameter("@OID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OID, o.OID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@isEnabled", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isEnabled, o.isEnabled.GetTypeCode()));
            arrParams[2] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ExternalRedirectFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ExternalRedirectFlag, o.ExternalRedirectFlag.GetTypeCode()));
            arrParams[5] = new SqlParameter("@RedirectURL", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RedirectURL, o.RedirectURL.GetTypeCode()));
            arrParams[6] = new SqlParameter("@MaxImpressions", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxImpressions, o.MaxImpressions.GetTypeCode()));
            arrParams[7] = new SqlParameter("@TotalImpressions", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TotalImpressions, o.TotalImpressions.GetTypeCode()));
            arrParams[8] = new SqlParameter("@SerialPrefix", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SerialPrefix, o.SerialPrefix.GetTypeCode()));
            arrParams[9] = new SqlParameter("@ZipCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode, o.ZipCode.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AgeStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeStart, o.AgeStart.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AgeEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeEnd, o.AgeEnd.GetTypeCode()));
            arrParams[12] = new SqlParameter("@ProgramId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgramId, o.ProgramId.GetTypeCode()));
            arrParams[13] = new SqlParameter("@BranchId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BranchId, o.BranchId.GetTypeCode()));
            arrParams[14] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[15] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[16] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[17] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[18] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Offer_Update", arrParams);

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

        public static int Delete(Offer o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@OID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OID, o.OID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Offer_Delete", arrParams);

                var fileName = (HttpContext.Current.Server.MapPath("~/Images/Offers/") + "\\" + o.OID.ToString() + ".png");
                File.Delete(fileName);
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Offers/") + "\\sm_" + o.OID.ToString() + ".png");
                File.Delete(fileName);
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Offers/") + "\\md_" + o.OID.ToString() + ".png");
                File.Delete(fileName);
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

