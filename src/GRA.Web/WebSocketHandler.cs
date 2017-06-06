using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRA.Controllers;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Web
{
    public class WebSocketHandler
    {
        private readonly ILogger<WebSocketHandler> _logger;
        private readonly ReportService _reportService;
        private readonly SiteLookupService _siteLookupService;
        public WebSocketHandler(ILogger<WebSocketHandler> logger,
            ReportService reportService,
            SiteLookupService siteLookupService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }
        public async Task Handle(HttpContext context)
        {
            var requestPath = context.Request.Path.Value;
            if (context.Request.Path.HasValue
                && requestPath.Contains("runreport")
                && context.User != null)
            {
                var sw = new Stopwatch();
                sw.Start();

                // validate user is logged in and exists
                if (context.User != null
                    && context.User.Claims != null
                    && context.User.Claims.Count() > 0)
                {
                    var siteClaim = context.User.Claims.Where(_ => _.Type == ClaimType.SiteId);
                    if (siteClaim != null
                        && siteClaim.Count() > 0)
                    {
                        if (int.TryParse(siteClaim.First().Value, out int siteId))
                        {
                            // we need these context items to be populated but it doesn't matter what the
                            // site stage is in order to do reporting
                            context.Items[ItemKey.SiteId] = siteId;
                            context.Items[ItemKey.SiteStage] = SiteStage.Unknown;
                        }
                    }
                }

                if (context.Items[ItemKey.SiteId] == null)
                {
                    // this will likely never be hit, MVC will return a 301 to the login page if
                    // the user is not logged in.
                    context.Response.StatusCode = 401;
                    return;
                }

                // extract report id
                int reportId;
                try
                {
                    string processedPath = requestPath;
                    if (processedPath.Contains('/')
                        && processedPath.LastIndexOf('/') + 1 == processedPath.Length)
                    {
                        processedPath = processedPath.TrimEnd('/');
                    }

                    string reportIdString = processedPath.Substring(processedPath.LastIndexOf('/') + 1);
                    reportId = int.Parse(reportIdString);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Report ID could not be parsed: {requestPath} - {ex.Message}");
                    context.Response.StatusCode = 400;
                    return;
                }

                using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    var reportToken = new CancellationTokenSource();
                    var wsToken = context.RequestAborted;

                    var progress = new Progress<OperationStatus>(async _ =>
                    {
                        var status = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_));

                        if (webSocket.State == WebSocketState.Open)
                        {
                            await webSocket.SendAsync(new ArraySegment<byte>(status),
                                WebSocketMessageType.Text,
                                true,
                                wsToken);
                        }
                    });

                    var reportTask = _reportService
                        .RunReport(reportId, reportToken.Token, progress);

                    var buffer = new ArraySegment<byte>(new byte[1024]);

                    while (webSocket.State == WebSocketState.Open
                        && reportTask.Status != TaskStatus.Canceled
                        && reportTask.Status != TaskStatus.Faulted
                        && reportTask.Status != TaskStatus.RanToCompletion)
                    {
                        await webSocket.ReceiveAsync(buffer, wsToken);
                    }

                    if (reportTask.Status != TaskStatus.Canceled
                        && reportTask.Status != TaskStatus.Faulted
                        && reportTask.Status != TaskStatus.RanToCompletion)
                    {
                        sw.Stop();
                        _logger.LogInformation($"Web socket closed before report was completed. Cancelling after {sw.Elapsed.TotalSeconds}s.");
                        reportToken.Cancel();
                    }

                    if (webSocket.State == WebSocketState.Open)
                    {
                        sw.Stop();
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                            $"Report run is complete, duration: {sw.Elapsed.TotalSeconds}s.",
                            wsToken);
                    }
                }
            }
            else
            {
                _logger.LogError($"Web socket request with no matching handler: {requestPath}");
                context.Response.StatusCode = 400;
            }
        }
    }
}
