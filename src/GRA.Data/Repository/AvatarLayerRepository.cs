using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class AvatarLayerRepository
        : AuditingRepository<Model.AvatarLayer, AvatarLayer>,
        IAvatarLayerRepository
    {
        public AvatarLayerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarLayerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<AvatarLayer>> GetAllAsync(int siteId, int languageId)
        {
            var layers = await DbSet.AsNoTracking()
               .Where(_ => _.SiteId == siteId)
               .OrderBy(_ => _.GroupId)
               .ThenBy(_ => _.SortOrder)
               .ProjectTo<AvatarLayer>(_mapper.ConfigurationProvider)
               .ToListAsync();
            if (layers.Count > 0)
            {
                foreach (var layer in layers.ToList())
                {
                    var layerText = _context.AvatarLayerText
                    .AsNoTracking()
                    .Where(_ => _.AvatarLayerId == layer.Id && _.LanguageId == languageId)
                    .FirstOrDefault();
                    layer.Name = layerText.Name;
                    layer.RemoveLabel = layerText.RemoveLabel;
                }
            }
            return layers;
        }

        public async Task<ICollection<AvatarLayer>> GetAllWithColorsAsync(int siteId, int languageId)
        {
            var layers = await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.GroupId)
                .ThenBy(_ => _.SortOrder)
                .ProjectTo<AvatarLayer>(_mapper.ConfigurationProvider, _ => _.AvatarColors)
                .ToListAsync();
            if (layers.Count > 0)
            {
                foreach (var layer in layers.ToList())
                {
                    var layerText = _context.AvatarLayerText
                    .AsNoTracking()
                    .Where(_ => _.AvatarLayerId == layer.Id && _.LanguageId == languageId)
                    .FirstOrDefault();
                    layer.Name = layerText.Name;
                    layer.RemoveLabel = layerText.RemoveLabel;
                }
            }
            return layers;
        }
    }
}
