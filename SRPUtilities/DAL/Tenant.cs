using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Data;
using System.IO;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;


namespace GRA.SRP.Core.Utilities
{
    [Serializable]
    public class Tenant : EntityBase
    {
        public override string Version { get { return "2.0"; } }

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        #region Fields 
        private int myTenID;
        private string myName = "";
        private string myLandingName = "";
        private string myAdminName = "";
        private bool myisActiveFlag = true;
        private bool myisMasterFlag=false;
        private string myDescription="";
        private string myDomainName="";
        private bool myshowNotifications=true;
        private bool myshowOffers=true;
        private bool myshowBadges=true;
        private bool myshowEvents=true;
        private string myNotificationsMenuText="Messages";
        private string myOffersMenuText="Offers";
        private string myBadgesMenuText="Badges";
        private string myEventsMenuText="Events";
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;
        private int myFldInt1=0;
        private int myFldInt2=0;
        private int myFldInt3=0;
        private bool myFldBit1=false;
        private bool myFldBit2=false;
        private bool myFldBit3=false;
        private string myFldText1="";
        private string myFldText2 = "";
        private string myFldText3 = "";
        #endregion Fields

        #region Accessors
        public int TenID
        {
            get { return myTenID; }
            set { myTenID = value; }
        }
        public string Name
        {
            get { return myName; }
            set { myName = value; }
        }
        public string LandingName
        {
            get { return myLandingName; }
            set { myLandingName = value; }
        }
        public string AdminName
        {
            get { return myAdminName; }
            set { myAdminName = value; }
        }
        public bool isActiveFlag
        {
            get { return myisActiveFlag; }
            set { myisActiveFlag = value; }
        }
        public bool isMasterFlag
        {
            get { return myisMasterFlag; }
            set { myisMasterFlag = value; }
        }
        public string Description
        {
            get { return myDescription; }
            set { myDescription = value; }
        }
        public string DomainName
        {
            get { return myDomainName; }
            set { myDomainName = value; }
        }
        public bool showNotifications
        {
            get { return myshowNotifications; }
            set { myshowNotifications = value; }
        }
        public bool showOffers
        {
            get { return myshowOffers; }
            set { myshowOffers = value; }
        }
        public bool showBadges
        {
            get { return myshowBadges; }
            set { myshowBadges = value; }
        }
        public bool showEvents
        {
            get { return myshowEvents; }
            set { myshowEvents = value; }
        }
        public string NotificationsMenuText
        {
            get { return myNotificationsMenuText; }
            set { myNotificationsMenuText = value; }
        }
        public string OffersMenuText
        {
            get { return myOffersMenuText; }
            set { myOffersMenuText = value; }
        }
        public string BadgesMenuText
        {
            get { return myBadgesMenuText; }
            set { myBadgesMenuText = value; }
        }
        public string EventsMenuText
        {
            get { return myEventsMenuText; }
            set { myEventsMenuText = value; }
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
        #endregion Accessors

        #region Constructors
        public Tenant()
        {
        }
        #endregion Constructors

        #region Methods

        public static int GetTenantByProgram(int PID)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@Return_Value", -1);
            arrParams[1].Direction = ParameterDirection.ReturnValue;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Tenant_GetByProgramID", arrParams);
            return ((int)arrParams[1].Value);
        }

        public static int GetTenantByDomainName(string domain)
        {
            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@Domain", domain);
            arrParams[1] = new SqlParameter("@Return_Value", -1);
            arrParams[1].Direction = ParameterDirection.ReturnValue;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Tenant_GetByDomainName", arrParams);
            return ((int)arrParams[1].Value);
        }

        public static int GetMasterID()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@Return_Value", -1) {Direction = ParameterDirection.ReturnValue};

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Tenant_GetMasterID", arrParams);
            return ((int)arrParams[0].Value);
        }

        public static DataSet GetAllActive()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Tenant_GetAllActive");
        } 

        public static DataSet GetAll()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Tenant_GetAll");
        }

        public static Tenant FetchObject(int TenID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Tenant_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Tenant result = new Tenant();

                DateTime _datetime;

                int _int;

                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
                result.Name = dr["Name"].ToString();
                result.LandingName = dr["LandingName"].ToString();
                result.AdminName = dr["AdminName"].ToString();
                result.isActiveFlag = bool.Parse(dr["isActiveFlag"].ToString());
                result.isMasterFlag = bool.Parse(dr["isMasterFlag"].ToString());
                result.Description = dr["Description"].ToString();
                result.DomainName = dr["DomainName"].ToString();
                result.showNotifications = bool.Parse(dr["showNotifications"].ToString());
                result.showOffers = bool.Parse(dr["showOffers"].ToString());
                result.showBadges = bool.Parse(dr["showBadges"].ToString());
                result.showEvents = bool.Parse(dr["showEvents"].ToString());
                result.NotificationsMenuText = dr["NotificationsMenuText"].ToString();
                result.OffersMenuText = dr["OffersMenuText"].ToString();
                result.BadgesMenuText = dr["BadgesMenuText"].ToString();
                result.EventsMenuText = dr["EventsMenuText"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();
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

        public bool Fetch(int TenID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Tenant_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Tenant result = new Tenant();

                DateTime _datetime;

                int _int;

                if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
                this.Name = dr["Name"].ToString();
                this.LandingName = dr["LandingName"].ToString();
                this.AdminName = dr["AdminName"].ToString();
                this.isActiveFlag = bool.Parse(dr["isActiveFlag"].ToString());
                this.isMasterFlag = bool.Parse(dr["isMasterFlag"].ToString());
                this.Description = dr["Description"].ToString();
                this.DomainName = dr["DomainName"].ToString();
                this.showNotifications = bool.Parse(dr["showNotifications"].ToString());
                this.showOffers = bool.Parse(dr["showOffers"].ToString());
                this.showBadges = bool.Parse(dr["showBadges"].ToString());
                this.showEvents = bool.Parse(dr["showEvents"].ToString());
                this.NotificationsMenuText = dr["NotificationsMenuText"].ToString();
                this.OffersMenuText = dr["OffersMenuText"].ToString();
                this.BadgesMenuText = dr["BadgesMenuText"].ToString();
                this.EventsMenuText = dr["EventsMenuText"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();
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

        public static int Insert(Tenant o)
        {

            SqlParameter[] arrParams = new SqlParameter[29];

            arrParams[0] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[1] = new SqlParameter("@LandingName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LandingName, o.LandingName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@isActiveFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isActiveFlag, o.isActiveFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@isMasterFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isMasterFlag, o.isMasterFlag.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DomainName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DomainName, o.DomainName.GetTypeCode()));
            arrParams[7] = new SqlParameter("@showNotifications", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showNotifications, o.showNotifications.GetTypeCode()));
            arrParams[8] = new SqlParameter("@showOffers", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showOffers, o.showOffers.GetTypeCode()));
            arrParams[9] = new SqlParameter("@showBadges", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showBadges, o.showBadges.GetTypeCode()));
            arrParams[10] = new SqlParameter("@showEvents", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showEvents, o.showEvents.GetTypeCode()));
            arrParams[11] = new SqlParameter("@NotificationsMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationsMenuText, o.NotificationsMenuText.GetTypeCode()));
            arrParams[12] = new SqlParameter("@OffersMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OffersMenuText, o.OffersMenuText.GetTypeCode()));
            arrParams[13] = new SqlParameter("@BadgesMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgesMenuText, o.BadgesMenuText.GetTypeCode()));
            arrParams[14] = new SqlParameter("@EventsMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventsMenuText, o.EventsMenuText.GetTypeCode()));
            arrParams[15] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[16] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[17] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[18] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));
            arrParams[28] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[28].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Tenant_Insert", arrParams);

            o.TenID = int.Parse(arrParams[28].Value.ToString());

            return o.TenID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Tenant o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[29];

            arrParams[0] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LandingName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LandingName, o.LandingName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@isActiveFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isActiveFlag, o.isActiveFlag.GetTypeCode()));
            arrParams[5] = new SqlParameter("@isMasterFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isMasterFlag, o.isMasterFlag.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[7] = new SqlParameter("@DomainName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DomainName, o.DomainName.GetTypeCode()));
            arrParams[8] = new SqlParameter("@showNotifications", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showNotifications, o.showNotifications.GetTypeCode()));
            arrParams[9] = new SqlParameter("@showOffers", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showOffers, o.showOffers.GetTypeCode()));
            arrParams[10] = new SqlParameter("@showBadges", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showBadges, o.showBadges.GetTypeCode()));
            arrParams[11] = new SqlParameter("@showEvents", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.showEvents, o.showEvents.GetTypeCode()));
            arrParams[12] = new SqlParameter("@NotificationsMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationsMenuText, o.NotificationsMenuText.GetTypeCode()));
            arrParams[13] = new SqlParameter("@OffersMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.OffersMenuText, o.OffersMenuText.GetTypeCode()));
            arrParams[14] = new SqlParameter("@BadgesMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgesMenuText, o.BadgesMenuText.GetTypeCode()));
            arrParams[15] = new SqlParameter("@EventsMenuText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventsMenuText, o.EventsMenuText.GetTypeCode()));
            arrParams[16] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[17] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[18] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[19] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[28] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Tenant_Update", arrParams);

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

        public static int Delete(Tenant o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Tenant_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion Methods
    }
}