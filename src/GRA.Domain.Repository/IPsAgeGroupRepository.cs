using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsAgeGroupRepository : IRepository<PsAgeGroup>
    {
        Task<IEnumerable<PsAgeGroup>> GetAllAsync();
        Task<DataWithCount<ICollection<PsAgeGroup>>> GetPaginatedListAsync(BaseFilter filter);
    }
}
