using System;
using GRA.Domain.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GRA.Domain.Report.Abstract
{
    public abstract class BaseReport
    {
        protected readonly ILogger _logger;
        protected readonly ServiceFacade.Report _serviceFacade;
        private Stopwatch _timer;

        public BaseReport(ILogger logger,
            ServiceFacade.Report serviceFacade)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceFacade = serviceFacade
                ?? throw new ArgumentNullException(nameof(serviceFacade));
        }

        protected void StartTimer()
        {
            if(_timer == null)
            {
                _timer = new Stopwatch();
            }
            _timer.Start();
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
    }
}
