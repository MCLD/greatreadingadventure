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
            _eventRepository = eventRepository
                ?? throw new ArgumentNullException(nameof(eventRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting initialization
            request = await StartRequestAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (criterion.SiteId == null)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            var report = new StoredReport
            {
                Title = ReportAttribute?.Name,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object>() {
                "Community experience",
                "Participants"
            };

            report.HeaderRow = headerRow.ToArray();

            // running totals
            long totalParticipants = 0;

            var communityExperiencesWithCount = await _eventRepository
                .GetCommunityExperienceAttendanceAsync(criterion);

            foreach (var communityExperienceWithCount in communityExperiencesWithCount)
            {
                var row = new List<object>()
                {
                    communityExperienceWithCount.Data.Name,
                    communityExperienceWithCount.Count
                };

                totalParticipants += communityExperienceWithCount.Count;

                reportData.Add(row.ToArray());
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>()
            {
                "Total",
                totalParticipants
            };

            report.FooterRow = footerRow.ToArray();
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
