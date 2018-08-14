using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GRA.Abstract;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class TemplateService : BaseService<TemplateService>
    {
        private readonly IConfigurationRoot _config;
        public TemplateService(ILogger<TemplateService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IConfigurationRoot config) : base(logger, dateTimeProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void SetupTemplates()
        {
            string viewsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");
            string templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "shared", "templates");

            var sharedViewsPath = Path.Combine(Directory.GetCurrentDirectory(), "shared", "views");
            Directory.CreateDirectory(sharedViewsPath);
            File.Copy(Path.Combine(viewsPath, "_ViewImports.cshtml"),
                Path.Combine(sharedViewsPath, "_ViewImports.cshtml"));

            var homeViewsPath = Path.Combine(viewsPath, "Home");
            var homeTemplatesPath = Path.Combine(templatesPath, "Home");
            Directory.CreateDirectory(homeTemplatesPath);

            File.Copy(Path.Combine(homeViewsPath, "Dashboard.cshtml"), 
                Path.Combine(homeTemplatesPath, "Dashboard.cshtml"), true);
            File.Copy(Path.Combine(homeViewsPath, "IndexAccessClosed.cshtml"),
                Path.Combine(homeTemplatesPath, "IndexAccessClosed.cshtml"), true);
            File.Copy(Path.Combine(homeViewsPath, "IndexBeforeRegistration.cshtml"),
                Path.Combine(homeTemplatesPath, "IndexBeforeRegistration.cshtml"), true);
            File.Copy(Path.Combine(homeViewsPath, "IndexProgramEnded.cshtml"),
                Path.Combine(homeTemplatesPath, "IndexProgramEnded.cshtml"), true);
            File.Copy(Path.Combine(homeViewsPath, "IndexProgramOpen.cshtml"),
                Path.Combine(homeTemplatesPath, "IndexProgramOpen.cshtml"), true);
            File.Copy(Path.Combine(homeViewsPath, "IndexRegistrationOpen.cshtml"),
                Path.Combine(homeTemplatesPath, "IndexRegistrationOpen.cshtml"), true);
        }
    }
}
