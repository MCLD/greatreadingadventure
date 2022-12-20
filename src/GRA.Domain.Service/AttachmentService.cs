using System;
using System.IO;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class AttachmentService : Abstract.BaseUserService<AttachmentService>
    {
        private const string AttachmentPath = "attachments/certificates";
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;

        public AttachmentService(ILogger<AttachmentService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IAttachmentRepository attachmentRepository,
            SiteLookupService siteLookupService,
            IPathResolver pathResolver)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _attachmentRepository = attachmentRepository;
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
        }

        public async Task<Attachment> AddAttachmentAsync(Attachment attachment, byte[] file)
        {
            attachment.SiteId = GetCurrentSiteId();
            attachment.IsCertificate = true;
            var result = await _attachmentRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), attachment);
            result.FileName = await WriteAttachmentFile(result, file);
            
            return await _attachmentRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), result);
        }

        public async Task<Attachment> GetByIdAsync(int attachmentId)
        {
            return await _attachmentRepository.GetByIdAsync(attachmentId);
        }

        public async Task<Attachment> ReplaceAttachmentFileAsync(Attachment attachment,
                    byte[] file)
        {
            var existingAttachment = await _attachmentRepository.GetByIdAsync(attachment.Id);

            if (file != null)
            {
                if (File.Exists(existingAttachment.FileName))
                {
                    File.Delete(existingAttachment.FileName);
                }

                attachment.FileName = await WriteAttachmentFile(existingAttachment, file);
            }

            return await _attachmentRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), attachment);
        }
        private string GetFilePath(string filename)
        {
            string contentDir = _pathResolver.ResolveContentFilePath();
            contentDir = System.IO.Path.Combine(contentDir,
                    $"site{GetCurrentSiteId()}",
                    AttachmentPath);

            if (!Directory.Exists(contentDir))
            {
                Directory.CreateDirectory(contentDir);
            }
            return Path.Combine(contentDir, filename);
        }

        private string GetUrlPath(string filename)
        {
            return $"site{GetCurrentSiteId()}/{AttachmentPath}/{filename}";
        }

        private async Task<string> WriteAttachmentFile(Attachment attachment,
            byte[] file)
        {
            string filename = $"certificate{attachment.Id}.pdf";
            string fullFilePath = GetFilePath(filename);

            try
            {
                _logger.LogDebug("Writing out attachment file {AttachmentFile}", fullFilePath);
                await File.WriteAllBytesAsync(fullFilePath, file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unknown image format exception on file {Filename}: {ErrorMessage}",
                    attachment.FileName,
                    ex.Message);
                throw new GraException("Unknown image type, please upload a PDF document.",
                    ex);
            }
            return GetUrlPath(filename);
        }
    }
}
