using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEnteredSchoolRepository : IRepository<Model.EnteredSchool>
    {
        Task ConvertSchoolAsync(EnteredSchool enteredSchool, int schoolId);
        Task<ICollection<EnteredSchool>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
    }
}
