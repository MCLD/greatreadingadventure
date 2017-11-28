using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class CategoryService : BaseUserService<CategoryService>
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ILogger<CategoryService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            ICategoryRepository categoryRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageCategories);
            _categoryRepository = Require.IsNotNull(categoryRepository,
                nameof(categoryRepository));
        }

        public async Task<IEnumerable<Category>> GetListAsync(bool hideEmpty = false)
        {
            return await _categoryRepository.GetAllAsync(GetCurrentSiteId(), hideEmpty);
        }

        public async Task<DataWithCount<IEnumerable<Category>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<IEnumerable<Category>>
            {
                Data = await _categoryRepository.PageAsync(filter),
                Count = await _categoryRepository.CountAsync(filter)
            };
        }

        public async Task<Category> AddAsync(Category category)
        {
            VerifyManagementPermission();
            category.SiteId = GetCurrentSiteId();
            if (string.IsNullOrWhiteSpace(category.Color))
            {
                category.Color = "#777";
            }
            return await _categoryRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), category);
        }

        public async Task<Category> EditAsync(Category category)
        {
            VerifyManagementPermission();
            var current = await _categoryRepository.GetByIdAsync(category.Id);
            category.SiteId = current.SiteId;
            return await _categoryRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), category);
        }

        public async Task RemoveAsync(int categoryId)
        {
            VerifyManagementPermission();
            await _categoryRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), categoryId);
        }
    }
}
