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

[Serializable]    public class PrizeTemplate : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myTID;
        private string myTName = "";
        private int myNumPrizes = 0;
        private bool myIncPrevWinnersFlag = false;
        private bool mySendNotificationFlag = false;
        private string myNotificationSubject ="";
        private string myNotificationMessage ="";
        private int myProgID = 0;
        private string myGender = "";
        private string mySchoolName = "";
        private int myPrimaryLibrary = 0;
        private int myMinPoints = 0;
        private int myMaxPoints= 0;
        private DateTime myLogDateStart;// = DateTime.MinValue;
        private DateTime myLogDateEnd;// = DateTime.MinValue;
        private int myMinReviews;
        private int myMaxReviews;
        private DateTime myReviewDateStart;
        private DateTime myReviewDateEnd;
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

        public int TID
        {
            get { return myTID; }
            set { myTID = value; }
        }
        public string TName
        {
            get { return myTName; }
            set { myTName = value; }
        }
        public int NumPrizes
        {
            get { return myNumPrizes; }
            set { myNumPrizes = value; }
        }
        public bool IncPrevWinnersFlag
        {
            get { return myIncPrevWinnersFlag; }
            set { myIncPrevWinnersFlag = value; }
        }
        public bool SendNotificationFlag
        {
            get { return mySendNotificationFlag; }
            set { mySendNotificationFlag = value; }
        }
        public string NotificationSubject
        {
            get { return myNotificationSubject; }
            set { myNotificationSubject = value; }
        }
        public string NotificationMessage
        {
            get { return myNotificationMessage; }
            set { myNotificationMessage = value; }
        }
        public int ProgID
        {
            get { return myProgID; }
            set { myProgID = value; }
        }
        public string Gender
        {
            get { return myGender; }
            set { myGender = value; }
        }
        public string SchoolName
        {
            get { return mySchoolName; }
            set { mySchoolName = value; }
        }
        public int PrimaryLibrary
        {
            get { return myPrimaryLibrary; }
            set { myPrimaryLibrary = value; }
        }
        public int MinPoints
        {
            get { return myMinPoints; }
            set { myMinPoints = value; }
        }
        public int MaxPoints
        {
            get { return myMaxPoints; }
            set { myMaxPoints = value; }
        }
        public DateTime LogDateStart
        {
            get { return myLogDateStart; }
            set { myLogDateStart = value; }
        }
        public DateTime LogDateEnd
        {
            get { return myLogDateEnd; }
            set { myLogDateEnd = value; }
        }
        public int MinReviews
        {
            get { return myMinReviews; }
            set { myMinReviews = value; }
        }
        public int MaxReviews
        {
            get { return myMaxReviews; }
            set { myMaxReviews = value; }
        }
        public DateTime ReviewDateStart
        {
            get { return myReviewDateStart; }
            set { myReviewDateStart = value; }
        }
        public DateTime ReviewDateEnd
        {
            get { return myReviewDateEnd; }
            set { myReviewDateEnd = value; }
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

        public PrizeTemplate()
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

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PrizeTemplate_GetAll", arrParams);
        }

        public static PrizeTemplate FetchObject(int TID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@TID", TID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PrizeTemplate_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PrizeTemplate result = new PrizeTemplate();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["TID"].ToString(), out _int)) result.TID = _int;
                result.TName = dr["TName"].ToString();
                if (int.TryParse(dr["NumPrizes"].ToString(), out _int)) result.NumPrizes = _int;
                result.IncPrevWinnersFlag = bool.Parse(dr["IncPrevWinnersFlag"].ToString());
                result.SendNotificationFlag = bool.Parse(dr["SendNotificationFlag"].ToString());
                result.NotificationSubject = dr["NotificationSubject"].ToString();
                result.NotificationMessage = dr["NotificationMessage"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) result.ProgID = _int;
                result.Gender = dr["Gender"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) result.PrimaryLibrary = _int;
                if (int.TryParse(dr["MinPoints"].ToString(), out _int)) result.MinPoints = _int;
                if (int.TryParse(dr["MaxPoints"].ToString(), out _int)) result.MaxPoints = _int;
                if (DateTime.TryParse(dr["LogDateStart"].ToString(), out _datetime)) result.LogDateStart = _datetime;
                if (DateTime.TryParse(dr["LogDateEnd"].ToString(), out _datetime)) result.LogDateEnd = _datetime;
                if (int.TryParse(dr["MinReviews"].ToString(), out _int)) result.MinReviews = _int;
                if (int.TryParse(dr["MaxReviews"].ToString(), out _int)) result.MaxReviews = _int;
                if (DateTime.TryParse(dr["ReviewDateStart"].ToString(), out _datetime)) result.ReviewDateStart = _datetime;
                if (DateTime.TryParse(dr["ReviewDateEnd"].ToString(), out _datetime)) result.ReviewDateEnd = _datetime;
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

        public bool Fetch(int TID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@TID", TID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PrizeTemplate_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PrizeTemplate result = new PrizeTemplate();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["TID"].ToString(), out _int)) this.TID = _int;
                this.TName = dr["TName"].ToString();
                if (int.TryParse(dr["NumPrizes"].ToString(), out _int)) this.NumPrizes = _int;
                this.IncPrevWinnersFlag = bool.Parse(dr["IncPrevWinnersFlag"].ToString());
                this.SendNotificationFlag = bool.Parse(dr["SendNotificationFlag"].ToString());
                this.NotificationSubject = dr["NotificationSubject"].ToString();
                this.NotificationMessage = dr["NotificationMessage"].ToString();
                if (int.TryParse(dr["ProgID"].ToString(), out _int)) this.ProgID = _int;
                this.Gender = dr["Gender"].ToString();
                this.SchoolName = dr["SchoolName"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) this.PrimaryLibrary = _int;
                if (int.TryParse(dr["MinPoints"].ToString(), out _int)) this.MinPoints = _int;
                if (int.TryParse(dr["MaxPoints"].ToString(), out _int)) this.MaxPoints = _int;
                if (DateTime.TryParse(dr["LogDateStart"].ToString(), out _datetime)) this.LogDateStart = _datetime;
                if (DateTime.TryParse(dr["LogDateEnd"].ToString(), out _datetime)) this.LogDateEnd = _datetime;
                if (int.TryParse(dr["MinReviews"].ToString(), out _int)) this.MinReviews = _int;
                if (int.TryParse(dr["MaxReviews"].ToString(), out _int)) this.MaxReviews = _int;
                if (DateTime.TryParse(dr["ReviewDateStart"].ToString(), out _datetime)) this.ReviewDateStart = _datetime;
                if (DateTime.TryParse(dr["ReviewDateEnd"].ToString(), out _datetime)) this.ReviewDateEnd = _datetime;
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

        public static int Insert(PrizeTemplate o)
        {

            SqlParameter[] arrParams = new SqlParameter[33];

            arrParams[0] = new SqlParameter("@TName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TName, o.TName.GetTypeCode()));
            arrParams[1] = new SqlParameter("@NumPrizes", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumPrizes, o.NumPrizes.GetTypeCode()));
            arrParams[2] = new SqlParameter("@IncPrevWinnersFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IncPrevWinnersFlag, o.IncPrevWinnersFlag.GetTypeCode()));
            arrParams[3] = new SqlParameter("@SendNotificationFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SendNotificationFlag, o.SendNotificationFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@NotificationSubject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationSubject, o.NotificationSubject.GetTypeCode()));
            arrParams[5] = new SqlParameter("@NotificationMessage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationMessage, o.NotificationMessage.GetTypeCode()));
            arrParams[6] = new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[8] = new SqlParameter("@SchoolName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[9] = new SqlParameter("@PrimaryLibrary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary, o.PrimaryLibrary.GetTypeCode()));
            arrParams[10] = new SqlParameter("@MinPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MinPoints, o.MinPoints.GetTypeCode()));
            arrParams[11] = new SqlParameter("@MaxPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxPoints, o.MaxPoints.GetTypeCode()));
            arrParams[12] = new SqlParameter("@LogDateStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogDateStart, o.LogDateStart.GetTypeCode()));
            arrParams[13] = new SqlParameter("@LogDateEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogDateEnd, o.LogDateEnd.GetTypeCode()));
            arrParams[14] = new SqlParameter("@MinReviews", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MinReviews, o.MinReviews.GetTypeCode()));
            arrParams[15] = new SqlParameter("@MaxReviews", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxReviews, o.MaxReviews.GetTypeCode()));
            arrParams[16] = new SqlParameter("@ReviewDateStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDateStart, o.ReviewDateStart.GetTypeCode()));
            arrParams[17] = new SqlParameter("@ReviewDateEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDateEnd, o.ReviewDateEnd.GetTypeCode()));
            arrParams[18] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[19] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[20] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[21] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[22] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[28] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[29] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[30] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[31] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[32] = new SqlParameter("@TID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TID, o.TID.GetTypeCode()));
            arrParams[32].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeTemplate_Insert", arrParams);

            o.TID = int.Parse(arrParams[32].Value.ToString());

            return o.TID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PrizeTemplate o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[33];

            arrParams[0] = new SqlParameter("@TID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TID, o.TID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TName, o.TName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@NumPrizes", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumPrizes, o.NumPrizes.GetTypeCode()));
            arrParams[3] = new SqlParameter("@IncPrevWinnersFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.IncPrevWinnersFlag, o.IncPrevWinnersFlag.GetTypeCode()));
            arrParams[4] = new SqlParameter("@SendNotificationFlag", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SendNotificationFlag, o.SendNotificationFlag.GetTypeCode()));
            arrParams[5] = new SqlParameter("@NotificationSubject", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationSubject, o.NotificationSubject.GetTypeCode()));
            arrParams[6] = new SqlParameter("@NotificationMessage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NotificationMessage, o.NotificationMessage.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[9] = new SqlParameter("@SchoolName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[10] = new SqlParameter("@PrimaryLibrary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary, o.PrimaryLibrary.GetTypeCode()));
            arrParams[11] = new SqlParameter("@MinPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MinPoints, o.MinPoints.GetTypeCode()));
            arrParams[12] = new SqlParameter("@MaxPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxPoints, o.MaxPoints.GetTypeCode()));
            arrParams[13] = new SqlParameter("@LogDateStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogDateStart, o.LogDateStart.GetTypeCode()));
            arrParams[14] = new SqlParameter("@LogDateEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LogDateEnd, o.LogDateEnd.GetTypeCode()));
            arrParams[15] = new SqlParameter("@MinReviews", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MinReviews, o.MinReviews.GetTypeCode()));
            arrParams[16] = new SqlParameter("@MaxReviews", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxReviews, o.MaxReviews.GetTypeCode()));
            arrParams[17] = new SqlParameter("@ReviewDateStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDateStart, o.ReviewDateStart.GetTypeCode()));
            arrParams[18] = new SqlParameter("@ReviewDateEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDateEnd, o.ReviewDateEnd.GetTypeCode()));
            arrParams[19] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[20] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[21] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[22] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            
            arrParams[23] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[28] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[29] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[30] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[31] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[32] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeTemplate_Update", arrParams);

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

        public static int Delete(PrizeTemplate o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@TID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TID, o.TID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeTemplate_Delete", arrParams);

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

