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
    public class SRPTemplateHelper
    {
        private static string conn = STG.SRP.Core.Utilities.GlobalUtilities.SRPDB;
        //ConfigurationManager.ConnectionStrings["CMSConn"].ToString();

        public static DataSet SelectAllTemplates()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSTemplate");
        }

        public static DataSet SelectTemplateInfo(int id)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@Id", id);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSTemplate where UID = @Id", arrParams);
        }

        public static bool Delete(int id)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@Id", id);
            SqlHelper.ExecuteScalar(conn, CommandType.Text, 
                    "Delete CMSTemplateWidgetSetting where TemplateID = @Id; " + 
                    "Delete CMSTemplateWidget where TemplateID = @Id; " +
                    "Delete CMSTemplate where UID = @Id", arrParams);
            return true;
        }

        public static bool Update(int id, string name, string placeholderNames)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@Id", id);
            arrParams[1] = new SqlParameter("@Name", name);
            arrParams[2] = new SqlParameter("@PlaceholderNames", placeholderNames);
            SqlHelper.ExecuteScalar(conn, CommandType.Text, "Update CMSTemplate set Name=@Name, PlaceholderNames=@PlaceholderNames", arrParams);
            return true;
        }

        public static bool Insert(string name, string placeholderNames)
        {
            var arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@Id", -1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@Name", name);
            arrParams[2] = new SqlParameter("@PlaceholderNames", placeholderNames);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "insert into CMSTemplate (Name, PlaceholderNames) values( @Name, @PlaceholderNames); Select @Id = SCOPE_IDENTITY()", arrParams);

            int id = int.Parse(arrParams[0].Value.ToString());

            return true;
        }


        //-----


        public static DataSet GetTemplate(int templateID)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@templateID", templateID);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSTemplate where UID=@templateID",arrParams);
        }

        public static DataSet GetTemplateWidgets(int templateID)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@templateID", templateID);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, 
                "Select t.UID, t.TemplateID, t.WidgetID, t.PlaceholderName, " + 
                        "w.Name, w.JavaScripts1, w.JavaScripts2, w.JavaScripts3, w.JavaScripts4, w.Location, w.ModuleID " + 
                "from CMSTemplateWidget t left join CMSModuleWidget w on t.WidgetID = w.UID "+
                "where TemplateID=@templateID",arrParams);
        }

        public static bool DeleteTemplateWidget(int id)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@Id", id);
            SqlHelper.ExecuteScalar(conn, CommandType.Text,
                                    "Delete CMSTemplateWidgetSetting where TWID = @Id; " +
                                    "Delete CMSTemplateWidget where UID = @Id; ", arrParams);
            return true;            
        }

        public static DataSet SelectAllTemplateWidgets()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select w.UID, m.Name + ' - ' + w.Name as Name from CMSModuleWidget w join CMSModule m on w.ModuleID = m.UID where TemplateOrContent in (1,2) order by m.name, w.name ");

        }


        public static DataSet GetTemplateWidgetSettings(int templateID)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@templateID", templateID);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select s.TWID, s.WidgetSettingID, n.Name, n.PropertyName, n.Type, Value, n.Editor, n.ListValues from CMSTemplateWidget w inner join CMSTemplateWidgetSetting s on w.UID = s.TWID inner join CMSWidgetSetting n on s.WidgetSettingID = n.UID where w.TemplateID=@templateID", arrParams);
        }

        public static int AddTemplateWidget(int templateId, int widgetId, string placeholder)
        {
            var arrParams = new SqlParameter[4];
            arrParams[0] = new SqlParameter("@Id", -1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@templateId", templateId);
            arrParams[2] = new SqlParameter("@widgetId", widgetId);
            arrParams[3] = new SqlParameter("@PlaceholderName", placeholder);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "insert into CMSTemplateWidget (templateId, widgetId, PlaceholderName) values( @templateId, @widgetId, @PlaceholderName); Select @Id = SCOPE_IDENTITY(); " +
                "insert into CMSTemplateWidgetSetting (templateid, twid, WidgetSettingId, value) select @templateId, @id, UID, '' from CMSWidgetSetting where WidgetId =  @widgetId;", arrParams);

            int id = int.Parse(arrParams[0].Value.ToString());

            return id;            
        }

        public static bool UpdateTemplateWidget(int uid, int templateId, int widgetId, string placeholder)
        {
            var arrParams = new SqlParameter[4];
            arrParams[0] = new SqlParameter("@Id", uid);
            arrParams[1] = new SqlParameter("@templateId", templateId);
            arrParams[2] = new SqlParameter("@widgetId", widgetId);
            arrParams[3] = new SqlParameter("@PlaceholderName", placeholder);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                "update CMSTemplateWidget set templateId = @templateId, widgetId = @widgetId, PlaceholderName = @PlaceholderName where UID = @Id;", 
                arrParams);

            return true;
        }

        public static DataSet SelectTemplateWidgetParameters(int TWID)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@TWID", TWID);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, 
                "Select s.UID, s.TemplateID, s.TWID, s.WidgetSettingID, n.Name, n.Type, s.Value, n.Editor, n.ListValues " +
                "from CMSTemplateWidgetSetting s inner join CMSWidgetSetting n on s.WidgetSettingID = n.UID where s.TWID=@TWID", arrParams);
            
        }

        public static bool UpdateTemplateWidgetSetting(int uid, int templateId, int TWID, int WSID, string value)
        {
            var arrParams = new SqlParameter[5];
            arrParams[0] = new SqlParameter("@Id", uid);
            arrParams[1] = new SqlParameter("@templateId", templateId);
            arrParams[2] = new SqlParameter("@TWID", TWID);
            arrParams[3] = new SqlParameter("@WidgetSettingID", WSID);
            arrParams[4] = new SqlParameter("@value", value);

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                "update CMSTemplateWidgetSetting set templateId = @templateId, TWID = @TWID, WidgetSettingID = @WidgetSettingID, value = @value where UID = @Id;", 
                arrParams);

            return true;
        }
    }
}