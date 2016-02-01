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

[Serializable]    public class BookListBooks : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myBLBID;
        private int myBLID;
        private string myAuthor  = "";
        private string myTitle = "";
        private string myISBN = "";
        private string myURL = "";
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
        #endregion

        #region Accessors

        public int BLBID
        {
            get { return myBLBID; }
            set { myBLBID = value; }
        }
        public int BLID
        {
            get { return myBLID; }
            set { myBLID = value; }
        }
        public string Author
        {
            get { return myAuthor; }
            set { myAuthor = value; }
        }
        public string Title
        {
            get { return myTitle; }
            set { myTitle = value; }
        }
        public string ISBN
        {
            get { return myISBN; }
            set { myISBN = value; }
        }
        public string URL
        {
            get { return myURL; }
            set { myURL = value; }
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

        public BookListBooks()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                if (string.IsNullOrEmpty(Title))
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Title", "Title", "You must enter a Title.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }
            return (ErrorCodes.Count == 0);
            //return true;
        }


        #region stored procedure wrappers

        public static DataSet GetForDisplay(int BLID, int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@BLID", BLID);
            arrParams[1] = new SqlParameter("@PID", PID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_BookListBooks_GetForPatronDisplay", arrParams);
        }



        public static DataSet GetAll(int BLID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BLID", BLID);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_BookListBooks_GetAll", arrParams);
        }

        public static BookListBooks FetchObject(int BLBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BLBID", BLBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_BookListBooks_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                BookListBooks result = new BookListBooks();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BLBID"].ToString(), out _int)) result.BLBID = _int;
                if (int.TryParse(dr["BLID"].ToString(), out _int)) result.BLID = _int;
                result.Author = dr["Author"].ToString();
                result.Title = dr["Title"].ToString();
                result.ISBN = dr["ISBN"].ToString();
                result.URL = dr["URL"].ToString();
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

        public bool Fetch(int BLBID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@BLBID", BLBID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_BookListBooks_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                BookListBooks result = new BookListBooks();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["BLBID"].ToString(), out _int)) this.BLBID = _int;
                if (int.TryParse(dr["BLID"].ToString(), out _int)) this.BLID = _int;
                this.Author = dr["Author"].ToString();
                this.Title = dr["Title"].ToString();
                this.ISBN = dr["ISBN"].ToString();
                this.URL = dr["URL"].ToString();
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

        public static int Insert(BookListBooks o)
        {

            SqlParameter[] arrParams = new SqlParameter[20];

            arrParams[0] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Author", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Author, o.Author.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ISBN", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ISBN, o.ISBN.GetTypeCode()));
            arrParams[4] = new SqlParameter("@URL", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.URL, o.URL.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[9] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[19] = new SqlParameter("@BLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLBID, o.BLBID.GetTypeCode()));
            arrParams[19].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookListBooks_Insert", arrParams);

            o.BLBID = int.Parse(arrParams[19].Value.ToString());

            return o.BLBID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(BookListBooks o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[20];

            arrParams[0] = new SqlParameter("@BLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLBID, o.BLBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@BLID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLID, o.BLID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Author", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Author, o.Author.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Title", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Title, o.Title.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ISBN", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ISBN, o.ISBN.GetTypeCode()));
            arrParams[5] = new SqlParameter("@URL", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.URL, o.URL.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[10] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookListBooks_Update", arrParams);

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

        public static int Delete(BookListBooks o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@BLBID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BLBID, o.BLBID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                        (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                -1 :
                                (int)HttpContext.Current.Session["TenantID"])
                    );

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_BookListBooks_Delete", arrParams);

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

