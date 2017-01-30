using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISchoolRepository : IRepository<School>
    {
        Task<ICollection<School>> GetAllAsync(int siteId,
            int? districtId = default(int?),
            int? typeId = default(int?));

        Task<bool> IsInUseAsync(int siteId, int schoolId);

        Task<DataWithCount<ICollection<School>>> GetPaginatedListAsync(int siteId,
            int skip,
            int take,
            int? districtId = default(int?),
            int? typeId = default(int?));
    }
}
