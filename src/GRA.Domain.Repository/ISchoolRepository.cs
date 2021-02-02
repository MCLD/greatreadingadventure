using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ISchoolRepository : IRepository<School>
    {
        Task<ICollection<School>> GetAllAsync(int siteId, int? districtId = default);

        Task<bool> IsInUseAsync(int siteId, int schoolId);

        Task<ICollection<School>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);

        Task<bool> ValidateAsync(int schoolId, int siteId);
        Task<IList<SchoolImportExport>> GetForExportAsync();
    }
}
