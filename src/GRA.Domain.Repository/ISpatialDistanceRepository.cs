using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ISpatialDistanceRepository : IRepository<Model.SpatialDistanceHeader>
    {
        Task<SpatialDistanceHeader> AddHeaderWithDetailsListAsync(
            SpatialDistanceHeader spatialHeader,
            List<SpatialDistanceDetail> detailList);

        Task<int?> GetIdByGeolocationAsync(int siteId, string geolocation);

        Task InvalidateHeadersAsync(int siteId);

        Task RemoveBranchReferencesAsync(int branchId);

        Task RemoveLocationReferencesAsync(int locationId);
    }
}
