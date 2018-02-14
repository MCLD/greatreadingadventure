using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISchoolDistrictRepository : IRepository<SchoolDistrict>
    {
        Task<ICollection<SchoolDistrict>> GetAllAsync(int siteId, bool excludeUserUnselectable);
        Task<ICollection<SchoolDistrict>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
    }
}
