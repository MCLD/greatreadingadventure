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

        public FileResult Get(string filePath)
        {
            var file = ResolvePrivateContentFilePath(filePath);
            if (!System.IO.File.Exists(file))
            {
                return null;
            }

            var fileBytes = System.IO.File.ReadAllBytes(file);

            var typeProvider = new FileExtensionContentTypeProvider();
            typeProvider.TryGetContentType(file, out string contentType);

            return File(fileBytes, contentType);
        }

        public string Upload(IFormFile file)
        {
            var folderPath = Path.Combine($"site{GetCurrentSiteId()}", UploadFilesPath);
            var contentDir = ResolvePrivateContentFilePath(folderPath);

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

            return Url.Action(nameof(Get), new { filePath });
        }

        private string ResolvePrivateContentFilePath(string filePath = default(string))
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "private", "content");

            if (!string.IsNullOrEmpty(filePath))
            {
                return Path.Combine(path, filePath);
            }
            else
            {
                return path;
            }
        }
    }
}
