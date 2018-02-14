using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
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

        Task<ICollection<School>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);

        Task<bool> ValidateAsync(int schoolId, int siteId);

        Task<bool> AnyPrivateSchoolsAsync(int siteId);
        Task<List<School>> GetPrivateSchoolListAsync(int siteId);
        Task<bool> AnyCharterSchoolsAsync(int siteId);
        Task<List<School>> GetCharterSchoolListAsync(int siteId);
    }
}
