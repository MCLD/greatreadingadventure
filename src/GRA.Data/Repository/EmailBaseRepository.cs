using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EmailBaseRepository
        : AuditingRepository<Model.EmailBase, Domain.Model.EmailBase>,
        IEmailBaseRepository
    {
        public EmailBaseRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EmailBaseRepository> logger)
            : base(repositoryFacade, logger)
        {
        }

        public async Task<EmailBase> GetWithTextByIdAsync(int emailBaseId, int languageId)
        {
            var emailBase = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == emailBaseId)
                .ProjectTo<EmailBase>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (emailBase != null)
            {
                emailBase.EmailBaseText = await _context
                    .EmailBaseTexts
                    .AsNoTracking()
                    .ProjectTo<EmailBaseText>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(_ => _.EmailBaseId == emailBase.Id
                        && _.LanguageId == languageId);
            }

            return emailBase;
        }
    }
}
