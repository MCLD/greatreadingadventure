using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class ClosingPush : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Program Close push -- BJ Hack");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var startPID = int.Parse(txtStart.Text);
            var numPoints = 500;
            var cnt1 = 0;
            var cnt2 = 0;
            var code = "final_push";


            //            var strSQL = string.Format("SELECT PID FROM Patron WHERE PID > {0} AND PrimaryLibrary NOT IN ({1}) ORDER BY PID ", startPID, "57,58,59,60,61,62");
            var strSQL = string.Format("SELECT PID FROM Patron WHERE PID > {0} AND PrimaryLibrary IN ({1}) ORDER BY PID ", startPID, "62");
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, strSQL);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cnt1++;
                cnt2++;
                int PID = Convert.ToInt32(row["PID"]);
                //int PGID = Convert.ToInt32(row["ProgID"]);

                var pa = new AwardPoints(PID);
                var sBadges= string.Empty;
                var points = numPoints;
                
                if (!PatronPoints.HasRedeemedKeywordPoints(PID, code))
                {
                    sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance,
                                                         eventCode: code, eventID: -1);

                    Response.Write(string.Format("{0}: {1}<br>",cnt1,PID));
                    Response.Flush();                    
                }
                else
                {
                    Response.Write(string.Format("XX{0}: {1}<br>",cnt2,PID));
                    Response.Flush();          
                }

            }

        }

   }
}