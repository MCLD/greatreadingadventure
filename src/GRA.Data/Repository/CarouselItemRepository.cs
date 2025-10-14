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
    public class CarouselItemRepository :
        AuditingRepository<Model.CarouselItem, Domain.Model.CarouselItem>,
        ICarouselItemRepository
    {
        public CarouselItemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<CarouselItemRepository> logger)
            : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<CarouselItem>> GetByCarouselIdAsync(int carouselId)
        {
            // select the list of items for the carousel excluding the item description
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.CarouselId == carouselId)
                .OrderBy(_ => _.SortOrder)
                .ProjectToType<CarouselItem>()
                .ToListAsync();
        }

        public async Task<IEnumerable<CarouselItem>> GetByCarouselIdNoDescriptionAsync(int carouselId)
        {
            // select the list of items for the carousel excluding the item description
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.CarouselId == carouselId)
                .OrderBy(_ => _.SortOrder)
                .Select(_ => new CarouselItem
                {
                    CarouselId = _.CarouselId,
                    CreatedAt = _.CreatedAt,
                    CreatedBy = _.CreatedBy,
                    Id = _.Id,
                    ImageUrl = _.ImageUrl,
                    SortOrder = _.SortOrder,
                    Title = _.Title
                })
                .ToListAsync();
        }

        public async Task<int> GetCountByCarouselIdAsync(int carouselId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.CarouselId == carouselId)
                .OrderBy(_ => _.SortOrder)
                .CountAsync();
        }
    }
}
