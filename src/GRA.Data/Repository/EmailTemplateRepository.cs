using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EmailTemplateRepository
        : AuditingRepository<Model.EmailTemplate, EmailTemplate>,
        IEmailTemplateRepository
    {
        public EmailTemplateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EmailTemplateRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<DataWithCount<ICollection<EmailTemplate>>> PageAsync(BaseFilter filter)
        {
            var templates = DbSet.AsNoTracking();

            var count = await templates.CountAsync();

            var templateList = await templates
                .OrderBy(_ => _.Description)
                .ApplyPagination(filter)
                .Select(_ => new EmailTemplate
                {
                    Id = _.Id,
                    Description = _.Description,
                    EmailsSent = _.EmailsSent
                })
                .ToListAsync();

            return new DataWithCount<ICollection<EmailTemplate>>
            {
                Data = templateList,
                Count = count
            };
        }
    }
}
