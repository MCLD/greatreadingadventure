using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsBlackoutDateRepository
        : AuditingRepository<Model.PsBlackoutDate, Domain.Model.PsBlackoutDate>, IPsBlackoutDateRepository
    {
        public PsBlackoutDateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsBlackoutDateRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsBlackoutDate> GetByDateAsync(DateTime date)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Date.Date == date.Date)
                .ProjectTo<PsBlackoutDate>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PsBlackoutDate>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Date)
                .ProjectTo<PsBlackoutDate>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<PsBlackoutDate>>> PageAsync(
            BaseFilter filter)
        {
            var blackoutDates = DbSet.AsNoTracking();

            var count = await blackoutDates.CountAsync();

            var blackoutDateList = await blackoutDates
                .OrderBy(_ => _.Date)
                .ApplyPagination(filter)
                .ProjectTo<PsBlackoutDate>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new DataWithCount<ICollection<PsBlackoutDate>>
            {
                Data = blackoutDateList,
                Count = count
            };
        }
    }
}
