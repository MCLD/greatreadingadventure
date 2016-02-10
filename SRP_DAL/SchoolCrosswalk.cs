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
using System.Collections.Generic;

namespace GRA.SRP.DAL
{

[Serializable]    public class SchoolCrosswalk : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myID;
        private int mySchoolID;
        private int mySchTypeID;
        private int myDistrictID;
        private string myCity;
        private int myMinGrade;
        private int myMaxGrade;
        private int myMinAge;
        private int myMaxAge;

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

        public int ID
        {
            get { return myID; }
            set { myID = value; }
        }
        public int SchoolID
        {
            get { return mySchoolID; }
            set { mySchoolID = value; }
        }
        public int SchTypeID
        {
            get { return mySchTypeID; }
            set { mySchTypeID = value; }
        }
        public int DistrictID
        {
            get { return myDistrictID; }
            set { myDistrictID = value; }
        }
        public string City
        {
            get { return myCity; }
            set { myCity = value; }
        }
        public int MinGrade
        {
            get { return myMinGrade; }
            set { myMinGrade = value; }
        }
        public int MaxGrade
        {
            get { return myMaxGrade; }
            set { myMaxGrade = value; }
        }
        public int MinAge
        {
            get { return myMinAge; }
            set { myMinAge = value; }
        }
        public int MaxAge
        {
            get { return myMaxAge; }
            set { myMaxAge = value; }
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

        public SchoolCrosswalk()
        {
            this.TenID = (HttpContext.Current.Session["TenantID"] == null ||
                 HttpContext.Current.Session["TenantID"].ToString() == ""
                     ? -1
                     : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetFilteredSchoolDDValues(int schTypeID, int districtID, string city, int grade, int age)
        {
            SqlParameter[] arrParams = new SqlParameter[6];
            arrParams[0] = new SqlParameter("@SchTypeID", schTypeID);
            arrParams[1] = new SqlParameter("@DistrictID", districtID);
            arrParams[2] = new SqlParameter("@City", city);
            arrParams[3] = new SqlParameter("@Grade", grade);
            arrParams[4] = new SqlParameter("@Age", age);
            arrParams[5] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_GetFilteredSchoolDDValues", arrParams);
        }


        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_GetAll", arrParams);
        }

        public static SchoolCrosswalk FetchObjectBySchoolID(int ID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", ID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_GetBySchoolID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SchoolCrosswalk result = new SchoolCrosswalk();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["ID"].ToString(), out _int)) result.ID = _int;
                if (int.TryParse(dr["SchoolID"].ToString(), out _int)) result.SchoolID = _int;
                if (int.TryParse(dr["SchTypeID"].ToString(), out _int)) result.SchTypeID = _int;
                if (int.TryParse(dr["DistrictID"].ToString(), out _int)) result.DistrictID = _int;
                result.City = dr["City"].ToString();
                if (int.TryParse(dr["MinGrade"].ToString(), out _int)) result.MinGrade = _int;
                if (int.TryParse(dr["MaxGrade"].ToString(), out _int)) result.MaxGrade = _int;
                if (int.TryParse(dr["MinAge"].ToString(), out _int)) result.MinAge = _int;
                if (int.TryParse(dr["MaxAge"].ToString(), out _int)) result.MaxAge = _int;

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

        public static SchoolCrosswalk FetchObject(int ID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", ID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SchoolCrosswalk result = new SchoolCrosswalk();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["ID"].ToString(), out _int)) result.ID = _int;
                if (int.TryParse(dr["SchoolID"].ToString(), out _int)) result.SchoolID = _int;
                if (int.TryParse(dr["SchTypeID"].ToString(), out _int)) result.SchTypeID = _int;
                if (int.TryParse(dr["DistrictID"].ToString(), out _int)) result.DistrictID = _int;
                result.City = dr["City"].ToString();
                if (int.TryParse(dr["MinGrade"].ToString(), out _int)) result.MinGrade = _int;
                if (int.TryParse(dr["MaxGrade"].ToString(), out _int)) result.MaxGrade = _int;
                if (int.TryParse(dr["MinAge"].ToString(), out _int)) result.MinAge = _int;
                if (int.TryParse(dr["MaxAge"].ToString(), out _int)) result.MaxAge = _int;

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

        public bool Fetch(int ID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", ID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SchoolCrosswalk result = new SchoolCrosswalk();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["ID"].ToString(), out _int)) this.ID = _int;
                if (int.TryParse(dr["SchoolID"].ToString(), out _int)) this.SchoolID = _int;
                if (int.TryParse(dr["SchTypeID"].ToString(), out _int)) this.SchTypeID = _int;
                if (int.TryParse(dr["DistrictID"].ToString(), out _int)) this.DistrictID = _int;
                this.City = dr["City"].ToString();
                if (int.TryParse(dr["MinGrade"].ToString(), out _int)) this.MinGrade = _int;
                if (int.TryParse(dr["MaxGrade"].ToString(), out _int)) this.MaxGrade = _int;
                if (int.TryParse(dr["MinAge"].ToString(), out _int)) this.MinAge = _int;
                if (int.TryParse(dr["MaxAge"].ToString(), out _int)) this.MaxAge = _int;

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

        public static int Insert(SchoolCrosswalk o)
        {

            SqlParameter[] arrParams = new SqlParameter[19];

            arrParams[0] = new SqlParameter("@SchoolID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolID, o.SchoolID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@SchTypeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchTypeID, o.SchTypeID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@DistrictID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DistrictID, o.DistrictID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@City", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode()));
            arrParams[4] = new SqlParameter("@MinGrade", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MinGrade, o.MinGrade.GetTypeCode()));
            arrParams[5] = new SqlParameter("@MaxGrade", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxGrade, o.MaxGrade.GetTypeCode()));
            arrParams[6] = new SqlParameter("@MinAge", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MinAge, o.MinAge.GetTypeCode()));
            arrParams[7] = new SqlParameter("@MaxAge", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MaxAge, o.MaxAge.GetTypeCode()));

            arrParams[8] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[18] = new SqlParameter("@ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ID, o.ID.GetTypeCode()));
            arrParams[18].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_Insert", arrParams);

            o.ID = int.Parse(arrParams[18].Value.ToString());

            return o.ID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SchoolCrosswalk o)
        {

            int iReturn = -1; //assume the worst

            var arrParams = new List<SqlParameter>();

            arrParams.Add(new SqlParameter("@ID", GlobalUtilities.DBSafeValue(o.ID, o.ID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchoolID", GlobalUtilities.DBSafeValue(o.SchoolID, o.SchoolID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@SchTypeID", GlobalUtilities.DBSafeValue(o.SchTypeID, o.SchTypeID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@DistrictID", GlobalUtilities.DBSafeValue(o.DistrictID, o.DistrictID.GetTypeCode())));
            arrParams.Add(new SqlParameter("@City", GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MinGrade", GlobalUtilities.DBSafeValue(o.MinGrade, o.MinGrade.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MaxGrade", GlobalUtilities.DBSafeValue(o.MaxGrade, o.MaxGrade.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MinAge", GlobalUtilities.DBSafeValue(o.MinAge, o.MinAge.GetTypeCode())));
            arrParams.Add(new SqlParameter("@MaxAge", GlobalUtilities.DBSafeValue(o.MaxAge, o.MaxAge.GetTypeCode())));
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

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_Update", arrParams.ToArray());

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

        public static int Delete(SchoolCrosswalk o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ID, o.ID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SchoolCrosswalk_Delete", arrParams);

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

