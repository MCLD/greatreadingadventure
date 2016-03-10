using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{
    [Serializable] public class AvatarPart : EntityBase
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;


        public int APID { get; set; }
        public int ComponentID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime LastModDate { get; set; }
        public string LastModUser { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedUser { get; set; }
        public int BadgeID { get; set; }

        public static DataSet GetQualifiedByPatron(int PID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PID", PID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_AvatarPart_GetQualifiedByPatron", arrParams);
        }


        public AvatarPart GetAvatarPart(int APID)
        {

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@APID", APID);

            SqlDataReader dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_AvatarPart_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                AvatarPart result = new AvatarPart();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["AID"].ToString(), out _int))
                    result.APID = _int;
                if (int.TryParse(dr["ComponentID"].ToString(), out _int))
                    result.ComponentID = _int;

                result.Name = dr["Name"].ToString();
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime))
                    result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime))
                    result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();
                if (int.TryParse(dr["BadgeID"].ToString(), out _int))
                    result.BadgeID = _int;

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


        public static int Insert(AvatarPart o)
        {

            var parameters = new List<SqlParameter>();

            /*
            parameters.Add(new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode())));
            arrParams[1] = new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[2] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[4] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[6] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[7] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[16] = new SqlParameter("@AID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AID, o.AID.GetTypeCode()));
            arrParams[16].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Avatar_Insert", arrParams);
            o.AID = int.Parse(arrParams[16].Value.ToString());
            return o.AID;
            */

            return 1;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(AvatarPart o)
        {

            /*
            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[17];

            arrParams[0] = new SqlParameter("@AID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AID, o.AID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@Name", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Name, o.Name.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[3] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[5] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[8] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_Avatar_Update", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }
            */

            return 1;
        }

        public int Delete()
        {

            return Delete(this);

        }

        public static int Delete(AvatarPart o)
        {
            return 1;
        }

        public enum ComponentType
        {
            Head = 0,
            Body = 1,
            Arms = 3
        }
    }
}
