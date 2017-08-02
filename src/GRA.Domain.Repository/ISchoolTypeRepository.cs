using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISchoolTypeRepository : IRepository<SchoolType>
    {
        Task<ICollection<SchoolType>> GetAllAsync(int siteId, int? districtId = default(int?));
        Task<ICollection<SchoolType>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
    }
}
