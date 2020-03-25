using System;
using System.IO;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessMissionControl)]
    public class FilesController : Base.MCController
    {
        private static readonly string UploadFilesPath = "uploads";

        private readonly ILogger<FilesController> _logger;
        public FilesController(ILogger<FilesController> logger,
            ServiceFacade.Controller context)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Get(string filename)
        {
            var filePath = Path.Combine($"site{GetCurrentSiteId()}", UploadFilesPath, filename);
            var file = _pathResolver.ResolvePrivateFilePath(filePath);
            if (!System.IO.File.Exists(file))
            {
                return NotFound();
            }

            var typeProvider = new FileExtensionContentTypeProvider();
            typeProvider.TryGetContentType(file, out string contentType);

            HttpContext.Response.Headers
                .Add("content-disposition", $"inline; filename=\"{filename}\"");

            return PhysicalFile(file, contentType);
        }

        public string Upload(IFormFile file)
        {
            var folderPath = Path.Combine($"site{GetCurrentSiteId()}", UploadFilesPath);
            var contentDir = _pathResolver.ResolvePrivateFilePath(folderPath);

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

            using (var fileStream = file.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                }
            }

            return Url.Action(nameof(Get), new { filename });
        }
    }
}
