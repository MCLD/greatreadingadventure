using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
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

        public async Task<int> AddSaveWithTextAsync(int userId, EmailBase emailBase)
        {
            var dbBase = await AddSaveAsync(userId, emailBase);

            if (emailBase?.EmailBaseText != null)
            {
                emailBase.EmailBaseText.EmailBaseId = dbBase.Id;
                var dbBaseText = _mapper.Map<EmailBaseText,
                    Model.EmailBaseText>(emailBase.EmailBaseText);
                try
                {
                    dbBaseText.CreatedAt = _dateTimeProvider.Now;
                    dbBaseText.CreatedBy = userId;
                    await _context.AddAsync(dbBaseText);
                    await _context.SaveChangesAsync();
                }
                catch (GraDbUpdateException gex)
                {
                    await RemoveSaveAsync(userId, dbBase.Id);
                    throw;
                }
            }

            return dbBase.Id;
        }

        public async Task<IEnumerable<EmailBase>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Name)
                .ProjectToType<EmailBase>()
                .ToListAsync();
        }

        public async Task<EmailBase> GetDefaultAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDefault)
                .ProjectToType<EmailBase>()
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<int>> GetTextLanguagesAsync(int emailBaseId)
        {
            return await _context
                .EmailBaseTexts
                .AsNoTracking()
                .Where(_ => _.EmailBaseId == emailBaseId)
                .Select(_ => _.LanguageId)
                .ToListAsync();
        }

        public async Task<EmailBase> GetWithTextAsync(int emailBaseId, int languageId)
        {
            var emailBase = await DbSet
                .AsNoTracking()
                .ProjectToType<EmailBase>()
                .SingleOrDefaultAsync(_ => _.Id == emailBaseId);

            if (emailBase != null)
            {
                emailBase.EmailBaseText = await _context
                    .EmailBaseTexts
                    .AsNoTracking()
                    .ProjectToType<EmailBaseText>()
                    .SingleOrDefaultAsync(_ => _.EmailBaseId == emailBaseId
                        && _.LanguageId == languageId);
            }
            return emailBase;
        }

        public async Task<EmailBase> GetWithTextByIdAsync(int emailBaseId, int languageId)
        {
            var emailBase = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == emailBaseId)
                .ProjectToType<EmailBase>()
                .SingleOrDefaultAsync();

            if (emailBase != null)
            {
                emailBase.EmailBaseText = await _context
                    .EmailBaseTexts
                    .AsNoTracking()
                    .ProjectToType<EmailBaseText>()
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

        public async Task<ICollectionWithCount<EmailBase>> PageAsync(BaseFilter filter)
        {
            var templates = DbSet.AsNoTracking();

            var count = await templates.CountAsync();

            var templateList = await templates
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<EmailBase>()
                .ToListAsync();

            return new ICollectionWithCount<EmailBase>
            {
                Data = templateList,
                Count = count
            };
        }

        public async Task UpdateSaveWithText(int userId, EmailBase emailBase)
        {
            if (emailBase == null)
            {
                throw new GraException("No email base template to update.");
            }

            var dbTemplate = await GetByIdAsync(emailBase.Id);

            if (dbTemplate == null)
            {
                throw new GraException($"Unable to find email template id {emailBase.Id}.");
            }

            bool changes = false;
            if (!string.IsNullOrEmpty(emailBase.Name)
                && emailBase.Name.Trim() != dbTemplate.Name)
            {
                dbTemplate.Name = emailBase.Name.Trim();
                changes = true;
            }

            if (changes)
            {
                await UpdateSaveAsync(userId, dbTemplate);
            }

            bool newRecord = false;
            if (emailBase?.EmailBaseText != null)
            {
                var dbBaseText = await _context
                    .EmailBaseTexts
                    .SingleOrDefaultAsync(_ => _.EmailBaseId == emailBase.Id
                        && _.LanguageId == emailBase.EmailBaseText.LanguageId);

                if (dbBaseText == null)
                {
                    newRecord = true;
                    dbBaseText = new Model.EmailBaseText
                    {
                        EmailBaseId = emailBase.Id,
                        CreatedAt = _dateTimeProvider.Now,
                        CreatedBy = userId,
                        LanguageId = emailBase.EmailBaseText.LanguageId
                    };
                }

                dbBaseText.TemplateHtml = emailBase.EmailBaseText.TemplateHtml?.Trim();
                dbBaseText.TemplateMjml = emailBase.EmailBaseText.TemplateMjml?.Trim();
                dbBaseText.TemplateText = emailBase.EmailBaseText.TemplateText?.Trim();

                if (newRecord)
                {
                    await _context.EmailBaseTexts.AddAsync(dbBaseText);
                }
                else
                {
                    _context.EmailBaseTexts.Update(dbBaseText);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
