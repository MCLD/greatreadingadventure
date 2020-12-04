﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPageRepository : IRepository<Page>
    {
        Task<IEnumerable<Page>> GetByHeaderIdAsync(int headerId);
        Task<Page> GetByHeaderAndLanguageAsync(int headerId, int languageId);
        Task<Page> GetByStubAsync(int siteId, string pageStub, int languageId);
        Task<IEnumerable<Page>> PageAllAsync(int siteId, int skip, int take);
        Task<int> GetCountAsync(int siteId);
        Task<IEnumerable<Page>> GetAreaPagesAsync(int siteId, bool navPages, int languageId);
    }
}