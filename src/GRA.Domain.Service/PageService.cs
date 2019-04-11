using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PageService : Abstract.BaseUserService<PageService>
    {
        private readonly IPageHeaderRepository _pageHeaderRepository;
        private readonly IPageRepository _pageRepository;
        private readonly LanguageService _languageService;

        public PageService(ILogger<PageService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPageHeaderRepository pageHeaderRepository,
            IPageRepository pageRepository,
            LanguageService languageService) : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManagePages);
            _pageHeaderRepository = pageHeaderRepository
                ?? throw new ArgumentNullException(nameof(pageHeaderRepository));
            _pageRepository = pageRepository
                ?? throw new ArgumentNullException(nameof(pageRepository));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
        }

        public async Task<DataWithCount<IEnumerable<PageHeader>>> GetPaginatedHeaderListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();

            filter.SiteId = GetCurrentSiteId();

            return await _pageHeaderRepository.PageAsync(filter);
        }

        public async Task<PageHeader> GetHeaderByIdAsync(int id)
        {
            VerifyManagementPermission();

            return await _pageHeaderRepository.GetByIdAsync(id);
        }

        public async Task<PageHeader> AddPageHeaderAsync(PageHeader header)
        {
            VerifyManagementPermission();

            var siteId = GetCurrentSiteId();
            var stub = header.Stub?.Trim().ToLowerInvariant();
            if (await _pageHeaderRepository.StubExistsAsync(siteId, stub))
            {
                throw new GraException("The stub already exists, please enter a different one.");
            }

            header.PageName = header.PageName?.Trim();
            header.SiteId = siteId;
            header.Stub = stub;

            return await _pageHeaderRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), header);
        }

        public async Task<PageHeader> EditPageHeaderAsync(PageHeader header)
        {
            VerifyManagementPermission();

            var currentHeader = await _pageHeaderRepository.GetByIdAsync(header.Id);

            currentHeader.PageName = header.PageName?.Trim();

            return await _pageHeaderRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentHeader);
        }

        public async Task DeletePageHeaderAsync(int headerId)
        {
            VerifyManagementPermission();

            var authId = GetClaimId(ClaimType.UserId);

            var pages = await _pageRepository.GetByHeaderIdAsync(headerId);
            foreach (var page in pages)
            {
                await _pageRepository.RemoveSaveAsync(authId, page.Id);
            }

            await _pageHeaderRepository.RemoveSaveAsync(authId, headerId);
        }

        public async Task<Page> GetByHeaderAndLanguageAsync(int headerId, int languageId)
        {
            VerifyManagementPermission();

            return await _pageRepository.GetByHeaderAndLanguageAsync(headerId, languageId);
        }

        public async Task<Page> AddPageAsync(Page page)
        {
            VerifyManagementPermission();

            var currentPage = await _pageRepository.GetByHeaderAndLanguageAsync(page.PageHeaderId,
                page.LanguageId);
            if (currentPage != null)
            {
                throw new GraException("Page already exists for header and language.");
            }

            page.FooterText = page.FooterText?.Trim();
            page.MetaDescription = page.MetaDescription?.Trim();
            page.NavText = page.NavText?.Trim();
            page.Title = page.Title?.Trim();

            return await _pageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), page);
        }

        public async Task<Page> EditPageAsync(Page page)
        {
            VerifyManagementPermission();

            var currentPage = await _pageRepository.GetByHeaderAndLanguageAsync(page.PageHeaderId,
                page.LanguageId);

            currentPage.Content = page.Content;
            currentPage.IsPublished = page.IsPublished;
            currentPage.FooterText = page.FooterText?.Trim();
            currentPage.MetaDescription = page.MetaDescription?.Trim();
            currentPage.NavText = page.NavText?.Trim();
            currentPage.Title = page.Title?.Trim();

            return await _pageRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentPage);
        }

        public async Task DeletePageAsync(int id)
        {
            VerifyManagementPermission();
            await _pageRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), id);
        }

        public async Task<Page> GetByStubAsync(string pageStub)
        {
            var siteId = GetCurrentSiteId();

            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            if (currentCultureName != null)
            {
                var currentLanguageId = await _languageService.GetLanguageIdAsync(currentCultureName);
                var localPage
                    = await _pageRepository.GetByStubAsync(siteId, pageStub, currentLanguageId);
                if (localPage != null
                    && (localPage.IsPublished || HasPermission(Permission.ViewUnpublishedPages)))
                {
                    return localPage;
                }
            }
            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
            var defaultPage
                = await _pageRepository.GetByStubAsync(siteId, pageStub, defaultLanguageId);

            if (defaultPage != null
                && (defaultPage.IsPublished || HasPermission(Permission.ViewUnpublishedPages)))
            {
                return defaultPage;
            }
            else
            {
                throw new GraException("The requested page could not be accessed or does not exist.");
            }
        }

        public async Task<IEnumerable<Page>> GetAreaPagesAsync(bool navPages)
        {
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            if (currentCultureName != null)
            {
                var currentLanguageId = await _languageService.GetLanguageIdAsync(currentCultureName);
                return await _pageRepository.GetAreaPagesAsync(GetCurrentSiteId(),
                    navPages,
                    currentLanguageId);
            }
            else
            {
                return null;
            }
        }
    }
}
