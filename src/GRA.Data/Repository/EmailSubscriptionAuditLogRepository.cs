using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EmailSubscriptionAuditLogRepository : IEmailSubscriptionAuditLogRepository
    {
        protected readonly Context _context;
        protected readonly ILogger _logger;
        protected readonly AutoMapper.IMapper _mapper;
        protected readonly IConfiguration _config;
        protected readonly IDateTimeProvider _dateTimeProvider;
        protected readonly IEntitySerializer _entitySerializer;

        public EmailSubscriptionAuditLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<IRepository<EmailSubscriptionAuditLog>> logger)
        {
            if (repositoryFacade == null)
            {
                throw new ArgumentNullException(nameof(repositoryFacade));
            }
            _context = repositoryFacade.context;
            _mapper = repositoryFacade.mapper;
            _config = repositoryFacade.config;
            _dateTimeProvider = repositoryFacade.dateTimeProvider;
            _entitySerializer = repositoryFacade.entitySerializer;
            _logger = Require.IsNotNull(logger, nameof(logger));
        }

        public async Task<ICollection<EmailSubscriptionAuditLog>> GetUserAuditLogAsync(int userId)
        {
            return await _context.EmailSubscriptionAuditLogs
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderBy(_ => _.CreatedAt)
                .ProjectTo<EmailSubscriptionAuditLog>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task AddEntryAsync(int auditId, int userId, bool subscribe)
        {
            var auditLogEntry = new Model.EmailSubscriptionAuditLog
            {
                CreatedAt = _dateTimeProvider.Now,
                CreatedBy = auditId,
                Subscribed = subscribe,
                UserId = userId
            };

            await _context.EmailSubscriptionAuditLogs.AddAsync(auditLogEntry);
        }
    }
}
