using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;

namespace GRA.Domain.Service
{
    public class CarouselService : Abstract.BaseUserService<CarouselService>
    {
        private readonly ICarouselRepository _carouselRepository;
        private readonly ICarouselItemRepository _carouselItemRepository;

        public CarouselService(Microsoft.Extensions.Logging.ILogger<CarouselService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            Abstract.IUserContextProvider userContextProvider,
            ICarouselRepository carouselRepository,
            ICarouselItemRepository carouselItemRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageCarousels);
            _carouselItemRepository = carouselItemRepository
                ?? throw new ArgumentNullException(nameof(carouselItemRepository));
            _carouselRepository = carouselRepository
                ?? throw new ArgumentNullException(nameof(carouselRepository));
        }

        public async Task<Carousel> GetCurrentForDashboardAsync()
        {
            var carousel = await _carouselRepository.GetCurrentDashboardAsync(new BaseFilter
            {
                SiteId = GetClaimId(ClaimType.SiteId),
                IsActive = true
            });

            if (carousel != null)
            {
                carousel.Items = await _carouselItemRepository
                    .GetByCarouselIdNoDescriptionAsync(carousel.Id);
                if (carousel.Items?.Count() > 0)
                {
                    return carousel;
                }
            }

            return null;
        }

        public async Task<Carousel> GetCarouselAsync(int carouselId)
        {
            var carousel = await _carouselRepository.GetByIdAsync(carouselId);
            if (carousel != null)
            {
                carousel.Items = await _carouselItemRepository.GetByCarouselIdAsync(carousel.Id);
            }
            return carousel;
        }

        public async Task<CarouselItem> GetItemAsync(int carouselItemId)
        {
            return await _carouselItemRepository.GetByIdAsync(carouselItemId);
        }

        public async Task<string> GetItemDescriptionAsync(int carouselItemId)
        {
            var item = await _carouselItemRepository.GetByIdAsync(carouselItemId);
            return item.Description;
        }

        /* adminsitrative calls */
        public async Task<DataWithCount<ICollection<Carousel>>>
            GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            var data = await _carouselRepository.PageAsync(filter);
            foreach (var item in data)
            {
                item.ItemCount = await _carouselItemRepository.GetCountByCarouselIdAsync(item.Id);
            }
            return new DataWithCount<ICollection<Carousel>>
            {
                Data = data,
                Count = await _carouselRepository.CountAsync(filter)
            };
        }

        public async Task<Carousel> AddAsync(Carousel carousel)
        {
            VerifyManagementPermission();
            carousel.Name = carousel.Name.Trim();
            carousel.Heading = carousel.Heading == null
                ? carousel.Heading
                : carousel.Heading.Trim();
            return await _carouselRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), carousel);
        }

        public async Task<Carousel> EditAsync(Carousel carousel)
        {
            VerifyManagementPermission();

            var currentCarousel = await _carouselRepository.GetByIdAsync(carousel.Id);
            currentCarousel.Name = carousel.Name.Trim();
            currentCarousel.Heading = carousel.Heading?.Trim();
            currentCarousel.StartTime = carousel.StartTime;

            return await _carouselRepository
                .UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentCarousel);
        }

        public async Task RemoveAsync(int carouselId)
        {
            VerifyManagementPermission();
            await _carouselRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), carouselId);
        }

        public async Task<CarouselItem> AddItemAsync(CarouselItem item)
        {
            VerifyManagementPermission();
            item.Title = item.Title.Trim();
            item.ImageUrl = item.ImageUrl.Trim();
            item.Description = item.Description.Trim();
            return await _carouselItemRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), item);
        }

        public async Task<CarouselItem> EditItemAsync(CarouselItem item)
        {
            VerifyManagementPermission();

            var current = await _carouselItemRepository.GetByIdAsync(item.Id);

            item.CarouselId = current.CarouselId;
            item.CreatedAt = current.CreatedAt;
            item.CreatedBy = current.CreatedBy;
            item.SortOrder = current.SortOrder;

            item.Title = item.Title.Trim();
            item.ImageUrl = item.ImageUrl.Trim();
            item.Description = item.Description.Trim();

            return await _carouselItemRepository
                .UpdateSaveAsync(GetClaimId(ClaimType.UserId), item);
        }

        public async Task DeleteItemAsync(int carouselItemId)
        {
            VerifyManagementPermission();
            await _carouselItemRepository
                .RemoveSaveAsync(GetClaimId(ClaimType.UserId), carouselItemId);
        }
    }
}
