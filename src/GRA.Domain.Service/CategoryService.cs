using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Utility;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class CategoryService : BaseUserService<CategoryService>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly SiteLookupService _siteLookupService;

        public CategoryService(ILogger<CategoryService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            ICategoryRepository categoryRepository,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(categoryRepository);
            ArgumentNullException.ThrowIfNull(siteLookupService);

            _categoryRepository = categoryRepository;
            _siteLookupService = siteLookupService;

            SetManagementPermission(Permission.ManageCategories);
        }

        public async Task<Category> AddAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            VerifyManagementPermission();
            category.Name = category.Name?.Trim();
            category.Description = category.Description?.Trim();
            category.SiteId = GetCurrentSiteId();
            if (string.IsNullOrWhiteSpace(category.Color))
            {
                category.Color = ColorConstants.DefaultColor;
            }
            await VerifyContrastAsync(_siteLookupService,
                category.Color,
                ColorConstants.WhiteBackground);

            return await _categoryRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), category);
        }

        public async Task<Category> EditAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            VerifyManagementPermission();
            var current = await _categoryRepository.GetByIdAsync(category.Id);
            current.Name = category.Name?.Trim();
            current.Description = category.Description?.Trim();
            current.Color = category.Color?.Trim();

            await VerifyContrastAsync(_siteLookupService,
                category.Color,
                ColorConstants.WhiteBackground);

            return await _categoryRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), current);
        }

        public async Task<IEnumerable<Category>> GetListAsync(bool hideEmpty = false)
        {
            return await _categoryRepository.GetAllAsync(GetCurrentSiteId(), hideEmpty);
        }

        public async Task<DataWithCount<IEnumerable<Category>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<IEnumerable<Category>>
            {
                Data = await _categoryRepository.PageAsync(filter),
                Count = await _categoryRepository.CountAsync(filter)
            };
        }

        public async Task RemoveAsync(int categoryId)
        {
            VerifyManagementPermission();
            await _categoryRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), categoryId);
        }
    }
}
