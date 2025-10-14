using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
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
                .OrderByDescending(_ => _.IsDefault)
                .ThenBy(_ => _.Name)
                .ProjectToType<Language>()
                .ToListAsync();
        }

        public async Task<ICollection<Language>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.IsDefault)
                .ThenBy(_ => _.Name)
                .ProjectToType<Language>()
                .ToListAsync();
        }

        public async Task<Language> GetActiveByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsActive && _.Id == id)
                .ProjectToType<Language>()
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetDefaultLanguageId()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsActive && _.IsDefault)
                .Select(_ => _.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetLanguageId(string culture)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsActive && _.Name == culture)
                .Select(_ => _.Id)
                .SingleOrDefaultAsync();
        }
    }
}
