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
    public class AvatarElementRepository
        : AuditingRepository<Model.AvatarElement, AvatarElement>,
        IAvatarElementRepository
    {
        public AvatarElementRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarElementRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<AvatarElement> GetByItemAndColorAsync(int item, int? color)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarItemId == item && _.AvatarColorId == color)
                .ProjectToType<AvatarElement>()
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<AvatarElement>> GetUserAvatarAsync(int userId, int languageId)
        {

            return await _context.UserAvatars.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => new AvatarElement
                {
                    AltText = _.AvatarElement.AvatarItem.Texts
                        .Where(_ => _.LanguageId == languageId)
                        .FirstOrDefault()
                        .AltText,
                    AvatarColorId = _.AvatarElement.AvatarColorId,
                    AvatarItemId = _.AvatarElement.AvatarItemId,
                    Filename = _.AvatarElement.Filename,
                    LayerId = _.AvatarElement.AvatarItem.AvatarLayerId,
                    LayerPosition = _.AvatarElement.AvatarItem.AvatarLayer.Position
                })
                .ToListAsync();
        }

        public async Task SetUserAvatarAsync(int userId, List<int> elementIds)
        {
            var userAvatar = await _context.UserAvatars
                .Where(_ => _.UserId == userId)
                .ToListAsync();

            var elementsToRemove = userAvatar.Where(_ => !elementIds.Contains(_.AvatarElementId));
            _context.RemoveRange(elementsToRemove);

            var elementsToAdd = elementIds.Except(userAvatar.Select(_ => _.AvatarElementId));
            foreach (var elementId in elementsToAdd)
            {
                await _context.AddAsync(new Model.UserAvatar
                {
                    UserId = userId,
                    AvatarElementId = elementId
                });
            }
            await _context.SaveChangesAsync();
        }

        public void RemoveByItemId(int id)
        {
            var elements = DbSet.Where(_ => _.AvatarItemId == id);
            DbSet.RemoveRange(elements);
        }
    }
}

