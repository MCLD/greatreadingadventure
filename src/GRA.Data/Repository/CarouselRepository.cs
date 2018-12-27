using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class CarouselRepository :
        AuditingRepository<Model.Carousel, Carousel>,
        ICarouselRepository
    {
        public CarouselRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<CarouselRepository> logger)
            : base(repositoryFacade, logger)
        {
        }

        public async Task<Carousel> GetCurrentDashboardAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .Where(_ => _.IsForDashboard)
                .ProjectTo<Carousel>()
                .FirstOrDefaultAsync();
        }

        #region Adminsitrative calls
        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task<ICollection<Carousel>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<Carousel>()
                .ToListAsync();
        }
        #endregion

        private IQueryable<Model.Carousel> ApplyFilters(BaseFilter filter)
        {
            var carousels = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.IsActive == true)
            {
                var currentCarouselId = carousels
                    .Where(_ => _.StartTime <= _dateTimeProvider.Now)
                    .OrderByDescending(_ => _.StartTime)
                    .Select(_ => _.Id)
                    .FirstOrDefault();

                return carousels
                    .Where(_ => _.StartTime > _dateTimeProvider.Now || _.Id == currentCarouselId);
            }
            else if (filter.IsActive == false)
            {
                return carousels
                    .Where(_ => _.StartTime < _dateTimeProvider.Now)
                    .OrderByDescending(_ => _.StartTime)
                    .Skip(1);
            }

            return carousels;
        }
    }
}
