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

namespace SRP_DAL
{
[Serializable]    public class SRPGroup : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        #region Entity Columns  
            private int _gid;
            private string _groupName;
            private string _groupDescription;
            private DateTime? _lastModDate;
            private bool _lastModDateNull = true;
            private string _lastModUser;
            private DateTime? _addedDate;
            private bool _addedDateNull = true;
            private string _addedUser;

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

        #region Constructors

		    public SRPGroup()
		    {
                _gid = 0;  // meaning it is not from database

		    }


            public SRPGroup(int gid)
		    {
                List<SRPPermission> perms = new List<SRPPermission>();

                var arrParams = new SqlParameter[1];
                arrParams[0] = new SqlParameter("@GID", gid);
                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPGroups_Get", arrParams);
                if (reader.Read())
                {
                    GID = (int)reader["GID"];
                    GroupName = reader["GroupName"].ToString(); ;
                    GroupDescription = reader["GroupDescription"].ToString();
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

		#endregion	

        #region Entity Column Properties
            /// <summary>
            /// Gets or sets the <c>GID</c> column value.
            /// </summary>
            /// <value>The <c>GID</c> column value.</value>
            public int GID
            {
                get { return _gid; }
                set { _gid = value; }
            }

            /// <summary>
            /// Gets or sets the <c>GroupName</c> column value.
            /// This column is nullable.
            /// </summary>
            /// <value>The <c>GroupName</c> column value.</value>
            public string GroupName
            {
                get { return _groupName; }
                set { _groupName = value; }
            }

            /// <summary>
            /// Gets or sets the <c>GroupDescription</c> column value.
            /// This column is nullable.
            /// </summary>
            /// <value>The <c>GroupDescription</c> column value.</value>
            public string GroupDescription
            {
                get { return _groupDescription; }
                set { _groupDescription = value; }
            }

            /// <summary>
            /// Gets or sets the <c>LastModDate</c> column value.
            /// This column is nullable.
            /// </summary>
            /// <value>The <c>LastModDate</c> column value.</value>
            public DateTime? LastModDate
            {
                get
                {
                    return _lastModDate;
                }
                set
                {
                    _lastModDateNull = false;
                    _lastModDate = value;
                }
            }

            /// <summary>
            /// Indicates whether the <see cref="LastModDate"/>
            /// property value is null.
            /// </summary>
            /// <value>true if the property value is null, otherwise false.</value>
            public bool IsLastModDateNull
            {
                get { return _lastModDateNull; }
                set { _lastModDateNull = value; }
            }

            /// <summary>
            /// Gets or sets the <c>LastModUser</c> column value.
            /// This column is nullable.
            /// </summary>
            /// <value>The <c>LastModUser</c> column value.</value>
            public string LastModUser
            {
                get { return _lastModUser; }
                set { _lastModUser = value; }
            }

            /// <summary>
            /// Gets or sets the <c>AddedDate</c> column value.
            /// This column is nullable.
            /// </summary>
            /// <value>The <c>AddedDate</c> column value.</value>
            public DateTime? AddedDate
            {
                get
                {
                     return _addedDate;
                }
                set
                {
                    _addedDateNull = false;
                    _addedDate = value;
                }
            }

            /// <summary>
            /// Indicates whether the <see cref="AddedDate"/>
            /// property value is null.
            /// </summary>
            /// <value>true if the property value is null, otherwise false.</value>
            public bool IsAddedDateNull
            {
                get { return _addedDateNull; }
                set { _addedDateNull = value; }
            }

            /// <summary>
            /// Gets or sets the <c>AddedUser</c> column value.
            /// This column is nullable.
            /// </summary>
            /// <value>The <c>AddedUser</c> column value.</value>
            public string AddedUser
            {
                get { return _addedUser; }
                set { _addedUser = value; }
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

        #region Entity Methods
            /// <summary>
            /// Returns the string representation of this instance.
            /// </summary>
            /// <returns>The string representation of this instance.</returns>
            public override string ToString()
            {
                var dynStr = new System.Text.StringBuilder(GetType().Name);
                dynStr.Append(':');
                dynStr.Append("  GID=");
                dynStr.Append(GID);
                dynStr.Append("  GroupName=");
                dynStr.Append(GroupName);
                dynStr.Append("  GroupDescription=");
                dynStr.Append(GroupDescription);
                dynStr.Append("  LastModDate=");
                dynStr.Append(IsLastModDateNull ? (object)"<NULL>" : LastModDate);
                dynStr.Append("  LastModUser=");
                dynStr.Append(LastModUser);
                dynStr.Append("  AddedDate=");
                dynStr.Append(IsAddedDateNull ? (object)"<NULL>" : AddedDate);
                dynStr.Append("  AddedUser=");
                dynStr.Append(AddedUser);
                return dynStr.ToString();
            }



            

            public bool Delete()
            {
                return Delete(GID);
            }

            public static bool Delete(int? u)
            {
                if (u == null) return false;

                var arrParams = new SqlParameter[2];
                arrParams[0] = new SqlParameter("@GID", u);
                arrParams[1] = new SqlParameter("@TenID",
                                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                                        -1 :
                                                        (int)HttpContext.Current.Session["TenantID"])
                                            );

                SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "cbspSRPGroups_DeleteByPrimaryKey", arrParams);
                return true;
            }


            public static DataTable GetUserList(int GID)
            {
                var reader = SqlHelper.ExecuteReader(conn, CommandType.Text,
                                                     String.Format("exec dbo.cbspSRPUserGroups_GetUsersFlagged {0}", GID));
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }

            public static DataTable GetPermissionList(int GID)
            {
                var reader = SqlHelper.ExecuteReader(conn, CommandType.Text,
                                                     String.Format(
                                                         "exec dbo.cbspSRPUserGroups_GetPermissionsFlagged {0}", GID));
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }

            public static bool UpdateMemberUsers(int pGID, string uidList, string actionUsername)
            {

                var arrParams = new SqlParameter[4];
                arrParams[0] = new SqlParameter("@GID", pGID);
                arrParams[1] = new SqlParameter("@UID_LIST", uidList);
                arrParams[2] = new SqlParameter("@ActionUsername", actionUsername);
                arrParams[3] = new SqlParameter("@Return_Value", -1);
                arrParams[3].Direction = ParameterDirection.ReturnValue;

                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUserGroups_UpdateUsers", arrParams);
                return ((int)arrParams[3].Value != 0);
            }

            public static bool UpdatePermissions(int pGID, string permissionIdList, string actionUsername)
            {

                var arrParams = new SqlParameter[4];
                arrParams[0] = new SqlParameter("@GID", pGID);
                arrParams[1] = new SqlParameter("@PermissionID_LIST", permissionIdList);
                arrParams[2] = new SqlParameter("@ActionUsername", actionUsername);
                arrParams[3] = new SqlParameter("@Return_Value", -1);
                arrParams[3].Direction = ParameterDirection.ReturnValue;

                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "cbspSRPUserGroups_UpdatePermissions", arrParams);
                return ((int)arrParams[3].Value != 0);
            }


            public static SRPGroup Fetch(int gid)
            {
                var arrParams = new SqlParameter[1];
                arrParams[0] = new SqlParameter("@GID", gid);
                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPGroups_Get", arrParams);
                SRPGroup u = GetFromReader(reader);
                return u;
            }

            public static SRPGroup GetFromReader(SqlDataReader reader)
            {
                SRPGroup returnVal = null;
                if (reader.Read())
                {
                    returnVal = new SRPGroup();
                    returnVal.GID = (int)reader["GID"];
                    returnVal.GroupName = reader["GroupName"].ToString(); ;
                    returnVal.GroupDescription = reader["GroupDescription"].ToString();
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

                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPGroups_GetAll", arrParams);

                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;

            }

            public static DataTable FetchAllAsDataTable(int TenID)
            {
                var arrParams = new SqlParameter[1];
                arrParams[0] = new SqlParameter("@TenID", TenID);

                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPGroups_GetAll", arrParams);

                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;

            }

            public static List<SRPGroup> FetchAll(bool forCurrentTenantOnly = true)
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

                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "cbspSRPGroups_GetAll", arrParams);
                List<SRPGroup> retValue = new List<SRPGroup>();

                while (reader.Read())
                {
                    SRPGroup aUser = new SRPGroup();
                    aUser.GID = (int)reader["GID"];
                    aUser.GroupName = reader["GroupName"].ToString(); ;
                    aUser.GroupDescription = reader["GroupDescription"].ToString();
                    aUser.LastModDate = reader.IsDBNull(reader.GetOrdinal("LastModDate")) ? null : (DateTime?)reader["LastModDate"];
                    aUser.AddedDate = reader.IsDBNull(reader.GetOrdinal("AddedDate")) ? null : (DateTime?)reader["AddedDate"];
                    aUser.LastModUser = reader["LastModUser"].ToString();
                    aUser.AddedUser = reader["AddedUser"].ToString();

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


            public bool Insert()
            {
                GID = Insert(this);
                return GID > 0;
            }

            public static int Insert(SRPGroup u)
            {
                if (u == null) return -1;
                var arrParams = new SqlParameter[14];
                arrParams[0] = new SqlParameter("@GroupName", u.GroupName);
                arrParams[1] = new SqlParameter("@GroupDescription", u.GroupDescription);
                arrParams[2] = new SqlParameter("@ActionUsername", ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);

                arrParams[3] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.TenID, u.TenID.GetTypeCode()));
                arrParams[4] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt1, u.FldInt1.GetTypeCode()));
                arrParams[5] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt2, u.FldInt2.GetTypeCode()));
                arrParams[6] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt3, u.FldInt3.GetTypeCode()));
                arrParams[7] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit1, u.FldBit1.GetTypeCode()));
                arrParams[8] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit2, u.FldBit2.GetTypeCode()));
                arrParams[9] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit3, u.FldBit3.GetTypeCode()));
                arrParams[10] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText1, u.FldText1.GetTypeCode()));
                arrParams[11] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText2, u.FldText2.GetTypeCode()));
                arrParams[12] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText3, u.FldText3.GetTypeCode()));

                arrParams[13] = new SqlParameter("@Return_Value", -1);
                arrParams[13].Direction = ParameterDirection.ReturnValue;
                u.GID = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "cbspSRPGroups_Insert", arrParams));
                return (int)u.GID;
            }


            public bool Update()
            {
                return Update(this);
            }

            public static bool Update(SRPGroup u)
            {
                if (u == null)
                {
                    u.AddErrorCode("Group object", "N/A", "Group object is null.", BusinessRulesValidationCode.UNSPECIFIED);
                    return false;
                }

                var arrParams = new SqlParameter[14];
                arrParams[0] = new SqlParameter("@GID", u.GID);
                arrParams[1] = new SqlParameter("@GroupName", u.GroupName);
                arrParams[2] = new SqlParameter("@GroupDescription", u.GroupDescription);
                arrParams[3] = new SqlParameter("@ActionUsername", ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);

                arrParams[4] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.TenID, u.TenID.GetTypeCode()));
                arrParams[5] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt1, u.FldInt1.GetTypeCode()));
                arrParams[6] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt2, u.FldInt2.GetTypeCode()));
                arrParams[7] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldInt3, u.FldInt3.GetTypeCode()));
                arrParams[8] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit1, u.FldBit1.GetTypeCode()));
                arrParams[9] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit2, u.FldBit2.GetTypeCode()));
                arrParams[10] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldBit3, u.FldBit3.GetTypeCode()));
                arrParams[11] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText1, u.FldText1.GetTypeCode()));
                arrParams[12] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText2, u.FldText2.GetTypeCode()));
                arrParams[13] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(u.FldText3, u.FldText3.GetTypeCode()));

                SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "cbspSRPGroups_Update", arrParams);
                return true;

            }



        #endregion

    }
}
