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
    public class AttachmentService : BaseUserService<AttachmentService>
    {
        public static readonly string Certificates = "certificates";
        private const string AttachmentPath = "attachments";
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IPathResolver _pathResolver;

        public AttachmentService(IAttachmentRepository attachmentRepository,
            IDateTimeProvider dateTimeProvider,
            ILogger<AttachmentService> logger,
            IPathResolver pathResolver,
            IUserContextProvider userContextProvider)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.TriggerAttachments);
            ArgumentNullException.ThrowIfNull(attachmentRepository);
            ArgumentNullException.ThrowIfNull(pathResolver);
            _attachmentRepository = attachmentRepository;
            _pathResolver = pathResolver;
        }

        public async Task<Attachment> AddAttachmentAsync(Attachment attachment,
            string attachmentType,
            byte[] file)
        {
            VerifyManagementPermission();

            if (attachmentType != Certificates)
            {
                throw new GraException($"Unknown attachment type: {attachmentType}");
            }

            attachment.SiteId = GetCurrentSiteId();
            attachment.IsCertificate = true;
            var result = await _attachmentRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), attachment);
            result.FileName = await WriteAttachmentFile(result, attachmentType, file);

            return await _attachmentRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), result);
        }

        public async Task<Attachment> GetByIdAsync(int attachmentId)
        {
            return await _attachmentRepository.GetByIdAsync(attachmentId);
        }

        public async Task RemoveAttachmentFile(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null)
            {
                throw new GraException("Attachment does not exist.");
            }
            File.Delete("shared/content/" + attachment.FileName);
            await _attachmentRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                attachmentId);
        }

        public async Task<Attachment> ReplaceAttachmentFileAsync(Attachment attachment,
                    string attachmentType,
            byte[] file)
        {
            VerifyManagementPermission();

            if (attachmentType != Certificates)
            {
                throw new GraException($"Unknown attachment type: {attachmentType}");
            }

            var existingAttachment = await _attachmentRepository.GetByIdAsync(attachment.Id);

            if (file != null)
            {
                if (File.Exists(existingAttachment.FileName))
                {
                    File.Delete(existingAttachment.FileName);
                }

                attachment.FileName = await WriteAttachmentFile(existingAttachment,
                    attachmentType,
                    file);
            }

            return await _attachmentRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                attachment);
        }

        private string GetFilePath(string filename, string attachmentType)
        {
            if (attachmentType != Certificates)
            {
                throw new GraException($"Unknown attachment type: {attachmentType}");
            }

            string contentDir = _pathResolver.ResolveContentFilePath();
            contentDir = Path.Combine(contentDir,
                    $"site{GetCurrentSiteId()}",
                    AttachmentPath,
                    Certificates);

            if (!Directory.Exists(contentDir))
            {
                Directory.CreateDirectory(contentDir);
            }

            return Path.Combine(contentDir, filename);
        }

        private string GetLinkPath(string filename, string attachmentType)
        {
            if (attachmentType != Certificates)
            {
                throw new GraException($"Unknown attachment type: {attachmentType}");
            }

            return string.Join("/", new[]
            {
                $"site{GetCurrentSiteId()}",
                AttachmentPath,
                Certificates,
                filename
            });
        }

        private async Task<string> WriteAttachmentFile(Attachment attachment,
            string attachmentType,
            byte[] file)
        {
            if (attachmentType != Certificates)
            {
                throw new GraException($"Unknown attachment type: {attachmentType}");
            }
            string filename = $"certificate{attachment.Id}.pdf";
            string fullFilePath = GetFilePath(filename, attachmentType);

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
            return GetLinkPath(filename, attachmentType);
        }
    }
}
