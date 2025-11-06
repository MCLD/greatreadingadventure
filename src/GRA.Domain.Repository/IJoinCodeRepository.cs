using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IJoinCodeRepository : IRepository<JoinCode>
    {
        Task<bool> CodeExistsAsync(string code);
        Task<JoinCode> GetByCodeAsync(string code);
        Task<JoinCode> GetByTypeAndBranchAsync(bool isQRCode, int? branchId);
        Task IncrementAccessCountAsync(int id);
        Task IncrementJoinCountAsync(int id);
        Task<DataWithCount<IEnumerable<JoinCode>>> PageAsync(BaseFilter filter);
    }
}
