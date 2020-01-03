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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Web
{
    public class WebSocketHandler
    {
        private const double MinimumSecondsElapsedBetweenUpdates = 0.1;

        private readonly ILogger<WebSocketHandler> _logger;

        public WebSocketHandler(ILogger<WebSocketHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(HttpContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Path.HasValue && context.User != null)
            {
                // validate user is logged in and exists
                if (context.User?.Claims?.Any() == true)
                {
                    var siteClaim = context.User.Claims.Where(_ => _.Type == ClaimType.SiteId);
                    if (siteClaim?.Any() == true
                        && int.TryParse(siteClaim.First().Value, out int siteId))
                    {
                        // we need these context items to be populated but it doesn't matter 
                        // what the site stage is in order to do reporting or imports
                        context.Items[ItemKey.SiteId] = siteId;
                        context.Items[ItemKey.SiteStage] = SiteStage.Unknown;
                    }
                }

                if (context.Items[ItemKey.SiteId] == null)
                {
                    // likely never hit, MVC 301's to login if the user isn't authenticated
                    context.Response.StatusCode = 401;
                    return;
                }

                if (context.Request.Path.Value.Contains("runjob", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    var jobService = context.RequestServices.GetRequiredService<JobService>();
                    await RunWsTaskAsync(context, jobService.RunJob);
                }
                else
                {
                    _logger.LogError("Could not map to Web Socket handler: {Path}",
                        context.Request.Path);
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                _logger.LogError("Web Socket {Path} not initiated for {User}",
                    context.Request.Path,
                    context.User);
                context.Response.StatusCode = 400;
            }
        }

        private async Task RunWsTaskAsync(HttpContext context,
            Func<string, CancellationToken, Progress<JobStatus>, Task<JobStatus>> task)
        {
            var sw = new Stopwatch();
            sw.Start();

            // extract parameter
            string processedPath = context.Request.Path.Value;
            if (processedPath.Contains('/', StringComparison.OrdinalIgnoreCase)
                && processedPath.LastIndexOf('/') + 1 == processedPath.Length)
            {
                processedPath = processedPath.TrimEnd('/');
            }
            string parameterValue = processedPath.Substring(processedPath.LastIndexOf('/') + 1);

            // accept the upgrade to WebSocket
            using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                // cancellation token for the process we are running
                var cancellationTokenSource = new CancellationTokenSource();
                // cancellation token for the WebSocket, bound to the context aborting
                var wsToken = context.RequestAborted;

                try
                {
                    // configure the last update sent timer - don't flood updates
                    double lastUpdateSent = 0;
                    var progress = new Progress<JobStatus>(_ =>
                    {
                        double passed = sw.Elapsed.TotalSeconds - lastUpdateSent;
                        if (passed > MinimumSecondsElapsedBetweenUpdates
                            && webSocket.State == WebSocketState.Open)
                        {
                            lastUpdateSent = sw.Elapsed.TotalSeconds;
                            try
                            {
                                var status = Encoding
                                    .UTF8
                                    .GetBytes(JsonConvert.SerializeObject(_));
                                webSocket.SendAsync(new ArraySegment<byte>(status),
                                    WebSocketMessageType.Text,
                                    true,
                                    wsToken).Wait();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Could not send WebSocket update: {Message}",
                                    ex.Message);
                            }
                        }
                    });

                    // create and start the task running
                    var runTask = task(parameterValue, cancellationTokenSource.Token, progress);

                    // continue receiving data as long as the socket is open and the task is running
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    while (webSocket.State == WebSocketState.Open && !runTask.IsCompleted)
                    {
                        await webSocket.ReceiveAsync(buffer, wsToken);
                    }

                    // out of this loop means that the WebSocket is not open or the task is done
                    sw.Stop();

                    // ideal state is socket still open, task complete
                    if (webSocket.State == WebSocketState.Open && runTask.IsCompleted)
                    {
                        _logger.LogTrace("WebSocket {Status}, socket {State}, sending final update after {Elapsed:c}.",
                            runTask.Status,
                            webSocket.State,
                            sw.Elapsed);

                        var result = runTask.Result;
                        result.Complete = true;

                        switch (runTask.Status)
                        {
                            case TaskStatus.RanToCompletion:
                                result.PercentComplete = 100;
                                break;
                            case TaskStatus.Faulted:
                                result.Error = true;
                                break;
                            case TaskStatus.Canceled:
                                break;
                            default:
                                _logger.LogError("Unknown task status: {Status}", runTask.Status);
                                result.Error = true;
                                break;
                        }

                        var last = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(runTask.Result));
                        try
                        {
                            await webSocket.SendAsync(new ArraySegment<byte>(last),
                                WebSocketMessageType.Text,
                                true,
                                wsToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Unable to send final WebSocket update: {Message}",
                                ex.Message);
                        }
                    }
                    else
                    {
                        // socket is not open or task is not complete. cancel everything!
                        _logger.LogDebug("Task {Status}, socket {State}, sending cancel after {TotalSeconds}s.",
                            runTask.Status,
                            webSocket.State,
                            sw.Elapsed.TotalSeconds);
                        if (!runTask.IsCompleted)
                        {
                            cancellationTokenSource.Cancel();
                            runTask.Wait();
                        }
                        _logger.LogDebug("Task {Status}, cancellation processed.",
                            runTask.Status);
                    }
                }
                finally
                {
                    // if the socket is still open, close it gracefully
                    if (webSocket?.State == WebSocketState.Open)
                    {
                        await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure,
                            "Task complete.",
                            wsToken);
                    }
                }
            }
        }
    }
}
