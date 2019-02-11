using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class LanguageRepository
        : AuditingRepository<Model.Language, Language>, ILanguageRepository
    {
        public LanguageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<LanguageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<Language>> GetActiveAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsActive)
                .OrderBy(_ => _.IsDefault)
                .ThenBy(_ => _.Name)
                .ProjectTo<Language>()
                .ToListAsync();
        }

        public async Task<ICollection<Language>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.IsDefault)
                .ThenBy(_ => _.Name)
                .ProjectTo<Language>()
                .ToListAsync();
        }
    }
}
