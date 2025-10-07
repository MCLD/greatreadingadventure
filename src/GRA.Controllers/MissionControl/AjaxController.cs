using System;
using System.IO;
using System.Threading.Tasks;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessMissionControl)]
    public class AjaxController(ILogger<AjaxController> logger, ServiceFacade.Controller context)
        : Base.MCController(context)
    {
        private const string UploadFilesPath = "uploads";

        private readonly ILogger<AjaxController> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (UserHasPermission(Permission.EditChallenges)
                || UserHasPermission(Permission.ManageDashboardContent)
                || UserHasPermission(Permission.ManagePages))
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
                            filename = $"{Path.GetFileNameWithoutExtension(file.FileName)}_"
                                + Path.GetRandomFileName()
                                    .Replace(".", "", StringComparison.OrdinalIgnoreCase)
                                + Path.GetExtension(file.FileName);
                        }

                        var filePath = Path.Combine(contentDir, filename);

                        _logger.LogDebug("Writing out task file {TaskFile}", filePath);
                        await using var fileStream = file.OpenReadStream();
                        await using var ms = new MemoryStream();
                        fileStream.CopyTo(ms);
                        System.IO.File.WriteAllBytes(filePath, ms.ToArray());

                        var siteUrl = await _siteLookupService.GetSiteLinkAsync(GetCurrentSiteId());
                        var contentPath = _pathResolver.ResolveContentPath(Path.Combine(folderPath,
                            filename));
                        var fileUrl = Path.Combine(siteUrl.ToString(), contentPath)
                            .Replace("\\", "/", StringComparison.OrdinalIgnoreCase);

                        return Json(fileUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "WMD fileupload error Type: {ContentType} Size: {Length} Exception: {Message}",
                            file.ContentType,
                            file.Length,
                            ex.Message);
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
