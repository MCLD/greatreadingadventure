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
    public class CMSPageHelper
    {

        private static string conn = STG.CMS.Core.CMSUtilities.CMSUtilities.CMSDB;
        //ConfigurationManager.ConnectionStrings["CMSConn"].ToString();


        public static DataSet SelectAllPages()
        {
            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSPage");
        }

        public static DataSet SelectPageInfo(int pageId)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@pageId", pageId);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSPage where UID = @pageId", arrParams);
        }

        public static DataSet SelectPageContent(int pageId)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@pageId", pageId);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSPageContent where PageID = @pageId order by ContentOrder", arrParams);
        }


        public static DataSet SelectPageWidgetSettings(int pageId)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@pageId", pageId);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select ws.*, s.Name, s.PropertyName, s.Type, s.Editor, s.ListValues from CMSPageWidgetSetting ws inner join CMSWidgetSetting s on ws.WidgetID = s.WidgetID and ws.SettingID = s.UID where PageId = @pageId", arrParams);
        }
        public static int FindPageId(string pageName)
        {
            int pageId = -1;
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@pageName", pageName.Replace("_", " "));

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSPage where Name = @pageName", arrParams);
            if (ds!=null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    pageId = (int) ds.Tables[0].Rows[0]["UID"];
                }
            }
            return pageId;
        }

         public static bool Delete (int pageId)
         {
             var arrParams = new SqlParameter[1];
             arrParams[0] = new SqlParameter("@pageId", pageId);
             SqlHelper.ExecuteScalar(conn, CommandType.Text, "Delete CMSPageContent where PageID = @pageId; Delete CMSPageWidgetSetting where PageID = @pageId; Delete CMSPage where UID = @pageId", arrParams);
             return true;
         }

         public static bool Update(int pageId, string pageName, string pageTitle, string pageMeta, bool metaOverride, string html)
         {
             var arrParams = new SqlParameter[5];
             arrParams[0] = new SqlParameter("@pageId", pageId);
             arrParams[1] = new SqlParameter("@pageName", pageName);
             arrParams[2] = new SqlParameter("@pageTitle", pageTitle);
             arrParams[3] = new SqlParameter("@pageMeta", pageMeta);
             arrParams[4] = new SqlParameter("@metaOverride", metaOverride);
             SqlHelper.ExecuteScalar(conn, CommandType.Text, "Update CMSPage set Name=@pageName, Title=@pageTitle, PageMeta=@pageMeta, PageMetaOverride=@metaOverride where UID = @pageId", arrParams);
             
             arrParams = new SqlParameter[2];
             arrParams[0] = new SqlParameter("@pageId", pageId);
             arrParams[1] = new SqlParameter("@pageHTML", html);
             SqlHelper.ExecuteScalar(conn, CommandType.Text, "Update CMSPageContent set ContentType=0, TextContent=@pageHTML where PageID = @pageId and ContentOrder = 1", arrParams);           

             return true;
         }

         public static int Insert(string pageName, string pageTitle, string pageMeta, bool metaOverride, string html)
         {
             var arrParams = new SqlParameter[5];
             arrParams[0] = new SqlParameter("@pageId",-1);
             arrParams[0].Direction = ParameterDirection.Output;

             arrParams[1] = new SqlParameter("@pageName", pageName);
             arrParams[2] = new SqlParameter("@pageTitle", pageTitle);
             arrParams[3] = new SqlParameter("@pageMeta", pageMeta);
             arrParams[4] = new SqlParameter("@metaOverride", metaOverride);
             SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "insert into CMSPage (Name, Title, PageMeta, PageMetaOverride, PageContent) values( @pageName, @pageTitle, @pageMeta, @metaOverride,''); Select @pageId = SCOPE_IDENTITY()", arrParams);

             int pageID = int.Parse(arrParams[0].Value.ToString());

             arrParams = new SqlParameter[2];
             arrParams[0] = new SqlParameter("@pageId", pageID);
             arrParams[1] = new SqlParameter("@pageHTML", html);
             SqlHelper.ExecuteScalar(conn, CommandType.Text, "Insert into CMSPageContent (PageID, ContentOrder, ContentType, TextContent, WidgetId) " +
                                                            "values(@pageId, 1, 0, @pageHTML, -1)", arrParams);

             return pageID;
         }

         public static DataSet SelectAllPageWidgets()
         {
             return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select w.UID, m.Name + ' - ' + w.Name as Name from CMSModuleWidget w join CMSModule m on w.ModuleID = m.UID where TemplateOrContent in (0,2) order by m.name, w.name ");

         }

         public static DataSet GetPageWidgets(int pageId)
         {
             var arrParams = new SqlParameter[1];

             arrParams[0] = new SqlParameter("@PageId", pageId);

             return SqlHelper.ExecuteDataset(conn, CommandType.Text,
                 "Select c.UID, c.PageId, c.ContentOrder, c.ContentType, c.WidgetId, " +
                         "w.Name " +
                 "from CMSPageContent c left join CMSModuleWidget w on c.WidgetID = w.UID " +
                 "where PageId=@PageId and ContentType = 1", arrParams);
         }

         public static DataSet SelectPageWidgetParameters(int contentId)
         {
             var arrParams = new SqlParameter[1];

             arrParams[0] = new SqlParameter("@ContentID", contentId);

             return SqlHelper.ExecuteDataset(conn, CommandType.Text,
                 "Select s.UID, s.ContentID, s.PageID, s.WidgetID, s.SettingID, n.Name, n.Type, s.Value, n.Editor, n.ListValues " +
                 "from CMSPageWidgetSetting s inner join CMSWidgetSetting n on s.SettingID = n.UID where s.ContentID=@contentId", arrParams);

         }

         public static bool DeletePageWidget(int id)
         {
             var arrParams = new SqlParameter[1];
             arrParams[0] = new SqlParameter("@Id", id);
             SqlHelper.ExecuteScalar(conn, CommandType.Text,
                                     "Delete CMSPageWidgetSetting where ContentID = @Id; " +
                                     "Delete CMSPageContent where UID = @Id; ", arrParams);
             return true;
         }


         public static int AddPageWidget(int pageID, int contentType, string textContent, int widgetId)
         {
             var arrParams = new SqlParameter[5];
             arrParams[0] = new SqlParameter("@Id", -1);
             arrParams[0].Direction = ParameterDirection.Output;

             arrParams[1] = new SqlParameter("@pageID", pageID);
             arrParams[2] = new SqlParameter("@contentType", contentType);
             arrParams[3] = new SqlParameter("@textContent", textContent);
             arrParams[4] = new SqlParameter("@widgetId", widgetId);

             SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                 "insert into CMSPageContent (PageID, ContentOrder, ContentType, TextContent, WidgetID) values( @pageID, (select max(ContentOrder)+1 from CMSPageContent where PageID = @pageID) , @contentType, @textContent, @widgetId); Select @Id = SCOPE_IDENTITY(); " +
                 "insert into CMSPageWidgetSetting (ContentId, PageId, WidgetId, SettingId, value) select @id, @pageId, @widgetID, UID, '' from CMSWidgetSetting where WidgetId =  @widgetId;", arrParams);

             int id = int.Parse(arrParams[0].Value.ToString());

             return id;
         }

        
         public static bool UpdatePageWidgetSetting(int uid, int contentId, int pageId, int widgetId, int settingId, string value)
         {
             var arrParams = new SqlParameter[6];
             arrParams[0] = new SqlParameter("@Id", uid);
             arrParams[1] = new SqlParameter("@contentId", contentId);
             arrParams[2] = new SqlParameter("@pageId", pageId);
             arrParams[3] = new SqlParameter("@widgetId", widgetId);
             arrParams[4] = new SqlParameter("@settingId", settingId);
             arrParams[5] = new SqlParameter("@value", value);

             SqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                 "update CMSPageWidgetSetting set contentId = @contentId, pageId = @pageId, widgetId = @widgetId,  settingId = @settingId, value = @value where UID = @Id;",
                 arrParams);

             return true;
         }

    }
}