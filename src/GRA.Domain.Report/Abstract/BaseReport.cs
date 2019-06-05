using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Attribute;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report.Abstract
{
    public abstract class BaseReport
    {
        protected readonly ILogger _logger;
        protected readonly ServiceFacade.Report _serviceFacade;
        private Stopwatch _timer;

        protected StoredReportSet ReportSet { get; set; }

        protected BaseReport(ILogger logger, ServiceFacade.Report serviceFacade)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceFacade = serviceFacade
                ?? throw new ArgumentNullException(nameof(serviceFacade));
            ReportSet = new StoredReportSet
            {
                Reports = new List<StoredReport>()
            };
        }

        protected async Task<ReportRequest> StartRequestAsync(ReportRequest request)
        {
            (_timer ?? (_timer = new Stopwatch())).Start();

            request.Started = _serviceFacade.DateTimeProvider.Now;
            request.Finished = null;
            request.Success = null;
            request.ResultJson = null;
            request.InstanceName = _serviceFacade.Config[ConfigurationKey.InstanceName];
            return await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
        }

        protected double StopTimer()
        {
            _timer.Stop();
            double elapsed = _timer.Elapsed.TotalSeconds;
            _timer = null;
            return elapsed;
        }

        public abstract Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null);

        protected void UpdateProgress(IProgress<JobStatus> progress, int percentComplete)
        {
            UpdateProgress(progress, percentComplete, null);
        }

        protected void UpdateProgress(IProgress<JobStatus> progress,
            string message,
            string title = null)
        {
            UpdateProgress(progress, null, message, title);
        }

        protected void UpdateProgress(IProgress<JobStatus> progress,
            int? percentComplete = null,
            string message = null,
            string title = null)
        {
            if (progress != null)
            {
                var status = new JobStatus();

                if (percentComplete != null)
                {
                    status.PercentComplete = (int)percentComplete;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    status.Status = message;
                }
                if (!string.IsNullOrEmpty(title))
                {
                    status.Title = title;
                }

                progress.Report(status);
            }
        }

        protected ReportInformationAttribute ReportAttribute
        {
            get
            {
                return GetType().GetTypeInfo().GetCustomAttribute<ReportInformationAttribute>();
            }
        }

        public TimeSpan? Elapsed
        {
            get
            {
                if (_timer == null)
                {
                    return null;
                }
                return _timer.Elapsed;
            }
        }

        protected async Task<ReportCriterion> GetCriterionAsync(ReportRequest request)
        {
            var criterion = await _serviceFacade
                .ReportCriterionRepository
                .GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (criterion.SiteId == null)
            {
                throw new ArgumentException($"Must provide {nameof(criterion.SiteId)} in report criteria.");
            }

            return criterion;
        }

        protected async Task FinishRequestAsync(ReportRequest request, bool success)
        {
            string result = success ? "ran" : "cancelled";
            _logger.LogInformation($"Report {request.Name} with criterion {request.ReportCriteriaId} {result} in {StopTimer()}");

            request.Success = success;
            request.Finished = _serviceFacade.DateTimeProvider.Now;

            if (success)
            {
                request.ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(ReportSet);
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
        }

        protected Task<bool> GetSiteSettingBoolAsync(ReportCriterion criterion, string key)
        {
            if (criterion == null)
            {
                throw new ArgumentNullException(nameof(criterion));
            }

            if (criterion.SiteId == null)
            {
                throw new ArgumentException($"Must provide {nameof(criterion.SiteId)} to execute a report.");
            }

            return GetSiteSettingBoolInternalAsync(criterion, key);
        }

        protected async Task<bool> GetSiteSettingBoolInternalAsync(ReportCriterion criterion, string key)
        {
            var settings
                = await _serviceFacade
                    .SiteSettingRepository
                    .GetBySiteIdAsync((int)criterion.SiteId);

            var setting = settings.Where(_ => _.Key == key);

            return setting.SingleOrDefault()?.Value != null;
        }
    }
}
