using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEnteredSchoolRepository : IRepository<Model.EnteredSchool>
    {
        Task ConvertSchoolAsync(EnteredSchool enteredSchool, int schoolId);
        Task<DataWithCount<ICollection<EnteredSchool>>> GetPaginatedListAsync(int siteId,
            int skip,
            int take);
    }
}
