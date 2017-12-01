using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class SystemInformationController : Base.MCController
    {
        private readonly ILogger<SystemInformationController> _logger;
        private readonly SystemInformationService _systemInformationService;
        public SystemInformationController(ILogger<SystemInformationController> logger,
            ServiceFacade.Controller context,
            SystemInformationService systemInformationService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _systemInformationService = Require.IsNotNull(systemInformationService,
                nameof(systemInformationService));
        }

        public async Task<IActionResult> Index()
        {
            if (!UserHasPermission(Domain.Model.Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code
                return RedirectToRoute(new
                {
                    area = "MissionControl",
                    controller = "Home",
                    action = "AuthorizationCode"
                });
            }

            string thisAssemblyName = "Unknown";
            string thisAssemblyVersion = "Unknown";
            string currentMigration = "Unknown";
            try
            {
                thisAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
                thisAssemblyVersion = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                int build = Assembly.GetEntryAssembly().GetName().Version.Build;
                if (build != 0)
                {
                    thisAssemblyVersion += " build " + build;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't determine current assembly version: {ex.Message}");
            }

            try
            {
                currentMigration = await _systemInformationService.GetCurrentMigrationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't determine current database migration: {ex.Message}");
            }

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

            var site = await GetCurrentSiteAsync();
            string siteLogoUrl = site.SiteLogoUrl 
                ?? Url.Content(Defaults.SiteLogoPath);

            var settings = new Dictionary<string, string>();

            settings.Add("GC latency mode", System.Runtime.GCSettings.LatencyMode.ToString());
            settings.Add("Server GC mode", System.Runtime.GCSettings.IsServerGC.ToString());

            return View(new SystemInformationViewModel
            {
                Assembly = thisAssemblyName,
                Version = thisAssemblyVersion,
                Migration = currentMigration,
                Assemblies = versions,
                Instance = _config[ConfigurationKey.InstanceName],
                Deploy = _config[ConfigurationKey.DeployDate],
                SiteLogoUrl = siteLogoUrl,
                Settings = settings
            });
        }
    }
}
