using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

    [Serializable()]
    public class Badge : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GlobalUtilities.SRPDB;

        #endregion

        #region Accessors

        public int BID { get; set; }
        public string AdminName { get; set; }
        public string UserName { get; set; }
        public bool GenNotificationFlag { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationBody { get; set; }
        public string CustomEarnedMessage { get; set; }
        public bool IncludesPhysicalPrizeFlag { get; set; }
        public string PhysicalPrizeName { get; set; }
        public bool AssignProgramPrizeCode { get; set; }
        public string PCNotificationSubject { get; set; }
        public string PCNotificationBody { get; set; }
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
        public bool HiddenFromPublic { get; set; }
        #endregion

        #region Constructors

        public Badge()
        {
            this.TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
            this.NotificationSubject = string.Empty;
            this.NotificationBody = string.Empty;
            this.CustomEarnedMessage = string.Empty;
            this.PhysicalPrizeName = string.Empty;
            this.PCNotificationSubject = string.Empty;
            this.PCNotificationBody = string.Empty;
            this.FldText1 = string.Empty;
            this.FldText2 = string.Empty;
            this.FldText3 = string.Empty;
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetList(string ids)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@ids", ids);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetList", arrParams);
        }

        public static int GetVisibleCount()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );


            var result = SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "app_Badge_GetVisibleCount", arrParams);
            if (result == null)
            {
                return 0;
            }
            else
            {
                return (int)result;
            }
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

                result.HiddenFromPublic = dr["HiddenFromPublic"] as bool? == true;

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

                result.HiddenFromPublic = dr["HiddenFromPublic"] as bool? == true;

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

                this.HiddenFromPublic = dr["HiddenFromPublic"] as bool? == true;

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

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@AdminName", GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@UserName", GlobalUtilities.DBSafeValue(o.UserName, o.UserName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GenNotificationFlag", GlobalUtilities.DBSafeValue(o.GenNotificationFlag, o.GenNotificationFlag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@NotificationSubject", GlobalUtilities.DBSafeValue(o.NotificationSubject, o.NotificationSubject.GetTypeCode())));
            arrParams.Add(new SqlParameter("@NotificationBody", GlobalUtilities.DBSafeValue(o.NotificationBody, o.NotificationBody.GetTypeCode())));
            arrParams.Add(new SqlParameter("@CustomEarnedMessage", GlobalUtilities.DBSafeValue(o.CustomEarnedMessage, o.CustomEarnedMessage.GetTypeCode())));
            arrParams.Add(new SqlParameter("@IncludesPhysicalPrizeFlag", GlobalUtilities.DBSafeValue(o.IncludesPhysicalPrizeFlag, o.IncludesPhysicalPrizeFlag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhysicalPrizeName", GlobalUtilities.DBSafeValue(o.PhysicalPrizeName, o.PhysicalPrizeName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AssignProgramPrizeCode", GlobalUtilities.DBSafeValue(o.AssignProgramPrizeCode, o.AssignProgramPrizeCode.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PCNotificationSubject", GlobalUtilities.DBSafeValue(o.PCNotificationSubject, o.PCNotificationSubject.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PCNotificationBody", GlobalUtilities.DBSafeValue(o.PCNotificationBody, o.PCNotificationBody.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModDate", GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModUser", GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedDate", GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedUser", GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

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

            arrParams.Add(new SqlParameter("@HiddenFromPublic", GlobalUtilities.DBSafeValue(o.HiddenFromPublic, o.HiddenFromPublic.GetTypeCode())));

            var param = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(o.BID, o.BID.GetTypeCode()));
            param.Direction = ParameterDirection.Output;
            arrParams.Add(param);

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_Insert", arrParams.ToArray());
            o.BID = int.Parse(param.Value.ToString());
            return o.BID;
        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Badge o)
        {

            int iReturn = -1; //assume the worst

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@BID", GlobalUtilities.DBSafeValue(o.BID, o.BID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AdminName", GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@UserName", GlobalUtilities.DBSafeValue(o.UserName, o.UserName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@GenNotificationFlag", GlobalUtilities.DBSafeValue(o.GenNotificationFlag, o.GenNotificationFlag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@NotificationSubject", GlobalUtilities.DBSafeValue(o.NotificationSubject, o.NotificationSubject.GetTypeCode())));
            arrParams.Add(new SqlParameter("@NotificationBody", GlobalUtilities.DBSafeValue(o.NotificationBody, o.NotificationBody.GetTypeCode())));
            arrParams.Add(new SqlParameter("@CustomEarnedMessage", GlobalUtilities.DBSafeValue(o.CustomEarnedMessage, o.CustomEarnedMessage.GetTypeCode())));
            arrParams.Add(new SqlParameter("@IncludesPhysicalPrizeFlag", GlobalUtilities.DBSafeValue(o.IncludesPhysicalPrizeFlag, o.IncludesPhysicalPrizeFlag.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PhysicalPrizeName", GlobalUtilities.DBSafeValue(o.PhysicalPrizeName, o.PhysicalPrizeName.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AssignProgramPrizeCode", GlobalUtilities.DBSafeValue(o.AssignProgramPrizeCode, o.AssignProgramPrizeCode.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PCNotificationSubject", GlobalUtilities.DBSafeValue(o.PCNotificationSubject, o.PCNotificationSubject.GetTypeCode())));
            arrParams.Add(new SqlParameter("@PCNotificationBody", GlobalUtilities.DBSafeValue(o.PCNotificationBody, o.PCNotificationBody.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModDate", GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@LastModUser", GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedDate", GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@AddedUser", GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

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

            arrParams.Add(new SqlParameter("@HiddenFromPublic", GlobalUtilities.DBSafeValue(o.HiddenFromPublic, o.HiddenFromPublic.GetTypeCode())));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_Update", arrParams.ToArray());

            }

            catch (SqlException exx)
            {
                "GRA.SRP.DAL.Badge".Log().Error("Error updating Badge: {0} - {1}",
                    exx.Message,
                    exx.StackTrace);

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

            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(o.BID, o.BID.GetTypeCode()));

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
                "GRA.SRP.DAL.Badge".Log().Error("Error deleting Badge: {0} - {1}",
                    exx.Message,
                    exx.StackTrace);

                System.Diagnostics.Debug.Write(exx.Message);
            }
            return iReturn;
        }

        public static DataSet GetBadgeBranches(int BID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeBranches", arrParams);
        }
        public void UpdateBadgeBranches(string checkedMembers) { UpdateBadgeBranches(this.BID, checkedMembers); }
        public static void UpdateBadgeBranches(int BID, string checkedMembers)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[0] = new SqlParameter("@BID", GlobalUtilities.DBSafeValue(BID, BID.GetTypeCode()));
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
            arrParams[2] = new SqlParameter("@List", SqlDbType.VarChar, 2000);
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

            return arrParams[2].Value == DBNull.Value ? string.Empty : (string)arrParams[2].Value;
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

        public static string GetBadgeGoal(int BID)
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


            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Badge_GetBadgeGoals", arrParams);

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

            return arrParams[2].Value == DBNull.Value ? string.Empty : (string)arrParams[2].Value;
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

        public static DataSet GetFiltered(string searchText, int branchId)
        {
            var arrParams = new List<SqlParameter>();
            arrParams.Add(new SqlParameter("@TenID",
                                HttpContext.Current.Session["TenantID"] == null
                                || string.IsNullOrEmpty(HttpContext.Current.Session["TenantID"].ToString())
                                ? -1
                                : (int)HttpContext.Current.Session["TenantID"])
                            );
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
                "app_Badge_Filter",
                arrParams.ToArray());
        }
        #endregion

    }//end class

}//end namespace

