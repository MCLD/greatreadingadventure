using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.IO;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;
using System.Text;

namespace GRA.SRP.DAL
{
    [Serializable] public class AvatarPart : EntityBase
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;


        public int APID { get; set; }
        public int ComponentID { get; set; }
        public int Ordering { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime LastModDate { get; set; }
        public string LastModUser { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedUser { get; set; }

        public int BadgeID { get; set; }
        public int TenID { get; set; }

        /* 
          BadgeID links to PatronBadges. This allows multiple avatar parts to be awarded from the same badge
          and an avatar can be awarded in multiple ways indrectly through the same badge in different Award Triggers.
          - Justin
        */

        public AvatarPart()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        public static DataSet GetQualifiedByPatron(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_AvatarPart_GetQualifiedByPatron", arrParams);
        }

        public static AvatarPart FetchObject(int APID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@APID", APID);

            SqlDataReader dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_AvatarPart_GetByID", arrParams);

            if (dr.Read())
            {
                AvatarPart result = new AvatarPart();

                DateTime _datetime;
                int _int;

                if (int.TryParse(dr["APID"].ToString(), out _int))
                    result.APID = _int;
                if (int.TryParse(dr["ComponentID"].ToString(), out _int))
                    result.ComponentID = _int;
                if (int.TryParse(dr["Ordering"].ToString(), out _int))
                    result.Ordering = _int;
                result.Name = dr["Name"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();
                if (int.TryParse(dr["BadgeID"].ToString(), out _int))
                    result.BadgeID = _int;
                if (int.TryParse(dr["TenID"].ToString(), out _int))
                    result.TenID = _int;

                dr.Close();
                return result;
            }

            dr.Close();
            return null;
        }

        public AvatarPart GetAvatarPart(int APID)
        {
            return AvatarPart.FetchObject(APID);
        }

        public static DataSet GetAll()
        {
            return GetAll(true);
        }

        public static DataSet GetAssociatedWithBadge(int badgeID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@BadgeID", badgeID);

            StringBuilder query = new StringBuilder("SELECT * FROM [AvatarPart]"
              + " WHERE BadgeID = @BadgeID"
              + " ORDER BY Ordering");


            var result = SqlHelper.ExecuteDataset(conn,
                                                 System.Data.CommandType.Text,
                                                 query.ToString(),
                                                 arrParams);

            return result;
        }

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

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_AvatarPart_GetAll", arrParams);
        }

        public static DataSet GetAll(int TenID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID", TenID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_AvatarPart_GetAll", arrParams);
        }

        public int Insert()
        {
            return Insert(this);
        }

        public static int Insert(AvatarPart o)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@ComponentID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ComponentID, o.ComponentID.GetTypeCode())));
            parameters.Add(new SqlParameter("@Ordering", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Ordering, o.Ordering.GetTypeCode())));
            parameters.Add(new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode())));
            parameters.Add(new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode())));

            parameters.Add(new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            parameters.Add(new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            parameters.Add(new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            parameters.Add(new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

            parameters.Add(new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode())));
            parameters.Add(new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));

            SqlParameter aidParmater = new SqlParameter("@APID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.APID, o.APID.GetTypeCode()));
            aidParmater.Direction = ParameterDirection.Output;

            parameters.Add(aidParmater);

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_AvatarPart_Insert", parameters.ToArray());
            o.APID = int.Parse(aidParmater.Value.ToString());
            return o.APID;
        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(AvatarPart o)
        {

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@APID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.APID, o.APID.GetTypeCode())));
            parameters.Add(new SqlParameter("@ComponentID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ComponentID, o.ComponentID.GetTypeCode())));
            parameters.Add(new SqlParameter("@Ordering", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Ordering, o.Ordering.GetTypeCode())));
            parameters.Add(new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode())));
            parameters.Add(new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode())));

            parameters.Add(new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode())));
            parameters.Add(new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode())));
            parameters.Add(new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode())));
            parameters.Add(new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode())));

            parameters.Add(new SqlParameter("@BadgeID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BadgeID, o.BadgeID.GetTypeCode())));
            parameters.Add(new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode())));

            int returnStatus = -1; 

            try
            {
                returnStatus = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_AvatarPart_Update", parameters.ToArray());
            }
            catch (SqlException exx)
            {
                System.Diagnostics.Debug.Write(exx.Message);
            }

            return returnStatus;
        }

        public int Delete()
        {

            return Delete(this);

        }

        public static int Delete(AvatarPart o)
        {
            int returnStatus = -1;

            var arrParams = new SqlParameter[2];
            arrParams[0] = new SqlParameter("@APID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.APID, o.APID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TenID",
                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                        -1 :
                        (int)HttpContext.Current.Session["TenantID"])
            );

            try
            {
                var fileName = (HttpContext.Current.Server.MapPath("~/Images/AvatarParts/") + "\\" + o.APID.ToString() + ".png");
                File.Delete(fileName);
                fileName = (HttpContext.Current.Server.MapPath("~/Images/AvatarParts/") + "\\sm_" + o.APID.ToString() + ".png");
                File.Delete(fileName);

                returnStatus = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_AvatarPart_Delete", arrParams);
            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return returnStatus;
        }
    }
}
