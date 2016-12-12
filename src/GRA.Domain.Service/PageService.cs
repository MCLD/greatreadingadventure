using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class PageService : Abstract.BaseUserService<PageService>
    {
        private readonly IPageRepository _pageRepository;
        public PageService(ILogger<PageService> logger,
            IUserContextProvider userContextProvider,
            IPageRepository pageRepository) : base(logger, userContextProvider)
        {
            _pageRepository = Require.IsNotNull(pageRepository, nameof(pageRepository));
        }

        public async Task<Page> AddPageAsync(Page page)
        {
            if(HasPermission(Permission.AddPages))
            {
                page.SiteId = GetClaimId(ClaimType.SiteId);
                return await _pageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), page);
            }
            else
            {
                throw new GraException("Permission denied.");
            }
        }

        public async Task DeletePageAsync(Page page)
        {
            if(HasPermission(Permission.DeletePages))
            {
                await _pageRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), page.Id);
            }
            else
            {
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Page> EditPageAsync(Page page)
        {
            if(HasPermission(Permission.EditPages))
            {
                page.SiteId = GetClaimId(ClaimType.SiteId);
                return await _pageRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), page);
            }
            else
            {
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Page> GetByStubAsync(string pageStub)
        {
            var page = await _pageRepository.GetByStubAsync(GetClaimId(ClaimType.SiteId), pageStub);
            if(page.IsPublished)
            {
                return page;
            }
            else
            {
                if(HasPermission(Permission.ViewUnpublishedPages))
                {
                    return page;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
