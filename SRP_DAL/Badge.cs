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

[Serializable()]    
    public class Badge : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myBID;
        private string myAdminName;
        private string myUserName;
        private bool myGenNotificationFlag;
        private string myNotificationSubject = "";
        private string myNotificationBody = "";
        private string myCustomEarnedMessage = "";
        private bool myIncludesPhysicalPrizeFlag;
        private string myPhysicalPrizeName = "";
        private bool myAssignProgramPrizeCode;
        private string myPCNotificationSubject="";
        private string myPCNotificationBody="";
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

        public int BID
        {
            get { return myBID; }
            set { myBID = value; }
        }
        public string AdminName
        {
            get { return myAdminName; }
            set { myAdminName = value; }
        }
        public string UserName
        {
            get { return myUserName; }
            set { myUserName = value; }
        }
        public bool GenNotificationFlag
        {
            get { return myGenNotificationFlag; }
            set { myGenNotificationFlag = value; }
        }
        public string NotificationSubject
        {
            get { return myNotificationSubject; }
            set { myNotificationSubject = value; }
        }
        public string NotificationBody
        {
            get { return myNotificationBody; }
            set { myNotificationBody = value; }
        }
        public string CustomEarnedMessage
        {
            get { return myCustomEarnedMessage; }
            set { myCustomEarnedMessage = value; }
        }
        public bool IncludesPhysicalPrizeFlag
        {
            get { return myIncludesPhysicalPrizeFlag; }
            set { myIncludesPhysicalPrizeFlag = value; }
        }
        public string PhysicalPrizeName
        {
            get { return myPhysicalPrizeName; }
            set { myPhysicalPrizeName = value; }
        }
        public bool AssignProgramPrizeCode
        {
            get { return myAssignProgramPrizeCode; }
            set { myAssignProgramPrizeCode = value; }
        }
        public string PCNotificationSubject
        {
            get { return myPCNotificationSubject; }
            set { myPCNotificationSubject = value; }
        }
        public string PCNotificationBody
        {
            get { return myPCNotificationBody; }
            set { myPCNotificationBody = value; }
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

        public Badge()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetList(string ids)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@ids", ids);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetList", arrParams);
        }

        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );


            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetAll", arrParams);
        }

        public static Badge FetchObject(int BID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BID", BID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Badge_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Badge result = new Badge();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BID"].ToString(), out _int)) result.BID = _int;
                result.AdminName = dr["AdminName"].ToString();
                result.UserName = dr["UserName"].ToString();
                result.GenNotificationFlag = bool.Parse(dr["GenNotificationFlag"].ToString());
                result.NotificationSubject = dr["NotificationSubject"].ToString();
                result.NotificationBody = dr["NotificationBody"].ToString();
                result.CustomEarnedMessage = dr["CustomEarnedMessage"].ToString();
                result.IncludesPhysicalPrizeFlag = bool.Parse(dr["IncludesPhysicalPrizeFlag"].ToString());
                result.PhysicalPrizeName = dr["PhysicalPrizeName"].ToString();
                result.AssignProgramPrizeCode = bool.Parse(dr["AssignProgramPrizeCode"].ToString());
                result.PCNotificationSubject = dr["PCNotificationSubject"].ToString();
                result.PCNotificationBody = dr["PCNotificationBody"].ToString();
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


        public static Badge GetBadge(int BID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BID", BID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Badge_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Badge result = new Badge();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BID"].ToString(), out _int)) result.BID = _int;
                result.AdminName = dr["AdminName"].ToString();
                result.UserName = dr["UserName"].ToString();
                result.GenNotificationFlag = bool.Parse(dr["GenNotificationFlag"].ToString());
                result.NotificationSubject = dr["NotificationSubject"].ToString();
                result.NotificationBody = dr["NotificationBody"].ToString();
                result.CustomEarnedMessage = dr["CustomEarnedMessage"].ToString();
                result.IncludesPhysicalPrizeFlag = bool.Parse(dr["IncludesPhysicalPrizeFlag"].ToString());
                result.PhysicalPrizeName = dr["PhysicalPrizeName"].ToString();
                result.AssignProgramPrizeCode = bool.Parse(dr["AssignProgramPrizeCode"].ToString());
                result.PCNotificationSubject = dr["PCNotificationSubject"].ToString();
                result.PCNotificationBody = dr["PCNotificationBody"].ToString();
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
        public bool Fetch(int BID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BID", BID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Badge_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Badge result = new Badge();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BID"].ToString(), out _int)) this.BID = _int;
                this.AdminName = dr["AdminName"].ToString();
                this.UserName = dr["UserName"].ToString();
                this.GenNotificationFlag = bool.Parse(dr["GenNotificationFlag"].ToString());
                this.NotificationSubject = dr["NotificationSubject"].ToString();
                this.NotificationBody = dr["NotificationBody"].ToString();
                this.CustomEarnedMessage = dr["CustomEarnedMessage"].ToString();
                this.IncludesPhysicalPrizeFlag = bool.Parse(dr["IncludesPhysicalPrizeFlag"].ToString());
                this.PhysicalPrizeName = dr["PhysicalPrizeName"].ToString();
                this.AssignProgramPrizeCode = bool.Parse(dr["AssignProgramPrizeCode"].ToString());
                this.PCNotificationSubject = dr["PCNotificationSubject"].ToString();
                this.PCNotificationBody = dr["PCNotificationBody"].ToString();
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

        public static int Insert(Badge o)
        {

            SqlParameter[] arrParams = new SqlParameter[26];

            arrParams[0] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[1] = new SqlParameter("@UserName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.UserName, o.UserName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@GenNotificationFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GenNotificationFlag, o.GenNotificationFlag.GetTypeCode()));
            arrParams[3] = new SqlParameter("@NotificationSubject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationSubject, o.NotificationSubject.GetTypeCode()));
            arrParams[4] = new SqlParameter("@NotificationBody", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationBody, o.NotificationBody.GetTypeCode()));
            arrParams[5] = new SqlParameter("@CustomEarnedMessage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CustomEarnedMessage, o.CustomEarnedMessage.GetTypeCode()));
            arrParams[6] = new SqlParameter("@IncludesPhysicalPrizeFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IncludesPhysicalPrizeFlag, o.IncludesPhysicalPrizeFlag.GetTypeCode()));
            arrParams[7] = new SqlParameter("@PhysicalPrizeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeName, o.PhysicalPrizeName.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AssignProgramPrizeCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AssignProgramPrizeCode, o.AssignProgramPrizeCode.GetTypeCode()));
            arrParams[9] = new SqlParameter("@PCNotificationSubject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCNotificationSubject, o.PCNotificationSubject.GetTypeCode()));
            arrParams[10] = new SqlParameter("@PCNotificationBody", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCNotificationBody, o.PCNotificationBody.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[12] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[13] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[14] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[15] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            
            arrParams[25] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BID, o.BID.GetTypeCode()));
            arrParams[25].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_Insert", arrParams);
            o.BID = int.Parse(arrParams[25].Value.ToString());
            return o.BID;
        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Badge o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[26];

            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BID, o.BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@UserName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.UserName, o.UserName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@GenNotificationFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GenNotificationFlag, o.GenNotificationFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@NotificationSubject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationSubject, o.NotificationSubject.GetTypeCode()));
            arrParams[5] = new SqlParameter("@NotificationBody", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationBody, o.NotificationBody.GetTypeCode()));
            arrParams[6] = new SqlParameter("@CustomEarnedMessage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CustomEarnedMessage, o.CustomEarnedMessage.GetTypeCode()));
            arrParams[7] = new SqlParameter("@IncludesPhysicalPrizeFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IncludesPhysicalPrizeFlag, o.IncludesPhysicalPrizeFlag.GetTypeCode()));
            arrParams[8] = new SqlParameter("@PhysicalPrizeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeName, o.PhysicalPrizeName.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AssignProgramPrizeCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AssignProgramPrizeCode, o.AssignProgramPrizeCode.GetTypeCode()));
            arrParams[10] = new SqlParameter("@PCNotificationSubject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCNotificationSubject, o.PCNotificationSubject.GetTypeCode()));
            arrParams[11] = new SqlParameter("@PCNotificationBody", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PCNotificationBody, o.PCNotificationBody.GetTypeCode()));
            arrParams[12] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[13] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[14] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[15] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            
            arrParams[16] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_Update", arrParams);

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

        public static int Delete(Badge o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BID, o.BID.GetTypeCode()));

            try
            {

                var fileName = (HttpContext.Current.Server.MapPath("~/Images/Badges/") + "\\" + o.BID.ToString() + ".png");
                File.Delete(fileName);
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Badges/") + "\\sm_" + o.BID.ToString() + ".png");
                File.Delete(fileName);
                fileName = (HttpContext.Current.Server.MapPath("~/Images/Badges/") + "\\md_" + o.BID.ToString() + ".png");
                File.Delete(fileName);

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public  static DataSet GetBadgeBranches (int BID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeBranches", arrParams);
        }

        public void UpdateBadgeBranches(string checkedMembers) {UpdateBadgeBranches(this.BID, checkedMembers);}
        public static void UpdateBadgeBranches(int BID, string checkedMembers)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[2] = new SqlParameter("@CID_LIST", checkedMembers);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_UpdateBadgeBranches", arrParams);
        }


        public static DataSet GetBadgeCategories(int BID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeCategories", arrParams);
        }
        public void UpdateBadgeCategories(string checkedMembers) { UpdateBadgeCategories(this.BID, checkedMembers); }
        public static void UpdateBadgeCategories(int BID, string checkedMembers)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[2] = new SqlParameter("@CID_LIST", checkedMembers);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_UpdateBadgeCategories", arrParams);
        }

        public static DataSet GetBadgeAgeGroups(int BID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeAgeGroups", arrParams);
        }
        public void UpdateBadgeAgeGroups(string checkedMembers) { UpdateBadgeAgeGroups(this.BID, checkedMembers); }
        public static void UpdateBadgeAgeGroups(int BID, string checkedMembers)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[2] = new SqlParameter("@CID_LIST", checkedMembers);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_UpdateBadgeAgeGroups", arrParams);
        }

        public static DataSet GetBadgeLocations(int BID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeLocations", arrParams);
        }
        public void UpdateBadgeLocations(string checkedMembers) { UpdateBadgeLocations(this.BID, checkedMembers); }
        public static void UpdateBadgeLocations(int BID, string checkedMembers)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@BID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[2] = new SqlParameter("@CID_LIST", checkedMembers);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_UpdateBadgeLocations", arrParams);
        }

        public static DataSet GetForGallery(int Age = 0, int Branch = 0, int Category = 0, int Location = 0)
        {
            SqlParameter[] arrParams = new SqlParameter[5];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@A", Age);
            arrParams[2] = new SqlParameter("@B", Branch);
            arrParams[3] = new SqlParameter("@C", Category);
            arrParams[4] = new SqlParameter("@L", Location);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeGallery", arrParams);
        }


        public static string GetEnrollmentPrograms(int BID)
        {
            var arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@BID", BID); 
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000 );
            arrParams[2].Direction = ParameterDirection.InputOutput;


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetEnrollmentPrograms", arrParams);

            return (string)arrParams[2].Value;
        }


        public static string GetBadgeBookLists(int BID)
        {
            var arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@BID", BID);
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000);
            arrParams[2].Direction = ParameterDirection.InputOutput;


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeBookLists", arrParams);

            return (string)arrParams[2].Value;
        }

        public static string GetBadgeReading(int BID)
        {
            var arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@BID", BID);
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000);
            arrParams[2].Direction = ParameterDirection.InputOutput;


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeReading", arrParams);

            return (string)arrParams[2].Value;
        }

        public static string GetBadgeGames(int BID)
        {
            var arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@BID", BID);
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000);
            arrParams[2].Direction = ParameterDirection.InputOutput;


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeGames", arrParams);

            return (string)arrParams[2].Value;
        }

        public static string GetBadgeEvents(int BID)
        {
            var arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@BID", BID);
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000);
            arrParams[2].Direction = ParameterDirection.InputOutput;


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeEvents", arrParams);

            return (string)arrParams[2].Value;
        }

        public static string GetBadgeEventIDs(int BID)
        {
            var arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            arrParams[1] = new SqlParameter("@BID", BID);
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000);
            arrParams[2].Direction = ParameterDirection.InputOutput;


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeEventIDS", arrParams);

            return (string)arrParams[2].Value;
        }
        #endregion

    }//end class

}//end namespace

