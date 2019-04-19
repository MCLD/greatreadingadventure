using System;
using System.IO;
using System.Linq;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class TemplateService : BaseService<TemplateService>
    {
        private const string TemplateExtension = "{0}.cshtml";
        public static readonly string TemplateDashboard = "Dashboard";
        public static readonly string TemplateAccessClosed = "IndexAccessClosed";
        public static readonly string TemplateBeforeRegistration = "IndexBeforeRegistration";
        public static readonly string TemplateProgramEnded = "IndexProgramEnded";
        public static readonly string TemplateProgramOpen = "IndexProgramOpen";
        public static readonly string TemplateRegistrationOpen = "IndexRegistrationOpen";

        private readonly string[] TemplateFiles =
        {
            TemplateDashboard + TemplateExtension,
            TemplateAccessClosed + TemplateExtension,
            TemplateBeforeRegistration + TemplateExtension,
            TemplateProgramEnded + TemplateExtension,
            TemplateProgramOpen + TemplateExtension,
            TemplateRegistrationOpen + TemplateExtension
        };

        public TemplateService(ILogger<TemplateService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider) : base(logger, dateTimeProvider)
        {
        }

        public void SetupTemplates()
        {
            var viewsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");
            var templatesPath
                = Path.Combine(Directory.GetCurrentDirectory(), "shared", "templates");
            var sharedViewsPath
                = Path.Combine(Directory.GetCurrentDirectory(), "shared", "views");

            Directory.CreateDirectory(sharedViewsPath);
            File.Copy(Path.Combine(viewsPath, "_ViewImports.cshtml"),
                Path.Combine(sharedViewsPath, "_ViewImports.cshtml"), true);
            File.Copy(Path.Combine(viewsPath, "_ViewStart.cshtml"),
                Path.Combine(sharedViewsPath, "_ViewStart.cshtml"), true);

            var homeViewsPath = Path.Combine(viewsPath, "Home");
            var homeTemplatesPath = Path.Combine(templatesPath, "Home");
            Directory.CreateDirectory(homeTemplatesPath);

            var cultures = Culture
                .SupportedCultures
                .Where(_ => _.Name != Culture.DefaultCulture.Name);

            foreach (var template in TemplateFiles)
            {
                string templatePath = Path.Combine(homeViewsPath, string.Format(template, ""));
                if (File.Exists(templatePath))
                {
                    try
                    {
                        File.Copy(templatePath,
                            Path.Combine(homeTemplatesPath, string.Format(templatePath, "")),
                            true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Could not copy template {templatePath} to {homeTemplatesPath}: {Message}",
                            templatePath,
                            homeTemplatesPath,
                            ex.Message);
                    }
                }

                foreach (var culture in cultures)
                {
                    templatePath = Path.Combine(homeViewsPath,
                        string.Format(template, "." + culture.TwoLetterISOLanguageName));

                    if (File.Exists(templatePath))
                    {
                        try
                        {
                            File.Copy(templatePath,
                                Path.Combine(homeTemplatesPath,
                                    string.Format(templatePath,
                                    culture.Name)),
                                true);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                "Could not copy template {templatePath} to {homeTemplatesPath}: {Message}",
                                templatePath,
                                homeTemplatesPath,
                                ex.Message);
                        }
                    }
                }
            }

            var defaultFaviconPath = Path.Combine(Directory.GetCurrentDirectory(),
                "assets",
                "defaultfavicon");

            if (!Directory.Exists(defaultFaviconPath))
            {
                _logger.LogError("Not copying default social images: {defaultSocialPath} doesn't exist", defaultFaviconPath);
            }
            else
            {
                var contentPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "shared",
                    "content");

                foreach (var filePath in Directory.EnumerateFiles(defaultFaviconPath, "*.*"))
                {
                    var contentPathFile = Path.Combine(contentPath,
                        Path.GetFileName(filePath));
                    if (!File.Exists(contentPathFile))
                    {
                        try
                        {
                            File.Copy(filePath, contentPathFile);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Unable to copy default social file {filePath} to content path: {Message}",
                                filePath,
                                ex.Message);
                        }
                    }
                }
            }
        }
    }
}
