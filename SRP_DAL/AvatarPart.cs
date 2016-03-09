using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{
    [Serializable] public class AvatarPart : EntityBase
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;


        public int PID { get; set; }
        public int ComponentID { get; set; }
        public string Name { get; set; }
        public DateTime LastModDate { get; set; }
        public string LastModUser { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedUser { get; set; }
        public int BadgeID { get; set; }

        public static DataSet GetAll(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_AvatarPart_GetAll", arrParams);
        }


        public enum ComponentType
        {
            Head = 0,
            Body = 1,
            Arms = 3
        }
    }
}
