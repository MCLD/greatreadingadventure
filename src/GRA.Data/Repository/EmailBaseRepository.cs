using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public async Task<IEnumerable<EmailBase>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Name)
                .ProjectTo<EmailBase>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<EmailBase> GetDefaultAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDefault)
                .ProjectTo<EmailBase>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
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

        public async Task ImportSaveTextAsync(int userId, EmailBaseText emailBaseText)
        {
            var dbEntity = _mapper.Map<EmailBaseText, Model.EmailBaseText>(emailBaseText);
            dbEntity.CreatedBy = userId;
            dbEntity.CreatedAt = _dateTimeProvider.Now;
            EntityEntry<Model.EmailBaseText> dbEntityEntry = _context.Entry(dbEntity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                await _context.EmailBaseTexts.AddAsync(dbEntity);
            }
            await SaveAsync();
        }
    }
}
