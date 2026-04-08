using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class AvatarLayerRepository(ServiceFacade.Repository repositoryFacade,
        ILogger<AvatarLayerRepository> logger)
            : AuditingRepository<Model.AvatarLayer, AvatarLayer>(repositoryFacade, logger),
            IAvatarLayerRepository
    {
        public async Task AddAvatarLayerTextAsync(int layerId, int languageId, AvatarLayerText text)
        {
            ArgumentNullException.ThrowIfNull(text);
            var layerText = new Model.AvatarLayerText
            {
                AvatarLayerId = layerId,
                LanguageId = languageId,
                Name = text.Name,
                RemoveLabel = text.RemoveLabel
            };

            await _context.AvatarLayerTexts
                .AddAsync(layerText);
        }

        public async Task<ICollection<AvatarLayer>> GetAllAsync(int siteId)
        {
            return await DbSet.AsNoTracking()
               .Where(_ => _.SiteId == siteId)
               .OrderBy(_ => _.GroupId)
               .ThenBy(_ => _.SortOrder)
               .ProjectToType<AvatarLayer>()
               .ToListAsync();
        }

        public async Task<ICollection<AvatarLayer>> GetAllWithColorsAsync(int siteId)
        {
            var forkedConfig = _mapper.Config
                .Fork(_ => _.NewConfig<Model.AvatarLayer, AvatarLayer>()
                    .Ignore(dest => dest.AvatarItems));

            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.GroupId)
                .ThenBy(_ => _.SortOrder)
                .ProjectToType<AvatarLayer>(forkedConfig)
                .ToListAsync();
        }

        public async Task<ICollection<AvatarLayerTransfer>> GetForExportAsync(int siteId)
        {
            var forkedConfig = _mapper.Config
                .Fork(_ => _.NewConfig<Model.AvatarLayer, AvatarLayerTransfer>());

            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.GroupId)
                .ThenBy(_ => _.SortOrder)
                .ProjectToType<AvatarLayerTransfer>(forkedConfig)
                .ToListAsync();
        }

        public Dictionary<string, string> GetNameAndLabelByLanguageId(int layerId, int languageId)
        {
            var layerText = _context.AvatarLayerTexts
                   .AsNoTracking()
                   .Where(_ => _.AvatarLayerId == layerId && _.LanguageId == languageId)
                   .FirstOrDefault();
            return new Dictionary<string, string>
            {
                { "Name", layerText.Name },
                { "RemoveLabel", layerText.RemoveLabel }
            };
        }

        public async Task<string> GetNameByLanguageIdAsync(int layerId, int languageId)
        {
            return await _context.AvatarLayerTexts
                   .AsNoTracking()
                   .Where(_ => _.AvatarLayerId == layerId && _.LanguageId == languageId)
                   .Select(_ => _.Name)
                   .FirstOrDefaultAsync();
        }
    }
}
