using GRA.Domain.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class NotificationRepository
        : AuditingRepository<Model.Notification, Notification>, INotificationRepository
    {
        public NotificationRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<NotificationRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderBy(_ => _.CreatedAt)
                .ProjectTo<Notification>()
                .ToListAsync();
        }

        public async Task RemoveByUserId(int userId)
        {
            var notifications = DbSet.Where(_ => _.UserId == userId);
            foreach(var notification in notifications)
            {
                DbSet.Remove(notification);
            }
            await SaveAsync();
        }
    }
}
