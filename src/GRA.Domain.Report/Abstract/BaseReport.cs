using System;
using GRA.Domain.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using GRA.Domain.Report.Attribute;
using System.Reflection;

namespace GRA.Domain.Report.Abstract
{
    public abstract class BaseReport
    {
        protected readonly ILogger _logger;
        protected readonly ServiceFacade.Report _serviceFacade;
        private Stopwatch _timer;

        protected StoredReportSet ReportSet { get; set; }

        public BaseReport(ILogger logger,
            ServiceFacade.Report serviceFacade)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceFacade = serviceFacade
                ?? throw new ArgumentNullException(nameof(serviceFacade));
            ReportSet = new StoredReportSet();
            ReportSet.Reports = new List<StoredReport>();
        }

        protected async Task<ReportRequest> StartRequestAsync(ReportRequest request)
        {
            if (_timer == null)
            {
                _timer = new Stopwatch();
            }
            _timer.Start();

            request.Started = _serviceFacade.DateTimeProvider.Now;
            request.Finished = null;
            request.Success = null;
            request.ResultJson = null;
            request.InstanceName = _serviceFacade.Config[ConfigurationKey.InstanceName];
            request.Name = request.Name;
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
            IProgress<OperationStatus> progress = null);

        protected void UpdateProgress(IProgress<OperationStatus> progress, int percentComplete)
        {
            UpdateProgress(progress, percentComplete, null);
        }

        protected void UpdateProgress(IProgress<OperationStatus> progress, 
            string message,
            string title = null)
        {
            UpdateProgress(progress, null, message, title);
        }

        protected void UpdateProgress(IProgress<OperationStatus> progress,
            int? percentComplete = null,
            string message = null,
            string title = null)
        {
            if (progress != null)
            {
                var status = new OperationStatus();

                if (percentComplete != null)
                {
                    status.PercentComplete = (int)percentComplete;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    status.Status = message;
                }
                if(!string.IsNullOrEmpty(title))
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
    }
}
