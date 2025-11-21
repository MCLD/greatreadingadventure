using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.Extensions.Logging;

namespace GRA.Web
{
    internal class JobTaskRunner
    {
        private readonly EmailBulkService _emailBulkService;
        private readonly ILogger _logger;
        private readonly UserService _userService;

        public JobTaskRunner(ILogger<JobTaskRunner> logger,
            EmailBulkService emailBulkService,
            UserService userService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(emailBulkService);
            ArgumentNullException.ThrowIfNull(userService);

            _logger = logger;
            _emailBulkService = emailBulkService;
            _userService = userService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Task runs headless and should log errors, not take down application.")]
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            var scheduledTasks = new Dictionary<string, Func<Task<bool>>>
            {
                ["PruneInactiveUsers"] = _userService.PruneInactiveScheduledTask,
                ["WelcomeEmail"] = _emailBulkService.SendWelcomeScheduledTask
            };

            int tasksDidWork = 0;

            foreach (var methodName in scheduledTasks.Keys)
            {
                try
                {
                    if (await scheduledTasks[methodName]())
                    {
                        tasksDidWork++;
                    }
                }
                catch (Exception ex)
                {
                    int preventLoop = 10;
                    var innerException = ex;
                    using (_logger.BeginScope(new Dictionary<string, object>
                    {
                        ["TopException"] = ex
                    }))
                    {
                        while (innerException.InnerException != null && preventLoop > 0)
                        {
                            innerException = innerException.InnerException;
                            preventLoop--;
                        }
                        _logger.LogCritical(ex,
                            "Critical error in scheduled task {MethodName}: {ErrorMessage}",
                            methodName,
                            innerException.Message);
                    }
                }
            }

            if (tasksDidWork > 0)
            {
                _logger.LogDebug("Scheduled tasks that completed work: {Count} in {Elapsed} ms",
                    tasksDidWork,
                    sw.ElapsedMilliseconds);
            }

            sw.Stop();
        }
    }
}
