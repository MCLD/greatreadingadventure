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
    [ReportInformation(8,
        "Activity By Program",
        "Registration count and activity, filterable by system and date, displayed by program.",
        "Program")]
    public class ActivityByProgramReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;

        public ActivityByProgramReport(ILogger<ActivityByProgramReport> logger,
            ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IPointTranslationRepository pointTranslationRepository,
            IProgramRepository programRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _pointTranslationRepository = pointTranslationRepository
                ?? throw new ArgumentNullException(nameof(pointTranslationRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
        }

        public async override Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            #region Reporting initialization
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request = await StartRequestAsync(request);
            var criterion = await GetCriterionAsync(request);

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
                "System Name",
                "Branch Name",
                "Program Name",
                "Registered Users",
            };

            var translations = new Dictionary<string, ICollection<int?>>();
            var translationTotals = new Dictionary<string, long>();

            var programDictionary = (await _programRepository.GetAllAsync((int)criterion.SiteId))
                .ToDictionary(_ => _.Id, _ => _.Name);

            foreach (var programId in programDictionary.Keys)
            {
                var pointTranslation = await _pointTranslationRepository
                    .GetByProgramIdAsync(programId);

                string description = pointTranslation.ActivityDescriptionPlural;

                if (!translations.ContainsKey(description))
                {
                    translations.Add(description, new List<int?> { pointTranslation.Id });
                    translationTotals.Add(description, 0);
                    if (description.Length > 2)
                    {
                        headerRow.Add(description.First().ToString().ToUpper()
                            + description.Substring(1));
                    }
                    else
                    {
                        headerRow.Add(description);
                    }
                }
                else
                {
                    translations[description].Add(pointTranslation.Id);
                }
            }
            report.HeaderRow = headerRow.ToArray();

            int count = 0;

            // running totals
            long totalRegistered = 0;

            var branches = criterion.SystemId != null
                ? await _branchRepository.GetBySystemAsync((int)criterion.SystemId)
                : await _branchRepository.GetAllAsync((int)criterion.SiteId);

            var systemIds = branches
                .OrderBy(_ => _.SystemName)
                .GroupBy(_ => _.SystemId)
                .Select(_ => _.First().SystemId);

            int totalItems = branches.Count() * programDictionary.Count();

            foreach (var systemId in systemIds)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                foreach (var branch in branches.Where(_ => _.SystemId == systemId))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    foreach (var programId in programDictionary.Keys)
                    {
                        UpdateProgress(progress,
                            ++count * 100 / totalItems,
                            $"Processing: {branch.SystemName} - {branch.Name}",
                            request.Name);

                        criterion.SystemId = systemId;
                        criterion.BranchId = branch.Id;
                        criterion.ProgramId = programId;

                        int users = await _userRepository.GetCountAsync(criterion);

                        totalRegistered += users;

                        var row = new List<object>() {
                        branch.SystemName,
                        branch.Name,
                        programDictionary[programId],
                        users
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
                }
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>()
            {
                "Total",
                string.Empty,
                string.Empty,
                totalRegistered,
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
