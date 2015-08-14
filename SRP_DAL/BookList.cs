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

[Serializable]    public class BookList : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myBLID;
        private string myAdminName = "";
        private string myListName = "";
        private string myAdminDescription = "";
        private string myDescription = "";
        private int myLiteracyLevel1 = 0;
        private int myLiteracyLevel2 = 0;
        private int myProgID = 0;
        private int myLibraryID = 0;
        private int myAwardBadgeID = 0;
        private int myAwardPoints = 0;
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

        private int myNumBooksToComplete = 0;
        #endregion

        #region Accessors

        public int BLID
        {
            get { return myBLID; }
            set { myBLID = value; }
        }
        public string AdminName
        {
            get { return myAdminName; }
            set { myAdminName = value; }
        }
        public string ListName
        {
            get { return myListName; }
            set { myListName = value; }
        }
        public string AdminDescription
        {
            get { return myAdminDescription; }
            set { myAdminDescription = value; }
        }
        public string Description
        {
            get { return myDescription; }
            set { myDescription = value; }
        }
        public int LiteracyLevel1
        {
            get { return myLiteracyLevel1; }
            set { myLiteracyLevel1 = value; }
        }
        public int LiteracyLevel2
        {
            get { return myLiteracyLevel2; }
            set { myLiteracyLevel2 = value; }
        }
        public int ProgID
        {
            get { return myProgID; }
            set { myProgID = value; }
        }
        public int LibraryID
        {
            get { return myLibraryID; }
            set { myLibraryID = value; }
        }
        public int AwardBadgeID
        {
            get { return myAwardBadgeID; }
            set { myAwardBadgeID = value; }
        }
        public int AwardPoints
        {
            get { return myAwardPoints; }
            set { myAwardPoints = value; }
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

        public int NumBooksToComplete
        {
            get { return myNumBooksToComplete; }
            set { myNumBooksToComplete = value; }
        }

        #endregion

        #region Constructors

        public BookList()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetForDisplay(int PID)
        {
            var TenID =
                (HttpContext.Current.Session["TenantID"] == null ||
                 HttpContext.Current.Session["TenantID"].ToString() == ""
                     ? -1
                     : (int)HttpContext.Current.Session["TenantID"]);
            return GetForDisplay(PID, TenID);
        }

        public static DataSet GetForDisplay(int PID, int TenID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", PID);
            arrParams[1] = new SqlParameter("@TenID", TenID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_BookList_GetForDisplay", arrParams);
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

            SqlParameter[] arrParams = new SqlParameter[26];

            arrParams[0] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[1] = new SqlParameter("@ListName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ListName, o.ListName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@AdminDescription", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminDescription, o.AdminDescription.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LiteracyLevel1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LiteracyLevel2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode()));
            arrParams[6] = new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LibraryID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryID, o.LibraryID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AwardBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeID, o.AwardBadgeID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AwardPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardPoints, o.AwardPoints.GetTypeCode()));
            arrParams[10] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[12] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[13] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[14] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[24] = new SqlParameter("@NumBooksToComplete", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumBooksToComplete, o.NumBooksToComplete.GetTypeCode()));
            
            arrParams[25] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[25].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookList_Insert", arrParams);

            o.BLID = int.Parse(arrParams[25].Value.ToString());

            return o.BLID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(BookList o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[26];

            arrParams[0] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@AdminName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminName, o.AdminName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ListName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ListName, o.ListName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@AdminDescription", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AdminDescription, o.AdminDescription.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LiteracyLevel1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LiteracyLevel2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ProgID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgID, o.ProgID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LibraryID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LibraryID, o.LibraryID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AwardBadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardBadgeID, o.AwardBadgeID.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AwardPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardPoints, o.AwardPoints.GetTypeCode()));
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

            arrParams[25] = new SqlParameter("@NumBooksToComplete", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumBooksToComplete, o.NumBooksToComplete.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookList_Update", arrParams);

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

    }//end class

}//end namespace

