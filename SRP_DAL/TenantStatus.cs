using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SRP_DAL
{
    public class TenantStatusReport
    {
        public int RegisteredPatrons { get; set; }
        public int PointsEarned { get; set; }
        public int PointsEarnedReading { get; set; }
        public int ChallengesCompleted { get; set; }
        public int SecretCodesRedeemed { get; set; }
        public int AdventuresCompleted { get; set; }
        public int BadgesAwarded { get; set; }
        public int RedeemedProgramCodes { get; set; }
    }
    public class TenantStatus
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        public int BranchId { get; set; }
        public int DistrictId { get; set; }
        public int ProgramId { get; set; }
        private int TenantId { get; set; }

        public TenantStatus(int tenantId = 0)
        {
            TenantId = tenantId;
        }

        public TenantStatusReport CurrentStatus()
        {
            var report = new TenantStatusReport();

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("TenId", TenantId));
            if (DistrictId > 0)
            {
                parameters.Add(new SqlParameter("DistrictId", DistrictId));
            }
            else if (BranchId > 0)
            {
                parameters.Add(new SqlParameter("BranchId", BranchId));
            }

            if (ProgramId > 0)
            {
                parameters.Add(new SqlParameter("ProgramId", ProgramId));
            }

            var result = SqlHelper.ExecuteReader(conn,
                                                 System.Data.CommandType.StoredProcedure,
                                                 "rpt_TenantStatusReport",
                                                 parameters.ToArray());

            if (result.Read())
            {
                report.RegisteredPatrons = result["RegisteredPatrons"] as int? ?? 0;
                report.PointsEarned = result["PointsEarned"] as int? ?? 0;
                report.PointsEarnedReading = result["PointsEarnedReading"] as int? ?? 0;
                report.ChallengesCompleted = result["ChallengesCompleted"] as int? ?? 0;
                report.SecretCodesRedeemed = result["SecretCodesRedeemed"] as int? ?? 0;
                report.AdventuresCompleted = result["AdventuresCompleted"] as int? ?? 0;
                report.BadgesAwarded = result["BadgesAwarded"] as int? ?? 0;
                report.RedeemedProgramCodes = result["RedeemedProgramCodes"] as int? ?? 0;

            }
            else {
                throw new Exception("No data returned.");
            }

            return report;
        }
    }
}
