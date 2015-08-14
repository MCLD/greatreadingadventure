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

[Serializable]    
    public class SRPSettings : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int mySID;
        private string myName;
        private string myValue;
        private string myStorageType;
        private string myEditType;
        private int myModID;
        private string myLabel;
        private string myDescription;
        private string myValueList;
        private string myDefaultValue;

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

        public int SID
        {
            get { return mySID; }
            set { mySID = value; }
        }
        public string Name
        {
            get { return myName; }
            set { myName = value; }
        }
        public string Value
        {
            get { return myValue; }
            set { myValue = value; }
        }
        public string StorageType
        {
            get { return myStorageType; }
            set { myStorageType = value; }
        }
        public string EditType
        {
            get { return myEditType; }
            set { myEditType = value; }
        }
        public int ModID
        {
            get { return myModID; }
            set { myModID = value; }
        }
        public string Label
        {
            get { return myLabel; }
            set { myLabel = value; }
        }
        public string Description
        {
            get { return myDescription; }
            set { myDescription = value; }
        }
        public string ValueList
        {
            get { return myValueList; }
            set { myValueList = value; }
        }
        public string DefaultValue
        {
            get { return myDefaultValue; }
            set { myDefaultValue = value; }
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

        public SRPSettings()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(bool forCurrentTenantOnly = true)
        {
            var arrParams = new SqlParameter[1];
            if (forCurrentTenantOnly)
            {
                arrParams[0] = new SqlParameter("@TenID",
                    (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                            -1 :
                            (int)HttpContext.Current.Session["TenantID"])
                );
            }
            else
            {
                arrParams[0] = new SqlParameter("@TenID", DBNull.Value);
            }

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SRPSettings_GetAll",arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",TenID);
            
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SRPSettings_GetAll", arrParams);
        }

        public static SRPSettings FetchObject(int SID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SID", SID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SRPSettings_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SRPSettings result = new SRPSettings();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                result.Name = dr["Name"].ToString();
                result.Value = dr["Value"].ToString();
                result.StorageType = dr["StorageType"].ToString();
                result.EditType = dr["EditType"].ToString();
                if (int.TryParse(dr["ModID"].ToString(), out _int)) result.ModID = _int;
                result.Label = dr["Label"].ToString();
                result.Description = dr["Description"].ToString();
                result.ValueList = dr["ValueList"].ToString();
                result.DefaultValue = dr["DefaultValue"].ToString();

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

        public SRPSettings GetSRPSettings(int SID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@SID", SID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SRPSettings_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SRPSettings result = new SRPSettings();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                result.Name = dr["Name"].ToString();
                result.Value = dr["Value"].ToString();
                result.StorageType = dr["StorageType"].ToString();
                result.EditType = dr["EditType"].ToString();
                if (int.TryParse(dr["ModID"].ToString(), out _int)) result.ModID = _int;
                result.Label = dr["Label"].ToString();
                result.Description = dr["Description"].ToString();
                result.ValueList = dr["ValueList"].ToString();
                result.DefaultValue = dr["DefaultValue"].ToString();

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


        public static string GetSettingValue(string Name)
        {
            var s = GetSRPSettingsByName(Name);
            return (s == null ? "" : s.Value);
        }

        public static string GetSettingValue(string Name, int TenID)
        {
            var s = GetSRPSettingsByName(Name, TenID);
            return (s == null ? "" : s.Value);
        }

        public static string GetSettingType(string Name)
        {
            var s = GetSRPSettingsByName(Name);
            return (s == null ? "" : s.StorageType);
        }

        public static string GetSettingType(string Name, int TenID)
        {
            var s = GetSRPSettingsByName(Name, TenID);
            return (s == null ? "" : s.StorageType);
        }

        public static SRPSettings GetSRPSettingsByName(string Name)
        {

            var TenID = HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                        -1 :
                        (int)HttpContext.Current.Session["TenantID"];
            return GetSRPSettingsByName(Name, TenID);

        }

        public static SRPSettings GetSRPSettingsByName(string Name, int TenID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@Name", Name);
            arrParams[1] = new SqlParameter("@TenID", TenID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SRPSettings_GetByName", arrParams);

            if (dr.Read())
            {

                // declare return value

                SRPSettings result = new SRPSettings();

                //DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["SID"].ToString(), out _int)) result.SID = _int;
                result.Name = dr["Name"].ToString();
                result.Value = dr["Value"].ToString();
                result.StorageType = dr["StorageType"].ToString();
                result.EditType = dr["EditType"].ToString();
                if (int.TryParse(dr["ModID"].ToString(), out _int)) result.ModID = _int;
                result.Label = dr["Label"].ToString();
                result.Description = dr["Description"].ToString();
                result.ValueList = dr["ValueList"].ToString();
                result.DefaultValue = dr["DefaultValue"].ToString();

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


        public int Insert()
        {

            return Insert(this);

        }

        public static int Insert(SRPSettings o)
        {

            SqlParameter[] arrParams = new SqlParameter[20];

            arrParams[0] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Value", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Value, o.Value.GetTypeCode()));
            arrParams[2] = new SqlParameter("@StorageType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StorageType, o.StorageType.GetTypeCode()));
            arrParams[3] = new SqlParameter("@EditType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EditType, o.EditType.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ModID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ModID, o.ModID.GetTypeCode()));
            arrParams[5] = new SqlParameter("@Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label, o.Label.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[7] = new SqlParameter("@ValueList", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ValueList, o.ValueList.GetTypeCode()));
            arrParams[8] = new SqlParameter("@DefaultValue", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DefaultValue, o.DefaultValue.GetTypeCode()));
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

            arrParams[19] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[19].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SRPSettings_Insert", arrParams);

            return int.Parse(arrParams[19].Value.ToString());

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SRPSettings o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[20];

            arrParams[0] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Value", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Value, o.Value.GetTypeCode()));
            arrParams[3] = new SqlParameter("@StorageType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StorageType, o.StorageType.GetTypeCode()));
            arrParams[4] = new SqlParameter("@EditType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EditType, o.EditType.GetTypeCode()));
            arrParams[5] = new SqlParameter("@ModID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ModID, o.ModID.GetTypeCode()));
            arrParams[6] = new SqlParameter("@Label", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Label, o.Label.GetTypeCode()));
            arrParams[7] = new SqlParameter("@Description", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Description, o.Description.GetTypeCode()));
            arrParams[8] = new SqlParameter("@ValueList", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ValueList, o.ValueList.GetTypeCode()));
            arrParams[9] = new SqlParameter("@DefaultValue", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DefaultValue, o.DefaultValue.GetTypeCode()));
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

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SRPSettings_Update", arrParams);

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

        public static int Delete(SRPSettings o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@SID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SID, o.SID.GetTypeCode()));

            arrParams[1] = new SqlParameter("@TenID",
                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                        -1 :
                        (int)HttpContext.Current.Session["TenantID"])
            );

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SRPSettings_Delete", arrParams);

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

