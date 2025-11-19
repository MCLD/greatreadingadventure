using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(28,
        "Staff Login Report",
        "Registered staff members, filterable by last login date",
        "Staff")]
    public class StaffLoginReport : BaseReport
    {
        private readonly IUserRepository _userRepository;

        public StaffLoginReport(ILogger<StaffLoginReport> logger,
            ServiceFacade.Report serviceFacade,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(userRepository);

            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(ReportRequest request,
           CancellationToken token,
           IProgress<JobStatus> progress)
        {
            #region Reporting initialization

            request = await StartRequestAsync(request);

            var criterion = await _serviceFacade.ReportCriterionRepository
                    .GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (criterion.SiteId == null)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            var report = new StoredReport(_reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            #endregion Reporting initialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "Staff Name",
                "Staff Username",
                "Last Login Date"
            };

            int count = 0;

            var staffMembers = await _userRepository.GetAdminUsersAsync(criterion);
            int staffMemberCount = staffMembers.Count();

            foreach (var staffMember in staffMembers.OrderBy(_ => _.LastAccess))
            {
                UpdateProgress(progress,
                    ++count * 100 / staffMemberCount,
                    $"Processing: {count}/{staffMemberCount}",
                    request.Name);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                var name = new StringBuilder(staffMember.FirstName);
                if (!string.IsNullOrEmpty(staffMember.LastName))
                {
                    name.Append(' ').Append(staffMember.LastName);
                }

                reportData.Add(new object[] {
                        name.ToString(),
                        staffMember.Username,
                        staffMember.LastAccess.Value.ToString("g", CultureInfo.CurrentCulture)
                    });
            }

            report.Data = reportData.ToArray();

            #endregion Collect data

            #region Finish up reporting

            if (!token.IsCancellationRequested)
            {
                ReportSet.Reports.Add(report);
            }
            await FinishRequestAsync(request, !token.IsCancellationRequested);

            #endregion Finish up reporting
        }
    }
}
