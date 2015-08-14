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

[Serializable]    public class SRPReport : EntityBase
    {
        public static new string Version { get { return "2.0"; } }

        #region Private Variables

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private int myRID = 0;
        private int myRTID = 0;
        private int myProgId = 0;
        private string myReportName = "";
        private bool myDisplayFilters = false;
        private int myReportFormat = 0;
        private DateTime myDOBFrom;
        private DateTime myDOBTo;
        private int myAgeFrom = 0;
        private int myAgeTo = 0;
        private string mySchoolGrade = "";
        private string myFirstName = "";
        private string myLastName = "";
        private string myGender = "";
        private string myEmailAddress = "";
        private string myPhoneNumber = "";
        private string myCity = "";
        private string myState = "";
        private string myZipCode = "";
        private string myCounty = "";
        private int myPrimaryLibrary = 0;
        private string mySchoolName = "";
        private string myDistrict = "";
        private string myTeacher = "";
        private string myGroupTeamName = "";
        private int mySchoolType = 0;
        private int myLiteracyLevel1 = 0;
        private int myLiteracyLevel2 = 0;
        private string myCustom1 = "";
        private string myCustom2 = "";
        private string myCustom3 = "";
        private string myCustom4 = "";
        private string myCustom5 = "";
        private DateTime myRegistrationDateStart;
        private DateTime myRegistrationDateEnd;
        private int myPointsMin = 0;
        private int myPointsMax = 0;
        private DateTime myPointsStart;
        private DateTime myPointsEnd;
        private string myEventCode = "";
        private int myEarnedBadge = 0;
        private string myPhysicalPrizeEarned = "";
        private bool myPhysicalPrizeRedeemed = false;
        private DateTime myPhysicalPrizeStartDate;
        private DateTime myPhysicalPrizeEndDate;
        private int myReviewsMin = 0;
        private int myReviewsMax = 0;
        private string myReviewTitle = "";
        private string myReviewAuthor = "";
        private DateTime myReviewStartDate;
        private DateTime myReviewEndDate;
        private string myRandomDrawingName = "";
        private int myRandomDrawingNum = 0;
        private DateTime myRandomDrawingStartDate;
        private DateTime myRandomDrawingEndDate;
        private bool myHasBeenDrawn = false;
        private bool myHasRedeemend = false;
        private bool myPIDInc = false;
        private bool myUsernameInc = false;
        private bool myDOBInc = false;
        private bool myAgeInc = false;
        private bool mySchoolGradeInc = false;
        private bool myFirstNameInc = false;
        private bool myLastNameInc = false;
        private bool myGenderInc = false;
        private bool myEmailAddressInc = false;
        private bool myPhoneNumberInc = false;
        private bool myCityInc = false;
        private bool myStateInc = false;
        private bool myZipCodeInc = false;
        private bool myCountyInc = false;
        private bool myPrimaryLibraryInc = false;
        private bool mySchoolNameInc = false;
        private bool myDistrictInc = false;
        private bool myTeacherInc = false;
        private bool myGroupTeamNameInc = false;
        private bool mySchoolTypeInc = false;
        private bool myLiteracyLevel1Inc = false;
        private bool myLiteracyLevel2Inc = false;
        private bool myCustom1Inc = false;
        private bool myCustom2Inc = false;
        private bool myCustom3Inc = false;
        private bool myCustom4Inc = false;
        private bool myCustom5Inc = false;
        private bool myRegistrationDateInc = false;
        private bool myPointsInc = false;
        private bool myEarnedBadgeInc = false;
        private bool myPhysicalPrizeNameInc = false;
        private bool myPhysicalPrizeDateInc = false;
        private bool myNumReviewsInc = false;
        private bool myReviewAuthorInc = false;
        private bool myReviewTitleInc = false;
        private bool myReviewDateInc = false;
        private bool myRandomDrawingNameInc = false;
        private bool myRandomDrawingNumInc = false;
        private bool myRandomDrawingDateInc = false;
        private bool myHasBeenDrawnInc = false;
        private bool myHasRedeemendInc = false;
        private DateTime myLastModDate;
        private string myLastModUser="N/A";
        private DateTime myAddedDate;
        private string myAddedUser="N/A";
        private string mySDistrict = "";
        private bool mySDistrictInc = false;

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

        private int myScore1From = 0;
        private int myScore1To = 0;
        private int myScore1PctFrom = 0;
        private int myScore1PctTo = 0;
        private int myScore2From = 0;
        private int myScore2To = 0;
        private int myScore2PctFrom = 0;
        private int myScore2PctTo = 0;

        private bool myScore1Inc = false;
        private bool myScore2Inc = false;
        private bool myScore1PctInc = false;
        private bool myScore2PctInc = false;

        #endregion

        #region Accessors

        public int RID
        {
            get { return myRID; }
            set { myRID = value; }
        }
        public int RTID
        {
            get { return myRTID; }
            set { myRTID = value; }
        }
        public int ProgId
        {
            get { return myProgId; }
            set { myProgId = value; }
        }
        public string ReportName
        {
            get { return myReportName; }
            set { myReportName = value; }
        }
        public bool DisplayFilters
        {
            get { return myDisplayFilters; }
            set { myDisplayFilters = value; }
        }
        public int ReportFormat
        {
            get { return myReportFormat; }
            set { myReportFormat = value; }
        }
        public DateTime DOBFrom
        {
            get { return myDOBFrom; }
            set { myDOBFrom = value; }
        }
        public DateTime DOBTo
        {
            get { return myDOBTo; }
            set { myDOBTo = value; }
        }
        public int AgeFrom
        {
            get { return myAgeFrom; }
            set { myAgeFrom = value; }
        }
        public int AgeTo
        {
            get { return myAgeTo; }
            set { myAgeTo = value; }
        }
        public string SchoolGrade
        {
            get { return mySchoolGrade; }
            set { mySchoolGrade = value; }
        }
        public string FirstName
        {
            get { return myFirstName; }
            set { myFirstName = value; }
        }
        public string LastName
        {
            get { return myLastName; }
            set { myLastName = value; }
        }
        public string Gender
        {
            get { return myGender; }
            set { myGender = value; }
        }
        public string EmailAddress
        {
            get { return myEmailAddress; }
            set { myEmailAddress = value; }
        }
        public string PhoneNumber
        {
            get { return myPhoneNumber; }
            set { myPhoneNumber = value; }
        }
        public string City
        {
            get { return myCity; }
            set { myCity = value; }
        }
        public string State
        {
            get { return myState; }
            set { myState = value; }
        }
        public string ZipCode
        {
            get { return myZipCode; }
            set { myZipCode = value; }
        }
        public string County
        {
            get { return myCounty; }
            set { myCounty = value; }
        }
        public int PrimaryLibrary
        {
            get { return myPrimaryLibrary; }
            set { myPrimaryLibrary = value; }
        }
        public string SchoolName
        {
            get { return mySchoolName; }
            set { mySchoolName = value; }
        }
        public string District
        {
            get { return myDistrict; }
            set { myDistrict = value; }
        }
        public string Teacher
        {
            get { return myTeacher; }
            set { myTeacher = value; }
        }
        public string GroupTeamName
        {
            get { return myGroupTeamName; }
            set { myGroupTeamName = value; }
        }
        public int SchoolType
        {
            get { return mySchoolType; }
            set { mySchoolType = value; }
        }
        public int LiteracyLevel1
        {
            get { return myLiteracyLevel1; }
            set { myLiteracyLevel1 = value; }
        }
        public int LiteracyLevel2
        {
            get { return myLiteracyLevel2; }
            set { myLiteracyLevel2 = value; }
        }
        public string Custom1
        {
            get { return myCustom1; }
            set { myCustom1 = value; }
        }
        public string Custom2
        {
            get { return myCustom2; }
            set { myCustom2 = value; }
        }
        public string Custom3
        {
            get { return myCustom3; }
            set { myCustom3 = value; }
        }
        public string Custom4
        {
            get { return myCustom4; }
            set { myCustom4 = value; }
        }
        public string Custom5
        {
            get { return myCustom5; }
            set { myCustom5 = value; }
        }
        public DateTime RegistrationDateStart
        {
            get { return myRegistrationDateStart; }
            set { myRegistrationDateStart = value; }
        }
        public DateTime RegistrationDateEnd
        {
            get { return myRegistrationDateEnd; }
            set { myRegistrationDateEnd = value; }
        }
        public int PointsMin
        {
            get { return myPointsMin; }
            set { myPointsMin = value; }
        }
        public int PointsMax
        {
            get { return myPointsMax; }
            set { myPointsMax = value; }
        }
        public DateTime PointsStart
        {
            get { return myPointsStart; }
            set { myPointsStart = value; }
        }
        public DateTime PointsEnd
        {
            get { return myPointsEnd; }
            set { myPointsEnd = value; }
        }
        public string EventCode
        {
            get { return myEventCode; }
            set { myEventCode = value; }
        }
        public int EarnedBadge
        {
            get { return myEarnedBadge; }
            set { myEarnedBadge = value; }
        }
        public string PhysicalPrizeEarned
        {
            get { return myPhysicalPrizeEarned; }
            set { myPhysicalPrizeEarned = value; }
        }
        public bool PhysicalPrizeRedeemed
        {
            get { return myPhysicalPrizeRedeemed; }
            set { myPhysicalPrizeRedeemed = value; }
        }
        public DateTime PhysicalPrizeStartDate
        {
            get { return myPhysicalPrizeStartDate; }
            set { myPhysicalPrizeStartDate = value; }
        }
        public DateTime PhysicalPrizeEndDate
        {
            get { return myPhysicalPrizeEndDate; }
            set { myPhysicalPrizeEndDate = value; }
        }
        public int ReviewsMin
        {
            get { return myReviewsMin; }
            set { myReviewsMin = value; }
        }
        public int ReviewsMax
        {
            get { return myReviewsMax; }
            set { myReviewsMax = value; }
        }
        public string ReviewTitle
        {
            get { return myReviewTitle; }
            set { myReviewTitle = value; }
        }
        public string ReviewAuthor
        {
            get { return myReviewAuthor; }
            set { myReviewAuthor = value; }
        }
        public DateTime ReviewStartDate
        {
            get { return myReviewStartDate; }
            set { myReviewStartDate = value; }
        }
        public DateTime ReviewEndDate
        {
            get { return myReviewEndDate; }
            set { myReviewEndDate = value; }
        }
        public string RandomDrawingName
        {
            get { return myRandomDrawingName; }
            set { myRandomDrawingName = value; }
        }
        public int RandomDrawingNum
        {
            get { return myRandomDrawingNum; }
            set { myRandomDrawingNum = value; }
        }
        public DateTime RandomDrawingStartDate
        {
            get { return myRandomDrawingStartDate; }
            set { myRandomDrawingStartDate = value; }
        }
        public DateTime RandomDrawingEndDate
        {
            get { return myRandomDrawingEndDate; }
            set { myRandomDrawingEndDate = value; }
        }
        public bool HasBeenDrawn
        {
            get { return myHasBeenDrawn; }
            set { myHasBeenDrawn = value; }
        }
        public bool HasRedeemend
        {
            get { return myHasRedeemend; }
            set { myHasRedeemend = value; }
        }
        public bool PIDInc
        {
            get { return myPIDInc; }
            set { myPIDInc = value; }
        }
        public bool UsernameInc
        {
            get { return myUsernameInc; }
            set { myUsernameInc = value; }
        }
        public bool DOBInc
        {
            get { return myDOBInc; }
            set { myDOBInc = value; }
        }
        public bool AgeInc
        {
            get { return myAgeInc; }
            set { myAgeInc = value; }
        }
        public bool SchoolGradeInc
        {
            get { return mySchoolGradeInc; }
            set { mySchoolGradeInc = value; }
        }
        public bool FirstNameInc
        {
            get { return myFirstNameInc; }
            set { myFirstNameInc = value; }
        }
        public bool LastNameInc
        {
            get { return myLastNameInc; }
            set { myLastNameInc = value; }
        }
        public bool GenderInc
        {
            get { return myGenderInc; }
            set { myGenderInc = value; }
        }
        public bool EmailAddressInc
        {
            get { return myEmailAddressInc; }
            set { myEmailAddressInc = value; }
        }
        public bool PhoneNumberInc
        {
            get { return myPhoneNumberInc; }
            set { myPhoneNumberInc = value; }
        }
        public bool CityInc
        {
            get { return myCityInc; }
            set { myCityInc = value; }
        }
        public bool StateInc
        {
            get { return myStateInc; }
            set { myStateInc = value; }
        }
        public bool ZipCodeInc
        {
            get { return myZipCodeInc; }
            set { myZipCodeInc = value; }
        }
        public bool CountyInc
        {
            get { return myCountyInc; }
            set { myCountyInc = value; }
        }
        public bool PrimaryLibraryInc
        {
            get { return myPrimaryLibraryInc; }
            set { myPrimaryLibraryInc = value; }
        }
        public bool SchoolNameInc
        {
            get { return mySchoolNameInc; }
            set { mySchoolNameInc = value; }
        }
        public bool DistrictInc
        {
            get { return myDistrictInc; }
            set { myDistrictInc = value; }
        }
        public bool TeacherInc
        {
            get { return myTeacherInc; }
            set { myTeacherInc = value; }
        }
        public bool GroupTeamNameInc
        {
            get { return myGroupTeamNameInc; }
            set { myGroupTeamNameInc = value; }
        }
        public bool SchoolTypeInc
        {
            get { return mySchoolTypeInc; }
            set { mySchoolTypeInc = value; }
        }
        public bool LiteracyLevel1Inc
        {
            get { return myLiteracyLevel1Inc; }
            set { myLiteracyLevel1Inc = value; }
        }
        public bool LiteracyLevel2Inc
        {
            get { return myLiteracyLevel2Inc; }
            set { myLiteracyLevel2Inc = value; }
        }
        public bool Custom1Inc
        {
            get { return myCustom1Inc; }
            set { myCustom1Inc = value; }
        }
        public bool Custom2Inc
        {
            get { return myCustom2Inc; }
            set { myCustom2Inc = value; }
        }
        public bool Custom3Inc
        {
            get { return myCustom3Inc; }
            set { myCustom3Inc = value; }
        }
        public bool Custom4Inc
        {
            get { return myCustom4Inc; }
            set { myCustom4Inc = value; }
        }
        public bool Custom5Inc
        {
            get { return myCustom5Inc; }
            set { myCustom5Inc = value; }
        }
        public bool RegistrationDateInc
        {
            get { return myRegistrationDateInc; }
            set { myRegistrationDateInc = value; }
        }
        public bool PointsInc
        {
            get { return myPointsInc; }
            set { myPointsInc = value; }
        }
        public bool EarnedBadgeInc
        {
            get { return myEarnedBadgeInc; }
            set { myEarnedBadgeInc = value; }
        }
        public bool PhysicalPrizeNameInc
        {
            get { return myPhysicalPrizeNameInc; }
            set { myPhysicalPrizeNameInc = value; }
        }
        public bool PhysicalPrizeDateInc
        {
            get { return myPhysicalPrizeDateInc; }
            set { myPhysicalPrizeDateInc = value; }
        }
        public bool NumReviewsInc
        {
            get { return myNumReviewsInc; }
            set { myNumReviewsInc = value; }
        }
        public bool ReviewAuthorInc
        {
            get { return myReviewAuthorInc; }
            set { myReviewAuthorInc = value; }
        }
        public bool ReviewTitleInc
        {
            get { return myReviewTitleInc; }
            set { myReviewTitleInc = value; }
        }
        public bool ReviewDateInc
        {
            get { return myReviewDateInc; }
            set { myReviewDateInc = value; }
        }
        public bool RandomDrawingNameInc
        {
            get { return myRandomDrawingNameInc; }
            set { myRandomDrawingNameInc = value; }
        }
        public bool RandomDrawingNumInc
        {
            get { return myRandomDrawingNumInc; }
            set { myRandomDrawingNumInc = value; }
        }
        public bool RandomDrawingDateInc
        {
            get { return myRandomDrawingDateInc; }
            set { myRandomDrawingDateInc = value; }
        }
        public bool HasBeenDrawnInc
        {
            get { return myHasBeenDrawnInc; }
            set { myHasBeenDrawnInc = value; }
        }
        public bool HasRedeemendInc
        {
            get { return myHasRedeemendInc; }
            set { myHasRedeemendInc = value; }
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


        public string SDistrict
        {
            get { return mySDistrict; }
            set { mySDistrict = value; }
        }

        public bool SDistrictInc
        {
            get { return mySDistrictInc; }
            set { mySDistrictInc = value; }
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

        public int Score1From
        {
            get { return myScore1From; }
            set { myScore1From = value; }
        }
        public int Score1To
        {
            get { return myScore1To; }
            set { myScore1To = value; }
        }
        public int Score1PctFrom
        {
            get { return myScore1PctFrom; }
            set { myScore1PctFrom = value; }
        }
        public int Score1PctTo
        {
            get { return myScore1PctTo; }
            set { myScore1PctTo = value; }
        }
        public int Score2From
        {
            get { return myScore2From; }
            set { myScore2From = value; }
        }
        public int Score2To
        {
            get { return myScore2To; }
            set { myScore2To = value; }
        }
        public int Score2PctFrom
        {
            get { return myScore2PctFrom; }
            set { myScore2PctFrom = value; }
        }
        public int Score2PctTo
        {
            get { return myScore2PctTo; }
            set { myScore2PctTo = value; }
        }

        public bool Score1Inc
        {
            get { return myScore1Inc; }
            set { myScore1Inc = value; }
        }
        public bool Score2Inc
        {
            get { return myScore2Inc; }
            set { myScore2Inc = value; }
        }

        public bool Score1PctInc
        {
            get { return myScore1PctInc; }
            set { myScore1PctInc = value; }
        }
        public bool Score2PctInc
        {
            get { return myScore2PctInc; }
            set { myScore2PctInc = value; }
        }

        #endregion

        #region Constructors

        public SRPReport()
        {
            TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);
        }

        #endregion

        #region stored procedure wrappers

        public static DataSet GetAll()
        {
            var arrParams = new SqlParameter[1];
            arrParams[0] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_SRPReport_GetAll", arrParams);
        }

        public static SRPReport FetchObject(int RID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@RID", RID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SRPReport_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SRPReport result = new SRPReport();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["RID"].ToString(), out _int)) result.RID = _int;
                if (int.TryParse(dr["RTID"].ToString(), out _int)) result.RTID = _int;
                if (int.TryParse(dr["ProgId"].ToString(), out _int)) result.ProgId = _int;
                result.ReportName = dr["ReportName"].ToString();
                result.DisplayFilters = bool.Parse(dr["DisplayFilters"].ToString());
                if (int.TryParse(dr["ReportFormat"].ToString(), out _int)) result.ReportFormat = _int;
                if (DateTime.TryParse(dr["DOBFrom"].ToString(), out _datetime)) result.DOBFrom = _datetime;
                if (DateTime.TryParse(dr["DOBTo"].ToString(), out _datetime)) result.DOBTo = _datetime;
                if (int.TryParse(dr["AgeFrom"].ToString(), out _int)) result.AgeFrom = _int;
                if (int.TryParse(dr["AgeTo"].ToString(), out _int)) result.AgeTo = _int;
                result.SchoolGrade = dr["SchoolGrade"].ToString();
                result.FirstName = dr["FirstName"].ToString();
                result.LastName = dr["LastName"].ToString();
                result.Gender = dr["Gender"].ToString();
                result.EmailAddress = dr["EmailAddress"].ToString();
                result.PhoneNumber = dr["PhoneNumber"].ToString();
                result.City = dr["City"].ToString();
                result.State = dr["State"].ToString();
                result.ZipCode = dr["ZipCode"].ToString();
                result.County = dr["County"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) result.PrimaryLibrary = _int;
                result.SchoolName = dr["SchoolName"].ToString();
                result.District = dr["District"].ToString();
                result.SDistrict = dr["SDistrict"].ToString();
                result.Teacher = dr["Teacher"].ToString();
                result.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int)) result.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) result.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) result.LiteracyLevel2 = _int;
                result.Custom1 = dr["Custom1"].ToString();
                result.Custom2 = dr["Custom2"].ToString();
                result.Custom3 = dr["Custom3"].ToString();
                result.Custom4 = dr["Custom4"].ToString();
                result.Custom5 = dr["Custom5"].ToString();
                if (DateTime.TryParse(dr["RegistrationDateStart"].ToString(), out _datetime)) result.RegistrationDateStart = _datetime;
                if (DateTime.TryParse(dr["RegistrationDateEnd"].ToString(), out _datetime)) result.RegistrationDateEnd = _datetime;
                if (int.TryParse(dr["PointsMin"].ToString(), out _int)) result.PointsMin = _int;
                if (int.TryParse(dr["PointsMax"].ToString(), out _int)) result.PointsMax = _int;
                if (DateTime.TryParse(dr["PointsStart"].ToString(), out _datetime)) result.PointsStart = _datetime;
                if (DateTime.TryParse(dr["PointsEnd"].ToString(), out _datetime)) result.PointsEnd = _datetime;
                result.EventCode = dr["EventCode"].ToString();
                if (int.TryParse(dr["EarnedBadge"].ToString(), out _int)) result.EarnedBadge = _int;
                result.PhysicalPrizeEarned = dr["PhysicalPrizeEarned"].ToString();
                result.PhysicalPrizeRedeemed = bool.Parse(dr["PhysicalPrizeRedeemed"].ToString());
                if (DateTime.TryParse(dr["PhysicalPrizeStartDate"].ToString(), out _datetime)) result.PhysicalPrizeStartDate = _datetime;
                if (DateTime.TryParse(dr["PhysicalPrizeEndDate"].ToString(), out _datetime)) result.PhysicalPrizeEndDate = _datetime;
                if (int.TryParse(dr["ReviewsMin"].ToString(), out _int)) result.ReviewsMin = _int;
                if (int.TryParse(dr["ReviewsMax"].ToString(), out _int)) result.ReviewsMax = _int;
                result.ReviewTitle = dr["ReviewTitle"].ToString();
                result.ReviewAuthor = dr["ReviewAuthor"].ToString();
                if (DateTime.TryParse(dr["ReviewStartDate"].ToString(), out _datetime)) result.ReviewStartDate = _datetime;
                if (DateTime.TryParse(dr["ReviewEndDate"].ToString(), out _datetime)) result.ReviewEndDate = _datetime;
                result.RandomDrawingName = dr["RandomDrawingName"].ToString();
                if (int.TryParse(dr["RandomDrawingNum"].ToString(), out _int)) result.RandomDrawingNum = _int;
                if (DateTime.TryParse(dr["RandomDrawingStartDate"].ToString(), out _datetime)) result.RandomDrawingStartDate = _datetime;
                if (DateTime.TryParse(dr["RandomDrawingEndDate"].ToString(), out _datetime)) result.RandomDrawingEndDate = _datetime;
                result.HasBeenDrawn = bool.Parse(dr["HasBeenDrawn"].ToString());
                result.HasRedeemend = bool.Parse(dr["HasRedeemend"].ToString());
                result.PIDInc = bool.Parse(dr["PIDInc"].ToString());
                result.UsernameInc = bool.Parse(dr["UsernameInc"].ToString());
                result.DOBInc = bool.Parse(dr["DOBInc"].ToString());
                result.AgeInc = bool.Parse(dr["AgeInc"].ToString());
                result.SchoolGradeInc = bool.Parse(dr["SchoolGradeInc"].ToString());
                result.FirstNameInc = bool.Parse(dr["FirstNameInc"].ToString());
                result.LastNameInc = bool.Parse(dr["LastNameInc"].ToString());
                result.GenderInc = bool.Parse(dr["GenderInc"].ToString());
                result.EmailAddressInc = bool.Parse(dr["EmailAddressInc"].ToString());
                result.PhoneNumberInc = bool.Parse(dr["PhoneNumberInc"].ToString());
                result.CityInc = bool.Parse(dr["CityInc"].ToString());
                result.StateInc = bool.Parse(dr["StateInc"].ToString());
                result.ZipCodeInc = bool.Parse(dr["ZipCodeInc"].ToString());
                result.CountyInc = bool.Parse(dr["CountyInc"].ToString());
                result.PrimaryLibraryInc = bool.Parse(dr["PrimaryLibraryInc"].ToString());
                result.SchoolNameInc = bool.Parse(dr["SchoolNameInc"].ToString());
                result.DistrictInc = bool.Parse(dr["DistrictInc"].ToString());
                result.SDistrictInc = bool.Parse(dr["SDistrictInc"].ToString());
                result.TeacherInc = bool.Parse(dr["TeacherInc"].ToString());
                result.GroupTeamNameInc = bool.Parse(dr["GroupTeamNameInc"].ToString());
                result.SchoolTypeInc = bool.Parse(dr["SchoolTypeInc"].ToString());
                result.LiteracyLevel1Inc = bool.Parse(dr["LiteracyLevel1Inc"].ToString());
                result.LiteracyLevel2Inc = bool.Parse(dr["LiteracyLevel2Inc"].ToString());
                result.Custom1Inc = bool.Parse(dr["Custom1Inc"].ToString());
                result.Custom2Inc = bool.Parse(dr["Custom2Inc"].ToString());
                result.Custom3Inc = bool.Parse(dr["Custom3Inc"].ToString());
                result.Custom4Inc = bool.Parse(dr["Custom4Inc"].ToString());
                result.Custom5Inc = bool.Parse(dr["Custom5Inc"].ToString());
                result.RegistrationDateInc = bool.Parse(dr["RegistrationDateInc"].ToString());
                result.PointsInc = bool.Parse(dr["PointsInc"].ToString());
                result.EarnedBadgeInc = bool.Parse(dr["EarnedBadgeInc"].ToString());
                result.PhysicalPrizeNameInc = bool.Parse(dr["PhysicalPrizeNameInc"].ToString());
                result.PhysicalPrizeDateInc = bool.Parse(dr["PhysicalPrizeDateInc"].ToString());
                result.NumReviewsInc = bool.Parse(dr["NumReviewsInc"].ToString());
                result.ReviewAuthorInc = bool.Parse(dr["ReviewAuthorInc"].ToString());
                result.ReviewTitleInc = bool.Parse(dr["ReviewTitleInc"].ToString());
                result.ReviewDateInc = bool.Parse(dr["ReviewDateInc"].ToString());
                result.RandomDrawingNameInc = bool.Parse(dr["RandomDrawingNameInc"].ToString());
                result.RandomDrawingNumInc = bool.Parse(dr["RandomDrawingNumInc"].ToString());
                result.RandomDrawingDateInc = bool.Parse(dr["RandomDrawingDateInc"].ToString());
                result.HasBeenDrawnInc = bool.Parse(dr["HasBeenDrawnInc"].ToString());
                result.HasRedeemendInc = bool.Parse(dr["HasRedeemendInc"].ToString());
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

                if (int.TryParse(dr["Score1From"].ToString(), out _int)) result.Score1From = _int;
                if (int.TryParse(dr["Score1To"].ToString(), out _int)) result.Score1To = _int;
                if (int.TryParse(dr["Score1PctFrom"].ToString(), out _int)) result.Score1PctFrom = _int;
                if (int.TryParse(dr["Score1PctTo"].ToString(), out _int)) result.Score1PctTo = _int;
                if (int.TryParse(dr["Score2From"].ToString(), out _int)) result.Score2From = _int;
                if (int.TryParse(dr["Score2To"].ToString(), out _int)) result.Score2To = _int;
                if (int.TryParse(dr["Score2PctFrom"].ToString(), out _int)) result.Score2PctFrom = _int;
                if (int.TryParse(dr["Score2PctTo"].ToString(), out _int)) result.Score2PctTo = _int;

                result.Score1Inc = bool.Parse(dr["Score1Inc"].ToString());
                result.Score2Inc = bool.Parse(dr["Score2Inc"].ToString());
                result.Score1PctInc = bool.Parse(dr["Score1PctInc"].ToString());
                result.Score2PctInc = bool.Parse(dr["Score2PctInc"].ToString());

                dr.Close();

                return result;

            }

            dr.Close();

            return null;

        }

        public bool Fetch(int RID)
        {

            // declare reader

            SqlDataReader dr;

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@RID", RID);

            dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_SRPReport_GetByID", arrParams);

            if (dr.Read())
            {

                // declare return value

                SRPReport result = new SRPReport();

                DateTime _datetime;

                int _int;

                //decimal _decimal;

                if (int.TryParse(dr["RID"].ToString(), out _int)) this.RID = _int;
                if (int.TryParse(dr["RTID"].ToString(), out _int)) this.RTID = _int;
                if (int.TryParse(dr["ProgId"].ToString(), out _int)) this.ProgId = _int;
                this.ReportName = dr["ReportName"].ToString();
                this.DisplayFilters = bool.Parse(dr["DisplayFilters"].ToString());
                if (int.TryParse(dr["ReportFormat"].ToString(), out _int)) this.ReportFormat = _int;
                if (DateTime.TryParse(dr["DOBFrom"].ToString(), out _datetime)) this.DOBFrom = _datetime;
                if (DateTime.TryParse(dr["DOBTo"].ToString(), out _datetime)) this.DOBTo = _datetime;
                if (int.TryParse(dr["AgeFrom"].ToString(), out _int)) this.AgeFrom = _int;
                if (int.TryParse(dr["AgeTo"].ToString(), out _int)) this.AgeTo = _int;
                this.SchoolGrade = dr["SchoolGrade"].ToString();
                this.FirstName = dr["FirstName"].ToString();
                this.LastName = dr["LastName"].ToString();
                this.Gender = dr["Gender"].ToString();
                this.EmailAddress = dr["EmailAddress"].ToString();
                this.PhoneNumber = dr["PhoneNumber"].ToString();
                this.City = dr["City"].ToString();
                this.State = dr["State"].ToString();
                this.ZipCode = dr["ZipCode"].ToString();
                this.County = dr["County"].ToString();
                if (int.TryParse(dr["PrimaryLibrary"].ToString(), out _int)) this.PrimaryLibrary = _int;
                this.SchoolName = dr["SchoolName"].ToString();
                this.District = dr["District"].ToString();
                this.SDistrict = dr["SDistrict"].ToString();
                this.Teacher = dr["Teacher"].ToString();
                this.GroupTeamName = dr["GroupTeamName"].ToString();
                if (int.TryParse(dr["SchoolType"].ToString(), out _int)) this.SchoolType = _int;
                if (int.TryParse(dr["LiteracyLevel1"].ToString(), out _int)) this.LiteracyLevel1 = _int;
                if (int.TryParse(dr["LiteracyLevel2"].ToString(), out _int)) this.LiteracyLevel2 = _int;
                this.Custom1 = dr["Custom1"].ToString();
                this.Custom2 = dr["Custom2"].ToString();
                this.Custom3 = dr["Custom3"].ToString();
                this.Custom4 = dr["Custom4"].ToString();
                this.Custom5 = dr["Custom5"].ToString();
                if (DateTime.TryParse(dr["RegistrationDateStart"].ToString(), out _datetime)) this.RegistrationDateStart = _datetime;
                if (DateTime.TryParse(dr["RegistrationDateEnd"].ToString(), out _datetime)) this.RegistrationDateEnd = _datetime;
                if (int.TryParse(dr["PointsMin"].ToString(), out _int)) this.PointsMin = _int;
                if (int.TryParse(dr["PointsMax"].ToString(), out _int)) this.PointsMax = _int;
                if (DateTime.TryParse(dr["PointsStart"].ToString(), out _datetime)) this.PointsStart = _datetime;
                if (DateTime.TryParse(dr["PointsEnd"].ToString(), out _datetime)) this.PointsEnd = _datetime;
                this.EventCode = dr["EventCode"].ToString();
                if (int.TryParse(dr["EarnedBadge"].ToString(), out _int)) this.EarnedBadge = _int;
                this.PhysicalPrizeEarned = dr["PhysicalPrizeEarned"].ToString();
                this.PhysicalPrizeRedeemed = bool.Parse(dr["PhysicalPrizeRedeemed"].ToString());
                if (DateTime.TryParse(dr["PhysicalPrizeStartDate"].ToString(), out _datetime)) this.PhysicalPrizeStartDate = _datetime;
                if (DateTime.TryParse(dr["PhysicalPrizeEndDate"].ToString(), out _datetime)) this.PhysicalPrizeEndDate = _datetime;
                if (int.TryParse(dr["ReviewsMin"].ToString(), out _int)) this.ReviewsMin = _int;
                if (int.TryParse(dr["ReviewsMax"].ToString(), out _int)) this.ReviewsMax = _int;
                this.ReviewTitle = dr["ReviewTitle"].ToString();
                this.ReviewAuthor = dr["ReviewAuthor"].ToString();
                if (DateTime.TryParse(dr["ReviewStartDate"].ToString(), out _datetime)) this.ReviewStartDate = _datetime;
                if (DateTime.TryParse(dr["ReviewEndDate"].ToString(), out _datetime)) this.ReviewEndDate = _datetime;
                this.RandomDrawingName = dr["RandomDrawingName"].ToString();
                if (int.TryParse(dr["RandomDrawingNum"].ToString(), out _int)) this.RandomDrawingNum = _int;
                if (DateTime.TryParse(dr["RandomDrawingStartDate"].ToString(), out _datetime)) this.RandomDrawingStartDate = _datetime;
                if (DateTime.TryParse(dr["RandomDrawingEndDate"].ToString(), out _datetime)) this.RandomDrawingEndDate = _datetime;
                this.HasBeenDrawn = bool.Parse(dr["HasBeenDrawn"].ToString());
                this.HasRedeemend = bool.Parse(dr["HasRedeemend"].ToString());
                this.PIDInc = bool.Parse(dr["PIDInc"].ToString());
                this.UsernameInc = bool.Parse(dr["UsernameInc"].ToString());
                this.DOBInc = bool.Parse(dr["DOBInc"].ToString());
                this.AgeInc = bool.Parse(dr["AgeInc"].ToString());
                this.SchoolGradeInc = bool.Parse(dr["SchoolGradeInc"].ToString());
                this.FirstNameInc = bool.Parse(dr["FirstNameInc"].ToString());
                this.LastNameInc = bool.Parse(dr["LastNameInc"].ToString());
                this.GenderInc = bool.Parse(dr["GenderInc"].ToString());
                this.EmailAddressInc = bool.Parse(dr["EmailAddressInc"].ToString());
                this.PhoneNumberInc = bool.Parse(dr["PhoneNumberInc"].ToString());
                this.CityInc = bool.Parse(dr["CityInc"].ToString());
                this.StateInc = bool.Parse(dr["StateInc"].ToString());
                this.ZipCodeInc = bool.Parse(dr["ZipCodeInc"].ToString());
                this.CountyInc = bool.Parse(dr["CountyInc"].ToString());
                this.PrimaryLibraryInc = bool.Parse(dr["PrimaryLibraryInc"].ToString());
                this.SchoolNameInc = bool.Parse(dr["SchoolNameInc"].ToString());
                this.DistrictInc = bool.Parse(dr["DistrictInc"].ToString());
                this.SDistrictInc = bool.Parse(dr["SDistrictInc"].ToString());
                this.TeacherInc = bool.Parse(dr["TeacherInc"].ToString());
                this.GroupTeamNameInc = bool.Parse(dr["GroupTeamNameInc"].ToString());
                this.SchoolTypeInc = bool.Parse(dr["SchoolTypeInc"].ToString());
                this.LiteracyLevel1Inc = bool.Parse(dr["LiteracyLevel1Inc"].ToString());
                this.LiteracyLevel2Inc = bool.Parse(dr["LiteracyLevel2Inc"].ToString());
                this.Custom1Inc = bool.Parse(dr["Custom1Inc"].ToString());
                this.Custom2Inc = bool.Parse(dr["Custom2Inc"].ToString());
                this.Custom3Inc = bool.Parse(dr["Custom3Inc"].ToString());
                this.Custom4Inc = bool.Parse(dr["Custom4Inc"].ToString());
                this.Custom5Inc = bool.Parse(dr["Custom5Inc"].ToString());
                this.RegistrationDateInc = bool.Parse(dr["RegistrationDateInc"].ToString());
                this.PointsInc = bool.Parse(dr["PointsInc"].ToString());
                this.EarnedBadgeInc = bool.Parse(dr["EarnedBadgeInc"].ToString());
                this.PhysicalPrizeNameInc = bool.Parse(dr["PhysicalPrizeNameInc"].ToString());
                this.PhysicalPrizeDateInc = bool.Parse(dr["PhysicalPrizeDateInc"].ToString());
                this.NumReviewsInc = bool.Parse(dr["NumReviewsInc"].ToString());
                this.ReviewAuthorInc = bool.Parse(dr["ReviewAuthorInc"].ToString());
                this.ReviewTitleInc = bool.Parse(dr["ReviewTitleInc"].ToString());
                this.ReviewDateInc = bool.Parse(dr["ReviewDateInc"].ToString());
                this.RandomDrawingNameInc = bool.Parse(dr["RandomDrawingNameInc"].ToString());
                this.RandomDrawingNumInc = bool.Parse(dr["RandomDrawingNumInc"].ToString());
                this.RandomDrawingDateInc = bool.Parse(dr["RandomDrawingDateInc"].ToString());
                this.HasBeenDrawnInc = bool.Parse(dr["HasBeenDrawnInc"].ToString());
                this.HasRedeemendInc = bool.Parse(dr["HasRedeemendInc"].ToString());
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


                if (int.TryParse(dr["Score1From"].ToString(), out _int)) this.Score1From = _int;
                if (int.TryParse(dr["Score1To"].ToString(), out _int)) this.Score1To = _int;
                if (int.TryParse(dr["Score1PctFrom"].ToString(), out _int)) this.Score1PctFrom = _int;
                if (int.TryParse(dr["Score1PctTo"].ToString(), out _int)) this.Score1PctTo = _int;
                if (int.TryParse(dr["Score2From"].ToString(), out _int)) this.Score2From = _int;
                if (int.TryParse(dr["Score2To"].ToString(), out _int)) this.Score2To = _int;
                if (int.TryParse(dr["Score2PctFrom"].ToString(), out _int)) this.Score2PctFrom = _int;
                if (int.TryParse(dr["Score2PctTo"].ToString(), out _int)) this.Score2PctTo = _int;

                this.Score1Inc = bool.Parse(dr["Score1Inc"].ToString());
                this.Score2Inc = bool.Parse(dr["Score2Inc"].ToString());
                this.Score1PctInc = bool.Parse(dr["Score1PctInc"].ToString());
                this.Score2PctInc = bool.Parse(dr["Score2PctInc"].ToString());

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

        public static int Insert(SRPReport o)
        {

            SqlParameter[] arrParams = new SqlParameter[126];

            arrParams[0] = new SqlParameter("@RTID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RTID, o.RTID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@ProgId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgId, o.ProgId.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ReportName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReportName, o.ReportName.GetTypeCode()));
            arrParams[3] = new SqlParameter("@DisplayFilters", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DisplayFilters, o.DisplayFilters.GetTypeCode()));
            arrParams[4] = new SqlParameter("@ReportFormat", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReportFormat, o.ReportFormat.GetTypeCode()));
            arrParams[5] = new SqlParameter("@DOBFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOBFrom, o.DOBFrom.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DOBTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOBTo, o.DOBTo.GetTypeCode()));
            arrParams[7] = new SqlParameter("@AgeFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeFrom, o.AgeFrom.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AgeTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeTo, o.AgeTo.GetTypeCode()));
            arrParams[9] = new SqlParameter("@SchoolGrade", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade, o.SchoolGrade.GetTypeCode()));
            arrParams[10] = new SqlParameter("@FirstName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName, o.FirstName.GetTypeCode()));
            arrParams[11] = new SqlParameter("@LastName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName, o.LastName.GetTypeCode()));
            arrParams[12] = new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[13] = new SqlParameter("@EmailAddress", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress, o.EmailAddress.GetTypeCode()));
            arrParams[14] = new SqlParameter("@PhoneNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber, o.PhoneNumber.GetTypeCode()));
            arrParams[15] = new SqlParameter("@City", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode()));
            arrParams[16] = new SqlParameter("@State", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State, o.State.GetTypeCode()));
            arrParams[17] = new SqlParameter("@ZipCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode, o.ZipCode.GetTypeCode()));
            arrParams[18] = new SqlParameter("@County", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County, o.County.GetTypeCode()));
            arrParams[19] = new SqlParameter("@PrimaryLibrary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary, o.PrimaryLibrary.GetTypeCode()));
            arrParams[20] = new SqlParameter("@SchoolName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[21] = new SqlParameter("@District", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District, o.District.GetTypeCode()));
            arrParams[22] = new SqlParameter("@Teacher", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher, o.Teacher.GetTypeCode()));
            arrParams[23] = new SqlParameter("@GroupTeamName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName, o.GroupTeamName.GetTypeCode()));
            arrParams[24] = new SqlParameter("@SchoolType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType, o.SchoolType.GetTypeCode()));
            arrParams[25] = new SqlParameter("@LiteracyLevel1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode()));
            arrParams[26] = new SqlParameter("@LiteracyLevel2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode()));
            arrParams[27] = new SqlParameter("@Custom1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode()));
            arrParams[28] = new SqlParameter("@Custom2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode()));
            arrParams[29] = new SqlParameter("@Custom3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode()));
            arrParams[30] = new SqlParameter("@Custom4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4, o.Custom4.GetTypeCode()));
            arrParams[31] = new SqlParameter("@Custom5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5, o.Custom5.GetTypeCode()));
            arrParams[32] = new SqlParameter("@RegistrationDateStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationDateStart, o.RegistrationDateStart.GetTypeCode()));
            arrParams[33] = new SqlParameter("@RegistrationDateEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationDateEnd, o.RegistrationDateEnd.GetTypeCode()));
            arrParams[34] = new SqlParameter("@PointsMin", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsMin, o.PointsMin.GetTypeCode()));
            arrParams[35] = new SqlParameter("@PointsMax", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsMax, o.PointsMax.GetTypeCode()));
            arrParams[36] = new SqlParameter("@PointsStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsStart, o.PointsStart.GetTypeCode()));
            arrParams[37] = new SqlParameter("@PointsEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsEnd, o.PointsEnd.GetTypeCode()));
            arrParams[38] = new SqlParameter("@EventCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventCode, o.EventCode.GetTypeCode()));
            arrParams[39] = new SqlParameter("@EarnedBadge", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EarnedBadge, o.EarnedBadge.GetTypeCode()));
            arrParams[40] = new SqlParameter("@PhysicalPrizeEarned", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeEarned, o.PhysicalPrizeEarned.GetTypeCode()));
            arrParams[41] = new SqlParameter("@PhysicalPrizeRedeemed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeRedeemed, o.PhysicalPrizeRedeemed.GetTypeCode()));
            arrParams[42] = new SqlParameter("@PhysicalPrizeStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeStartDate, o.PhysicalPrizeStartDate.GetTypeCode()));
            arrParams[43] = new SqlParameter("@PhysicalPrizeEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeEndDate, o.PhysicalPrizeEndDate.GetTypeCode()));
            arrParams[44] = new SqlParameter("@ReviewsMin", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewsMin, o.ReviewsMin.GetTypeCode()));
            arrParams[45] = new SqlParameter("@ReviewsMax", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewsMax, o.ReviewsMax.GetTypeCode()));
            arrParams[46] = new SqlParameter("@ReviewTitle", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewTitle, o.ReviewTitle.GetTypeCode()));
            arrParams[47] = new SqlParameter("@ReviewAuthor", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewAuthor, o.ReviewAuthor.GetTypeCode()));
            arrParams[48] = new SqlParameter("@ReviewStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewStartDate, o.ReviewStartDate.GetTypeCode()));
            arrParams[49] = new SqlParameter("@ReviewEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewEndDate, o.ReviewEndDate.GetTypeCode()));
            arrParams[50] = new SqlParameter("@RandomDrawingName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingName, o.RandomDrawingName.GetTypeCode()));
            arrParams[51] = new SqlParameter("@RandomDrawingNum", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingNum, o.RandomDrawingNum.GetTypeCode()));
            arrParams[52] = new SqlParameter("@RandomDrawingStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingStartDate, o.RandomDrawingStartDate.GetTypeCode()));
            arrParams[53] = new SqlParameter("@RandomDrawingEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingEndDate, o.RandomDrawingEndDate.GetTypeCode()));
            arrParams[54] = new SqlParameter("@HasBeenDrawn", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasBeenDrawn, o.HasBeenDrawn.GetTypeCode()));
            arrParams[55] = new SqlParameter("@HasRedeemend", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasRedeemend, o.HasRedeemend.GetTypeCode()));
            arrParams[56] = new SqlParameter("@PIDInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PIDInc, o.PIDInc.GetTypeCode()));
            arrParams[57] = new SqlParameter("@UsernameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.UsernameInc, o.UsernameInc.GetTypeCode()));
            arrParams[58] = new SqlParameter("@DOBInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOBInc, o.DOBInc.GetTypeCode()));
            arrParams[59] = new SqlParameter("@AgeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeInc, o.AgeInc.GetTypeCode()));
            arrParams[60] = new SqlParameter("@SchoolGradeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGradeInc, o.SchoolGradeInc.GetTypeCode()));
            arrParams[61] = new SqlParameter("@FirstNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstNameInc, o.FirstNameInc.GetTypeCode()));
            arrParams[62] = new SqlParameter("@LastNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastNameInc, o.LastNameInc.GetTypeCode()));
            arrParams[63] = new SqlParameter("@GenderInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GenderInc, o.GenderInc.GetTypeCode()));
            arrParams[64] = new SqlParameter("@EmailAddressInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddressInc, o.EmailAddressInc.GetTypeCode()));
            arrParams[65] = new SqlParameter("@PhoneNumberInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumberInc, o.PhoneNumberInc.GetTypeCode()));
            arrParams[66] = new SqlParameter("@CityInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CityInc, o.CityInc.GetTypeCode()));
            arrParams[67] = new SqlParameter("@StateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StateInc, o.StateInc.GetTypeCode()));
            arrParams[68] = new SqlParameter("@ZipCodeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCodeInc, o.ZipCodeInc.GetTypeCode()));
            arrParams[69] = new SqlParameter("@CountyInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CountyInc, o.CountyInc.GetTypeCode()));
            arrParams[70] = new SqlParameter("@PrimaryLibraryInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibraryInc, o.PrimaryLibraryInc.GetTypeCode()));
            arrParams[71] = new SqlParameter("@SchoolNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolNameInc, o.SchoolNameInc.GetTypeCode()));
            arrParams[72] = new SqlParameter("@DistrictInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DistrictInc, o.DistrictInc.GetTypeCode()));
            arrParams[73] = new SqlParameter("@TeacherInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TeacherInc, o.TeacherInc.GetTypeCode()));
            arrParams[74] = new SqlParameter("@GroupTeamNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamNameInc, o.GroupTeamNameInc.GetTypeCode()));
            arrParams[75] = new SqlParameter("@SchoolTypeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolTypeInc, o.SchoolTypeInc.GetTypeCode()));
            arrParams[76] = new SqlParameter("@LiteracyLevel1Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1Inc, o.LiteracyLevel1Inc.GetTypeCode()));
            arrParams[77] = new SqlParameter("@LiteracyLevel2Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2Inc, o.LiteracyLevel2Inc.GetTypeCode()));
            arrParams[78] = new SqlParameter("@Custom1Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1Inc, o.Custom1Inc.GetTypeCode()));
            arrParams[79] = new SqlParameter("@Custom2Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2Inc, o.Custom2Inc.GetTypeCode()));
            arrParams[80] = new SqlParameter("@Custom3Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3Inc, o.Custom3Inc.GetTypeCode()));
            arrParams[81] = new SqlParameter("@Custom4Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4Inc, o.Custom4Inc.GetTypeCode()));
            arrParams[82] = new SqlParameter("@Custom5Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5Inc, o.Custom5Inc.GetTypeCode()));
            arrParams[83] = new SqlParameter("@RegistrationDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationDateInc, o.RegistrationDateInc.GetTypeCode()));
            arrParams[84] = new SqlParameter("@PointsInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsInc, o.PointsInc.GetTypeCode()));
            arrParams[85] = new SqlParameter("@EarnedBadgeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EarnedBadgeInc, o.EarnedBadgeInc.GetTypeCode()));
            arrParams[86] = new SqlParameter("@PhysicalPrizeNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeNameInc, o.PhysicalPrizeNameInc.GetTypeCode()));
            arrParams[87] = new SqlParameter("@PhysicalPrizeDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeDateInc, o.PhysicalPrizeDateInc.GetTypeCode()));
            arrParams[88] = new SqlParameter("@NumReviewsInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumReviewsInc, o.NumReviewsInc.GetTypeCode()));
            arrParams[89] = new SqlParameter("@ReviewAuthorInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewAuthorInc, o.ReviewAuthorInc.GetTypeCode()));
            arrParams[90] = new SqlParameter("@ReviewTitleInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewTitleInc, o.ReviewTitleInc.GetTypeCode()));
            arrParams[91] = new SqlParameter("@ReviewDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDateInc, o.ReviewDateInc.GetTypeCode()));
            arrParams[92] = new SqlParameter("@RandomDrawingNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingNameInc, o.RandomDrawingNameInc.GetTypeCode()));
            arrParams[93] = new SqlParameter("@RandomDrawingNumInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingNumInc, o.RandomDrawingNumInc.GetTypeCode()));
            arrParams[94] = new SqlParameter("@RandomDrawingDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingDateInc, o.RandomDrawingDateInc.GetTypeCode()));
            arrParams[95] = new SqlParameter("@HasBeenDrawnInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasBeenDrawnInc, o.HasBeenDrawnInc.GetTypeCode()));
            arrParams[96] = new SqlParameter("@HasRedeemendInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasRedeemendInc, o.HasRedeemendInc.GetTypeCode()));
            arrParams[97] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[98] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[99] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[100] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[101] = new SqlParameter("@SDistrict", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict, o.SDistrict.GetTypeCode()));
            arrParams[102] = new SqlParameter("@SDistrictInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrictInc, o.SDistrictInc.GetTypeCode()));

            arrParams[103] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[104] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[105] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[106] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[107] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[108] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[109] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[110] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[111] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[112] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[113] = new SqlParameter("@Score1From", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1From, o.Score1From.GetTypeCode()));
            arrParams[114] = new SqlParameter("@Score1To", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1To, o.Score1To.GetTypeCode()));
            arrParams[115] = new SqlParameter("@Score1PctFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1PctFrom, o.Score1PctFrom.GetTypeCode()));
            arrParams[116] = new SqlParameter("@Score1PctTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1PctTo, o.Score1PctTo.GetTypeCode()));
            arrParams[117] = new SqlParameter("@Score2From", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2From, o.Score2From.GetTypeCode()));
            arrParams[118] = new SqlParameter("@Score2To", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2To, o.Score2To.GetTypeCode()));
            arrParams[119] = new SqlParameter("@Score2PctFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2PctFrom, o.Score2PctFrom.GetTypeCode()));
            arrParams[120] = new SqlParameter("@Score2PctTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2PctTo, o.Score2PctTo.GetTypeCode()));

            arrParams[121] = new SqlParameter("@Score1Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1Inc, o.Score1Inc.GetTypeCode()));
            arrParams[122] = new SqlParameter("@Score2Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2Inc, o.Score2Inc.GetTypeCode()));
            arrParams[123] = new SqlParameter("@Score1PctInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1PctInc, o.Score1PctInc.GetTypeCode()));
            arrParams[124] = new SqlParameter("@Score2PctInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2PctInc, o.Score2PctInc.GetTypeCode()));

            arrParams[125] = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));
            arrParams[125].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SRPReport_Insert", arrParams);

            o.RID = int.Parse(arrParams[125].Value.ToString());

            return o.RID;

        }

        public int Update()
        {

            return Update(this);

        }

        public static int Update(SRPReport o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[126];

            arrParams[0] = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));
            arrParams[1] = new SqlParameter("@RTID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RTID, o.RTID.GetTypeCode()));
            arrParams[2] = new SqlParameter("@ProgId", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ProgId, o.ProgId.GetTypeCode()));
            arrParams[3] = new SqlParameter("@ReportName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReportName, o.ReportName.GetTypeCode()));
            arrParams[4] = new SqlParameter("@DisplayFilters", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DisplayFilters, o.DisplayFilters.GetTypeCode()));
            arrParams[5] = new SqlParameter("@ReportFormat", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReportFormat, o.ReportFormat.GetTypeCode()));
            arrParams[6] = new SqlParameter("@DOBFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOBFrom, o.DOBFrom.GetTypeCode()));
            arrParams[7] = new SqlParameter("@DOBTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOBTo, o.DOBTo.GetTypeCode()));
            arrParams[8] = new SqlParameter("@AgeFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeFrom, o.AgeFrom.GetTypeCode()));
            arrParams[9] = new SqlParameter("@AgeTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeTo, o.AgeTo.GetTypeCode()));
            arrParams[10] = new SqlParameter("@SchoolGrade", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGrade, o.SchoolGrade.GetTypeCode()));
            arrParams[11] = new SqlParameter("@FirstName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstName, o.FirstName.GetTypeCode()));
            arrParams[12] = new SqlParameter("@LastName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastName, o.LastName.GetTypeCode()));
            arrParams[13] = new SqlParameter("@Gender", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Gender, o.Gender.GetTypeCode()));
            arrParams[14] = new SqlParameter("@EmailAddress", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddress, o.EmailAddress.GetTypeCode()));
            arrParams[15] = new SqlParameter("@PhoneNumber", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumber, o.PhoneNumber.GetTypeCode()));
            arrParams[16] = new SqlParameter("@City", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.City, o.City.GetTypeCode()));
            arrParams[17] = new SqlParameter("@State", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.State, o.State.GetTypeCode()));
            arrParams[18] = new SqlParameter("@ZipCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCode, o.ZipCode.GetTypeCode()));
            arrParams[19] = new SqlParameter("@County", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.County, o.County.GetTypeCode()));
            arrParams[20] = new SqlParameter("@PrimaryLibrary", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibrary, o.PrimaryLibrary.GetTypeCode()));
            arrParams[21] = new SqlParameter("@SchoolName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolName, o.SchoolName.GetTypeCode()));
            arrParams[22] = new SqlParameter("@District", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.District, o.District.GetTypeCode()));
            arrParams[23] = new SqlParameter("@Teacher", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Teacher, o.Teacher.GetTypeCode()));
            arrParams[24] = new SqlParameter("@GroupTeamName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamName, o.GroupTeamName.GetTypeCode()));
            arrParams[25] = new SqlParameter("@SchoolType", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolType, o.SchoolType.GetTypeCode()));
            arrParams[26] = new SqlParameter("@LiteracyLevel1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1, o.LiteracyLevel1.GetTypeCode()));
            arrParams[27] = new SqlParameter("@LiteracyLevel2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2, o.LiteracyLevel2.GetTypeCode()));
            arrParams[28] = new SqlParameter("@Custom1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1, o.Custom1.GetTypeCode()));
            arrParams[29] = new SqlParameter("@Custom2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2, o.Custom2.GetTypeCode()));
            arrParams[30] = new SqlParameter("@Custom3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3, o.Custom3.GetTypeCode()));
            arrParams[31] = new SqlParameter("@Custom4", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4, o.Custom4.GetTypeCode()));
            arrParams[32] = new SqlParameter("@Custom5", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5, o.Custom5.GetTypeCode()));
            arrParams[33] = new SqlParameter("@RegistrationDateStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationDateStart, o.RegistrationDateStart.GetTypeCode()));
            arrParams[34] = new SqlParameter("@RegistrationDateEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationDateEnd, o.RegistrationDateEnd.GetTypeCode()));
            arrParams[35] = new SqlParameter("@PointsMin", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsMin, o.PointsMin.GetTypeCode()));
            arrParams[36] = new SqlParameter("@PointsMax", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsMax, o.PointsMax.GetTypeCode()));
            arrParams[37] = new SqlParameter("@PointsStart", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsStart, o.PointsStart.GetTypeCode()));
            arrParams[38] = new SqlParameter("@PointsEnd", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsEnd, o.PointsEnd.GetTypeCode()));
            arrParams[39] = new SqlParameter("@EventCode", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EventCode, o.EventCode.GetTypeCode()));
            arrParams[40] = new SqlParameter("@EarnedBadge", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EarnedBadge, o.EarnedBadge.GetTypeCode()));
            arrParams[41] = new SqlParameter("@PhysicalPrizeEarned", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeEarned, o.PhysicalPrizeEarned.GetTypeCode()));
            arrParams[42] = new SqlParameter("@PhysicalPrizeRedeemed", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeRedeemed, o.PhysicalPrizeRedeemed.GetTypeCode()));
            arrParams[43] = new SqlParameter("@PhysicalPrizeStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeStartDate, o.PhysicalPrizeStartDate.GetTypeCode()));
            arrParams[44] = new SqlParameter("@PhysicalPrizeEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeEndDate, o.PhysicalPrizeEndDate.GetTypeCode()));
            arrParams[45] = new SqlParameter("@ReviewsMin", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewsMin, o.ReviewsMin.GetTypeCode()));
            arrParams[46] = new SqlParameter("@ReviewsMax", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewsMax, o.ReviewsMax.GetTypeCode()));
            arrParams[47] = new SqlParameter("@ReviewTitle", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewTitle, o.ReviewTitle.GetTypeCode()));
            arrParams[48] = new SqlParameter("@ReviewAuthor", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewAuthor, o.ReviewAuthor.GetTypeCode()));
            arrParams[49] = new SqlParameter("@ReviewStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewStartDate, o.ReviewStartDate.GetTypeCode()));
            arrParams[50] = new SqlParameter("@ReviewEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewEndDate, o.ReviewEndDate.GetTypeCode()));
            arrParams[51] = new SqlParameter("@RandomDrawingName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingName, o.RandomDrawingName.GetTypeCode()));
            arrParams[52] = new SqlParameter("@RandomDrawingNum", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingNum, o.RandomDrawingNum.GetTypeCode()));
            arrParams[53] = new SqlParameter("@RandomDrawingStartDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingStartDate, o.RandomDrawingStartDate.GetTypeCode()));
            arrParams[54] = new SqlParameter("@RandomDrawingEndDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingEndDate, o.RandomDrawingEndDate.GetTypeCode()));
            arrParams[55] = new SqlParameter("@HasBeenDrawn", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasBeenDrawn, o.HasBeenDrawn.GetTypeCode()));
            arrParams[56] = new SqlParameter("@HasRedeemend", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasRedeemend, o.HasRedeemend.GetTypeCode()));
            arrParams[57] = new SqlParameter("@PIDInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PIDInc, o.PIDInc.GetTypeCode()));
            arrParams[58] = new SqlParameter("@UsernameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.UsernameInc, o.UsernameInc.GetTypeCode()));
            arrParams[59] = new SqlParameter("@DOBInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DOBInc, o.DOBInc.GetTypeCode()));
            arrParams[60] = new SqlParameter("@AgeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AgeInc, o.AgeInc.GetTypeCode()));
            arrParams[61] = new SqlParameter("@SchoolGradeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolGradeInc, o.SchoolGradeInc.GetTypeCode()));
            arrParams[62] = new SqlParameter("@FirstNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FirstNameInc, o.FirstNameInc.GetTypeCode()));
            arrParams[63] = new SqlParameter("@LastNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastNameInc, o.LastNameInc.GetTypeCode()));
            arrParams[64] = new SqlParameter("@GenderInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GenderInc, o.GenderInc.GetTypeCode()));
            arrParams[65] = new SqlParameter("@EmailAddressInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EmailAddressInc, o.EmailAddressInc.GetTypeCode()));
            arrParams[66] = new SqlParameter("@PhoneNumberInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhoneNumberInc, o.PhoneNumberInc.GetTypeCode()));
            arrParams[67] = new SqlParameter("@CityInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CityInc, o.CityInc.GetTypeCode()));
            arrParams[68] = new SqlParameter("@StateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.StateInc, o.StateInc.GetTypeCode()));
            arrParams[69] = new SqlParameter("@ZipCodeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ZipCodeInc, o.ZipCodeInc.GetTypeCode()));
            arrParams[70] = new SqlParameter("@CountyInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.CountyInc, o.CountyInc.GetTypeCode()));
            arrParams[71] = new SqlParameter("@PrimaryLibraryInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PrimaryLibraryInc, o.PrimaryLibraryInc.GetTypeCode()));
            arrParams[72] = new SqlParameter("@SchoolNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolNameInc, o.SchoolNameInc.GetTypeCode()));
            arrParams[73] = new SqlParameter("@DistrictInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.DistrictInc, o.DistrictInc.GetTypeCode()));
            arrParams[74] = new SqlParameter("@TeacherInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TeacherInc, o.TeacherInc.GetTypeCode()));
            arrParams[75] = new SqlParameter("@GroupTeamNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GroupTeamNameInc, o.GroupTeamNameInc.GetTypeCode()));
            arrParams[76] = new SqlParameter("@SchoolTypeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SchoolTypeInc, o.SchoolTypeInc.GetTypeCode()));
            arrParams[77] = new SqlParameter("@LiteracyLevel1Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel1Inc, o.LiteracyLevel1Inc.GetTypeCode()));
            arrParams[78] = new SqlParameter("@LiteracyLevel2Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LiteracyLevel2Inc, o.LiteracyLevel2Inc.GetTypeCode()));
            arrParams[79] = new SqlParameter("@Custom1Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom1Inc, o.Custom1Inc.GetTypeCode()));
            arrParams[80] = new SqlParameter("@Custom2Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom2Inc, o.Custom2Inc.GetTypeCode()));
            arrParams[81] = new SqlParameter("@Custom3Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom3Inc, o.Custom3Inc.GetTypeCode()));
            arrParams[82] = new SqlParameter("@Custom4Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom4Inc, o.Custom4Inc.GetTypeCode()));
            arrParams[83] = new SqlParameter("@Custom5Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Custom5Inc, o.Custom5Inc.GetTypeCode()));
            arrParams[84] = new SqlParameter("@RegistrationDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RegistrationDateInc, o.RegistrationDateInc.GetTypeCode()));
            arrParams[85] = new SqlParameter("@PointsInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PointsInc, o.PointsInc.GetTypeCode()));
            arrParams[86] = new SqlParameter("@EarnedBadgeInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.EarnedBadgeInc, o.EarnedBadgeInc.GetTypeCode()));
            arrParams[87] = new SqlParameter("@PhysicalPrizeNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeNameInc, o.PhysicalPrizeNameInc.GetTypeCode()));
            arrParams[88] = new SqlParameter("@PhysicalPrizeDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PhysicalPrizeDateInc, o.PhysicalPrizeDateInc.GetTypeCode()));
            arrParams[89] = new SqlParameter("@NumReviewsInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.NumReviewsInc, o.NumReviewsInc.GetTypeCode()));
            arrParams[90] = new SqlParameter("@ReviewAuthorInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewAuthorInc, o.ReviewAuthorInc.GetTypeCode()));
            arrParams[91] = new SqlParameter("@ReviewTitleInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewTitleInc, o.ReviewTitleInc.GetTypeCode()));
            arrParams[92] = new SqlParameter("@ReviewDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.ReviewDateInc, o.ReviewDateInc.GetTypeCode()));
            arrParams[93] = new SqlParameter("@RandomDrawingNameInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingNameInc, o.RandomDrawingNameInc.GetTypeCode()));
            arrParams[94] = new SqlParameter("@RandomDrawingNumInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingNumInc, o.RandomDrawingNumInc.GetTypeCode()));
            arrParams[95] = new SqlParameter("@RandomDrawingDateInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RandomDrawingDateInc, o.RandomDrawingDateInc.GetTypeCode()));
            arrParams[96] = new SqlParameter("@HasBeenDrawnInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasBeenDrawnInc, o.HasBeenDrawnInc.GetTypeCode()));
            arrParams[97] = new SqlParameter("@HasRedeemendInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.HasRedeemendInc, o.HasRedeemendInc.GetTypeCode()));
            arrParams[98] = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));
            arrParams[99] = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
            arrParams[100] = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
            arrParams[101] = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
            arrParams[102] = new SqlParameter("@SDistrict", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrict, o.SDistrict.GetTypeCode()));
            arrParams[103] = new SqlParameter("@SDistrictInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.SDistrictInc, o.SDistrictInc.GetTypeCode()));

            arrParams[104] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
            arrParams[105] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
            arrParams[106] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
            arrParams[107] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
            arrParams[108] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
            arrParams[109] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
            arrParams[110] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
            arrParams[111] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
            arrParams[112] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
            arrParams[113] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

            arrParams[114] = new SqlParameter("@Score1From", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1From, o.Score1From.GetTypeCode()));
            arrParams[115] = new SqlParameter("@Score1To", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1To, o.Score1To.GetTypeCode()));
            arrParams[116] = new SqlParameter("@Score1PctFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1PctFrom, o.Score1PctFrom.GetTypeCode()));
            arrParams[117] = new SqlParameter("@Score1PctTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1PctTo, o.Score1PctTo.GetTypeCode()));
            arrParams[118] = new SqlParameter("@Score2From", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2From, o.Score2From.GetTypeCode()));
            arrParams[119] = new SqlParameter("@Score2To", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2To, o.Score2To.GetTypeCode()));
            arrParams[120] = new SqlParameter("@Score2PctFrom", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2PctFrom, o.Score2PctFrom.GetTypeCode()));
            arrParams[121] = new SqlParameter("@Score2PctTo", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2PctTo, o.Score2PctTo.GetTypeCode()));


            arrParams[122] = new SqlParameter("@Score1Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1Inc, o.Score1Inc.GetTypeCode()));
            arrParams[123] = new SqlParameter("@Score2Inc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2Inc, o.Score2Inc.GetTypeCode()));
            arrParams[124] = new SqlParameter("@Score1PctInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score1PctInc, o.Score1PctInc.GetTypeCode()));
            arrParams[125] = new SqlParameter("@Score2PctInc", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Score2PctInc, o.Score2PctInc.GetTypeCode()));
            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SRPReport_Update", arrParams);

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

        public static int Delete(SRPReport o)
        {

            int iReturn = -1; //assume the worst

            SqlParameter[] arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@RID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.RID, o.RID.GetTypeCode()));

            try
            {

                iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_SRPReport_Delete", arrParams);

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

