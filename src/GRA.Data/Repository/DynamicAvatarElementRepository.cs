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

        public override Task<DynamicAvatarElement> GetByIdAsync(int id)
        {
            throw new System.Exception("Can not look up dynamic avatar elements by id only.");
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
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .OrderBy(_ => _.Position)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception($"Couldn't find first element for layer {dynamicAvatarLayerId}");
            }

            return entity.Id;
        }

        public async Task<int> GetIdByLayerIdAsync(int dynamicAvatarLayerId)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .OrderBy(_ => _.Position)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception("Couldn't find an appropriate avatar part!");
            }
            return entity.Id;
        }

        public async Task<int> GetLastElement(int dynamicAvatarLayerId)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId)
                .OrderByDescending(_ => _.Position)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception($"Couldn't find first element for layer {dynamicAvatarLayerId}");
            }

            return entity.Id;
        }

        public async Task<int?> GetNextElement(int dynamicAvatarLayerId, int elementId)
        {
            int? position = await GetPosition(dynamicAvatarLayerId, elementId);
            if (position != null)
            {
                var nextElement = await DbSet
                    .AsNoTracking()
                    .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId
                        && _.Position > position)
                    .OrderBy(_ => _.Position)
                    .FirstOrDefaultAsync();
                if (nextElement != null)
                {
                    return nextElement.Id;
                }
            }
            return await GetFirstElement(dynamicAvatarLayerId);
        }

        public async Task<int?> GetPreviousElement(int dynamicAvatarLayerId, int elementId)
        {
            int? position = await GetPosition(dynamicAvatarLayerId, elementId);
            if (position != null)
            {
                var previousElement = await DbSet
                    .AsNoTracking()
                    .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId
                        && _.Position < position)
                    .OrderByDescending(_ => _.Position)
                    .FirstOrDefaultAsync();
                if (previousElement != null)
                {
                    return previousElement.Id;
                }
            }
            return await GetLastElement(dynamicAvatarLayerId);
        }

        public override async Task<DynamicAvatarElement> AddSaveAsync(int userId, DynamicAvatarElement domainEntity)
        {
            var nextId = await DbSet
                .AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == domainEntity.DynamicAvatarLayerId)
                .OrderByDescending(_ => _.Id)
                .FirstOrDefaultAsync();

            if (nextId != null)
            {
                domainEntity.Id = nextId.Id + 1;
            }
            else
            {
                domainEntity.Id = 1;
            }

            return await base.AddSaveAsync(userId, domainEntity);
        }

        private async Task<int?> GetPosition(int dynamicAvatarLayerId, int elementId)
        {
            var currentPosition = await DbSet
             .AsNoTracking()
             .Where(_ => _.DynamicAvatarLayerId == dynamicAvatarLayerId && _.Id == elementId)
             .FirstOrDefaultAsync();

            if (currentPosition == null)
            {
                return null;
            }

            return currentPosition.Position;
        }
    }
}
