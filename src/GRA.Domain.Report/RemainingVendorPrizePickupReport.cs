﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(22,
        "Remaining Vendor Prize Pick-up Report",
        "Participants with vendor prizes to be picked up by branch.",
        "Participants")]
    public class RemainingVendorPrizePickupReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public RemainingVendorPrizePickupReport(ILogger<RemainingVendorPrizePickupReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentNullException(nameof(vendorCodeRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting intialization

            request = await StartRequestAsync(request);

            var criterion
                    = await _serviceFacade.ReportCriterionRepository
                        .GetByIdAsync(request.ReportCriteriaId)
                    ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (!criterion.SiteId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            if (!criterion.BranchId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.BranchId));
            }

            var branch = await _branchRepository.GetByIdAsync(criterion.BranchId.Value);

            var report = new StoredReport
            {
                Title = branch.Name,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();

            #endregion Reporting intialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[]
            {
                "First Name",
                "Last Name",
                "Username",
                "Email",
                "Phone",
                "Item name",
                "Item arrival",
                "Email sent"
            };

            int count = 0;

            var remainingPrizes = await _vendorCodeRepository
                .GetRemainingPrizesForBranchAsync(criterion.BranchId.Value);

            foreach (var prize in remainingPrizes)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / remainingPrizes.Count,
                    $"Processing: {branch.Name}",
                    request.Name);

                var user = await _userRepository.GetByIdAsync(prize.UserId.Value);

                if (user != null)
                {
                    reportData.Add(new object[]
                    {
                        user.FirstName,
                        user.LastName,
                        user.Username,
                        user.Email,
                        user.PhoneNumber,
                        prize.Details,
                        prize.ArrivalDate?.ToString("g", CultureInfo.CurrentCulture),
                        prize.EmailSentAt?.ToString("g", CultureInfo.CurrentCulture)
                    });
                }
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
