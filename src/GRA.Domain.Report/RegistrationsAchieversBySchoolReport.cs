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
    [ReportInformation(19,
    "Registrations And Achievers By School Report",
    "Registered participants and achievers by school (filterable by district and school).",
    "Program")]
    public class RegistrationsAchieversBySchoolReport : BaseReport
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IUserRepository _userRepository;

        public RegistrationsAchieversBySchoolReport(ILogger<RegistrationsAchieversReport> logger,
            ServiceFacade.Report serviceFacade,
            ISchoolRepository schoolRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            _schoolRepository = schoolRepository
                ?? throw new ArgumentNullException(nameof(schoolRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
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

            var askIfFirstTime
                = await GetSiteSettingBoolAsync(criterion, SiteSettingKey.Users.AskIfFirstTime);
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object>() {
                "School Name",
                "Registered Users"
            };

            if (askIfFirstTime)
            {
                headerRow.Add("First time Participants");
            }

            headerRow.Add("Achievers");

            report.HeaderRow = headerRow.ToArray();

            int count = 0;

            // running totals
            long totalRegistered = 0;
            long totalFirstTime = 0;
            long totalAchiever = 0;

            ICollection<School> schools = null;

            if (criterion.SchoolId != null)
            {
                schools = new List<School>()
                {
                    await _schoolRepository.GetByIdAsync((int)criterion.SchoolId)
                };
            }
            else if (criterion.SchoolDistrictId != null)
            {
                schools = await _schoolRepository.GetAllAsync((int)criterion.SiteId,
                    criterion.SchoolDistrictId);
            }

            if (schools == null)
            {
                throw new GraFatalException("Could not find any school(s) to report on.");
            }

            foreach (var school in schools)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / schools.Count,
                    $"Processing: {school.Name}",
                    request.Name);

                criterion.SchoolDistrictId = null;
                criterion.SchoolId = school.Id;

                int users = await _userRepository.GetCountAsync(criterion);
                int achievers = await _userRepository.GetAchieverCountAsync(criterion);
                totalRegistered += users;
                totalAchiever += achievers;

                var row = new List<object>()
                {
                    school.Name,
                    users
                };

                if (askIfFirstTime)
                {
                    int firstTime = await _userRepository.GetFirstTimeCountAsync(criterion);
                    totalFirstTime += firstTime;

                    row.Add(firstTime);
                }

                row.Add(achievers);

                reportData.Add(row.ToArray());
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>()
            {
                "Total",
                totalRegistered
            };

            if (askIfFirstTime)
            {
                footerRow.Add(totalFirstTime);
            }

            footerRow.Add(totalAchiever);

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
