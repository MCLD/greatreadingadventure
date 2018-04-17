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
            Domain.Report.ServiceFacade.Report serviceFacade,
            IGroupInfoRepository groupInfoRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            _groupInfoRepository = groupInfoRepository
                ?? throw new ArgumentNullException(nameof(groupInfoRepository));
            _userRepository = userRepository
                ?? throw new ArgumentOutOfRangeException(nameof(userRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentException(nameof(vendorCodeRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            #region Reporting initialization
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request = await StartRequestAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (criterion.SiteId == null)
            {
                throw new ArgumentNullException(nameof(criterion.SiteId));
            }

            var groupInfo = await _groupInfoRepository.GetByIdAsync(criterion.GroupInfoId.Value);

            string title = groupInfo.Name;

            var report = new StoredReport
            {
                Title = title,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "Name",
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

                var row = new List<object>() {
                        user.FullName,
                        vendorCode?.IsDonated == false ? vendorCode.Code : ""
                };

                reportData.Add(row.ToArray());
            }

            report.Data = reportData.ToArray();

            #endregion Collect data

            #region Finish up reporting
            _logger.LogInformation($"Report {GetType().Name} with criterion {criterion.Id} ran in {StopTimer()}");

            request.Success = !token.IsCancellationRequested;

            if (request.Success == true)
            {
                ReportSet.Reports.Add(report);
                request.Finished = _serviceFacade.DateTimeProvider.Now;
                request.ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(ReportSet);
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
            #endregion Finish up reporting
        }
    }
}
