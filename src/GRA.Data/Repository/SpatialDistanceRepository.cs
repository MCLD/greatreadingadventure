using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SpatialDistanceRepository
        : AuditingRepository<Model.SpatialDistanceHeader, SpatialDistanceHeader>,
        ISpatialDistanceRepository
    {

        public SpatialDistanceRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SpatialDistanceRepository> logger)
            : base(repositoryFacade, logger) { }

        public async Task<int?> GetIdByGeolocationAsync(int siteId, string geolocation)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Geolocation == geolocation && _.IsValid)
                .Select(_ => (int?)_.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<SpatialDistanceHeader> AddHeaderWithDetailsListAsync(
            SpatialDistanceHeader spatialHeader,
            List<SpatialDistanceDetail> detailList)
        {
            var dbHeader = _mapper
                .Map<SpatialDistanceHeader, Model.SpatialDistanceHeader>(spatialHeader);
            await DbSet.AddAsync(dbHeader);


            var dbDetailList = _mapper
                .Map<List<SpatialDistanceDetail>, List<Model.SpatialDistanceDetail>>(detailList);
            dbDetailList.ForEach(_ => _.SpatialDistanceHeaderId = dbHeader.Id);
            await _context.SpatialDistanceDetails.AddRangeAsync(dbDetailList);

            await _context.SaveChangesAsync();

            return _mapper.Map<Model.SpatialDistanceHeader, SpatialDistanceHeader>(dbHeader);
        }

        public async Task InvalidateHeadersAsync(int siteId)
        {
            var headers = await DbSet.Where(_ => _.SiteId == siteId && _.IsValid).ToListAsync();

            headers.ForEach(_ => _.IsValid = false);

            await _context.SaveChangesAsync();
        }
    }
}
