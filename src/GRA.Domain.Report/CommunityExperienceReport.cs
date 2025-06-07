using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(17,
        "Community Experience Report",
        "See participants that have attended a community experience filterable by system and branch",
        "Participants")]
    public class CommunityExperiencesReport : BaseReport
    {
        private readonly IEventRepository _eventRepository;

        public CommunityExperiencesReport(ILogger<CurrentStatusReport> logger,
            ServiceFacade.Report serviceFacade,
            IEventRepository eventRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(eventRepository);

            _eventRepository = eventRepository;
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

            var report = new StoredReport(_reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            #endregion Reporting initialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object> {
                "Community experience",
                "Participants"
            };

            report.HeaderRow = headerRow.ToArray();

            // running totals
            long totalParticipants = 0;

            UpdateProgress(progress, 1, "Looking up community experiences...");

            var communityExperiencesWithCount = await _eventRepository
                .GetCommunityExperienceAttendanceAsync(criterion);

            int onRow = 0;

            foreach (var communityExperienceWithCount in communityExperiencesWithCount)
            {
                if (onRow % 10 == 0)
                {
                    UpdateProgress(progress,
                        ++onRow * 100 / Math.Max(communityExperiencesWithCount.Count, 1),
                        $"Tabulating {communityExperienceWithCount.Data.Name}...");
                }

                var row = new List<object>
                {
                    communityExperienceWithCount.Data.Name,
                    communityExperienceWithCount.Count
                };

                totalParticipants += communityExperienceWithCount.Count;

                reportData.Add(row.ToArray());
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>
            {
                "Total",
                totalParticipants
            };

            report.FooterRow = footerRow.ToArray();

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
