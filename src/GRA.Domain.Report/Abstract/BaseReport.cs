using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
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
        protected readonly ReportInformationAttribute _reportInformation;
        protected readonly ServiceFacade.Report _serviceFacade;
        private Stopwatch _timer;

        protected BaseReport(ILogger logger, ServiceFacade.Report serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(serviceFacade);

            _logger = logger;
            _serviceFacade = serviceFacade;

            _reportInformation = GetType().GetCustomAttribute<ReportInformationAttribute>();

            ReportSet = new StoredReportSet
            {
                Reports = new List<StoredReport>()
            };
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

        protected ReportInformationAttribute ReportAttribute
        {
            get
            {
                return GetType().GetTypeInfo().GetCustomAttribute<ReportInformationAttribute>();
            }
        }

        protected StoredReportSet ReportSet { get; set; }

        public abstract Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null);

        protected static void UpdateProgress(IProgress<JobStatus> progress, int percentComplete)
        {
            UpdateProgress(progress, percentComplete, null);
        }

        protected static void UpdateProgress(IProgress<JobStatus> progress,
            string message,
            string title = null)
        {
            UpdateProgress(progress, null, message, title);
        }

        protected static void UpdateProgress(IProgress<JobStatus> progress,
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
                status.Status = message;
                status.Title = title;
                status.SuccessRedirect = true;

                progress.Report(status);
            }
        }

        protected async Task FinishRequestAsync(ReportRequest request, bool success)
        {
            ArgumentNullException.ThrowIfNull(request);

            string result = success ? "ran" : "cancelled";
            _logger.LogInformation("Report {ReportName} with criterion {ReportCriteriaId} {Result} in {Elapsed} ms",
                request.Name,
                request.ReportCriteriaId,
                result,
                StopTimerOutputMs());

            request.Success = success;
            if (!success)
            {
                request.Message = "Report generation was cancelled.";
            }
            request.Finished = _serviceFacade.DateTimeProvider.Now;

            if (success)
            {
                request.ResultJson = JsonSerializer.Serialize(ReportSet);
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
        }

        protected async Task<ReportCriterion> GetCriterionAsync(ReportRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

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

        protected Task<bool> GetSiteSettingBoolAsync(ReportCriterion criterion, string key)
        {
            ArgumentNullException.ThrowIfNull(criterion);

            if (criterion.SiteId == null)
            {
                throw new ArgumentException($"Must provide {nameof(criterion.SiteId)} to execute a report.");
            }

            return GetSiteSettingBoolInternalAsync(criterion, key);
        }

        protected async Task<bool> GetSiteSettingBoolInternalAsync(ReportCriterion criterion, string key)
        {
            ArgumentNullException.ThrowIfNull(criterion);

            var settings
                = await _serviceFacade
                    .SiteSettingRepository
                    .GetBySiteIdAsync((int)criterion.SiteId);

            var setting = settings.Where(_ => _.Key == key);

            return setting.SingleOrDefault()?.Value != null;
        }

        protected bool HasPermission(string permissionName)
        {
            return _serviceFacade
                .HttpContextAccessor?
                .HttpContext?
                .User?
                .HasClaim(ClaimType.Permission, permissionName) == true;
        }

        protected async Task<ReportRequest> StartRequestAsync(ReportRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var stopwatch = _timer ??= new Stopwatch();
            stopwatch.Start();

            request.Started = _serviceFacade.DateTimeProvider.Now;
            request.Finished = null;
            request.Success = null;
            request.ResultJson = null;
            request.InstanceName = _serviceFacade.Config[ConfigurationKey.InstanceName];
            return await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
        }

        protected string StopTimerOutputMs()
        {
            _timer?.Stop();
            string elapsed = _timer?
                .ElapsedMilliseconds
                .ToString(CultureInfo.InvariantCulture) ?? "0";
            _timer = null;
            return elapsed;
        }
    }
}
