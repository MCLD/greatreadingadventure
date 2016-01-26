using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SRP_DAL {
    public class ProgramStatusReport {
        public int PointsEarned { get; set; }
        public int BadgesAwarded { get; set; }
        public int ChallengesCompleted { get; set; }
        public string Since { get; set; }

    }
    public class ProgramStatus {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        private DateTime StartDate { get; set; }
        private int TenantId { get; set; }

        public ProgramStatus(int tenantId = 0) {
            this.TenantId = tenantId;
        }
        public ProgramStatus(DateTime startDate, int tenantId = 0) {
            this.StartDate = startDate;
            this.TenantId = tenantId;
        }

        public ProgramStatusReport CurrentStatus() {
            var report = new ProgramStatusReport();

            var parameters = new List<SqlParameter>();
            StringBuilder query = new StringBuilder("SELECT SUM(pp.[NumPoints]) AS [PointsEarned],"
                + " SUM(CASE WHEN pp.[IsBookList] = 1 THEN 1 ELSE 0 END) AS [ChallengesCompleted]"
                + " FROM [PatronPoints] pp"
                + " INNER JOIN [Patron] p on pp.[PID] = p.[PID] AND p.[TenID] = @TenId");

            parameters.Add(new SqlParameter("TenId", this.TenantId));

            if(this.StartDate != null && this.StartDate > DateTime.MinValue) {
                query.Append(" WHERE CAST(pp.[AwardDate] AS DATE) >= CAST(@startDate AS DATE)");
                report.Since = this.StartDate.ToShortDateString();
                parameters.Add(new SqlParameter("startDate", report.Since));
            }

            var result = SqlHelper.ExecuteReader(conn,
                                                 System.Data.CommandType.Text,
                                                 query.ToString(),
                                                 parameters.ToArray());
            if(result.Read()) {
                report.PointsEarned = result["PointsEarned"] as int? ?? 0;
                //report.BadgesAwarded = result["BadgesAwarded"] as int? ?? 0;
                report.ChallengesCompleted = result["ChallengesCompleted"] as int? ?? 0;
            } else {
                throw new Exception("No data returned.");
            }
            query = new StringBuilder("SELECT COUNT(ba.[PBID]) AS [BadgesAwarded] "
               + " FROM [PatronBadges] ba"
               + " INNER JOIN [Patron] p on ba.[PID] = p.[PID] AND p.[TenID] = @TenId");

            parameters.Clear();
            parameters.Add(new SqlParameter("TenId", this.TenantId));

            if(this.StartDate != null && this.StartDate > DateTime.MinValue) {
                query.Append(" WHERE CAST(ba.[DateEarned] AS DATE) >= CAST(@startDate AS DATE)");
                parameters.Add(new SqlParameter("startDate", report.Since));
            }

            result = SqlHelper.ExecuteReader(conn,
                                             System.Data.CommandType.Text,
                                             query.ToString(),
                                             parameters.ToArray());

            if(result.Read()) {
                report.BadgesAwarded = result["BadgesAwarded"] as int? ?? 0;
            } else {
                throw new Exception("No badge data returned.");
            }

            return report;
        }
    }
}
