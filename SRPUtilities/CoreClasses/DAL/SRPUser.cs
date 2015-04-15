using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Data;
using System.IO;
using Microsoft.ApplicationBlocks.Data;
using STG.SRP.Core.Utilities;
using STG.SRP.Utilities;


namespace STG.SRP.Core.Utilities
{
    [Serializable]
    public class SRPUser: EntityBase
    {
        public override string Version { get { return "2.0"; } }

        private static string conn = STG.SRP.Core.Utilities.GlobalUtilities.SRPDB;
        //ConfigurationManager.ConnectionStrings["SRPConn"].ToString();

        #region Fields (18)

        private DateTime? _addedDate;
        private string _addedUser;
        private DateTime? _deletedDate;
        private string _department;
        private string _division;
        private string _emailAddress;
        private string _firstName;
        private bool _isActive = true;
        private bool _isDeleted = false;
        private DateTime? _lastModDate;
        private string _lastModUser;
        private string _lastName;
        private DateTime? _lastPasswordReset;
        private bool _mustResetPassword = false;
        private string _password;
        private string _title;
        private int? _uid;

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

        #endregion Fields

        #region Constructors (2)
        public SRPUser(int pUid)
        {
            List<SRPPermission> perms = new List<SRPPermission>();

            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@UID", pUid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_Get", arrParams);
            if (reader.Read())
            {
                Uid = (int)reader["UID"];
                Username = reader["Username"].ToString(); ;
                Password = reader["Password"].ToString();
                FirstName = reader["FirstName"].ToString();
                LastName = reader["LastName"].ToString();
                EmailAddress = reader["EmailAddress"].ToString();
                Division = reader["Division"].ToString();
                Department = reader["Department"].ToString();
                Title = reader["Title"].ToString();
                IsActive = (bool)reader["IsActive"];
                MustResetPassword = (bool)reader["MustResetPassword"];
                IsDeleted = (bool)reader["IsDeleted"];
                LastPasswordReset = reader.IsDBNull(reader.GetOrdinal("LastPasswordReset")) ? null : (DateTime?)reader["LastPasswordReset"];
                DeletedDate = reader.IsDBNull(reader.GetOrdinal("DeletedDate")) ? null : (DateTime?)reader["DeletedDate"];
                LastModDate = reader.IsDBNull(reader.GetOrdinal("LastModDate")) ? null : (DateTime?)reader["LastModDate"];
                AddedDate = reader.IsDBNull(reader.GetOrdinal("AddedDate")) ? null : (DateTime?)reader["AddedDate"];
                LastModUser = reader["LastModUser"].ToString();
                AddedUser = reader["AddedUser"].ToString();

                TenID = (int)reader["TenID"];
                FldInt1 = (int)reader["FldInt1"];
                FldInt2 = (int)reader["FldInt2"];
                FldInt3 = (int)reader["FldInt3"];
                FldBit1 = (bool)reader["FldBit1"];
                FldBit2 = (bool)reader["FldBit2"];
                FldBit3 = (bool)reader["FldBit3"];
                FldText1 = reader["FldText1"].ToString();
                FldText2 = reader["FldText2"].ToString();
                FldText3 = reader["FldText3"].ToString();
            }

        }

        public SRPUser()
        {
            _uid = null;  // meaning it is not from database
        }

        #endregion Constructors

        #region Properties (18)

        public DateTime? AddedDate
        {
            get { return _addedDate; }
            set { _addedDate = value; }
        }

        public string AddedUser
        {
            get { return _addedUser; }
            set { _addedUser = value; }
        }

        public DateTime? DeletedDate
        {
            get { return _deletedDate; }
            set { _deletedDate = value; }
        }

        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }

        public string Division
        {
            get { return _division; }
            set { _division = value; }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; }
        }

        public DateTime? LastModDate
        {
            get { return _lastModDate; }
            set { _lastModDate = value; }
        }

        public string LastModUser
        {
            get { return _lastModUser; }
            set { _lastModUser = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public DateTime? LastPasswordReset
        {
            get { return _lastPasswordReset; }
            set { _lastPasswordReset = value; }
        }

        public bool MustResetPassword
        {
            get { return _mustResetPassword; }
            set { _mustResetPassword = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public int? Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public string Username { get; set; }

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
        #endregion Properties

        #region Methods (27)


        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            ClearErrorCodes();
            if (validationMode == BusinessRulesValidationMode.INSERT)
            {
                SRPUser obj = FetchByUsername(Username);
                if (obj != null)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Username", "Username", "Username already exists.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }
            }
            return (ErrorCodes.Count == 0);
            //return true;
        }

        public bool Delete()
        {
            return Delete(Uid);
        }

        public static bool Delete(int? u)
        {
            if (u == null) return false;

            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@UID", u);
            arrParams[1] = new SqlParameter("@ActionUsername", ((SRPUser) HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);
            SqlHelper.ExecuteScalar(conn, CommandType.Text, "UPDATE dbo.SRPUser SET isDeleted = 1, DeletedDate = getdate(), LastModDate = getdate(), LastModUser = @ActionUsername WHERE UID = @UID", arrParams);
            return true;
        }

        //public List<SRPFolder> EffectiveUserFolders()
        //{
        //    return EffectiveUserFolders(_uid);
        //}

        //public static List<SRPFolder> EffectiveUserFolders(int? pUid)
        //{
        //    if (pUid == null) return (new List<SRPFolder>());

        //    List<SRPFolder> folders = new List<SRPFolder>();
        //    using (DAC dac = GenericSingleton<DAC>.GetInstance())
        //    {
        //        List<IDbDataParameter> parm = new List<IDbDataParameter> { new SqlParameter("@UID", pUid) };
        //        SqlDataReader reader = (SqlDataReader)dac.ExecuteReader("dbo.cbspSRPUser_GetAllFolders", parm);

        //        while (reader.Read())
        //        {
        //            SRPFolder folder = new SRPFolder();
        //            folder.Folder = reader["Folder"].ToString();
        //            folders.Add(folder);
        //        }
        //        reader.Close();
        //        dac.Dispose();
        //        return folders;
        //    }
        //}

        public List<SRPPermission> EffectiveUserPermissions()
        {
            return EffectiveUserPermissions(_uid);
        }

        public static List<SRPPermission> EffectiveUserPermissions(int? pUid)
        {
            if (pUid == null) return (new List<SRPPermission>());

            List<SRPPermission> perms = new List<SRPPermission>();

            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@UID", pUid);

            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetAllPermissions", arrParams);

            while (reader.Read())
            {
                SRPPermission perm = new SRPPermission();
                perm.Permission = (int)reader["PermissionID"];
                perm.Name = reader["PermissionName"].ToString();
                perm.Description = reader["PermissionDesc"].ToString();
                perms.Add(perm);
            }
            reader.Close();
            return perms;
        }

        public static bool EmailExists(string pEmail)
        {
            if (pEmail.Length == 0) return false;

            var arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@EmailAddress", pEmail);
            arrParams[1] = new SqlParameter("@Return_Value", -1);
            arrParams[1].Direction = ParameterDirection.ReturnValue;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUser_EmailExists", arrParams);
            return ((int)arrParams[1].Value == 1);
        }

        public static SRPUser Fetch(int pUid)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@UID", pUid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_Get", arrParams);
            SRPUser u = GetFromReader(reader);
            return u;            
        }

        public static SRPUser GetFromReader(SqlDataReader reader)
        {
            SRPUser returnVal = null;
            if (reader.Read())
            {
                returnVal = new SRPUser();
                returnVal.Uid = (int)reader["UID"];
                returnVal.Username = reader["Username"].ToString(); ;
                returnVal.Password = reader["Password"].ToString();
                returnVal.FirstName = reader["FirstName"].ToString();
                returnVal.LastName = reader["LastName"].ToString();
                returnVal.EmailAddress = reader["EmailAddress"].ToString();
                returnVal.Division = reader["Division"].ToString();
                returnVal.Department = reader["Department"].ToString();
                returnVal.Title = reader["Title"].ToString();
                returnVal.IsActive = (bool)reader["IsActive"];
                returnVal.MustResetPassword = (bool)reader["MustResetPassword"];
                returnVal.IsDeleted = (bool)reader["IsDeleted"];
                returnVal.LastPasswordReset = reader.IsDBNull(reader.GetOrdinal("LastPasswordReset")) ? null : (DateTime?)reader["LastPasswordReset"];
                returnVal.DeletedDate = reader.IsDBNull(reader.GetOrdinal("DeletedDate")) ? null : (DateTime?)reader["DeletedDate"];
                returnVal.LastModDate = reader.IsDBNull(reader.GetOrdinal("LastModDate")) ? null : (DateTime?)reader["LastModDate"];
                returnVal.AddedDate = reader.IsDBNull(reader.GetOrdinal("AddedDate")) ? null : (DateTime?)reader["AddedDate"];
                returnVal.LastModUser = reader["LastModUser"].ToString();
                returnVal.AddedUser = reader["AddedUser"].ToString();

                returnVal.TenID = (int)reader["TenID"];
                returnVal.FldInt1 = (int)reader["FldInt1"];
                returnVal.FldInt2 = (int)reader["FldInt2"];
                returnVal.FldInt3 = (int)reader["FldInt3"];
                returnVal.FldBit1 = (bool)reader["FldBit1"];
                returnVal.FldBit2 = (bool)reader["FldBit2"];
                returnVal.FldBit3 = (bool)reader["FldBit3"];
                returnVal.FldText1 = reader["FldText1"].ToString();
                returnVal.FldText2 = reader["FldText2"].ToString();
                returnVal.FldText3 = reader["FldText3"].ToString();

            }
            reader.Close();
            return returnVal;
        }


        public static DataTable FetchAllAsDataTable(bool forCurrentTenantOnly = true)
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

            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetAll",arrParams);

            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;

        }


        public static DataTable FetchAllAsDataTable(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetAll", arrParams);

            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;

        }

        public static List<SRPUser> FetchAll(bool forCurrentTenantOnly = true)
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

            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetAll", arrParams);
            List<SRPUser> retValue = new List<SRPUser>();

            while (reader.Read())
            {
                SRPUser aUser = new SRPUser();
                aUser.Uid = (int)reader["UID"];
                aUser.Username = (string)reader["Username"];
                aUser.Password = (string)reader["Password"];
                aUser.FirstName = (string)reader["FirstName"];
                aUser.LastName = (string)reader["LastName"];
                aUser.EmailAddress = (string)reader["EmailAddress"];
                aUser.Division = (string)reader["Division"];
                aUser.Department = (string)reader["Department"];
                aUser.Title = (string)reader["Title"];
                aUser.IsActive = (bool)reader["IsActive"];
                aUser.MustResetPassword = (bool)reader["MustResetPassword"];
                aUser.IsDeleted = (bool)reader["IsDeleted"];
                aUser.LastPasswordReset = reader.IsDBNull(reader.GetOrdinal("LastPasswordReset"))
                                              ? null
                                              : (DateTime?)reader["LastPasswordReset"];
                aUser.DeletedDate = reader.IsDBNull(reader.GetOrdinal("DeletedDate"))
                                        ? null
                                        : (DateTime?)reader["DeletedDate"];
                aUser.LastModDate = reader.IsDBNull(reader.GetOrdinal("LastModDate"))
                                        ? null
                                        : (DateTime?)reader["LastModDate"];
                aUser.AddedDate = reader.IsDBNull(reader.GetOrdinal("AddedDate"))
                                      ? null
                                      : (DateTime?)reader["AddedDate"];
                aUser.LastModUser = (string)reader["LastModUser"];
                aUser.AddedUser = (string)reader["AddedUser"];

                aUser.TenID = (int)reader["TenID"];
                aUser.FldInt1 = (int)reader["FldInt1"];
                aUser.FldInt2 = (int)reader["FldInt2"];
                aUser.FldInt3 = (int)reader["FldInt3"];
                aUser.FldBit1 = (bool)reader["FldBit1"];
                aUser.FldBit2 = (bool)reader["FldBit2"];
                aUser.FldBit3 = (bool)reader["FldBit3"];
                aUser.FldText1 = reader["FldText1"].ToString();
                aUser.FldText2 = reader["FldText2"].ToString();
                aUser.FldText3 = reader["FldText3"].ToString();

                retValue.Add(aUser);
            }
            return retValue;
        }

        public static SRPUser FetchByUsername(string pUsername)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@Username", pUsername);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetByUsername", arrParams);
            SRPUser u = GetFromReader(reader);
            return u;

        }

        public bool Insert()
        {
            Uid = Insert(this);
            return Uid > 0;
        }

        public static int Insert(SRPUser u)
        {
            if (u == null) return -1;
            var arrParams = new SqlParameter[22];
            arrParams[0] = new SqlParameter("@Username", u.Username);
            arrParams[1] = new SqlParameter("@Password", u.Password);
            arrParams[2] = new SqlParameter("@FirstName", u.FirstName);
            arrParams[3] = new SqlParameter("@LastName", u.LastName);
            arrParams[4] = new SqlParameter("@EmailAddress", u.EmailAddress);
            arrParams[5] = new SqlParameter("@Division", u.Division);
            arrParams[6] = new SqlParameter("@Department", u.Department);
            arrParams[7] = new SqlParameter("@Title", u.Title);
            arrParams[8] = new SqlParameter("@IsActive", u.IsActive);
            arrParams[9] = new SqlParameter("@MustResetPassword", u.MustResetPassword);
            arrParams[10] = new SqlParameter("@ActionUsername", ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);

            arrParams[11] = new SqlParameter("@TenID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.TenID, u.TenID.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt1, u.FldInt1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldInt2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt2, u.FldInt2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt3, u.FldInt3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit1, u.FldBit1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldBit2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit2, u.FldBit2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit3, u.FldBit3.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText1, u.FldText1.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldText2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText2, u.FldText2.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText3, u.FldText3.GetTypeCode()));


            arrParams[21] = new SqlParameter("@Return_Value", -1);
            arrParams[21].Direction = ParameterDirection.ReturnValue;
            u.Uid = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_Insert", arrParams));
            return (int)u.Uid;
        }

        public static bool Login(string logon, string password, string session, string ip, string machine, string browser)
        {
            var arrParams = new SqlParameter[6];
            arrParams[0] = new SqlParameter("@UserName", logon);
            arrParams[1] = new SqlParameter("@Password", password);
            arrParams[2] = new SqlParameter("@SessionId", session);
            arrParams[3] = new SqlParameter("@MachineName", machine);
            arrParams[4] = new SqlParameter("@Browser", browser);
            arrParams[5] = new SqlParameter("@IP", ip);

            int result = (int)SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_Login", arrParams);
            return (result == 1);
        }

        public bool Logoff()
        {
            return Logoff(_uid);

        }

        public static bool Logoff(int? uid)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@UID", uid);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUser_Logout", arrParams);
            try
            {
                HttpContext.Current.Session.Remove(SessionData.IsLoggedIn.ToString());
                HttpContext.Current.Session.Remove(SessionData.UserProfile.ToString());
                HttpContext.Current.Session.Remove(SessionData.StringPermissionList.ToString());
                //HttpContext.Current.Session.RemoveAll();
            }catch{}
            return true;
        }

        public static void LogoffAll()
        {
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUser_LogoutAll");
        }

        public bool Update()
        {
            return Update(this);
        }

        public static bool Update(SRPUser u)
        {
            if (u == null)
            {
                u.AddErrorCode("User object", "N/A", "User object is null.", BusinessRulesValidationCode.UNSPECIFIED);
                return false;
            }
            var arrParams = new SqlParameter[23];
            arrParams[0] = new SqlParameter("@UID", u.Uid);
            arrParams[1] = new SqlParameter("@Username", u.Username);
            arrParams[2] = new SqlParameter("@Password", u.Password);
            arrParams[3] = new SqlParameter("@FirstName", u.FirstName);
            arrParams[4] = new SqlParameter("@LastName", u.LastName);
            arrParams[5] = new SqlParameter("@EmailAddress", u.EmailAddress);
            arrParams[6] = new SqlParameter("@Division", u.Division);
            arrParams[7] = new SqlParameter("@Department", u.Department);
            arrParams[8] = new SqlParameter("@Title", u.Title);
            arrParams[9] = new SqlParameter("@IsActive", u.IsActive);
            arrParams[10] = new SqlParameter("@MustResetPassword", u.MustResetPassword);
            arrParams[11] = new SqlParameter("@LastPasswordReset", u.LastPasswordReset);
            arrParams[12] = new SqlParameter("@ActionUsername", ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);

            arrParams[13] = new SqlParameter("@TenID", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.TenID, u.TenID.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldInt1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt1, u.FldInt1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldInt2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt2, u.FldInt2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldInt3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt3, u.FldInt3.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldBit1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit1, u.FldBit1.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldBit2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit2, u.FldBit2.GetTypeCode()));
            arrParams[19] = new SqlParameter("@FldBit3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit3, u.FldBit3.GetTypeCode()));
            arrParams[20] = new SqlParameter("@FldText1", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText1, u.FldText1.GetTypeCode()));
            arrParams[21] = new SqlParameter("@FldText2", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText2, u.FldText2.GetTypeCode()));
            arrParams[22] = new SqlParameter("@FldText3", STG.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText3, u.FldText3.GetTypeCode()));

            SqlHelper.ExecuteNonQuery( conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_Update", arrParams);
            return true;
        }

        public static bool UsernameExists(string pUsername)
        {
            if (pUsername.Length == 0) return false;

            var parm = new SqlParameter[2];
            parm[0] = new SqlParameter("@EmailAddress", pUsername);
            parm[1] = new SqlParameter("@Return_Value", -1);
            parm[1].Direction = ParameterDirection.ReturnValue;
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_UsernameExists", parm);
            return ((int)parm[1].Value == 1);
        }


        // ////////////////////////////////////////////////////////////////////////////

        protected static DataTable CreateDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "SRPUser";
            DataColumn dataColumn;
            dataColumn = dataTable.Columns.Add("UID", typeof(int));
            dataColumn.Caption = "User Id";
            dataColumn.AllowDBNull = false;
            dataColumn.ReadOnly = true;
            dataColumn.Unique = true;
            dataColumn.AutoIncrement = true;
            dataColumn = dataTable.Columns.Add("Username", typeof(string));
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("Password", typeof(string));
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("FirstName", typeof(string));
            dataColumn.Caption = "First Name";
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("LastName", typeof(string));
            dataColumn.Caption = "Last Name";
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("EmailAddress", typeof(string));
            dataColumn.Caption = "Email Address";
            dataColumn.MaxLength = 128;
            dataColumn = dataTable.Columns.Add("Division", typeof(string));
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("Department", typeof(string));
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("Title", typeof(string));
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("IsActive", typeof(bool));
            dataColumn.Caption = "Is Active";
            dataColumn = dataTable.Columns.Add("MustResetPassword", typeof(bool));
            dataColumn.Caption = "Must Reset Password";
            dataColumn = dataTable.Columns.Add("IsDeleted", typeof(bool));
            dataColumn.Caption = "Is Deleted";
            dataColumn = dataTable.Columns.Add("LastPasswordReset", typeof(DateTime));
            dataColumn.Caption = "Last Password Reset";
            dataColumn = dataTable.Columns.Add("DeletedDate", typeof(DateTime));
            dataColumn.Caption = "Deleted Date";
            dataColumn = dataTable.Columns.Add("LastModDate", typeof(DateTime));
            dataColumn.Caption = " Modified Date";
            dataColumn = dataTable.Columns.Add("LastModUser", typeof(string));
            dataColumn.Caption = "Modified By";
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("AddedDate", typeof(DateTime));
            dataColumn.Caption = "Added Date";
            dataColumn = dataTable.Columns.Add("AddedUser", typeof(string));
            dataColumn.Caption = "Added By";
            dataColumn.MaxLength = 50;

            dataColumn = dataTable.Columns.Add("TenID", typeof(int));
            dataColumn.Caption = "Tenant Id";
            dataColumn = dataTable.Columns.Add("FldInt1", typeof(int));
            dataColumn.Caption = "FldInt1";
            dataColumn = dataTable.Columns.Add("FldInt2", typeof(int));
            dataColumn.Caption = "FldInt2";
            dataColumn = dataTable.Columns.Add("FldInt3", typeof(int));
            dataColumn.Caption = "FldInt3";
            dataColumn = dataTable.Columns.Add("FldBit1", typeof(bool));
            dataColumn.Caption = "FldBit1";
            dataColumn = dataTable.Columns.Add("FldBit2", typeof(bool));
            dataColumn.Caption = "FldBit2";
            dataColumn = dataTable.Columns.Add("FldBit3", typeof(bool));
            dataColumn.Caption = "FldBit3";
            dataColumn = dataTable.Columns.Add("FldText1", typeof(string));
            dataColumn.Caption = "FldText1";
            dataColumn = dataTable.Columns.Add("FldText2", typeof(string));
            dataColumn.Caption = "FldText2";
            dataColumn = dataTable.Columns.Add("FldText3", typeof(string));
            dataColumn.Caption = "FldText3";

            return dataTable;
        }

        protected static DataTable MapRecordsToDataTable(IDataReader reader,
                                        int startIndex, int length, ref int totalRecordCount)
        {
            if (0 > startIndex)
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "StartIndex cannot be less than zero.");
            if (0 > length)
                throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than zero.");

            int columnCount = reader.FieldCount;
            int ri = -startIndex;

            DataTable dataTable = CreateDataTable();
            dataTable.BeginLoadData();
            object[] values = new object[columnCount];

            while (reader.Read())
            {
                ri++;
                if (ri > 0 && ri <= length)
                {
                    reader.GetValues(values);
                    dataTable.LoadDataRow(values, true);

                    if (ri == length && 0 != totalRecordCount)
                        break;
                }
            }
            dataTable.EndLoadData();
            reader.Close();
            totalRecordCount = 0 == totalRecordCount ? ri + startIndex : -1;
            return dataTable;
        }

        protected static DataTable MapRecordsToDataTable(IDataReader reader)
        {
            int totalRecordCount = 0;
            return MapRecordsToDataTable(reader, 0, int.MaxValue, ref totalRecordCount);
        }

        public static DataTable GetAllAsDataTable()
        {
            SqlDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_GetAll");
            DataTable dt = MapRecordsToDataTable(reader);
            return dt;
        }


        // ////////////////////////////////////////////////////////////////////////////

        protected static DataTable CreateLoginHistoryDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "SRPUserLoginHistory";
            DataColumn dataColumn;
            dataColumn = dataTable.Columns.Add("UIDLH", typeof(int));
            //dataColumn.AllowDBNull = false;
            //dataColumn.ReadOnly = true;
            //dataColumn.Unique = true;
            //dataColumn.AutoIncrement = true;
            dataColumn = dataTable.Columns.Add("UID", typeof(string));
            dataColumn.MaxLength = 10;
            dataColumn = dataTable.Columns.Add("SessionsID", typeof(string));
            dataColumn.Caption = "Session ID";
            dataColumn.MaxLength = 128;
            dataColumn = dataTable.Columns.Add("StartDateTime", typeof(DateTime));
            dataColumn.Caption = "Session Start";
            dataColumn = dataTable.Columns.Add("IP", typeof(string));
            dataColumn.Caption = "IP Address";
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("MachineName", typeof(string));
            dataColumn.Caption = "Machine Name";
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("Browser", typeof(string));
            dataColumn.MaxLength = 50;
            dataColumn = dataTable.Columns.Add("EndDateTime", typeof(DateTime));
            dataColumn.Caption = "Session End";
            dataColumn = dataTable.Columns.Add("Username", typeof(string));
            dataColumn.Caption = "Username";
            dataColumn.MaxLength = 64;
            dataColumn = dataTable.Columns.Add("Name", typeof(string));
            dataColumn.Caption = "name";
            dataColumn.MaxLength = 100;
            dataColumn = dataTable.Columns.Add("Tenant", typeof(string));
            dataColumn.Caption = "Tenant";
            dataColumn.MaxLength = 100;
            return dataTable;
        }

        protected static DataTable MapLoginHistoryRecordsToDataTable(IDataReader reader,
                                int startIndex, int length, ref int totalRecordCount)
        {
            if (0 > startIndex)
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "StartIndex cannot be less than zero.");
            if (0 > length)
                throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than zero.");

            int columnCount = reader.FieldCount;
            int ri = -startIndex;

            DataTable dataTable = CreateLoginHistoryDataTable();
            dataTable.BeginLoadData();
            object[] values = new object[columnCount];

            while (reader.Read())
            {
                ri++;
                if (ri > 0 && ri <= length)
                {
                    reader.GetValues(values);
                    dataTable.LoadDataRow(values, true);

                    if (ri == length && 0 != totalRecordCount)
                        break;
                }
            }
            dataTable.EndLoadData();
            reader.Close();
            totalRecordCount = 0 == totalRecordCount ? ri + startIndex : -1;
            return dataTable;
        }

        protected static DataTable MapLoginHistoryRecordsToDataTable(IDataReader reader)
        {
            int totalRecordCount = 0;
            return MapLoginHistoryRecordsToDataTable(reader, 0, int.MaxValue, ref totalRecordCount);
        }

        public static DataTable GetLoginHistory(int pUid)
        {
            var parm = new SqlParameter[1];
            parm[0] = new SqlParameter("@UID", pUid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_GetLoginHistory", parm);
            DataTable dt = MapLoginHistoryRecordsToDataTable(reader);
            return dt;
        }

        public static DataTable GetLogedInNowAll()
        {
            SqlDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_GetLoginNow");
            DataTable dt = MapLoginHistoryRecordsToDataTable(reader);
            return dt;

        }

        public static DataTable GetLogedInNow(int TenID)
        {
            var parm = new SqlParameter[1];
            parm[0] = new SqlParameter("@TenID", TenID);
            SqlDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_GetLoginNowTenID", parm);
            DataTable dt = MapLoginHistoryRecordsToDataTable(reader);
            return dt;
        }

        public static DataTable GetGroupList(int uid)
        {

            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@UID", uid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetGroupsFlagged", arrParams);


            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;


        }

        public static DataTable GetUserGroupList(int uid)
        {

            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@UID", uid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPUser_GetGroups", arrParams);


            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;

        }

        public static DataTable GetPermissionList(int uid)
        {
            var parm = new SqlParameter[1];
            parm[0] = new SqlParameter("@UID", uid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_GetSpecialUserPermissionsFlagged", parm);
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public static DataTable GetUserPermissionsAuditList(int uid)
        {
            var parm = new SqlParameter[1];
            parm[0] = new SqlParameter("@UID", uid);
            var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "dbo.cbspSRPUser_GetAllPermissionsAUDIT", parm);
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        //public static DataTable GetFolderList(int uid)
        //{
        //    var dbDt = new DataTable();
        //    string foldersString = "#~#";
        //    DataTable fsDt = CreateFoldersDataTable();

        //    using (DAC dac = GenericSingleton<DAC>.GetInstance())
        //    {
        //        var parm = new List<IDbDataParameter> { new SqlParameter("@UID", uid) };
        //        var reader = (SqlDataReader)dac.ExecuteReader("dbo.[cbspSRPUser_GetSpecialUserFolders]", parm);
        //        var dt = new DataTable();
        //        dbDt.Load(reader);
        //        foreach (DataRow row in dbDt.Rows)
        //        {
        //            foldersString = string.Format("{0}#~#{1}", foldersString, row["Folder"].ToString());
        //        }
        //        foldersString = string.Format("{0}#~#", foldersString);
        //        reader.Close();
        //        dac.Dispose();
        //    }

        //    string root = HttpContext.Current.Server.MapPath("/");
        //    // make sure it starts and ends with a comma...
        //    string hideFolders = string.Format(",{0},", SRPSettings.GetSetting("HideFolders"));

        //    //AddFolderToResultSet(fsDt, GID, root, FoldersString);
        //    //Directory d = Directory.GetDirectories(root)
        //    RecurseDirectories(fsDt, uid, root, foldersString, hideFolders, root);

        //    return fsDt;

        //}

        //public static DataTable GetUserFolderAuditList(int uid)
        //{
        //    using (DAC dac = GenericSingleton<DAC>.GetInstance())
        //    {
        //        List<IDbDataParameter> parm = new List<IDbDataParameter> { new SqlParameter("@UID", uid) };
        //        SqlDataReader reader = (SqlDataReader)dac.ExecuteReader("dbo.[cbspSRPUser_GetAllFoldersAUDIT]", parm);
        //        DataTable dt = new DataTable();
        //        dt.Load(reader);
        //        dac.Dispose();
        //        return dt;

        //    }
        //}

        //public static DataTable GetUserFolderList(int uid)
        //{
        //    using (DAC dac = GenericSingleton<DAC>.GetInstance())
        //    {
        //        List<IDbDataParameter> parm = new List<IDbDataParameter> { new SqlParameter("@UID", uid) };
        //        SqlDataReader reader = (SqlDataReader)dac.ExecuteReader("dbo.[cbspSRPUser_GetAllFolders]", parm);
        //        DataTable dt = new DataTable();
        //        dt.Load(reader);
        //        dac.Dispose();
        //        return dt;

        //    }
        //}

        //private static void RecurseDirectories(DataTable fsDt, int uid, string folder, string selectedFolders, string restricted, string root)
        //{
        //    string lastFolderInPath = folder.Substring(folder.LastIndexOf("\\") + 1);
        //    bool isRestricted = restricted.ToLower().Contains(string.Format(",{0},", lastFolderInPath.ToLower()));
        //    if (!isRestricted)
        //    {
        //        string relFolder = folder.Replace(root, "\\");
        //        AddFolderToResultSet(fsDt, uid, relFolder, selectedFolders);

        //        foreach (var path in Directory.GetDirectories(folder))
        //        {
        //            RecurseDirectories(fsDt, uid, path, selectedFolders, restricted, root);
        //        }
        //    }
        //}

        //private static void AddFolderToResultSet(DataTable fsDt, int uid, string folder, string selectedFolders)
        //{
        //    object[] values = new object[3];
        //    values[0] = uid;
        //    values[1] = folder;
        //    values[2] = selectedFolders.Contains("#~#" + folder + "#~#");

        //    fsDt.LoadDataRow(values, true);
        //}

        //protected static DataTable CreateFoldersDataTable()
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.TableName = "FolderList";
        //    DataColumn dataColumn;
        //    dataColumn = dataTable.Columns.Add("UID", typeof(int));
        //    dataColumn.Caption = "User Id";
        //    dataColumn.AllowDBNull = false;
        //    dataColumn = dataTable.Columns.Add("Folder", typeof(string));
        //    dataColumn.Caption = "Folder";
        //    dataColumn.MaxLength = 255;
        //    dataColumn = dataTable.Columns.Add("IsSelected", typeof(bool));
        //    dataColumn.Caption = "Is Selected";

        //    return dataTable;
        //}

        public static bool UpdateMemberGroups(int pUID, string gidList, string actionUsername)
        {
            //using (DAC dac = GenericSingleton<DAC>.GetInstance())
            //{
            //    List<IDbDataParameter> parm = new List<IDbDataParameter>();
            //    SqlParameter p = new SqlParameter("@UID", pUID);
            //    parm.Add(p);
            //    p = new SqlParameter("@GID_LIST", gidList);
            //    parm.Add(p);
            //    p = new SqlParameter("@ActionUsername", actionUsername);
            //    parm.Add(p);
            //    dac.ExecuteNonQuery("dbo.cbspSRPUser_UpdateGroups", parm);
            //    dac.Dispose();
            //    return true;
            //}




            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@UID", pUID);
            arrParams[1] = new SqlParameter("@GID_LIST", gidList);
            arrParams[2] = new SqlParameter("@ActionUsername", actionUsername);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUser_UpdateGroups", arrParams);



            return true;
        }

        public static bool UpdatePermissions(int pUID, string permissionIDList, string actionUsername)
        {
            var parm = new SqlParameter[3];
            parm[0] = new SqlParameter("@UID", pUID);
            parm[1] = new SqlParameter("@PermissionID_LIST", permissionIDList);
            parm[2] = new SqlParameter("@ActionUsername", actionUsername);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUser_UpdateSpecialUserPermissions", parm);

            return true;
        }

        //public static bool UpdateFolders(int pUID, string folderList, string actionUsername)
        //{
        //    using (DAC dac = GenericSingleton<DAC>.GetInstance())
        //    {
        //        List<IDbDataParameter> parm = new List<IDbDataParameter>();
        //        SqlParameter p = new SqlParameter("@UID", pUID);
        //        parm.Add(p);
        //        p = new SqlParameter("@Folder_LIST", folderList);
        //        parm.Add(p);
        //        p = new SqlParameter("@ActionUsername", actionUsername);
        //        parm.Add(p);
        //        dac.ExecuteNonQuery("cbspSRPUser_UpdateSpecialUserFolders", parm);
        //        dac.Dispose();
        //        return true;
        //    }
        //}


        #endregion Methods
    }
}
