using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ISpatialDistanceRepository : IRepository<Model.SpatialDistanceHeader>
    {
        Task<int?> GetIdByGeolocationAsync(int siteId, string geolocation);
        Task<SpatialDistanceHeader> AddHeaderWithDetailsListAsync(
            SpatialDistanceHeader spatialHeader,
            List<SpatialDistanceDetail> detailList);
        Task InvalidateHeadersAsync(int siteId);
    }
}
