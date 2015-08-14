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

[Serializable]    public class Award : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myAID;
        private string myAwardName ="";
        private int myBadgeID;
        private int myNumPoints;
        private int myBranchID;
        private int myProgramID;
        private string myDistrict="";
        private string mySchoolName="";
        private string myBadgeList="";
        private DateTime myLastModDate;
        private string myLastModUser="N/A";
        private DateTime myAddedDate;
        private string myAddedUser="N/A";

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

        public int AID
        {
            get { return myAID; }
            set { myAID = value; }
        }
        public string AwardName
        {
            get { return myAwardName; }
            set { myAwardName = value; }
        }
        public int BadgeID
        {
            get { return myBadgeID; }
            set { myBadgeID = value; }
        }
        public int NumPoints
        {
            get { return myNumPoints; }
            set { myNumPoints = value; }
        }
        public int BranchID
        {
            get { return myBranchID; }
            set { myBranchID = value; }
        }
        public int ProgramID
        {
            get { return myProgramID; }
            set { myProgramID = value; }
        }
        public string District
        {
            get { return myDistrict; }
            set { myDistrict = value; }
        }
        public string SchoolName
        {
            get { return mySchoolName; }
            set { mySchoolName = value; }
        }
        public string BadgeList
        {
            get { return myBadgeList; }
            set { myBadgeList = value; }
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

        public Award()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetMatchingAwards (int pid)
        {
            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PID", pid);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Award_GetPatronQualifyingAwards", arrParams);
        }

        public static DataSet GetMatchingAwards(int pid, int tenid)
        {
            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@PID", pid);
            arrParams[1] = new SqlParameter("@TenId", tenid);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Award_GetPatronQualifyingAwardsWTenant", arrParams);
        }

        public static DataSet GetBadgeListMembership(string list)
        {
            //SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@BadgeList", list);
            arrParams[1] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Award_GetBadgeListMembership", arrParams);
        }


        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Award_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Award_GetAll", arrParams);
        }

        public static Award FetchObject(int AID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@AID", AID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Award_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Award result = new Award();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["AID"].ToString(), out _int)) result.AID = _int;
                result.AwardName = dr["AwardName"].ToString();
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) result.BadgeID = _int;
                if (int.TryParse(dr["NumPoints"].ToString(), out _int)) result.NumPoints = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) result.BranchID = _int;
                if (int.TryParse(dr["ProgramID"].ToString(), out _int)) result.ProgramID = _int;
                result.District = dr["District"].ToString();
                result.SchoolName = dr["SchoolName"].ToString();
                result.BadgeList = dr["BadgeList"].ToString();
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

        public bool Fetch(int AID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@AID", AID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_Award_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                Award result = new Award();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["AID"].ToString(), out _int)) this.AID = _int;
                this.AwardName = dr["AwardName"].ToString();
                if (int.TryParse(dr["BadgeID"].ToString(), out _int)) this.BadgeID = _int;
                if (int.TryParse(dr["NumPoints"].ToString(), out _int)) this.NumPoints = _int;
                if (int.TryParse(dr["BranchID"].ToString(), out _int)) this.BranchID = _int;
                if (int.TryParse(dr["ProgramID"].ToString(), out _int)) this.ProgramID = _int;
                this.District = dr["District"].ToString();
                this.SchoolName = dr["SchoolName"].ToString();
                this.BadgeList = dr["BadgeList"].ToString();
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

        public static int Insert(Award o)
        {

            SqlParameter[] arrParams = new SqlParameter[23];

            arrParams[0] = new SqlParameter("@AwardName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardName, o.AwardName.GetTypeCode()));
            arrParams[1] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@NumPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumPoints, o.NumPoints.GetTypeCode()));
            arrParams[3] = new SqlParameter("@BranchID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ProgramID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgramID, o.ProgramID.GetTypeCode()));
            arrParams[5] = new SqlParameter("@District", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District, o.District.GetTypeCode()));
            arrParams[6] = new SqlParameter("@SchoolName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[7] = new SqlParameter("@BadgeList", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeList, o.BadgeList.GetTypeCode()));
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

            arrParams[22] = new SqlParameter("@AID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AID, o.AID.GetTypeCode()));
            arrParams[22].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Award_Insert", arrParams);

            o.AID = int.Parse(arrParams[22].Value.ToString());

            return o.AID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(Award o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[23];

            arrParams[0] = new SqlParameter("@AID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AID, o.AID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@AwardName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AwardName, o.AwardName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@NumPoints", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumPoints, o.NumPoints.GetTypeCode()));
            arrParams[4] = new SqlParameter("@BranchID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BranchID, o.BranchID.GetTypeCode()));
            arrParams[5] = new SqlParameter("@ProgramID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgramID, o.ProgramID.GetTypeCode()));
            arrParams[6] = new SqlParameter("@District", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District, o.District.GetTypeCode()));
            arrParams[7] = new SqlParameter("@SchoolName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[8] = new SqlParameter("@BadgeList", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeList, o.BadgeList.GetTypeCode()));
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

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Award_Update", arrParams);

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

        public static int Delete(Award o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@AID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AID, o.AID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Award_Delete", arrParams);

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

