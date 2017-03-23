using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<bool> ExistsAsync(int dynamicAvatarLayerId, int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .AnyAsync();
        }

        public async Task<DynamicAvatarElement> GetByIdLayerAsync(int id, int dynamicAvatarLayerId)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .SingleOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"{nameof(DynamicAvatarElement)} id {id} with layer id {dynamicAvatarLayerId} could not be found.");
            }
            return _mapper.Map<DynamicAvatarElement>(entity);
        }

        public async Task<int> GetFirstElement(int dynamicAvatarLayerId)
        {
            var element = await _context.DynamicAvatars
                .AsNoTracking()
                .OrderBy(_ => _.Position)
                .SelectMany(_ => _.Elements)
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .FirstOrDefaultAsync();

            if (element == null)
            {
                throw new Exception($"Couldn't find first element for layer {dynamicAvatarLayerId}");
            }

            return element.Id;
        }
        public async Task<int> GetLastElement(int dynamicAvatarLayerId)
        {
            var element = await _context.DynamicAvatars
                .AsNoTracking()
                .OrderByDescending(_ => _.Position)
                .SelectMany(_ => _.Elements)
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .FirstOrDefaultAsync();

            if (element == null)
            {
                throw new Exception($"Couldn't find first element for layer {dynamicAvatarLayerId}");
            }

            return element.Id;
        }

        public async Task<int?> GetNextElement(int dynamicAvatarLayerId, int elementId)
        {
            var element = await DbSet.AsNoTracking()
                                      .Where(_ => _.Id == elementId)
                                      .SingleOrDefaultAsync();

            if (element == null)
            {
                throw new Exception($"Couldn't find  element {elementId}");
            }

            var avatar = await _context.DynamicAvatars.AsNoTracking()
                                    .Where(_ => _.Id == element.DynamicAvatarId)
                                    .SingleOrDefaultAsync();

            var nextElement = await _context.DynamicAvatars
                .AsNoTracking()
                .Where(_ => _.Position > avatar.Position)
                .OrderBy(_ => _.Position)
                .SelectMany(_ => _.Elements)
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .FirstOrDefaultAsync();

            if (nextElement != null)
            {
                return nextElement.Id;
            }

            return await GetFirstElement(dynamicAvatarLayerId);
        }

        public async Task<int?> GetPreviousElement(int dynamicAvatarLayerId, int elementId)
        {
            var element = await DbSet.AsNoTracking()
                                     .Where(_ => _.Id == elementId)
                                     .SingleOrDefaultAsync();

            if (element == null)
            {
                throw new Exception($"Couldn't find  element {elementId}");
            }

            var avatar = await _context.DynamicAvatars.AsNoTracking()
                                                      .Where(_ => _.Id == element.DynamicAvatarId)
                                                      .SingleOrDefaultAsync();

            var previousElement = await _context.DynamicAvatars
                .AsNoTracking()
                .Where(_ => _.Position < avatar.Position)
                .OrderByDescending(_ => _.Position)
                .SelectMany(_ => _.Elements)
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .FirstOrDefaultAsync();

            if (previousElement != null)
            {
                return previousElement.Id;
            }

            return await GetLastElement(dynamicAvatarLayerId);
        }
    }
}
