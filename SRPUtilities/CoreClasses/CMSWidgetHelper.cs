using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.ApplicationBlocks.Data;

namespace SRPApp.Classes
{
    public class CMSWidgetHelper
    {

        private static string conn = STG.CMS.Core.CMSUtilities.CMSUtilities.CMSDB;
            //ConfigurationManager.ConnectionStrings["CMSConn"].ToString();

        public static DataSet GetModuleWidgets(int id)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Id", id);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from  CMSModuleWidget where ModuleID = @Id", arrParams);
        }

        public static DataSet SelectWidgetParameters(int id)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Id", id);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSWidgetSetting where WidgetID = @Id", arrParams);
        }
        

        // ------------------


        public static int InstallWidget(int moduleId, string name, string description, int templateOrContent, string location, string javaScripts1, string javaScripts2, string javaScripts3, string javaScripts4)
        {
            var arrParams = new SqlParameter[10];
            arrParams[0] = new SqlParameter("@Id", -1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@moduleId", moduleId);
            arrParams[2] = new SqlParameter("@Name", name);
            arrParams[3] = new SqlParameter("@description", description);
            arrParams[4] = new SqlParameter("@templateOrContent", templateOrContent);
            arrParams[5] = new SqlParameter("@location", location);
            arrParams[6] = new SqlParameter("@javaScripts1", javaScripts1);
            arrParams[7] = new SqlParameter("@javaScripts2", javaScripts2);
            arrParams[8] = new SqlParameter("@javaScripts3", javaScripts3);
            arrParams[9] = new SqlParameter("@javaScripts4", javaScripts4);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, 
                "insert into  CMSModuleWidget (ModuleID, Name, description, templateOrContent, location, javaScripts1, javaScripts2, javaScripts3, javaScripts4) " +
                "values(@moduleId, @Name, @description, @templateOrContent, @location, @javaScripts1, @javaScripts2, @javaScripts3, @javaScripts4); " + 
                "Select @Id = SCOPE_IDENTITY()", arrParams);

            var id = int.Parse(arrParams[0].Value.ToString());

            return id;
        }

        public static int InstallWidgetParameter(int moduleId, int widgetId, string name, string property, string type, string editor, string valueList)
        {
            var arrParams = new SqlParameter[8];
            arrParams[0] = new SqlParameter("@Id", -1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@moduleId", moduleId);
            arrParams[2] = new SqlParameter("@widgetId", widgetId);
            arrParams[3] = new SqlParameter("@Name", name);
            arrParams[4] = new SqlParameter("@PropertyName", property.Replace(" ", "_"));
            arrParams[5] = new SqlParameter("@type", type);
            arrParams[6] = new SqlParameter("@editor", editor);
            arrParams[7] = new SqlParameter("@valueList", valueList);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                "insert into CMSWidgetSetting (ModuleID, widgetId, Name, PropertyName, type, Editor, ListValues) " +
                "values(@moduleId, @widgetId, @Name, @PropertyName, @type, @editor, @valueList); " +
                "Select @Id = SCOPE_IDENTITY()", arrParams);

            var id = int.Parse(arrParams[0].Value.ToString());

            return id;
        }



        //---------------


        public static DataSet SelectWidgetInformation(int widgetId)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@widgetId", widgetId);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from  CMSModuleWidget where UID = @widgetId", arrParams);
        }


        public static object GetValue(string stringValue, string typeName)
        {
            switch (typeName.ToLower())
            {
                case "string":
                    return (object) stringValue;
                case "bool":
                    return (object)(bool.Parse(stringValue));
                case "int":
                    return (object)(int.Parse(stringValue));
                case "decimal":
                    return (object)(decimal.Parse(stringValue));
                case "datetime":
                    return (object)(DateTime.Parse(stringValue));
                default: return (object)stringValue;
            }

            return null;
        }
    }
}