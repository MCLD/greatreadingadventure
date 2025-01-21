using System;
using System.Collections.Generic;
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
    [ReportInformation(24,
        "Staff-registered Participants",
        "The number of participants registered by staff members.",
        "Staff")]
    public class StaffRegisteredParticipants : BaseReport
    {
        private readonly IUserRepository _userRepository;

        public StaffRegisteredParticipants(ILogger<StaffRegisteredParticipants> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
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
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "Staff Name",
                "Staff Username",
                "Registered Users"
            };

            int count = 0;

            IDictionary<User, int> staffRegisteredParticipants = await _userRepository.GetStaffRegisteredParticipantsAsync(criterion);

            foreach (KeyValuePair<User, int> staffRegisteredParticipant in staffRegisteredParticipants)
            {
                UpdateProgress(progress,
                    ++count * 100 / staffRegisteredParticipants.Count(),
                    $"Processing: {count}/{staffRegisteredParticipants.Count()}",
                    request.Name);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                reportData.Add(new object[] {
                        count,
                        staffRegisteredParticipant.Key.FullName,
                        staffRegisteredParticipant.Key.Username,
                        staffRegisteredParticipant.Value
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

