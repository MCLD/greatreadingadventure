using System.IO;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class TemplateService : BaseService<TemplateService>
    {
        public TemplateService(ILogger<TemplateService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider) : base(logger, dateTimeProvider)
        {
        }

        public void SetupTemplates()
        {
            string viewsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");
            string templatesPath = Path.Combine(Directory.GetCurrentDirectory(), 
                "shared", 
                "templates");

            var sharedViewsPath = Path.Combine(Directory.GetCurrentDirectory(), "shared", "views");
            Directory.CreateDirectory(sharedViewsPath);
            File.Copy(Path.Combine(viewsPath, "_ViewImports.cshtml"),
                Path.Combine(sharedViewsPath, "_ViewImports.cshtml"), true);
            File.Copy(Path.Combine(viewsPath, "_ViewStart.cshtml"),
                Path.Combine(sharedViewsPath, "_ViewStart.cshtml"), true);

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
