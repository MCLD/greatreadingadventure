using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DirectEmailTemplateRepository
        : AuditingRepository<Model.DirectEmailTemplate, DirectEmailTemplate>,
        IDirectEmailTemplateRepository
    {
        public DirectEmailTemplateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DirectEmailTemplateRepository> logger)
            : base(repositoryFacade, logger)
        {
        }

        public async Task<int> AddSaveWithTextAsync(int userId,
            DirectEmailTemplate directEmailTemplate)
        {
            var dbTemplate = await AddSaveAsync(userId, directEmailTemplate);

            if (directEmailTemplate?.DirectEmailTemplateText != null)
            {
                directEmailTemplate.DirectEmailTemplateText.DirectEmailTemplateId = dbTemplate.Id;
                var dbTemplateText = _mapper.Map<DirectEmailTemplateText,
                    Model.DirectEmailTemplateText>(directEmailTemplate.DirectEmailTemplateText);
                try
                {
                    dbTemplateText.CreatedAt = _dateTimeProvider.Now;
                    dbTemplateText.CreatedBy = userId;
                    await _context.AddAsync(dbTemplateText);
                    await _context.SaveChangesAsync();
                }
                catch (GraDbUpdateException gex)
                {
                    await RemoveSaveAsync(userId, dbTemplate.Id);
                    throw;
                }
            }

            return dbTemplate.Id;
        }

        public async Task<IDictionary<int, string>> GetAllUserTemplatesAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => string.IsNullOrEmpty(_.SystemEmailId))
                .OrderBy(_ => _.Description)
                .ToDictionaryAsync(k => k.Id, v => v.Description);
        }

        public async Task<(int, List<int>)> GetIdAndLanguagesBySystemIdAsync(string systemEmailId)
        {
            var directEmailTemplate = await DbSet
                .AsNoTracking()
                .SingleOrDefaultAsync(_ => _.SystemEmailId == systemEmailId);

            if (directEmailTemplate == null)
            {
                throw new GraException($"Unable to find DirectEmailTemplate with system email id {systemEmailId}");
            }

            var languages = await _context
                .DirectEmailTemplateTexts
                .AsNoTracking()
                .Where(_ => _.DirectEmailTemplateId == directEmailTemplate.Id)
                .Select(_ => _.LanguageId)
                .ToListAsync();

            return (directEmailTemplate.Id, languages);
        }

        public async Task<IDictionary<int, bool>> GetLanguageUnsubAsync(int directEmailTemplateId)
        {
            return await _context
                .DirectEmailTemplateTexts
                .AsNoTracking()
                .Where(_ => _.DirectEmailTemplateId == directEmailTemplateId)
                .ToDictionaryAsync(k => k.LanguageId,
                    v => v.Footer.Contains("{{UnsubscribeLink}}",
                        System.StringComparison.OrdinalIgnoreCase));
        }

        public async Task<DirectEmailTemplate> GetWithTextByIdAsync(int directEmailTemplateId,
                    int languageId)
        {
            var directEmailTemplate = await DbSet
                .AsNoTracking()
                .ProjectToType<DirectEmailTemplate>()
                .SingleOrDefaultAsync(_ => _.Id == directEmailTemplateId);
            if (directEmailTemplate != null)
            {
                directEmailTemplate.DirectEmailTemplateText = await _context
                    .DirectEmailTemplateTexts
                    .AsNoTracking()
                    .ProjectToType<DirectEmailTemplateText>()
                    .SingleOrDefaultAsync(_ => _.DirectEmailTemplateId == directEmailTemplateId
                        && _.LanguageId == languageId);
            }
            return directEmailTemplate;
        }

        public async Task<DirectEmailTemplate> GetWithTextBySystemId(string systemEmailId,
            int languageId)
        {
            var directEmailTemplate = await DbSet
                .AsNoTracking()
                .ProjectToType<DirectEmailTemplate>()
                .SingleOrDefaultAsync(_ => _.SystemEmailId == systemEmailId);
            if (directEmailTemplate != null)
            {
                directEmailTemplate.DirectEmailTemplateText = await _context
                    .DirectEmailTemplateTexts
                    .AsNoTracking()
                    .ProjectToType<DirectEmailTemplateText>()
                    .SingleOrDefaultAsync(_ => _.DirectEmailTemplateId == directEmailTemplate.Id
                        && _.LanguageId == languageId);
            }
            return directEmailTemplate;
        }

        public async Task ImportSaveTextAsync(int userId,
            DirectEmailTemplateText directEmailTemplateText)
        {
            var dbEntity = _mapper.Map<DirectEmailTemplateText,
                Model.DirectEmailTemplateText>(directEmailTemplateText);
            dbEntity.CreatedBy = userId;
            dbEntity.CreatedAt = _dateTimeProvider.Now;
            EntityEntry<Model.DirectEmailTemplateText> dbEntityEntry = _context.Entry(dbEntity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                await _context.DirectEmailTemplateTexts.AddAsync(dbEntity);
            }
            await SaveAsync();
        }

        public async Task IncrementSentCountAsync(int directEmailTemplateId, int incrementBy)
        {
            var template = await DbSet.FindAsync(directEmailTemplateId);
            template.EmailsSent += incrementBy;
            _context.Update(template);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollectionWithCount<EmailTemplateListItem>> PageAsync(BaseFilter filter)
        {
            var templates = DbSet.AsNoTracking();

            var count = await templates.CountAsync();

            var templateList = await templates
                .OrderBy(_ => _.Description)
                .ApplyPagination(filter)
                .Select(_ => new EmailTemplateListItem
                {
                    Description = _.Description,
                    EmailsSent = _.EmailsSent,
                    Id = _.Id,
                    IsBulkSent = _.SentBulk,
                    IsSystem = !string.IsNullOrEmpty(_.SystemEmailId)
                })
                .ToListAsync();

            var languageLookup = _context.DirectEmailTemplateTexts
                .AsNoTracking()
                .Where(_ => templateList.Select(_ => _.Id).Contains(_.DirectEmailTemplateId))
                .ToLookup(_ => _.DirectEmailTemplateId, _ => _.LanguageId);

            foreach (var template in templateList)
            {
                if (languageLookup.Contains(template.Id))
                {
                    template.LanguageIds = languageLookup[template.Id];
                }
            }

            return new ICollectionWithCount<EmailTemplateListItem>
            {
                Data = templateList,
                Count = count
            };
        }

        public async Task<bool> SystemEmailIdExistsAsync(string systemEmailId)
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync(_ => _.SystemEmailId == systemEmailId);
        }

        public async Task<DirectEmailTemplate>
            UpdateSaveWithTextAsync(int userId, DirectEmailTemplate directEmailTemplate)
        {
            if (directEmailTemplate == null)
            {
                throw new GraException("No email template to update.");
            }

            DirectEmailTemplate updated = null;

            var dbTemplate = await GetByIdAsync(directEmailTemplate.Id);

            if (dbTemplate == null)
            {
                throw new GraException($"Unable to find email template id {directEmailTemplate.Id}.");
            }

            bool changes = false;
            if (!string.IsNullOrEmpty(directEmailTemplate.Description)
                && directEmailTemplate.Description.Trim() != dbTemplate.Description)
            {
                dbTemplate.Description = directEmailTemplate.Description.Trim();
                changes = true;
            }
            if (directEmailTemplate.EmailBaseId != dbTemplate.EmailBaseId)
            {
                dbTemplate.EmailBaseId = directEmailTemplate.EmailBaseId;
                changes = true;
            }

            updated = changes
                ? await UpdateSaveAsync(userId, dbTemplate)
                : await GetByIdAsync(directEmailTemplate.Id);

            bool newRecord = false;
            if (directEmailTemplate?.DirectEmailTemplateText != null)
            {
                var dbTemplateText = await _context
                    .DirectEmailTemplateTexts
                    .SingleOrDefaultAsync(_ => _.DirectEmailTemplateId == directEmailTemplate.Id
                        && _.LanguageId == directEmailTemplate.DirectEmailTemplateText.LanguageId);

                if (dbTemplateText == null)
                {
                    newRecord = true;
                    dbTemplateText = new Model.DirectEmailTemplateText
                    {
                        DirectEmailTemplateId = directEmailTemplate.Id,
                        CreatedAt = _dateTimeProvider.Now,
                        CreatedBy = userId,
                        LanguageId = directEmailTemplate.DirectEmailTemplateText.LanguageId
                    };
                }

                dbTemplateText.BodyCommonMark
                    = directEmailTemplate.DirectEmailTemplateText.BodyCommonMark?.Trim();
                dbTemplateText.Footer
                    = directEmailTemplate?.DirectEmailTemplateText.Footer?.Trim();
                dbTemplateText.Preview
                    = directEmailTemplate?.DirectEmailTemplateText.Preview?.Trim();
                dbTemplateText.Subject
                    = directEmailTemplate?.DirectEmailTemplateText.Subject?.Trim();
                dbTemplateText.Title
                    = directEmailTemplate?.DirectEmailTemplateText.Title?.Trim();

                if (newRecord)
                {
                    await _context.DirectEmailTemplateTexts.AddAsync(dbTemplateText);
                }
                else
                {
                    _context.DirectEmailTemplateTexts.Update(dbTemplateText);
                }

                await _context.SaveChangesAsync();
            }

            return updated;
        }

        public async Task UpdateSentBulkAsync(int directEmailTemplateId)
        {
            var directEmailTemplate = await DbSet
                .Where(_ => _.Id == directEmailTemplateId && !_.SentBulk)
                .ProjectToType<DirectEmailTemplate>()
                .SingleOrDefaultAsync();

            if (directEmailTemplate != null)
            {
                directEmailTemplate.SentBulk = true;
                await UpdateSaveNoAuditAsync(directEmailTemplate);
            }
        }
    }
}
