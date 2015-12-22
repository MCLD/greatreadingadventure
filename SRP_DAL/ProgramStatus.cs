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
        public ProgramStatus() { }
        public ProgramStatus(DateTime startDate) {
            this.StartDate = startDate;
        }

        public ProgramStatusReport CurrentStatus() {
            var report = new ProgramStatusReport();

            var parameters = new List<SqlParameter>();
            StringBuilder query = new StringBuilder("SELECT SUM([NumPoints]) AS [PointsEarned],"
                + " SUM(CASE WHEN[BadgeAwardedFlag] = 1 THEN 1 ELSE 0 END) AS [BadgesAwarded],"
                + " SUM(CASE WHEN[IsBookList] = 1 THEN 1 ELSE 0 END) AS [ChallengesCompleted]"
                + " FROM [PatronPoints]");


            if(this.StartDate != null && this.StartDate > DateTime.MinValue) {
                query.Append(" WHERE CAST([AwardDate] AS DATE) >= CAST(@startDate AS DATE)");
                report.Since = this.StartDate.ToShortDateString();
                parameters.Add(new SqlParameter("startDate", report.Since));
            }

            var result = SqlHelper.ExecuteReader(conn,
                                                 System.Data.CommandType.Text,
                                                 query.ToString(),
                                                 parameters.ToArray());
            if(result.Read()) {
                report.PointsEarned = result["PointsEarned"] as int? ?? 0;
                report.BadgesAwarded = result["BadgesAwarded"] as int? ?? 0;
                report.ChallengesCompleted = result["ChallengesCompleted"] as int? ?? 0;
            } else {
                throw new Exception("No data returned.");
            }
            return report;
        }
    }
}
