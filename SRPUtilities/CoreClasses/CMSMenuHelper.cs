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
    public class CMSMenuHelper
    {
        private static string conn = STG.CMS.Core.CMSUtilities.CMSUtilities.CMSDB;
        //ConfigurationManager.ConnectionStrings["CMSConn"].ToString();

        public static DataSet LoadMenusByParentUID(int parentId)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@parentId", parentId);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSSiteMenu where ParentMenu = @parentId order by RelativeOrder asc", arrParams);
        }


        public static DataSet LoadMenusForEdit()
        {

            var sql =
                "WITH  abcd " +
                "AS ( " +
                "    SELECT  uid, MenuLabel, MenuTag, ParentMenu,RelativeOrder, URL, " +
                "            CAST(0 AS int) AS MenuLevel " +
                "            ,CAST ('1' + CONVERT(varchar, RelativeOrder) AS VARCHAR(1000)) AS \"Path\" " +
                "            ,case RelativeOrder when 1 then 0 else 1 end as MoveUp " +
                "    FROM    CMSSiteMenu  " +
                "    WHERE   ParentMenu = -1 " +
                "    UNION ALL " +
                "    SELECT  t.uid, t.MenuLabel, t.MenuTag, t.ParentMenu,t.RelativeOrder, t.URL, " +
                "            CAST(a.MenuLevel + 1 AS int) AS MenuLevel " +
                "            ,CAST((a.path + CONVERT(varchar, t.RelativeOrder)) AS VARCHAR(1000)) AS \"Path\" " +
                "            ,case t.RelativeOrder when 1 then 0 else 1 end as MoveUp " +
                "    FROM    CMSSiteMenu AS t " +
                "            JOIN abcd AS a " +
                "              ON t.ParentMenu = a.uid " +
                "   ) " +
                "SELECT *, " +
                "	case abcd.RelativeOrder when (select MAX(sm.RelativeOrder) from CMSSiteMenu sm where sm.ParentMenu = abcd.ParentMenu)  then 0 else 1 end as MoveDn " +
                "FROM abcd " +
                "order by Path;";
            return SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
        }


//        (uid, MenuLabel, MenuTag, int.Parse(ParentMenu), int.Parse(RelativeOrder), URL);//
        public static bool UpdateMenuItem(int uid, string menuLabel, string menuTag, int parentMenu, int relativeOrder, string url)
         {
             var arrParams = new SqlParameter[6];
             arrParams[0] = new SqlParameter("@uid", uid);
             arrParams[1] = new SqlParameter("@MenuLabel", menuLabel);
             arrParams[2] = new SqlParameter("@MenuTag", menuTag);
             arrParams[3] = new SqlParameter("@ParentMenu", parentMenu);
             arrParams[4] = new SqlParameter("@RelativeOrder", relativeOrder);
             arrParams[5] = new SqlParameter("@url", url);
             SqlHelper.ExecuteScalar(conn, CommandType.Text, "Update CMSSiteMenu set MenuLabel=@MenuLabel, MenuTag=@MenuTag, ParentMenu=@ParentMenu, RelativeOrder=@RelativeOrder, url=@url where UID = @uid", arrParams);
             
             return true;
         }

        public static int InsertMenuItem(string menuLabel, string menuTag, int parentMenu, string url)
        {
            var arrParams = new SqlParameter[5];
            arrParams[0] = new SqlParameter("@uid",-1);
            arrParams[0].Direction = ParameterDirection.Output;

            arrParams[1] = new SqlParameter("@MenuLabel", menuLabel);
            arrParams[2] = new SqlParameter("@MenuTag", menuTag);
            arrParams[3] = new SqlParameter("@ParentMenu", parentMenu);
            arrParams[4] = new SqlParameter("@url", url);
            SqlHelper.ExecuteScalar(conn, CommandType.Text,
                "Insert into CMSSiteMenu (MenuLabel,MenuTag,ParentMenu,RelativeOrder,URL) " +
                "values (@MenuLabel, @MenuTag, @ParentMenu, (select isnull(max(RelativeOrder),0)+1 from CMSSiteMenu where ParentMenu = @ParentMenu), @url);" +
                "; Select @uid = SCOPE_IDENTITY()", arrParams);

            return int.Parse(arrParams[0].Value.ToString());             
        }
            
        public static bool MoveUpMenuItem(int uid)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@uid", uid);

            String SQL = 
                 "Declare @CurrOrder int, @UID2 int; Declare @ParentMenu int; " +
                 "Select @CurrOrder = RelativeOrder, @ParentMenu=ParentMenu from CMSSiteMenu where uid = @uid; " +
                 "if @CurrOrder > 1 " +
                 "begin " +
                 "   Select @UID2 = UID from CMSSiteMenu where RelativeOrder = (@CurrOrder - 1) and ParentMenu = @ParentMenu; " +
                 "   update CMSSiteMenu set RelativeOrder = @CurrOrder - 1 " +
                 "       where uid = @uid ; " +
                 "   update CMSSiteMenu set RelativeOrder = @CurrOrder  " +
                 "   where uid = @UID2 ; " +
                 "end";
            SqlHelper.ExecuteScalar(conn, CommandType.Text, SQL, arrParams);

            return true;
        }
        public static bool MoveDnMenuItem(int uid)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@uid", uid);

            String SQL = "Declare @CurrOrder int, @UID2 int; Declare @ParentMenu int; " +
             "Select @CurrOrder = RelativeOrder, @ParentMenu=ParentMenu from CMSSiteMenu where uid = @uid; " +
             "if @CurrOrder < (Select MAX(RelativeOrder) from CMSSiteMenu where ParentMenu = @ParentMenu) " +
             "begin " +
             "   Select @UID2 = UID from CMSSiteMenu where RelativeOrder = (@CurrOrder + 1) and ParentMenu = @ParentMenu; " +
             "   update CMSSiteMenu set RelativeOrder = @CurrOrder + 1 " +
             "       where uid = @uid ; " +
             "   update CMSSiteMenu set RelativeOrder = @CurrOrder  " +
             "   where uid = @UID2 ; " +
             "end";
            SqlHelper.ExecuteScalar(conn, CommandType.Text, SQL, arrParams);

            return true;
        }

        public static bool DeleteMenuItem(int uid)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@uid", uid);
            SqlHelper.ExecuteScalar(conn, CommandType.Text,
                "Delete CMSSiteMenu where ParentMenu = @uid; Delete CMSSiteMenu where uid = @uid;", arrParams);
            return true;
        }


    }
}