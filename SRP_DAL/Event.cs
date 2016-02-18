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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GRA.SRP.DAL
{

    [Serializable]
    public class Event : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GlobalUtilities.SRPDB;

        #endregion

        #region Accessors

        public int EID { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string EventTime { get; set; }
        public string HTML { get; set; }
        public string SecretCode { get; set; }
        public int NumberPoints { get; set; }
        public int BadgeID { get; set; }
        public int BranchID { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
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
        public string ShortDescription { get; set; }
        public DateTime EndDate { get; set; }
        public string EndTime { get; set; }
        public string ExternalLinkToEvent { get; set; }
        public bool HiddenFromPublic { get; set; }

        #endregion

        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT
                || validationMode == BusinessRulesValidationMode.UPDATE)
            {
                SecretCode = SecretCode.ToLower();
                if (!string.IsNullOrEmpty(SecretCode))
                {
                    var allowdups = SRPSettings.GetSettingValue("DupEvtCodes").ToUpper() == "TRUE";

                    if (SecretCode.Length > 50)
                    {
                        AddErrorCode(new BusinessRulesValidationMessage("Secret Code", "Secret Code", "The Secret Code must be 50 characters or less.",
                                                                        BusinessRulesValidationCode.UNSPECIFIED));
                    }
                    else if (!Regex.IsMatch(SecretCode, @"^[a-z0-9]+$"))
                    {
                        AddErrorCode(new BusinessRulesValidationMessage("Secret Code", "Secret Code", "The Secret Code can only contain letters and numbers.",
                                                                        BusinessRulesValidationCode.UNSPECIFIED));
                    }
                    else if (!allowdups)
                    {
                        int eventsWithCode = 0;
                        switch (validationMode)
                        {
                            case BusinessRulesValidationMode.UPDATE:
                                eventsWithCode = GetEventCountByEventCode(EID, SecretCode);
                                break;
                            case BusinessRulesValidationMode.INSERT:
                                eventsWithCode = GetEventCountByEventCode(SecretCode);
                                break;
                        }
                        if (eventsWithCode != 0)
                        {
                            AddErrorCode(new BusinessRulesValidationMessage("Secret Code", "Secret Code", "The Secret Code you have chosen is already in use.  Please select a different Secret Code.",
                                                                            BusinessRulesValidationCode.UNSPECIFIED));
                        }
                    }
                }
            }

            return (ErrorCodes.Count == 0);
            //return true;
        }




        #region Constructors

        public Event()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
            EventTitle = string.Empty;
            EventTime = string.Empty;
            HTML = string.Empty;
            SecretCode = string.Empty;
            Custom1 = string.Empty;
            Custom2 = string.Empty;
            Custom3 = string.Empty;
            LastModUser = string.Empty;
            AddedUser = string.Empty;
            FldText1 = string.Empty;
            FldText2 = string.Empty;
            FldText3 = string.Empty;
            ShortDescription = string.Empty;
            EndTime = string.Empty;
            ExternalLinkToEvent = string.Empty;
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


        public static DataSet GetEventByEventCode(string key)
        {
            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@Key", key));
            arrParams.Add(new SqlParameter("@TenID",
                                 (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                         -1 :
                                         (int)HttpContext.Current.Session["TenantID"]))
                             );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetEventsByEventCode", arrParams.ToArray());
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

            arrParams[0] = new SqlParameter("@src", GlobalUtilities.DBSafeValue(src, src.GetTypeCode()));
            arrParams[1] = new SqlParameter("@dst", GlobalUtilities.DBSafeValue(dst, dst.GetTypeCode()));
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

                result.HiddenFromPublic = (bool)dr["HiddenFromPublic"];
                result.ExternalLinkToEvent = dr["ExternalLinkToEvent"].ToString();

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

                result.HiddenFromPublic = (bool)dr["HiddenFromPublic"];
                result.ExternalLinkToEvent = dr["ExternalLinkToEvent"].ToString();

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

                this.HiddenFromPublic = (bool)dr["HiddenFromPublic"];
                this.ExternalLinkToEvent = dr["ExternalLinkToEvent"].ToString();

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
            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@EventTitle", GlobalUtilities.DBSafeValue(o.EventTitle, o.EventTitle.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EventDate", GlobalUtilities.DBSafeValue(o.EventDate, o.EventDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EventTime", GlobalUtilities.DBSafeValue(o.EventTime, o.EventTime.GetTypeCode())));
            arrParams.Add(new SqlParameter("@HTML", GlobalUtilities.DBSafeValue(o.HTML, o.HTML.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SecretCode", GlobalUtilities.DBSafeValue(o.SecretCode, o.SecretCode.GetTypeCode())));
            arrParams.Add(new SqlParameter("@NumberPoints", GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BadgeID", GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchID", GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1", GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2", GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3", GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode())));
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

            arrParams.Add(new SqlParameter("@ShortDescription", GlobalUtilities.DBSafeValue(o.ShortDescription, o.ShortDescription.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EndDate", GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EndTime", GlobalUtilities.DBSafeValue(o.EndTime, o.EndTime.GetTypeCode())));

            arrParams.Add(new SqlParameter("@ExternalLinkToEvent", GlobalUtilities.DBSafeValue(o.ExternalLinkToEvent, o.ExternalLinkToEvent.GetTypeCode())));
            arrParams.Add(new SqlParameter("@HiddenFromPublic", GlobalUtilities.DBSafeValue(o.HiddenFromPublic, o.HiddenFromPublic.GetTypeCode())));

            var param = new SqlParameter("@EID", GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));
            param.Direction = ParameterDirection.Output;
            arrParams.Add(param);

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_Insert", arrParams.ToArray());

            o.EID = int.Parse(param.Value.ToString());

            return o.EID;
        }

        public int Update()
        {
            return Update(this);
        }

        public static int Update(Event o)
        {
            int iReturn = -1; //assume the worst

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@EID", GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EventTitle", GlobalUtilities.DBSafeValue(o.EventTitle, o.EventTitle.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EventDate", GlobalUtilities.DBSafeValue(o.EventDate, o.EventDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EventTime", GlobalUtilities.DBSafeValue(o.EventTime, o.EventTime.GetTypeCode())));
            arrParams.Add(new SqlParameter("@HTML", GlobalUtilities.DBSafeValue(o.HTML, o.HTML.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SecretCode", GlobalUtilities.DBSafeValue(o.SecretCode, o.SecretCode.GetTypeCode())));
            arrParams.Add(new SqlParameter("@NumberPoints", GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BadgeID", GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@BranchID", GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom1", GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom2", GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode())));
            arrParams.Add(new SqlParameter("@Custom3", GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode())));
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

            arrParams.Add(new SqlParameter("@ShortDescription", GlobalUtilities.DBSafeValue(o.ShortDescription, o.ShortDescription.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EndDate", GlobalUtilities.DBSafeValue(o.EndDate, o.EndDate.GetTypeCode())));
            arrParams.Add(new SqlParameter("@EndTime", GlobalUtilities.DBSafeValue(o.EndTime, o.EndTime.GetTypeCode())));

            arrParams.Add(new SqlParameter("@ExternalLinkToEvent", GlobalUtilities.DBSafeValue(o.ExternalLinkToEvent, o.ExternalLinkToEvent.GetTypeCode())));
            arrParams.Add(new SqlParameter("@HiddenFromPublic", GlobalUtilities.DBSafeValue(o.HiddenFromPublic, o.HiddenFromPublic.GetTypeCode())));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_Update", arrParams.ToArray());

            }
            catch (SqlException exx)
            {
                "GRA.SRP.DAL.Event".Log().Error("Error updating Event: {0} - {1}",
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

        public static int Delete(Event o)
        {
            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@EID", GlobalUtilities.DBSafeValue(o.EID, o.EID.GetTypeCode()));

            try
            {
                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Event_Delete", arrParams);
            }
            catch (SqlException exx)
            {
                "GRA.SRP.DAL.Event".Log().Error("Error deleting Event: {0} - {1}",
                    exx.Message,
                    exx.StackTrace);
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
        public static string DisplayEventDateTime(Event e)
        {
            return string.Format("{0} {1}",
                e.EventDate.ToShortDateString(),
                e.EventDate.ToShortTimeString());
        }

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
                "app_Event_Filter",
                arrParams.ToArray());
        }
    }//end class

}//end namespace

