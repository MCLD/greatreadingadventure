using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Linq;
using System;

namespace GRA.Data.Repository
{
    public class DynamicAvatarRepository
        : AuditingRepository<Model.DynamicAvatar, DynamicAvatar>,
        IDynamicAvatarRepository
    {
        public DynamicAvatarRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DynamicAvatar>> GetPaginatedAvatarListAsync(
                    int siteId,
                    int skip,
                    int take,
                    string search = default(string))
        {
            var avatars = DbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                avatars = avatars.Where(_ => _.Name.Contains(search));
            }

            return await avatars.OrderBy(_ => _.Position)
                    .ThenBy(_ => _.Name)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<DynamicAvatar>()
                    .ToListAsync();
        }

        new public async Task<DynamicAvatar> GetByIdAsync(int id)
        {
            var avatar = await DbSet
               .AsNoTracking()
               .Include(_ => _.Elements)
               .Where(_ => _.Id == id)
               .ProjectTo<DynamicAvatar>()
               .SingleOrDefaultAsync();

            return avatar;
        }
        public override async Task RemoveSaveAsync(int userId, int id)
        {
            _context.DynamicAvatarElements
                               .AsNoTracking()
                               .Where(_ => _.DynamicAvatarId == id)
                               .ToList().ForEach(x => _context.DynamicAvatarElements.Remove(x));

            _context.DynamicAvatarElements.RemoveRange();
            await base.RemoveSaveAsync(userId, id);
        }
    }
}
