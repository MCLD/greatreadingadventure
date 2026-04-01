using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class AvatarTransferRepository(ServiceFacade.Repository repositoryFacade,
        ILogger<AvatarTransferRepository> logger)
            : AuditingRepository<Model.AvatarTransfer, AvatarTransfer>(repositoryFacade, logger),
            IAvatarTransferRepository
    {
        public async Task<ICollection<AvatarTransfer>> GetAllAsync()
        {
            return await DbSet
                .OrderBy(_ => _.CreatedAt)
                .ProjectToType<AvatarTransfer>()
                .ToListAsync();
        }
    }
}
