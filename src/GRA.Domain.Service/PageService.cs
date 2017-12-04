using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using System.Collections.Generic;

namespace GRA.Domain.Service
{
    public class PageService : Abstract.BaseUserService<PageService>
    {
        private readonly IPageRepository _pageRepository;
        public PageService(ILogger<PageService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPageRepository pageRepository) : base(logger, dateTimeProvider, userContextProvider)
        {
            _pageRepository = Require.IsNotNull(pageRepository, nameof(pageRepository));
        }

        public async Task<DataWithCount<IEnumerable<Page>>> GetPaginatedPageListAsync(int skip,
            int take)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            if (HasPermission(Permission.ViewUnpublishedPages))
            {
                return new DataWithCount<IEnumerable<Page>>
                {
                    Data = await _pageRepository.PageAllAsync(siteId, skip, take),
                    Count = await _pageRepository.GetCountAsync(siteId)
                };
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to view all pages.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Page> AddPageAsync(Page page)
        {
            if (HasPermission(Permission.AddPages))
            {
                var siteId = GetClaimId(ClaimType.SiteId);
                var existingPage = await _pageRepository.GetByStubAsync(siteId, page.Stub);
                if (existingPage != null)
                {
                    throw new GraException("The stub already exists, please enter a different one.");
                }

                page.SiteId = siteId;
                page.Stub = page.Stub.ToLower();
                return await _pageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), page);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to add pages.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task DeletePageAsync(int id)
        {
            if (HasPermission(Permission.DeletePages))
            {
                await _pageRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), id);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to delete pages.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Page> EditPageAsync(Page page)
        {
            if (HasPermission(Permission.EditPages))
            {
                var siteId = GetClaimId(ClaimType.SiteId);
                var currentPage = await _pageRepository
                    .GetByStubAsync(GetClaimId(ClaimType.SiteId), page.Stub);

                currentPage.Title = page.Title;
                currentPage.Content = page.Content;
                currentPage.FooterText = page.FooterText;
                currentPage.NavText = page.NavText;
                currentPage.IsPublished = page.IsPublished;

                return await _pageRepository
                    .UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentPage);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to edit pages.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Page> GetByStubAsync(string pageStub, bool excludeDashboardPage)
        {
            var siteId = GetCurrentSiteId();
            var page = await _pageRepository.GetByStubAsync(siteId, pageStub);
            if (page != null && (page.IsPublished || HasPermission(Permission.ViewUnpublishedPages)))
            {
                return page;
            }
            else
            {
                throw new GraException("The requested page could not be accessed or does not exist.");
            }
        }

        public async Task<IEnumerable<Page>> GetAreaPagesAsync(bool navPages)
        {
            return await _pageRepository.GetAreaPagesAsync(GetCurrentSiteId(), navPages);
        }
    }
}
