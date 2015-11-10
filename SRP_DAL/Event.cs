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
using System.Text;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.DAL
{

[Serializable]    public class Event : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myEID;
        private string myEventTitle = "";
        private DateTime myEventDate;
        private string myEventTime = "";
        private string myHTML = "";
        private string mySecretCode = "";
        private int myNumberPoints=0;
        private int myBadgeID=0;
        private int myBranchID=0;
        private string myCustom1 = "";
        private string myCustom2 = "";
        private string myCustom3 = "";
        private DateTime myLastModDate;
        private string myLastModUser = "";
        private DateTime myAddedDate;
        private string myAddedUser = "";

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

        private string myShortDescription = "";
        private DateTime myEndDate;
        private string myEndTime = "";


        #endregion

        #region Accessors

        public int EID
        {
            get { return myEID; }
            set { myEID = value; }
        }
        public string EventTitle
        {
            get { return myEventTitle; }
            set { myEventTitle = value; }
        }
        public DateTime EventDate
        {
            get { return myEventDate; }
            set { myEventDate = value; }
        }
        public string EventTime
        {
            get { return myEventTime; }
            set { myEventTime = value; }
        }
        public string HTML
        {
            get { return myHTML; }
            set { myHTML = value; }
        }
        public string SecretCode
        {
            get { return mySecretCode; }
            set { mySecretCode = value; }
        }
        public int NumberPoints
        {
            get { return myNumberPoints; }
            set { myNumberPoints = value; }
        }
        public int BadgeID
        {
            get { return myBadgeID; }
            set { myBadgeID = value; }
        }
        public int BranchID
        {
            get { return myBranchID; }
            set { myBranchID = value; }
        }
        public string Custom1
        {
            get { return myCustom1; }
            set { myCustom1 = value; }
        }
        public string Custom2
        {
            get { return myCustom2; }
            set { myCustom2 = value; }
        }
        public string Custom3
        {
            get { return myCustom3; }
            set { myCustom3 = value; }
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

        public string ShortDescription
        {
            get { return myShortDescription; }
            set { myShortDescription = value; }
        }
        public DateTime EndDate
        {
            get { return myEndDate; }
            set { myEndDate = value; }
        }
        public string EndTime
        {
            get { return myEndTime; }
            set { myEndTime = value; }
        }

        #endregion

        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                var allowdups = SRPSettings.GetSettingValue("DupEvtCodes").ToUpper() == "TRUE";
                if (!allowdups && GetEventCountByEventCode(SecretCode) != 0)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Secret Code", "Secret Code", "The Secret Code you have chosen is already in use.  Please select a different Secret Code.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }

            if (validationMode == BusinessRulesValidationMode.UPDATE)
            {
                var allowdups = SRPSettings.GetSettingValue("DupEvtCodes").ToUpper() == "TRUE";
                if (!allowdups && GetEventCountByEventCode(EID, SecretCode) != 0)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Secret Code", "Secret Code", "The Secret Code you have chosen is already in use.  Please select a different Secret Code.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }


            return (ErrorCodes.Count == 0);
            //return true;
        }




        #region Constructors

        public Event()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static int GetEventCountByEventCode(int EID, string key)
        {
            SqlParameter[] arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@EID", EID);
            arrParams[1] = new SqlParameter("@Key", key);
            arrParams[2] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetEventCountByEventCode", arrParams);
            return Convert.ToInt32(ds.Tables[0].Rows[0]["NumCodes"]);
        }

        public static int GetEventCountByEventCode(string key)
        {
            SqlParameter[] arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@EID", DBNull.Value);
            arrParams[1] = new SqlParameter("@Key", key);
            arrParams[2] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetEventCountByEventCode", arrParams);
            return Convert.ToInt32(ds.Tables[0].Rows[0]["NumCodes"]);
        }


        public static DataSet GetEventByEventCode(string startDate, string endDate, string key)
        {
            SqlParameter[] arrParams = new SqlParameter[4];

            arrParams[0] = new SqlParameter("@startDate", GlobalUtilities.DBSafeDate(startDate));
            arrParams[1] = new SqlParameter("@endDate", GlobalUtilities.DBSafeDate(endDate)); // (string.IsNullOrEmpty(endDate) ? (object)DBNull.Value : DateTime.Parse(endDate)));
            arrParams[2] = new SqlParameter("@Key", key);
            arrParams[3] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetEventsByEventCode", arrParams);            
        }


        public static DataSet GetUpcomingDisplay(string startDate, string endDate, int branchID)
        {
            SqlParameter[] arrParams = new SqlParameter[4];

            arrParams[0] = new SqlParameter("@startDate", GlobalUtilities.DBSafeDate(startDate));
            arrParams[1] = new SqlParameter("@endDate", GlobalUtilities.DBSafeDate(endDate)); // (string.IsNullOrEmpty(endDate) ? (object)DBNull.Value : DateTime.Parse(endDate)));
            arrParams[2] = new SqlParameter("@branchID", branchID);
            arrParams[3] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetUpcomingDisplay", arrParams);
        }


        public static DataSet GetAdminSearch(string startDate, string endDate, int branchID)
        {
            SqlParameter[] arrParams = new SqlParameter[4];

            arrParams[0] = new SqlParameter("@startDate", GlobalUtilities.DBSafeDate(startDate));
            arrParams[1] = new SqlParameter("@endDate", GlobalUtilities.DBSafeDate(endDate)); // (string.IsNullOrEmpty(endDate) ? (object)DBNull.Value : DateTime.Parse(endDate)));
            arrParams[2] = new SqlParameter("@branchID", branchID);
            arrParams[3] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetAdminSearch", arrParams);
        }


        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetAll", arrParams);
        }

        public static void TenantInitialize(int src, int dst)
        {
            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@src", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(src, src.GetTypeCode()));
            arrParams[1] = new SqlParameter("@dst", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(dst, dst.GetTypeCode()));
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_InitTenant", arrParams);
        }

        public Event FetchObject(int EID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@EID", EID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Event_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Event result = new Event();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["EID"].ToString(), out _int)) result.EID = _int;
                result.EventTitle = dr["EventTitle"].ToString();
                if (DateTime.TryParse(dr["EventDate"].ToString(), out _datetime)) result.EventDate = _datetime;
                result.EventTime = dr["EventTime"].ToString();
                result.HTML = dr["HTML"].ToString();
                result.SecretCode = dr["SecretCode"].ToString();
                if (int.TryParse(dr["NumberPoints"].ToString(), out _int)) result.NumberPoints = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) result.BranchID = _int;
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
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

                result.ShortDescription = dr["ShortDescription"].ToString();
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) result.EndDate = _datetime;
                result.EndTime = dr["EndTime"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public static Event GetEvent(int EID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@EID", EID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Event_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Event result = new Event();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["EID"].ToString(), out _int)) result.EID = _int;
                result.EventTitle = dr["EventTitle"].ToString();
                if (DateTime.TryParse(dr["EventDate"].ToString(), out _datetime)) result.EventDate = _datetime;
                result.EventTime = dr["EventTime"].ToString();
                result.HTML = dr["HTML"].ToString();
                result.SecretCode = dr["SecretCode"].ToString();
                if (int.TryParse(dr["NumberPoints"].ToString(), out _int)) result.NumberPoints = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) result.BranchID = _int;
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
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


                result.ShortDescription = dr["ShortDescription"].ToString();
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) result.EndDate = _datetime;
                result.EndTime = dr["EndTime"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }


        public bool Fetch(int EID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@EID", EID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Event_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Event result = new Event();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["EID"].ToString(), out _int)) this.EID = _int;
                this.EventTitle = dr["EventTitle"].ToString();
                if (DateTime.TryParse(dr["EventDate"].ToString(), out _datetime)) this.EventDate = _datetime;
                this.EventTime = dr["EventTime"].ToString();
                this.HTML = dr["HTML"].ToString();
                this.SecretCode = dr["SecretCode"].ToString();
                if (int.TryParse(dr["NumberPoints"].ToString(), out _int)) this.NumberPoints = _int;
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) this.BadgeID = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) this.BranchID = _int;
                this.Custom1 = dr["Custom1"].ToString();
                this.Custom2 = dr["Custom2"].ToString();
                this.Custom3 = dr["Custom3"].ToString();
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


                this.ShortDescription = dr["ShortDescription"].ToString();
                if (DateTime.TryParse(dr["EndDate"].ToString(), out _datetime)) this.EndDate = _datetime;
                this.EndTime = dr["EndTime"].ToString();

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

        public static int Insert(Event o)
        {

            SqlParameter[] arrParams = new SqlParameter[29];

            arrParams[0] = new SqlParameter("@EventTitle", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventTitle, o.EventTitle.GetTypeCode()));
            arrParams[1] = new SqlParameter("@EventDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventDate, o.EventDate.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EventTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventTime, o.EventTime.GetTypeCode()));
            arrParams[3] = new SqlParameter("@HTML", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML, o.HTML.GetTypeCode()));
            arrParams[4] = new SqlParameter("@SecretCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SecretCode, o.SecretCode.GetTypeCode()));
            arrParams[5] = new SqlParameter("@NumberPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode()));
            arrParams[6] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@BranchID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Custom1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode()));
            arrParams[9] = new SqlParameter("@Custom2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode()));
            arrParams[10] = new SqlParameter("@Custom3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode()));
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

            arrParams[25] = new SqlParameter("@ShortDescription", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShortDescription, o.ShortDescription.GetTypeCode()));
            arrParams[26] = new SqlParameter("@EndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode()));
            arrParams[27] = new SqlParameter("@EndTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndTime, o.EndTime.GetTypeCode()));

            arrParams[28] = new SqlParameter("@EID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));
            arrParams[28].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_Insert", arrParams);

            o.EID = int.Parse(arrParams[28].Value.ToString());

            return o.EID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Event o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[29];

            arrParams[0] = new SqlParameter("@EID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@EventTitle", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventTitle, o.EventTitle.GetTypeCode()));
            arrParams[2] = new SqlParameter("@EventDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventDate, o.EventDate.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EventTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventTime, o.EventTime.GetTypeCode()));
            arrParams[4] = new SqlParameter("@HTML", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HTML, o.HTML.GetTypeCode()));
            arrParams[5] = new SqlParameter("@SecretCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SecretCode, o.SecretCode.GetTypeCode()));
            arrParams[6] = new SqlParameter("@NumberPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode()));
            arrParams[7] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@BranchID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@Custom1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode()));
            arrParams[10] = new SqlParameter("@Custom2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode()));
            arrParams[11] = new SqlParameter("@Custom3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode()));
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

            arrParams[26] = new SqlParameter("@ShortDescription", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ShortDescription, o.ShortDescription.GetTypeCode()));
            arrParams[27] = new SqlParameter("@EndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode()));
            arrParams[28] = new SqlParameter("@EndTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EndTime, o.EndTime.GetTypeCode()));


            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_Update", arrParams);

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

        public static int Delete(Event o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@EID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }


        public static DataSet GetEventList(string list)
        {
            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@List", list);
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetEventList", arrParams);
        }

        #endregion

        /// <summary>
        /// Formats an event date to be nice for end users.
        /// 
        /// Must have an Event object with the following properties specified: EventDate,
        /// EventTime, EndDate, EndTime
        /// </summary>
        /// <param name="e">The Event object</param>
        /// <returns>A nice string describing start and end times.</returns>
        public static string DisplayEventDateTime(Event e) {
            StringBuilder sb = new StringBuilder(e.EventDate.ToNormalDate());
            if(!string.IsNullOrWhiteSpace(e.EventTime)) {
                sb.AppendFormat(" {0}", e.EventTime);
            }
            if(e.EndDate != null && e.EndDate > e.EventDate) {
                sb.AppendFormat(" until {0}",
                                e.EndDate.ToNormalDate().Replace("01/01/1900",
                                                                 string.Empty));
                if(!string.IsNullOrWhiteSpace(e.EndTime)) {
                    sb.AppendFormat(" {0}", e.EndTime);
                }
            }
            return sb.ToString();
        }
    }//end class

}//end namespace

