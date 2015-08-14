using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public class TestingBL
    {
        public static bool CheckPatronNeedsPreTest()
        {
            return PatronNeedsPreTest();
        }
        public static bool PatronNeedsPreTest()
        {
            var p = (Patron) HttpContext.Current.Session["Patron"];
            var pg = Programs.FetchObject(p.ProgID);

            if (pg.PreTestID != 0)
            {
                var resID = 0;
                if (PatronHasCompletedTest(p.PID, pg.PreTestID, 1, pg.PID, out resID))
                {
                    return false;
                }
                else
                {
                    if (pg.PreTestEndDate <= DateTime.Now)
                        return false;

                    var QNum = 0;
                    if (resID > 0)
                    {
                        var sr = SurveyResults.FetchObject(resID);
                        QNum = sr.LastAnswered;
                        // has started ... needs to continue ...
                    }
                    
                    HttpContext.Current.Session["PreTestMandatory"] = pg.PreTestMandatory;
                    HttpContext.Current.Session["SRID"] = resID;            // which results to continue
                    HttpContext.Current.Session["SID"] = pg.PreTestID;      // the test to restart 
                    HttpContext.Current.Session["QNum"] = QNum;  // question to restart from
                    HttpContext.Current.Session["SSrc"] = Survey.Source(1); // pre - testing
                    HttpContext.Current.Session["SSrcID"] = pg.PID;         // program id

                    HttpContext.Current.Response.Redirect("AddlSurvey.aspx");
                    
                }
            }
            return false;
        }

        public static bool CheckPatronNeedsPostTest()
        {
            return PatronNeedsPostTest();
        }
        public static bool PatronNeedsPostTest()
        {
            var p = (Patron)HttpContext.Current.Session["Patron"];
            var pg = Programs.FetchObject(p.ProgID);

            if (pg.PostTestID != 0)
            {
                var resID = 0;
                if (PatronHasCompletedTest(p.PID, pg.PostTestID, 2, pg.PID, out resID))
                {
                    return false;
                }
                else
                {
                    if (pg.PostTestStartDate > DateTime.Now)
                        return false;

                    var QNum = 0;
                    if (resID > 0)
                    {
                        var sr = SurveyResults.FetchObject(resID);
                        QNum = sr.LastAnswered;
                        // has started ... needs to continue ...
                    }
                    
                    HttpContext.Current.Session["SRID"] = resID;            // which results to continue
                    HttpContext.Current.Session["SID"] = pg.PostTestID;      // the test to restart 
                    HttpContext.Current.Session["QNum"] = QNum;  // question to restart from
                    HttpContext.Current.Session["SSrc"] = Survey.Source(2); // pre - testing
                    HttpContext.Current.Session["SSrcID"] = pg.PID;         // program id

                    HttpContext.Current.Response.Redirect("AddlSurvey.aspx");
                    
                }
            }
            return false;
        }

        public static bool PatronHasTakenTest(int PID, int SID, int TestType, int SrcID, out int SRID)
        {
            SRID = 0;
            var s = SurveyResults.FetchObject(PID, SID, Survey.Source(TestType), SrcID);
            if (s == null) return false;
            SRID = s.SRID;
            return true;
        }

        public static bool PatronHasCompletedTest(int PID, int SID, int TestType, int SrcID, out int SRID)
        {
            SRID = 0;
            var s = SurveyResults.FetchObject(PID, SID, Survey.Source(TestType), SrcID);
            if (s == null) return false;
            SRID = s.SRID;
            return s.IsComplete;
        }
    }
}