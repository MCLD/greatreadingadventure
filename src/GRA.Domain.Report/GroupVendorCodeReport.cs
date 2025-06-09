using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(14,
        "Group Vendor Code Report",
        "Vendor codes earned by a group.",
        "Participants")]
    public class GroupVendorCodeReport : BaseReport
    {
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public GroupVendorCodeReport(ILogger<TopScoresReport> logger,
            ServiceFacade.Report serviceFacade,
            IGroupInfoRepository groupInfoRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(groupInfoRepository);
            ArgumentNullException.ThrowIfNull(userRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);

            _groupInfoRepository = groupInfoRepository;
            _userRepository = userRepository;
            _vendorCodeRepository = vendorCodeRepository;
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
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

            var groupInfo = await _groupInfoRepository.GetByIdAsync(criterion.GroupInfoId.Value);

            var report = new StoredReport(groupInfo.Name ?? _reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            #endregion Reporting initialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "Name",
                "Age",
                "Vendor Code",
            };

            var head = await _userRepository.GetByIdAsync(groupInfo.UserId);
            var users = await _userRepository.GetHouseholdAsync(groupInfo.UserId);
            users = users.Prepend(head);

            foreach (var user in users)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var vendorCode = await _vendorCodeRepository.GetUserVendorCode(user.Id);

                var row = new List<object> {
                        user.FullName,
                        user.Age,
                        vendorCode?.IsDonated == false ? vendorCode.Code : ""
                };

                reportData.Add(row.ToArray());
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
