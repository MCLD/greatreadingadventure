using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class CategoryService : BaseUserService<CategoryService>
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ILogger<CategoryService> logger,
            IUserContextProvider userContextProvider,
            ICategoryRepository categoryRepository) : base(logger, userContextProvider)
        {
            _categoryRepository = Require.IsNotNull(categoryRepository,
                nameof(categoryRepository));
        }

        public async Task<DataWithCount<IEnumerable<Category>>>
            GetPaginatedListAsync(int skip,
            int take)
        {
            int siteId = GetCurrentSiteId();
            return new DataWithCount<IEnumerable<Category>>
            {
                Data = await _categoryRepository.PageAllAsync(siteId, skip, take),
                Count = await _categoryRepository.GetCountAsync(siteId)
            };
        }

        public async Task<Category> AddAsync(Category category)
        {
            int userId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.AddCategories))
            {
                category.SiteId = GetCurrentSiteId();
                return await _categoryRepository.AddSaveAsync(userId, category);
            }
            _logger.LogError($"User {userId} doesn't have permission to add a category.");
            throw new Exception("Permission denied.");
        }

        public async Task<Category> EditAsync(Category category)
        {
            int userId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditCategories))
            {
                var current = await _categoryRepository.GetByIdAsync(category.Id);
                category.SiteId = current.SiteId;
                return await _categoryRepository.UpdateSaveAsync(userId, category);
            }
            _logger.LogError($"User {userId} doesn't have permission to edit category {category.Id}.");
            throw new Exception("Permission denied.");
        }

        public async Task RemoveAsync(int categoryId)
        {
            int userId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.DeleteCategories))
            {
                await _categoryRepository.RemoveSaveAsync(userId, categoryId);
            }
            else
            {
                _logger.LogError($"User {userId} doesn't have permission to remove category {categoryId}.");
                throw new Exception("Permission denied.");
            }
        }
    }
}
