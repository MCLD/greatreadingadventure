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

namespace GRA.SRP.DAL
{

[Serializable]    public class Minigame : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myMGID;
        private int myMiniGameType;
        private string myMiniGameTypeName;
        private string myAdminName="";
        private string myGameName="";
        private bool myisActive;
        private int myNumberPoints;
        private int myAwardedBadgeID;
        private string myAcknowledgements = "";
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

        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public int MiniGameType
        {
            get { return myMiniGameType; }
            set { myMiniGameType = value; }
        }
        public string MiniGameTypeName
        {
            get { return myMiniGameTypeName; }
            set { myMiniGameTypeName = value; }
        }
        public string AdminName
        {
            get { return myAdminName; }
            set { myAdminName = value; }
        }
        public string GameName
        {
            get { return myGameName; }
            set { myGameName = value; }
        }
        public bool isActive
        {
            get { return myisActive; }
            set { myisActive = value; }
        }
        public int NumberPoints
        {
            get { return myNumberPoints; }
            set { myNumberPoints = value; }
        }
        public int AwardedBadgeID
        {
            get { return myAwardedBadgeID; }
            set { myAwardedBadgeID = value; }
        }
        public string Acknowledgements
        {
            get { return myAcknowledgements; }
            set { myAcknowledgements = value; }
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

        public Minigame()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetMinigamesList(string IDList)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@IDList", IDList);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Minigame_GetList", arrParams);
        }

        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Minigame_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Minigame_GetAll", arrParams);
        }

        public static Minigame FetchObject(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Minigame_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Minigame result = new Minigame();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["MiniGameType"].ToString(), out _int)) result.MiniGameType = _int;
                result.MiniGameTypeName = dr["MiniGameTypeName"].ToString();
                result.AdminName = dr["AdminName"].ToString();
                result.GameName = dr["GameName"].ToString();
                result.isActive = bool.Parse(dr["isActive"].ToString());
                if (int.TryParse(dr["NumberPoints"].ToString(), out _int)) result.NumberPoints = _int;
                if (int.TryParse(dr["AwardedBadgeID"].ToString(), out _int)) result.AwardedBadgeID = _int;
                result.Acknowledgements = dr["Acknowledgements"].ToString();
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

        public bool Fetch(int MGID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Minigame_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Minigame result = new Minigame();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["MiniGameType"].ToString(), out _int)) this.MiniGameType = _int;
                this.MiniGameTypeName = dr["MiniGameTypeName"].ToString();
                this.AdminName = dr["AdminName"].ToString();
                this.GameName = dr["GameName"].ToString();
                this.isActive = bool.Parse(dr["isActive"].ToString());
                if (int.TryParse(dr["NumberPoints"].ToString(), out _int)) this.NumberPoints = _int;
                if (int.TryParse(dr["AwardedBadgeID"].ToString(), out _int)) this.AwardedBadgeID = _int;
                this.Acknowledgements = dr["Acknowledgements"].ToString();
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

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Minigame o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[23];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MiniGameType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiniGameType, o.MiniGameType.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MiniGameTypeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiniGameTypeName, o.MiniGameTypeName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@GameName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameName, o.GameName.GetTypeCode()));
            arrParams[5] = new SqlParameter("@isActive", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isActive, o.isActive.GetTypeCode()));
            arrParams[6] = new SqlParameter("@NumberPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AwardedBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardedBadgeID, o.AwardedBadgeID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Acknowledgements", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Acknowledgements, o.Acknowledgements.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[12] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[13] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Minigame_Update", arrParams);

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

        public static int Delete(Minigame o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Minigame_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }


        public int Insert()
        {

            return Insert(this);

        }

        public int InsertBaseOnly()
        {

            return InsertBaseOnly(this);

        }

        public static int Insert(Minigame o)
        {

            SqlParameter[] arrParams = new SqlParameter[23];

            arrParams[0] = new SqlParameter("@MiniGameType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiniGameType, o.MiniGameType.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MiniGameTypeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiniGameTypeName, o.MiniGameTypeName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@GameName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameName, o.GameName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@isActive", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isActive, o.isActive.GetTypeCode()));
            arrParams[5] = new SqlParameter("@NumberPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AwardedBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardedBadgeID, o.AwardedBadgeID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Acknowledgements", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Acknowledgements, o.Acknowledgements.GetTypeCode())); 
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[12] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[22] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[22].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Minigame_Insert", arrParams);

            o.MGID = int.Parse(arrParams[22].Value.ToString());


            if (o.MiniGameType == 1)
            {
                var o2 = new MGOnlineBook {MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate,LastModUser = o.LastModUser };
                o2.Insert();
            }
            if (o.MiniGameType == 2)
            {
                var o2 = new MGMixAndMatch() { MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate, LastModUser = o.LastModUser };
                o2.Insert();
            }
            if (o.MiniGameType == 3)
            {
                var o2 = new MGCodeBreaker() { MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate, LastModUser = o.LastModUser };
                o2.Insert();
            }            
            
            if (o.MiniGameType == 4)
            {
                var o2 = new MGWordMatch() { MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate, LastModUser = o.LastModUser };
                o2.Insert();
            }


            if (o.MiniGameType == 5)
            {
                var o2 = new MGMatchingGame() { MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate, LastModUser = o.LastModUser };
                o2.Insert();
            }


            if (o.MiniGameType == 6)
            {
                var o2 = new MGHiddenPic() { MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate, LastModUser = o.LastModUser };
                o2.Insert();
            }


            if (o.MiniGameType == 7)
            {
                var o2 = new MGChooseAdv() { MGID = o.MGID, AddedDate = o.AddedDate, AddedUser = o.AddedUser, LastModDate = o.LastModDate, LastModUser = o.LastModUser };
                o2.Insert();
            }
            

            return o.MGID;

        }

        public static int InsertBaseOnly(Minigame o)
        {

            SqlParameter[] arrParams = new SqlParameter[23];

            arrParams[0] = new SqlParameter("@MiniGameType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiniGameType, o.MiniGameType.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MiniGameTypeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MiniGameTypeName, o.MiniGameTypeName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@GameName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameName, o.GameName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@isActive", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.isActive, o.isActive.GetTypeCode()));
            arrParams[5] = new SqlParameter("@NumberPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumberPoints, o.NumberPoints.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AwardedBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardedBadgeID, o.AwardedBadgeID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Acknowledgements", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Acknowledgements, o.Acknowledgements.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[12] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[22] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[22].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Minigame_Insert", arrParams);

            o.MGID = int.Parse(arrParams[22].Value.ToString());


            return o.MGID;

        }


        #endregion


        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                
                if (MiniGameType == 0)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("MiniGameType", "Adventure Type", "You must choose an Adventure Type.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }
            return (ErrorCodes.Count == 0);
            //return true;
        }

        public static string GetEditPage(int miniGameType)
        {
            if (miniGameType == 1) return "MGOnlineBookAddEdit.aspx";
            if (miniGameType == 2) return "MGMixAndMatchAddEdit.aspx";
            if (miniGameType == 3) return "MGCodeBreakerAddEdit.aspx";
            if (miniGameType == 4) return "MGWordMatchAddEdit.aspx";
            if (miniGameType == 5) return "MGMatchingGameAddEdit.aspx";
            if (miniGameType == 6) return "MGHiddenPicAddEdit.aspx";
            if (miniGameType == 7) return "MGChooseAdvAddEdit.aspx";

            

            return "/";
        } 

    }//end class

}//end namespace

