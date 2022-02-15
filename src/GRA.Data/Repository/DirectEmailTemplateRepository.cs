using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
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

        public async Task<DirectEmailTemplate> GetWithTextByIdAsync(int directEmailTemplateId,
            int languageId)
        {
            var directEmailTemplate = await DbSet
                .AsNoTracking()
                .ProjectTo<DirectEmailTemplate>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(_ => _.Id == directEmailTemplateId);
            if (directEmailTemplate != null)
            {
                directEmailTemplate.DirectEmailTemplateText = await _context
                    .DirectEmailTemplateTexts
                    .AsNoTracking()
                    .ProjectTo<DirectEmailTemplateText>(_mapper.ConfigurationProvider)
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
                .ProjectTo<DirectEmailTemplate>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(_ => _.SystemEmailId == systemEmailId);
            if (directEmailTemplate != null)
            {
                directEmailTemplate.DirectEmailTemplateText = await _context
                    .DirectEmailTemplateTexts
                    .AsNoTracking()
                    .ProjectTo<DirectEmailTemplateText>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(_ => _.DirectEmailTemplateId == directEmailTemplate.Id
                        && _.LanguageId == languageId);
            }
            return directEmailTemplate;
        }
    }
}
