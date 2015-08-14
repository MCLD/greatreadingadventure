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

[Serializable]    public class CustomRegistrationFields : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myCID;
        private bool myUse1 = false;
        private string myLabel1 = "";
        private string myDDValues1 ="";
        private bool myUse2 = false;
        private bool myUse3 = false;
        private bool myUse4 = false;
        private bool myUse5 = false;
        private string myLabel2 = "";
        private string myLabel3 = "";
        private string myLabel4 = "";
        private string myLabel5 = "";
        private string myDDValues2 = "";
        private string myDDValues3 = "";
        private string myDDValues4 = "";
        private string myDDValues5 = "";
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

        public int CID
        {
            get { return myCID; }
            set { myCID = value; }
        }
        public bool Use1
        {
            get { return myUse1; }
            set { myUse1 = value; }
        }
        public string Label1
        {
            get { return myLabel1; }
            set { myLabel1 = value; }
        }
        public string DDValues1
        {
            get { return myDDValues1; }
            set { myDDValues1 = value; }
        }
        public bool Use2
        {
            get { return myUse2; }
            set { myUse2 = value; }
        }
        public bool Use3
        {
            get { return myUse3; }
            set { myUse3 = value; }
        }
        public bool Use4
        {
            get { return myUse4; }
            set { myUse4 = value; }
        }
        public bool Use5
        {
            get { return myUse5; }
            set { myUse5 = value; }
        }
        public string Label2
        {
            get { return myLabel2; }
            set { myLabel2 = value; }
        }
        public string Label3
        {
            get { return myLabel3; }
            set { myLabel3 = value; }
        }
        public string Label4
        {
            get { return myLabel4; }
            set { myLabel4 = value; }
        }
        public string Label5
        {
            get { return myLabel5; }
            set { myLabel5 = value; }
        }
        public string DDValues2
        {
            get { return myDDValues2; }
            set { myDDValues2 = value; }
        }
        public string DDValues3
        {
            get { return myDDValues3; }
            set { myDDValues3 = value; }
        }
        public string DDValues4
        {
            get { return myDDValues4; }
            set { myDDValues4 = value; }
        }
        public string DDValues5
        {
            get { return myDDValues5; }
            set { myDDValues5 = value; }
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

        public CustomRegistrationFields()
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
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_CustomRegistrationFields_GetAll", arrParams);
        }

        public static CustomRegistrationFields FetchObject()
        {

            // declare reader

            SqlDataReader dr;

            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_CustomRegistrationFields_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                CustomRegistrationFields result = new CustomRegistrationFields();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CID"].ToString(), out _int)) result.CID = _int;
                result.Use1 = bool.Parse(dr["Use1"].ToString());
                result.Label1 = dr["Label1"].ToString();
                result.DDValues1 = dr["DDValues1"].ToString();
                result.Use2 = bool.Parse(dr["Use2"].ToString());
                result.Use3 = bool.Parse(dr["Use3"].ToString());
                result.Use4 = bool.Parse(dr["Use4"].ToString());
                result.Use5 = bool.Parse(dr["Use5"].ToString());
                result.Label2 = dr["Label2"].ToString();
                result.Label3 = dr["Label3"].ToString();
                result.Label4 = dr["Label4"].ToString();
                result.Label5 = dr["Label5"].ToString();
                result.DDValues2 = dr["DDValues2"].ToString();
                result.DDValues3 = dr["DDValues3"].ToString();
                result.DDValues4 = dr["DDValues4"].ToString();
                result.DDValues5 = dr["DDValues5"].ToString();
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

        public bool Fetch()
        {

            // declare reader

            SqlDataReader dr;

            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_CustomRegistrationFields_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                CustomRegistrationFields result = new CustomRegistrationFields();

                DateTime _datetime;

                int _int;

               // decimal _decimal;

                if (int.TryParse(dr["CID"].ToString(), out _int)) this.CID = _int;
                this.Use1 = bool.Parse(dr["Use1"].ToString());
                this.Label1 = dr["Label1"].ToString();
                this.DDValues1 = dr["DDValues1"].ToString();
                this.Use2 = bool.Parse(dr["Use2"].ToString());
                this.Use3 = bool.Parse(dr["Use3"].ToString());
                this.Use4 = bool.Parse(dr["Use4"].ToString());
                this.Use5 = bool.Parse(dr["Use5"].ToString());
                this.Label2 = dr["Label2"].ToString();
                this.Label3 = dr["Label3"].ToString();
                this.Label4 = dr["Label4"].ToString();
                this.Label5 = dr["Label5"].ToString();
                this.DDValues2 = dr["DDValues2"].ToString();
                this.DDValues3 = dr["DDValues3"].ToString();
                this.DDValues4 = dr["DDValues4"].ToString();
                this.DDValues5 = dr["DDValues5"].ToString();
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

        public static int Insert(CustomRegistrationFields o)
        {

            SqlParameter[] arrParams = new SqlParameter[30];

            arrParams[0] = new SqlParameter("@Use1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use1, o.Use1.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Label1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label1, o.Label1.GetTypeCode()));
            arrParams[2] = new SqlParameter("@DDValues1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues1, o.DDValues1.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Use2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use2, o.Use2.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Use3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use3, o.Use3.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Use4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use4, o.Use4.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Use5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use5, o.Use5.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Label2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label2, o.Label2.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Label3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label3, o.Label3.GetTypeCode()));
            arrParams[9] = new SqlParameter("@Label4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label4, o.Label4.GetTypeCode()));
            arrParams[10] = new SqlParameter("@Label5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label5, o.Label5.GetTypeCode()));
            arrParams[11] = new SqlParameter("@DDValues2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues2, o.DDValues2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@DDValues3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues3, o.DDValues3.GetTypeCode()));
            arrParams[13] = new SqlParameter("@DDValues4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues4, o.DDValues4.GetTypeCode()));
            arrParams[14] = new SqlParameter("@DDValues5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues5, o.DDValues5.GetTypeCode()));
            arrParams[15] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[16] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[17] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[18] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[19] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[28] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[29] = new SqlParameter("@CID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CID, o.CID.GetTypeCode()));
            arrParams[29].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_CustomRegistrationFields_Insert", arrParams);

            o.CID = int.Parse(arrParams[29].Value.ToString());

            return o.CID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(CustomRegistrationFields o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[30];

            arrParams[0] = new SqlParameter("@CID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CID, o.CID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Use1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use1, o.Use1.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Label1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label1, o.Label1.GetTypeCode()));
            arrParams[3] = new SqlParameter("@DDValues1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues1, o.DDValues1.GetTypeCode()));
            arrParams[4] = new SqlParameter("@Use2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use2, o.Use2.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Use3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use3, o.Use3.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Use4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use4, o.Use4.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Use5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Use5, o.Use5.GetTypeCode()));
            arrParams[8] = new SqlParameter("@Label2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label2, o.Label2.GetTypeCode()));
            arrParams[9] = new SqlParameter("@Label3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label3, o.Label3.GetTypeCode()));
            arrParams[10] = new SqlParameter("@Label4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label4, o.Label4.GetTypeCode()));
            arrParams[11] = new SqlParameter("@Label5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label5, o.Label5.GetTypeCode()));
            arrParams[12] = new SqlParameter("@DDValues2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues2, o.DDValues2.GetTypeCode()));
            arrParams[13] = new SqlParameter("@DDValues3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues3, o.DDValues3.GetTypeCode()));
            arrParams[14] = new SqlParameter("@DDValues4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues4, o.DDValues4.GetTypeCode()));
            arrParams[15] = new SqlParameter("@DDValues5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DDValues5, o.DDValues5.GetTypeCode()));
            arrParams[16] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[17] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[18] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[19] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[20] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[23] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[24] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[25] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[26] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[27] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[28] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[29] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_CustomRegistrationFields_Update", arrParams);

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

        public static int Delete(CustomRegistrationFields o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CID, o.CID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_CustomRegistrationFields_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

        public static string F1Label()
        {
            var o = CustomRegistrationFields.FetchObject();
            if (!o.Use1) return "Custom Field 1 (Not Active)";
            return o.Label1;
        }

        public static string F2Label()
        {
            var o = CustomRegistrationFields.FetchObject();
            if (!o.Use2) return "Custom Field 2 (Not Active)";
            return o.Label2;
        }

        public static string F3Label()
        {
            var o = CustomRegistrationFields.FetchObject();
            if (!o.Use3) return "Custom Field 3 (Not Active)";
            return o.Label3;
        }

        public static string F4Label()
        {
            var o = CustomRegistrationFields.FetchObject();
            if (!o.Use4) return "Custom Field 4 (Not Active)";
            return o.Label4;
        }

        public static string F5Label()
        {
            var o = CustomRegistrationFields.FetchObject();
            if (!o.Use5) return "Custom Field 5 (Not Active)";
            return o.Label5;
        }



    }//end class

}//end namespace

