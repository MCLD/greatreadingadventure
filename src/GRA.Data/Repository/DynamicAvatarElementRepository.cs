using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class DynamicAvatarElementRepository
        : AuditingRepository<Model.DynamicAvatarElement, DynamicAvatarElement>,
        IDynamicAvatarElementRepository
    {
        public DynamicAvatarElementRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarElementRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<DynamicAvatarElement> GetByItemAndColorAsync(int item, int? color)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DynamicAvatarItemId == item && _.DynamicAvatarColorId == color)
                .ProjectTo<DynamicAvatarElement>()
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<DynamicAvatarElement>> GetUserAvatarAsync(int userId)
        {
            return await _context.UserDynamicAvatars.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.DynamicAvatarElement)
                .ProjectTo<DynamicAvatarElement>()
                .ToListAsync();
        }

        public async Task SetUserAvatarAsync(int userId, List<int> elementIds)
        {
            var userAvatar = await _context.UserDynamicAvatars
                .Where(_ => _.UserId == userId)
                .ToListAsync();

            var elementsToRemove = userAvatar.Where(_ => !elementIds.Contains(_.DynamicAvatarElementId));
            _context.RemoveRange(elementsToRemove);

            var elementsToAdd = elementIds.Except(userAvatar.Select(_ => _.DynamicAvatarElementId));
            foreach (var elementId in elementsToAdd)
            {
               await _context.AddAsync(new Model.UserDynamicAvatar
                {
                    UserId = userId,
                    DynamicAvatarElementId = elementId
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}

