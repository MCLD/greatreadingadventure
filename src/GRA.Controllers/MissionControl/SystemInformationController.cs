﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class SystemInformationController : Base.MCController
    {
        private readonly ILogger<SystemInformationController> _logger;
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly SystemInformationService _systemInformationService;

        public SystemInformationController(ILogger<SystemInformationController> logger,
            ServiceFacade.Controller context,
            IOptions<RequestLocalizationOptions> l10nOptions,
            SystemInformationService systemInformationService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _l10nOptions = l10nOptions ?? throw new ArgumentNullException(nameof(l10nOptions));
            _systemInformationService = systemInformationService
                ?? throw new ArgumentNullException(nameof(systemInformationService));
        }

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
                thisAssemblyVersion = new Version().GetVersion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't determine current assembly version: {ex.Message}");
            }

            var settings = new Dictionary<string, string>
            {
                {"Version", thisAssemblyVersion ?? "Unknown"}
            };

            try
            {
                var currentMigration = await _systemInformationService.GetCurrentMigrationAsync();
                settings.Add("Database version", currentMigration);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't determine current database migration: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.InstanceName]))
            {
                settings.Add("Instance name", _config[ConfigurationKey.InstanceName]);
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.DeployDate]))
            {
                settings.Add("Deploy date", _config[ConfigurationKey.DeployDate]);
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.ApplicationDiscriminator]))
            {
                settings.Add("Application discriminator",
                    _config[ConfigurationKey.ApplicationDiscriminator]);
            }

            if (_l10nOptions.Value?.SupportedCultures.Count > 0)
            {
                settings.Add("Supported Cultures", string.Join(",", _l10nOptions
                    .Value
                    .SupportedCultures
                    .Select(_ => $"{_.DisplayName} [{_.Name}]")));
            }

            var site = await GetCurrentSiteAsync();
            settings.Add("Site created", site.CreatedAt.ToString());

            var versions = new Dictionary<string, string>();
            var assemblies = Assembly.GetEntryAssembly()
                .GetReferencedAssemblies()
                .Where(_ => _.Name.StartsWith("GRA"));

            foreach (var assemblyName in assemblies)
            {
                var version = Assembly.Load(assemblyName)
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                versions.Add(assemblyName.Name, version);
            }

            var runtimeSettings = new Dictionary<string, string>
            {
                { "GC latency mode", System.Runtime.GCSettings.LatencyMode.ToString() },
                { "Server GC mode", System.Runtime.GCSettings.IsServerGC.ToString() },
            };

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrEmpty(env))
            {
                runtimeSettings.Add("ASP.NET Core environment", env);
            }

            var dotnetTargetFrameworkVersion = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;
            if (!string.IsNullOrEmpty(dotnetTargetFrameworkVersion))
            {
                runtimeSettings.Add(".NET target framework version", dotnetTargetFrameworkVersion);
            }

            var dotnetVersion = Environment.GetEnvironmentVariable("DOTNET_VERSION");
            if (!string.IsNullOrEmpty(dotnetVersion))
            {
                runtimeSettings.Add(".NET Environment version", dotnetVersion);
            }

            if (!string.IsNullOrEmpty(_config["ASPNETCORE_VERSION"]))
            {
                runtimeSettings.Add("ASP.NET Core version", _config["ASPNETCORE_VERSION"]);
            }

            if (!string.IsNullOrEmpty(_config["DOTNET_RUNNING_IN_CONTAINER"]))
            {
                runtimeSettings.Add("Containerized", "Yes");
            }

            var imageVersion = _config["org.opencontainers.image.version"];
            if (!string.IsNullOrEmpty(imageVersion))
            {
                runtimeSettings.Add("Container image version", imageVersion);
            }

            var imageCreated = _config["org.opencontainers.image.created"];
            if (!string.IsNullOrEmpty(imageCreated))
            {
                runtimeSettings.Add("Container image created", imageCreated);
            }

            var imageRevision = _config["org.opencontainers.image.revision"];
            if (!string.IsNullOrEmpty(imageRevision))
            {
                runtimeSettings.Add("Container image revision", imageRevision);
            }

            return View(new SystemInformationViewModel
            {
                Assembly = thisAssemblyName ?? "Unknown",
                Version = thisAssemblyVersion,
                Assemblies = versions,
                Settings = settings,
                RuntimeSettings = runtimeSettings
            });
        }
    }
}
