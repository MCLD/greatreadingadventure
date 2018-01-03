using System;
using System.IO;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class AjaxController : Base.MCController
    {
        private const string UploadFilesPath = "uploads";

        private readonly ILogger<AjaxController> _logger;
        private readonly SiteService _siteService;
        public AjaxController(ILogger<AjaxController> logger,
            ServiceFacade.Controller context,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (UserHasPermission(Permission.AddPages)
                || UserHasPermission(Permission.EditPages)
                || UserHasPermission(Permission.EditChallenges)
                || UserHasPermission(Permission.ManageDashboardContent))
            {
                if (file != null)
                {
                    try
                    {
                        var folderPath = Path.Combine($"site{GetCurrentSiteId()}",
                            UploadFilesPath);
                        var contentDir = _pathResolver.ResolveContentFilePath(folderPath);

                        if (!Directory.Exists(contentDir))
                        {
                            Directory.CreateDirectory(contentDir);
                        }

                        var filename = file.FileName;
                        while (System.IO.File.Exists(Path.Combine(contentDir, filename)))
                        {
                            filename = $"{Path.GetFileNameWithoutExtension(file.FileName)}_" +
                                Path.GetRandomFileName().Replace(".", "") +
                                Path.GetExtension(file.FileName);
                        }

                        var filePath = Path.Combine(contentDir, filename);

                        _logger.LogInformation($"Writing out task file {filePath}...");
                        using (var fileStream = file.OpenReadStream())
                        {
                            using (var ms = new MemoryStream())
                            {
                                fileStream.CopyTo(ms);
                                System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                            }
                        }
                        var siteUrl = await _siteService.GetBaseUrl(Request.Scheme,
                            Request.Host.Value);
                        var contentPath = _pathResolver.ResolveContentPath(Path.Combine(folderPath,
                            filename));
                        var fileUrl = Path.Combine(siteUrl, contentPath).Replace("\\", "/");

                        return Json(fileUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"WMD fileupload error Type: {file.ContentType} Size: {file.Length} Exception: {ex}");
                        return Json("Error");
                    }
                }
                else
                {
                    return Json("Empty");
                }
            }
            else
            {
                return Json("UnAuth");
            }
        }
    }
}
