using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISchoolDistrictRepository : IRepository<SchoolDistrict>
    {
        Task<ICollection<SchoolDistrict>> GetAllAsync(int siteId);
        Task<DataWithCount<ICollection<SchoolDistrict>>> GetPaginatedListAsync(int siteId,
            int skip,
            int take);
    }
}
