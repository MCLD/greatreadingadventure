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
    [ReportInformation(-9,
    "Registrations and Achievers by School Report",
    "Registered participants and achievers by school (filterable by district and school).",
    "Program")]
    public class RegistrationsAchieversBySchoolReport : BaseReport
    {
        private readonly ISchoolDistrictRepository _schoolDistrictRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolTypeRepository _schoolTypeRepository;
        private readonly IUserRepository _userRepository;
        public RegistrationsAchieversBySchoolReport(ILogger<RegistrationsAchieversReport> logger,
            ServiceFacade.Report serviceFacade,
            ISchoolDistrictRepository schoolDistrictRepository,
            ISchoolRepository schoolRepository,
            ISchoolTypeRepository schoolTypeRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            _schoolDistrictRepository = schoolDistrictRepository
                ?? throw new ArgumentNullException(nameof(schoolDistrictRepository));
            _schoolRepository = schoolRepository
                ?? throw new ArgumentNullException(nameof(schoolRepository));
            _schoolTypeRepository = schoolTypeRepository
                ?? throw new ArgumentNullException(nameof(schoolTypeRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
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
            report.HeaderRow = new object[]
            {
                "School Name",
                "School Type",
                "Registered Users",
                "Achievers"
            };

            int count = 0;

            // running totals
            long totalRegistered = 0;
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
                throw new Exception("Could not find any school(s) to report on.");
            }

            foreach (var school in schools)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / schools.Count(),
                    $"Processing: {school.Name}",
                    request.Name);

                criterion.SchoolDistrictId = null;
                criterion.SchoolId = school.Id;

                int users = await _userRepository.GetCountAsync(criterion);
                int achievers = await _userRepository.GetAchieverCountAsync(criterion);
                totalRegistered += users;
                totalAchiever += achievers;

                var schoolType = await _schoolTypeRepository.GetByIdAsync(school.SchoolTypeId);

                // add row
                reportData.Add(new object[] {
                        school.Name,
                        schoolType.Name,
                        users,
                        achievers
                    });
            }

            report.Data = reportData.ToArray();

            // total row
            report.FooterRow = new object[]
            {
                "Total",
                string.Empty,
                totalRegistered,
                totalAchiever
            };

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
