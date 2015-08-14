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

[Serializable]    public class MGChooseAdvSlides : EntityBase
    {

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myCASID;
        private int myCAID;
        private int myMGID;
        private int myDifficulty;
        private int myStepNumber=-1;
        private string mySlideText = "";
        private int myFirstImageGoToStep=1;
        private int mySecondImageGoToStep=1;
        private DateTime myLastModDate;
        private string myLastModUser;
        private DateTime myAddedDate;
        private string myAddedUser;

        #endregion

        #region Accessors

        public int CASID
        {
            get { return myCASID; }
            set { myCASID = value; }
        }
        public int CAID
        {
            get { return myCAID; }
            set { myCAID = value; }
        }
        public int MGID
        {
            get { return myMGID; }
            set { myMGID = value; }
        }
        public int Difficulty
        {
            get { return myDifficulty; }
            set { myDifficulty = value; }
        }
        public int StepNumber
        {
            get { return myStepNumber; }
            set { myStepNumber = value; }
        }
        public string SlideText
        {
            get { return mySlideText; }
            set { mySlideText = value; }
        }
        public int FirstImageGoToStep
        {
            get { return myFirstImageGoToStep; }
            set { myFirstImageGoToStep = value; }
        }
        public int SecondImageGoToStep
        {
            get { return mySecondImageGoToStep; }
            set { mySecondImageGoToStep = value; }
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

        #endregion

        #region Constructors

        public MGChooseAdvSlides()
        {
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll(int MGID)
        {
            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@MGID", MGID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_GetAll", arrParams);
        }

        public static DataSet GetAllByDifficulty(int MGID, int Difficulty)
        {
            SqlParameter[] arrParams = new SqlParameter[2];

            arrParams[0] = new SqlParameter("@MGID", MGID);
            arrParams[1] = new SqlParameter("@Diff", Difficulty);
            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_GetAllByDifficulty", arrParams);
        }


        public static void MoveUp(int CASID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@CASID", CASID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_MoveUp", arrParams);
        }

        public static void MoveDn(int CASID)
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@CASID", CASID);
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_MoveDn", arrParams);
        }


        public static MGChooseAdvSlides FetchPlaySlide(int CAID, int Step, int Difficulty)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@CAID", CAID);
            arrParams[1] = new SqlParameter("@Step", Step);
            arrParams[2] = new SqlParameter("@Difficulty", Difficulty);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_GetPlaySlide", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGChooseAdvSlides result = new MGChooseAdvSlides();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CASID"].ToString(), out _int)) result.CASID = _int;
                if (int.TryParse(dr["CAID"].ToString(), out _int)) result.CAID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["Difficulty"].ToString(), out _int)) result.Difficulty = _int;
                if (int.TryParse(dr["StepNumber"].ToString(), out _int)) result.StepNumber = _int;
                result.SlideText = dr["SlideText"].ToString();
                if (int.TryParse(dr["FirstImageGoToStep"].ToString(), out _int)) result.FirstImageGoToStep = _int;
                if (int.TryParse(dr["SecondImageGoToStep"].ToString(), out _int)) result.SecondImageGoToStep = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }



        public static MGChooseAdvSlides FetchObject(int CASID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CASID", CASID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGChooseAdvSlides result = new MGChooseAdvSlides();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CASID"].ToString(), out _int)) result.CASID = _int;
                if (int.TryParse(dr["CAID"].ToString(), out _int)) result.CAID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) result.MGID = _int;
                if (int.TryParse(dr["Difficulty"].ToString(), out _int)) result.Difficulty = _int;
                if (int.TryParse(dr["StepNumber"].ToString(), out _int)) result.StepNumber = _int;
                result.SlideText = dr["SlideText"].ToString();
                if (int.TryParse(dr["FirstImageGoToStep"].ToString(), out _int)) result.FirstImageGoToStep = _int;
                if (int.TryParse(dr["SecondImageGoToStep"].ToString(), out _int)) result.SecondImageGoToStep = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;
                result.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
                result.AddedUser = dr["AddedUser"].ToString();

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int CASID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CASID", CASID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                MGChooseAdvSlides result = new MGChooseAdvSlides();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["CASID"].ToString(), out _int)) this.CASID = _int;
                if (int.TryParse(dr["CAID"].ToString(), out _int)) this.CAID = _int;
                if (int.TryParse(dr["MGID"].ToString(), out _int)) this.MGID = _int;
                if (int.TryParse(dr["Difficulty"].ToString(), out _int)) this.Difficulty = _int;
                if (int.TryParse(dr["StepNumber"].ToString(), out _int)) this.StepNumber = _int;
                this.SlideText = dr["SlideText"].ToString();
                if (int.TryParse(dr["FirstImageGoToStep"].ToString(), out _int)) this.FirstImageGoToStep = _int;
                if (int.TryParse(dr["SecondImageGoToStep"].ToString(), out _int)) this.SecondImageGoToStep = _int;
                if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;
                this.LastModUser = dr["LastModUser"].ToString();
                if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
                this.AddedUser = dr["AddedUser"].ToString();

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

        public static int Insert(MGChooseAdvSlides o)
        {

            SqlParameter[] arrParams = new SqlParameter[12];

            arrParams[0] = new SqlParameter("@CAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CAID, o.CAID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@Difficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Difficulty, o.Difficulty.GetTypeCode()));
            arrParams[3] = new SqlParameter("@StepNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StepNumber, o.StepNumber.GetTypeCode()));
            arrParams[4] = new SqlParameter("@SlideText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SlideText, o.SlideText.GetTypeCode()));
            arrParams[5] = new SqlParameter("@FirstImageGoToStep", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstImageGoToStep, o.FirstImageGoToStep.GetTypeCode()));
            arrParams[6] = new SqlParameter("@SecondImageGoToStep", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SecondImageGoToStep, o.SecondImageGoToStep.GetTypeCode()));
            arrParams[7] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[11] = new SqlParameter("@CASID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CASID, o.CASID.GetTypeCode()));
            arrParams[11].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_Insert", arrParams);

            o.CASID = int.Parse(arrParams[11].Value.ToString());

            return o.CASID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(MGChooseAdvSlides o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[12];

            arrParams[0] = new SqlParameter("@CASID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CASID, o.CASID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@CAID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CAID, o.CAID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@MGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MGID, o.MGID.GetTypeCode()));
            arrParams[3] = new SqlParameter("@Difficulty", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Difficulty, o.Difficulty.GetTypeCode()));
            arrParams[4] = new SqlParameter("@StepNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StepNumber, o.StepNumber.GetTypeCode()));
            arrParams[5] = new SqlParameter("@SlideText", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SlideText, o.SlideText.GetTypeCode()));
            arrParams[6] = new SqlParameter("@FirstImageGoToStep", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstImageGoToStep, o.FirstImageGoToStep.GetTypeCode()));
            arrParams[7] = new SqlParameter("@SecondImageGoToStep", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SecondImageGoToStep, o.SecondImageGoToStep.GetTypeCode()));
            arrParams[8] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[9] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[10] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[11] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_Update", arrParams);

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

        public static int Delete(MGChooseAdvSlides o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@CASID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CASID, o.CASID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_MGChooseAdvSlides_Delete", arrParams);

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

