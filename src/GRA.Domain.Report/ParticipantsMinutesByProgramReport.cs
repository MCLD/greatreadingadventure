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
    [ReportInformation(16,
    "Participant Count and Minutes By Program Report",
    "Select a system, see participant count, achievers, and minutes reported by program.",
    "Program")]
    public class ParticipantCountMinutesByProgram : BaseReport
    {
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;

        public ParticipantCountMinutesByProgram(ILogger<ParticipantCountMinutesByProgram> logger,
            ServiceFacade.Report serviceFacade,
            IPointTranslationRepository pointTranslationRepository,
            IProgramRepository programRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(pointTranslationRepository);
            ArgumentNullException.ThrowIfNull(programRepository);
            ArgumentNullException.ThrowIfNull(systemRepository);
            ArgumentNullException.ThrowIfNull(userLogRepository);
            ArgumentNullException.ThrowIfNull(userRepository);
            _pointTranslationRepository = pointTranslationRepository;

            _programRepository = programRepository;
            _systemRepository = systemRepository;
            _userLogRepository = userLogRepository;
            _userRepository = userRepository;
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

            var report = new StoredReport(_reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            #endregion Reporting initialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object> {
                "System Name",
                "Program Name",
                "Registered Users",
                "Achievers"
            };

            var translations = new Dictionary<string, ICollection<int?>>();
            var translationTotals = new Dictionary<string, long>();

            var system = await _systemRepository.GetByIdAsync((int)criterion.SystemId);

            var programDictionary = (await _programRepository.GetAllAsync((int)criterion.SiteId))
                .ToDictionary(_ => _.Id, _ => _.Name);

            foreach (var programId in programDictionary.Keys)
            {
                var pointTranslation = await _pointTranslationRepository
                    .GetByProgramIdAsync(programId);

                string description = pointTranslation.ActivityDescriptionPlural;

                if (!translations.TryGetValue(description, out ICollection<int?> value))
                {
                    translations.Add(description, new List<int?> { pointTranslation.Id });
                    translationTotals.Add(description, 0);
                    if (description.Length > 2)
                    {
                        headerRow.Add(description[0]
                            .ToString()
                            .ToUpper(System.Globalization.CultureInfo.InvariantCulture)
                                + description[1..]);
                    }
                    else
                    {
                        headerRow.Add(description);
                    }
                }
                else
                {
                    value.Add(pointTranslation.Id);
                }
            }
            report.HeaderRow = headerRow.ToArray();

            int count = 0;

            // running totals
            long totalRegistered = 0;
            long totalAchievers = 0;

            int totalItems = programDictionary.Count;

            foreach (var programId in programDictionary.Keys)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                UpdateProgress(progress,
                    ++count * 100 / totalItems,
                    $"Processing: {programDictionary[programId]}",
                    request.Name);

                criterion.SystemId = system.Id;
                criterion.ProgramId = programId;

                int users = await _userRepository.GetCountAsync(criterion);

                int achievers = await _userRepository.GetAchieverCountAsync(criterion);

                totalRegistered += users;
                totalAchievers += achievers;

                var row = new List<object> {
                        system.Name,
                        programDictionary[programId],
                        users,
                        achievers
                    };

                foreach (var translationName in translations.Keys)
                {
                    long total = await _userLogRepository.TranslationEarningsAsync(criterion,
                        translations[translationName]);
                    row.Add(total);
                    translationTotals[translationName] += total;
                }

                reportData.Add(row.ToArray());

                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>
            {
                "Total",
                string.Empty,
                totalRegistered,
                totalAchievers
            };

            foreach (var total in translationTotals.Values)
            {
                footerRow.Add(total);
            }

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
