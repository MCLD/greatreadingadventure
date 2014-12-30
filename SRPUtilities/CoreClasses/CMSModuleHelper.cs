using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace SRPApp.Classes
{
    public class CMSModuleHelper
    {
        private static string conn = STG.SRP.Core.Utilities.Utilities.CMSDB;//= ConfigurationManager.ConnectionStrings["CMSConn"].ToString();



        public static DataSet SelectAllModules()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSModule");
        }

        public static DataSet SelectModuleInfo(int id)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Id", id);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSModule where UID = @Id", arrParams);
        }




        public static bool Delete(int id)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@Id", id);
            SqlHelper.ExecuteScalar(conn, CommandType.Text,
                "Delete CMSPageWidgetSetting where WidgetID in (select UID from CMSModuleWidget where ModuleID = @Id); " +
                "Delete CMSPageContent where WidgetID in (select UID from CMSModuleWidget where ModuleID = @Id); " +
                "Delete CMSGlobalModuleSetting where ModuleID = @Id; " +
                "Delete CMSTemplateWidgetSetting where TWID in (select UID from CMSModuleWidget where ModuleID = @Id); " +
                "Delete CMSTemplateWidget where WidgetID in (select UID from CMSModuleWidget where ModuleID = @Id); " +
                "Delete CMSWidgetSetting where ModuleID = @Id; " +
                "Delete SRPUserPermissions where PermissionId in (select PermissionID from SRPPermissionsMaster where MODID = @Id);" +
                "Delete SRPPermissionsMaster where MODID = @Id; " + 
                "Delete CMSModuleWidget where ModuleId = @Id; " + 
                "Delete CMSModule where UID = @Id", arrParams);
            return true;
        }


        public static DataSet SelectModuleGlobalSettings(int id)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Id", id);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSGlobalModuleSetting where ModuleID = @Id", arrParams);
        }

        public static bool UpdateGlobalValue(int id, string value)
        {
            var arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@Id", id);
            arrParams[1] = new SqlParameter("@Value", value);
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "update CMSGlobalModuleSetting set Value = @Value where UID = @Id", arrParams);
            return true;
        }

        //-----------------------------------


        public static int InstallModule(string name, string description, string imgFile1, string imgFile2, string imgFile3, string managementLocation)
        {
            var arrParams = new SqlParameter[7];
            arrParams[0] = new SqlParameter("@Id", -1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@Name", name);
            arrParams[2] = new SqlParameter("@description", description);
            arrParams[3] = new SqlParameter("@imgFile1", imgFile1);
            arrParams[4] = new SqlParameter("@imgFile2", imgFile2);
            arrParams[5] = new SqlParameter("@imgFile3", imgFile3);
            arrParams[6] = new SqlParameter("@ManagementLocation", managementLocation);


            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "insert into CMSModule (Name, description, ImgSm, ImgMd, ImgLg, ManagementLocation) values( @Name, @description, @imgFile1, @imgFile2, @imgFile3, @ManagementLocation); Select @Id = SCOPE_IDENTITY()", arrParams);

            var id = int.Parse(arrParams[0].Value.ToString());

            return id;
        }

        public static int InstallModuleSecurityPermission(int permissinID, int moduleId, string permName, string permDesc )
        {
            var arrParams = new SqlParameter[4];
            arrParams[0] = new SqlParameter("@Id", permissinID);
            arrParams[1] = new SqlParameter("@name", permName);
            arrParams[2] = new SqlParameter("@description", permDesc);
            arrParams[3] = new SqlParameter("@moduleId", moduleId);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                    "insert into SRPPermissionsMaster (PermissionID, PermissionName, PermissionDesc, MODID) values(@Id, @name, @description, @moduleId); ", 
                    arrParams);

            //var id = int.Parse(arrParams[0].Value.ToString());

            return 1;
            
        }

        public static int InstallModuleSetting(int moduleID, string name, string property, string type, string value, string editor, string listValues)
        {
            var arrParams = new SqlParameter[8];
            arrParams[0] = new SqlParameter("@Id", -1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@moduleID", moduleID);
            arrParams[2] = new SqlParameter("@name", name);
            arrParams[3] = new SqlParameter("@property", property.Replace(" ", "_"));
            arrParams[4] = new SqlParameter("@type", type);
            arrParams[5] = new SqlParameter("@value", value);
            arrParams[6] = new SqlParameter("@editor", editor);
            arrParams[7] = new SqlParameter("@listValues", listValues);


            SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                "insert into CMSGlobalModuleSetting (ModuleID, name, PropertyName, type, value, editor, listValues) values( @moduleID, @name, @property, @type, @value, @editor, @listValues); Select @Id = SCOPE_IDENTITY()", arrParams);

            var id = int.Parse(arrParams[0].Value.ToString());

            return id;
        }

        //-----------------------------------
        public static string GetGlobalSetting(string name)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@settingName", name);

            //var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSGlobalModuleSetting where Name = @settingName", arrParams);
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSGlobalModuleSetting where PropertyName = @settingName", arrParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Value"].ToString();
            }
            return "";
        }



    }
}