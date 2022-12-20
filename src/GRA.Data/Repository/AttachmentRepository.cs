using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class AttachmentRepository
        : AuditingRepository<Model.Attachment, Domain.Model.Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AttachmentRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
