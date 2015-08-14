using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.ApplicationBlocks.Data;
using System.Collections;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{

[Serializable]    public class PrizeDrawing : EntityBase
    {

        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myPDID;
        private string myPrizeName = "";
        private int myTID = 0;
        private DateTime myDrawingDateTime;
        private int myNumWinners;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

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

        #region Accessors

        public int PDID
        {
            get { return myPDID; }
            set { myPDID = value; }
        }
        public string PrizeName
        {
            get { return myPrizeName; }
            set { myPrizeName = value; }
        }
        public int TID
        {
            get { return myTID; }
            set { myTID = value; }
        }
        public DateTime DrawingDateTime
        {
            get { return myDrawingDateTime; }
            set { myDrawingDateTime = value; }
        }
        public int NumWinners
        {
            get { return myNumWinners; }
            set { myNumWinners = value; }
        }
        public DateTime LastModDate
        {
            get { return myLastModDate; }
            set { myLastModDate = value; }
        }
        public string LastModUser
        {
            get { return myLastModUser; }
            set { myLastModUser = value; }
        }
        public DateTime AddedDate
        {
            get { return myAddedDate; }
            set { myAddedDate = value; }
        }
        public string AddedUser
        {
            get { return myAddedUser; }
            set { myAddedUser = value; }
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

        #region Constructors

        public PrizeDrawing()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion


        protected override bool CheckBusinessRules(BusinessRulesValidationMode validationMode)
        {
            // Remove any old error Codes
            ClearErrorCodes();

            if (validationMode == BusinessRulesValidationMode.INSERT)
            {

                if (TID == 0)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Template", "Template", "You must select a Template.",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }

            }

            if (validationMode == BusinessRulesValidationMode.UPDATE)
            {

                if (TID == 0)
                {
                    AddErrorCode(new BusinessRulesValidationMessage("Template", "Template", "You must select a .",
                                                                    BusinessRulesValidationCode.UNSPECIFIED));
                }

            }
            return (ErrorCodes.Count == 0);
            //return true;
        }


        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PrizeDrawing_GetAll", arrParams);
        }


        public static int DrawWinners(int PDID, int numWinners, int additional=0)
        {
            SqlParameter[] arrParams = new SqlParameter[3];
            arrParams[0] = new SqlParameter("@PDID", PDID);
            arrParams[1] = new SqlParameter("@NumWinners", numWinners);
            arrParams[2] = new SqlParameter("@Additional", additional);
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PrizeDrawing_DrawWinners", arrParams);

            var ret = ds.Tables[0].Rows.Count;

            var pd = PrizeDrawing.FetchObject(PDID);
            var pt = PrizeTemplate.FetchObject(pd.TID);
            var now = DateTime.Now;

            for (var i = 0; i < ds.Tables[0].Rows.Count; i++ )
            {
                int PID = (int) ds.Tables[0].Rows[i]["PatronID"];
                int NID = (int)ds.Tables[0].Rows[i]["NotificationID"];
                int PDWID = (int)ds.Tables[0].Rows[i]["PDWID"];

                // insert patron prize
                var pp = new PatronPrizes
                {
                    PID = PID,
                    PrizeSource = 0,
                    BadgeID = 0,
                    DrawingID = PDWID,
                    PrizeName = pd.PrizeName,
                    RedeemedFlag = false,
                    AddedDate = now,
                    LastModDate = now,
                    AddedUser = "N/A",
                    LastModUser = "N/A"
                };
                pp.Insert();

                if (pt.SendNotificationFlag)
                {
                    // generate notification
                    var not = new Notifications
                    {
                        PID_To = PID,
                        PID_From = 0,  //0 == System Notification
                        Subject = pt.NotificationSubject,
                        Body = pt.NotificationMessage,
                        isQuestion = false,
                        AddedDate = now,
                        LastModDate = now,
                        AddedUser = "N/A",
                        LastModUser = "N/A"
                    };
                    not.Insert();

                    var w = PrizeDrawingWinners.FetchObject(PDWID);
                    w.NotificationID = not.NID;
                    w.Update();
                }
                else
                {
                    var w = PrizeDrawingWinners.FetchObject(PDWID);
                    w.NotificationID = -1;
                    w.Update();
                }
            }

            return ret;

        }


        public static DataSet GetAllWinners(int PDID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@PDID", PDID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_PrizeDrawing_GetAllWinners", arrParams);
        }


        public static PrizeDrawing FetchObject(int PDID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PDID", PDID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PrizeDrawing_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PrizeDrawing result = new PrizeDrawing();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PDID"].ToString(), out _int)) result.PDID = _int;
                result.PrizeName = dr["PrizeName"].ToString();
                if (int.TryParse(dr["TID"].ToString(), out _int)) result.TID = _int;
                if (DateTime.TryParse(dr["DrawingDateTime"].ToString(), out _datetime)) result.DrawingDateTime = _datetime;
                if (int.TryParse(dr["NumWinners"].ToString(), out _int)) result.NumWinners = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int)) result.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int)) result.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int)) result.FldInt3 = _int;
                result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                result.FldText1 = dr["FldText1"].ToString();
                result.FldText2 = dr["FldText2"].ToString();
                result.FldText3 = dr["FldText3"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int PDID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PDID", PDID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_PrizeDrawing_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                PrizeDrawing result = new PrizeDrawing();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["PDID"].ToString(), out _int)) this.PDID = _int;
                this.PrizeName = dr["PrizeName"].ToString();
                if (int.TryParse(dr["TID"].ToString(), out _int)) this.TID = _int;
                if (DateTime.TryParse(dr["DrawingDateTime"].ToString(), out _datetime)) this.DrawingDateTime = _datetime;
                if (int.TryParse(dr["NumWinners"].ToString(), out _int)) this.NumWinners = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

                if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
                if (int.TryParse(dr["FldInt1"].ToString(), out _int)) this.FldInt1 = _int;
                if (int.TryParse(dr["FldInt2"].ToString(), out _int)) this.FldInt2 = _int;
                if (int.TryParse(dr["FldInt3"].ToString(), out _int)) this.FldInt3 = _int;
                this.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
                this.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
                this.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
                this.FldText1 = dr["FldText1"].ToString();
                this.FldText2 = dr["FldText2"].ToString();
                this.FldText3 = dr["FldText3"].ToString();

                dr.Close();

                return true;

            }

            dr.Close();

            return false;

        }

        public int Insert()
        {

            return Insert(this);

        }

        public static int Insert(PrizeDrawing o)
        {

            SqlParameter[] arrParams = new SqlParameter[19];

            arrParams[0] = new SqlParameter("@PrizeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizeName, o.PrizeName.GetTypeCode()));
            arrParams[1] = new SqlParameter("@TID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TID, o.TID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@DrawingDateTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DrawingDateTime, o.DrawingDateTime.GetTypeCode()));
            arrParams[3] = new SqlParameter("@NumWinners", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumWinners, o.NumWinners.GetTypeCode()));
            arrParams[4] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[6] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[8] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[9] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[18] = new SqlParameter("@PDID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDID, o.PDID.GetTypeCode()));
            arrParams[18].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeDrawing_Insert", arrParams);

            o.PDID = int.Parse(arrParams[18].Value.ToString());

            return o.PDID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(PrizeDrawing o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[19];

            arrParams[0] = new SqlParameter("@PDID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDID, o.PDID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@PrizeName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrizeName, o.PrizeName.GetTypeCode()));
            arrParams[2] = new SqlParameter("@TID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TID, o.TID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@DrawingDateTime", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DrawingDateTime, o.DrawingDateTime.GetTypeCode()));
            arrParams[4] = new SqlParameter("@NumWinners", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumWinners, o.NumWinners.GetTypeCode()));
            arrParams[5] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[6] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            arrParams[9] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[12] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[13] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[14] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[15] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[16] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[17] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[18] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeDrawing_Update", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        public int Delete()
        {

            return Delete(this);

        }

        public static int Delete(PrizeDrawing o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@PDID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PDID, o.PDID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_PrizeDrawing_Delete", arrParams);

            }

            catch (SqlException exx)
            {

                System.Diagnostics.Debug.Write(exx.Message);

            }

            return iReturn;

        }

        #endregion

    }//end class

}//end namespace

