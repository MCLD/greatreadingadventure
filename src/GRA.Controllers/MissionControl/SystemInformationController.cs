using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class SystemInformationController : Base.MCController
    {
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly ILogger<SystemInformationController> _logger;
        private readonly SystemInformationService _systemInformationService;

        public SystemInformationController(ILogger<SystemInformationController> logger,
            IOptions<RequestLocalizationOptions> l10nOptions,
            ServiceFacade.Controller context,
            SystemInformationService systemInformationService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(l10nOptions);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(systemInformationService);

            _l10nOptions = l10nOptions;
            _logger = logger;
            _systemInformationService = systemInformationService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "System information screen can omit calls that throw errors")]
        public async Task<IActionResult> Index()
        {
            if (!UserHasPermission(Domain.Model.Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code
                return RedirectToRoute(new
                {
                    area = nameof(MissionControl),
                    controller = "Home",
                    action = nameof(HomeController.AuthorizationCode)
                });
            }

            string thisAssemblyName = null;
            string thisAssemblyVersion = null;
            try
            {
                thisAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
                thisAssemblyVersion = Version.GetVersion();
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't determine current assembly version: {ErrorMessage}",
                    ex.Message);
            }

            var viewModel = new SystemInformationViewModel
            {
                Assembly = thisAssemblyName ?? "Unknown",
                Version = thisAssemblyVersion,
            };

            viewModel.Settings.Add("Version", thisAssemblyVersion ?? "Unknown");

            try
            {
                var currentMigration = await _systemInformationService.GetCurrentMigrationAsync();
                viewModel.Settings.Add("Database version", currentMigration);
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't determine current database migration: {ErrorMessage}",
                    ex.Message);
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.InstanceName]))
            {
                viewModel.Settings.Add("Instance name", _config[ConfigurationKey.InstanceName]);
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.DeployDate]))
            {
                viewModel.Settings.Add("Deploy date", _config[ConfigurationKey.DeployDate]);
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.ApplicationDiscriminator]))
            {
                viewModel.Settings.Add("Application discriminator",
                    _config[ConfigurationKey.ApplicationDiscriminator]);
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.JobSleepSeconds]))
            {
                viewModel.Settings.Add("Job schedule (seconds)",
                    _config[ConfigurationKey.JobSleepSeconds]);
            }

            if (_l10nOptions.Value?.SupportedCultures.Count > 0)
            {
                viewModel.Settings.Add("Supported Cultures", string.Join(",", _l10nOptions
                    .Value
                    .SupportedCultures
                    .Select(_ => $"{_.DisplayName} [{_.Name}]")));
            }

            var site = await GetCurrentSiteAsync();
            viewModel.Settings.Add("Site created",
                site.CreatedAt.ToString(CultureInfo.InvariantCulture));

            var assemblies = Assembly.GetEntryAssembly()
                .GetReferencedAssemblies()
                .Where(_ => _.Name.StartsWith("GRA"));

            foreach (var assemblyName in assemblies)
            {
                var version = Assembly.Load(assemblyName)
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                viewModel.Assemblies.Add(assemblyName.Name, version);
            }

            viewModel.RuntimeSettings.Add("GC latency mode",
                System.Runtime.GCSettings.LatencyMode.ToString());
            viewModel.RuntimeSettings.Add("Server GC mode",
                System.Runtime.GCSettings.IsServerGC.ToString());

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrEmpty(env))
            {
                viewModel.RuntimeSettings.Add("ASP.NET Core environment", env);
            }

            var dotnetTargetFrameworkVersion = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;
            if (!string.IsNullOrEmpty(dotnetTargetFrameworkVersion))
            {
                viewModel.RuntimeSettings.Add(".NET target framework version",
                    dotnetTargetFrameworkVersion);
            }

            var dotnetVersion = Environment.GetEnvironmentVariable("DOTNET_VERSION");
            if (!string.IsNullOrEmpty(dotnetVersion))
            {
                viewModel.RuntimeSettings.Add(".NET Environment version", dotnetVersion);
            }

            if (!string.IsNullOrEmpty(_config["ASPNETCORE_VERSION"]))
            {
                viewModel.RuntimeSettings.Add("ASP.NET Core version",
                    _config["ASPNETCORE_VERSION"]);
            }

            if (!string.IsNullOrEmpty(_config["DOTNET_RUNNING_IN_CONTAINER"]))
            {
                viewModel.RuntimeSettings.Add("Containerized", "Yes");
            }

            var imageVersion = _config["org.opencontainers.image.version"];
            if (!string.IsNullOrEmpty(imageVersion))
            {
                viewModel.RuntimeSettings.Add("Container image version", imageVersion);
            }

            var imageCreated = _config["org.opencontainers.image.created"];
            if (!string.IsNullOrEmpty(imageCreated))
            {
                viewModel.RuntimeSettings.Add("Container image created", imageCreated);
            }

            var imageRevision = _config["org.opencontainers.image.revision"];
            if (!string.IsNullOrEmpty(imageRevision))
            {
                viewModel.RuntimeSettings.Add("Container image revision", imageRevision);
            }

            ThreadPool.GetMinThreads(out int minThreads, out int minCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out int availWorkerThreads,
                out int availCompletionPortThreads);
            ThreadPool.GetMaxThreads(out int maxThreads, out int maxCompletionPortThreads);

            viewModel.RuntimeSettings.Add("Worker threads (min/available/max)",
                $"{minThreads}/{availWorkerThreads}/{maxThreads}");
            viewModel.RuntimeSettings.Add("Completion port threads (min/available/max)",
                $"{minCompletionPortThreads}/{availCompletionPortThreads}/{maxCompletionPortThreads}");

            viewModel.RuntimeSettings.Add("Completed work item count",
                ThreadPool.CompletedWorkItemCount.ToString(CultureInfo.InvariantCulture));
            viewModel.RuntimeSettings.Add("Pending work item count",
                ThreadPool.PendingWorkItemCount.ToString(CultureInfo.InvariantCulture));

            return View(viewModel);
        }
    }
}
